-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetReviewQueuePage]
@MappingProjectId uniqueidentifier,
@SearchText varchar(MAX),
@ItemTypeIds dbo.IntId READONLY,
@WorkflowStatusIds dbo.IntId READONLY,
@Flagged bit,
@Unflagged bit,
@AutoMapped bit,
@UserMapped bit,
@Extended bit,
@Base bit,
@DomainIds dbo.UniqueIdentiferId READONLY,
@CreateByIds dbo.UniqueIdentiferId READONLY,
@UpdateByIds dbo.UniqueIdentiferId READONLY,
@MethodIds dbo.IntId READONLY,
@OrderBy int, --Column Number (1 = DomainItemPath, 2 = Logic, 3 = WorkflowStatusId)
@SortDirection varchar(10), --ASC or DESC
@Start int,
@Take varchar(20)
AS

DECLARE @SourceDataStandardId uniqueidentifier
DECLARE @NumberOfDomains int
DECLARE @NumberOfCreaters int
DECLARE @NumberOfUpdaters int
DECLARE @NumberOfItemTypes int
DECLARE @NumberOfMappingMethods int
DECLARE @NumberOfWorkflowStatuses int
DECLARE @FilterCount int

SELECT @SourceDataStandardId = SourceDataStandardMappedSystemId
FROM dbo.MappingProject WHERE MappingProjectId = @MappingProjectId;

SELECT @NumberOfDomains = COUNT(*) FROM @DomainIds
SELECT @NumberOfCreaters = COUNT(*) FROM @CreateByIds
SELECT @NumberOfUpdaters = COUNT(*) FROM @UpdateByIds
SELECT @NumberOfItemTypes = COUNT(*) FROM @ItemTypeIds
SELECT @NumberOfMappingMethods = COUNT(*) FROM @MethodIds
SELECT @NumberOfWorkflowStatuses = COUNT(*) FROM @WorkflowStatusIds

BEGIN

WITH result as (
SELECT Definition,
		CAST(ISNULL(Definition, ' ') AS NVARCHAR(MAX)) AS DefinitionPath,
        CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
	    CAST(SystemItemId AS NVARCHAR(MAX)) as DomainItemPathIds,
	    SystemItemId as DomainId,
		IsExtended,
		CAST(IsExtended AS NVARCHAR(MAX)) as IsExtendedPath,
	    ItemTypeId,
	    MappedSystemId,
	    ParentSystemItemId,
	    SystemItemId
FROM dbo.SystemItem
WHERE ParentSystemItemId is null AND MappedSystemId = @SourceDataStandardId AND
		(@NumberOfDomains = 0 OR SystemItemId in (SELECT Id FROM @DomainIds as domain))
UNION all
SELECT i2.Definition,
		result.DefinitionPath + ';' + ISNULL(i2.Definition, ' '),
        result.DomainItemPath + '.' + i2.ItemName,
        result.DomainItemPathIds + ';' + CAST(i2.SystemItemId AS NVARCHAR(50)),
	    result.DomainId,
		i2.IsExtended,
		result.IsExtendedPath + ';' + CAST(i2.IsExtended AS VARCHAR(1)),
	    i2.ItemTypeId,
	    i2.MappedSystemId,
	    i2.ParentSystemItemId,
	    i2.SystemItemId
FROM SystemItem AS i2
inner join result
    ON result.SystemItemId = i2.ParentSystemItemId)

SELECT map.CreateBy,
		map.CreateDate,
		DefinitionPath,
        DomainItemPath,
	    DomainItemPathIds,
		IsAutoMapped,
		map.MappedBy,
		IsExtended,
		IsExtendedPath,
	    item.ItemTypeId,
		it.ItemTypeName,
	    item.SystemItemId,
	    ISNULL(map.Logic, 'Not Yet Mapped') as Logic,
	    map.Flagged,
	    ISNULL(map.WorkflowStatusTypeId, 0) as WorkflowStatusTypeId,
	    map.MappingMethodTypeId,
	    map.SystemItemMapId,
		ISNULL(map.MappedEnumerations, 0) as MappedEnumerations,
		ISNULL(sei.Count, 0) as TotalEnumerations,
		map.UpdateBy,
		map.UpdateDate
INTO #TEMP
FROM result as item
	LEFT JOIN
	(SELECT CASE WHEN MappingMethodTypeId = 4 THEN 'Marked For Omission'
					WHEN MappingMethodTypeId = 3 THEN 'Marked for Extension' 
					ELSE BusinessLogic END as Logic,
			WorkflowStatusTypeId,
			MappingMethodTypeId,
			Flagged,
			CreateById,
			CreateDate,
			UpdateById,
			UpdateDate,
			IsAutoMapped,
			SourceSystemItemId,
			sim.SystemItemMapId,
            MappingProjectId,
			CASE WHEN IsAutoMapped = 1 THEN 'Auto Mapped'
					WHEN creater.FirstName is NULL THEN 'N/A'
					ELSE CONCAT(SUBSTRING(creater.FirstName, 0, 2), '. ', creater.LastName)
					END as MappedBy,
			CASE WHEN creater.FirstName is NULL THEN ''
					ELSE CONCAT(SUBSTRING(creater.FirstName, 0, 2), '. ', creater.LastName)
					END as CreateBy,
			CASE WHEN updater.FirstName is NULL THEN ''
					ELSE CONCAT(SUBSTRING(updater.FirstName, 0, 2), '. ', updater.LastName)  END as UpdateBy,
			seim.Count as MappedEnumerations
	    FROM dbo.SystemItemMap as sim

		LEFT JOIN AspNetUsers creater on 
		sim.CreateById = creater.Id

	LEFT JOIN AspNetUsers updater on 
		sim.UpdateById = updater.Id

		LEFT JOIN (SELECT SystemItemMapId, COUNT(*) as Count
				FROM SystemEnumerationItemMap
				GROUP BY SystemItemMapId) seim
		ON seim.SystemItemMapId = sim.SystemItemMapId
	    WHERE sim.MappingProjectId = @MappingProjectId AND 
			(@NumberOfMappingMethods = 0 OR ISNULL(sim.MappingMethodTypeId, 0) in (SELECT Id FROM @MethodIds)) AND
			(@NumberOfWorkflowStatuses = 0 OR ISNULL(sim.WorkflowStatusTypeId, 0) in (SELECT Id FROM @WorkflowStatusIds)) AND
			(@NumberOfCreaters = 0 OR creater.Id in (SELECT Id FROM @CreateByIds)) AND
			(@NumberOfUpdaters = 0 OR updater.Id in (SELECT Id FROM @UpdateByIds)) AND
			((@Unflagged = 0 AND @Flagged = 0) OR (@Unflagged = 1 AND (sim.Flagged = 0 OR sim.Flagged is null)) OR (@Flagged = 1 AND sim.Flagged = 1 AND sim.SystemItemMapId is not null)) AND
			((@AutoMapped = 0 AND @UserMapped = 0) OR (@AutoMapped = 1 AND sim.IsAutoMapped = 1) OR (@UserMapped = 1 AND sim.IsAutoMapped = 0))) map ON SystemItemId = map.SourceSystemItemId
		JOIN ItemType as it on it.ItemTypeId = item.ItemTypeId

LEFT JOIN (SELECT SystemItemId, COUNT(*) as COUNT
			FROM SystemEnumerationItem
			GROUP BY SystemItemId) sei on item.SystemItemId = sei.SystemItemId

WHERE (ISNULL(map.Logic, 'Not Yet Mapped') like '%' + @SearchText + '%' OR 
		DomainItemPath like '%' + @SearchText + '%' OR 
		Definition like '%' + @SearchText + '%' OR 
		it.ItemTypeName like '%' + @SearchText + '%') AND
		((@Extended = 0 AND @Base = 0) OR (@Extended = 1 AND IsExtended = 1) OR (@Base = 1 AND IsExtended = 0)) AND
		((@NumberOfItemTypes = 0 AND (item.ItemTypeId = 4 OR item.ItemTypeId = 5)) OR item.ItemTypeId in (SELECT Id FROM @ItemTypeIds)) AND
	    (@NumberOfMappingMethods = 0 OR ISNULL(map.MappingMethodTypeId, 0) in (SELECT Id FROM @MethodIds)) AND
		(@NumberOfWorkflowStatuses = 0 OR ISNULL(map.WorkflowStatusTypeId, 0) in (SELECT Id FROM @WorkflowStatusIds)) AND
		(@NumberOfCreaters = 0 OR map.CreateById in (SELECT Id FROM @CreateByIds)) AND
		(@NumberOfUpdaters = 0 OR map.UpdateById in (SELECT Id FROM @UpdateByIds)) AND
	    ((@Unflagged = 0 AND @Flagged = 0) OR (@Unflagged = 1 AND (map.Flagged = 0 OR map.Flagged is null)) OR (@Flagged = 1 AND map.Flagged = 1 AND map.SystemItemMapId is not null)) AND
		((@AutoMapped = 0 AND @UserMapped = 0) OR (@AutoMapped = 1 AND map.IsAutoMapped = 1) OR (@UserMapped = 1 AND map.IsAutoMapped = 0));

SELECT CreateBy,
		CreateDate,
		DefinitionPath,
        DomainItemPath,
	    DomainItemPathIds,
		IsAutoMapped,
		IsExtended,
		IsExtendedPath,
	    ItemTypeId,
	    SystemItemId,
	    Logic,
	    Flagged,
	    WorkflowStatusTypeId,
	    MappingMethodTypeId,
	    SystemItemMapId,
		MappedEnumerations,
		TotalEnumerations,
		UpdateBy,
		UpdateDate
FROM #TEMP
ORDER BY
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 0 THEN DomainItemPath END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 1 THEN ItemTypeName END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 2 THEN Logic END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 3 THEN WorkflowStatusTypeId END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 4 THEN CreateBy END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 5 THEN CreateDate END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 6 THEN UpdateBy END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 7 THEN UpdateDate END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 8 THEN MappedBy END ASC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 0 THEN DomainItemPath END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 1 THEN ItemTypeName END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 2 THEN Logic END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 3 THEN WorkflowStatusTypeId END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 4 THEN CreateBy END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 5 THEN CreateDate END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 6 THEN UpdateBy END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 7 THEN UpdateDate END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 8 THEN MappedBy END DESC

OFFSET @Start ROW
FETCH NEXT CASE WHEN @Take = 'All' THEN (SELECT COUNT(*) FROM #TEMP)
			    ELSE CAST(@Take as int)
			END ROWS ONLY;

SELECT (SELECT COUNT(*) FROM #TEMP) as Filtered,
		(Select COUNT(*) FROM SystemItem as si
						WHERE si.MappedSystemId = @SourceDataStandardId AND 
								(si.ItemTypeId = 4 OR si.ItemTypeId = 5) 
								AND si.IsActive = 1) as Total;
END