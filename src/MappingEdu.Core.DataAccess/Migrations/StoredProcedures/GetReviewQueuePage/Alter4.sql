-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetReviewQueuePage]
@MappingProjectId uniqueidentifier,
@SearchText varchar(MAX),
@ItemTypeIds varchar(MAX),
@WorkflowStatusIds varchar(50),
@DomainIds varchar(MAX),
@MethodIds varchar(50),
@Flagged varchar(50),
@OrderBy int, --Column Number (1 = DomainItemPath, 2 = Logic, 3 = WorkflowStatusId)
@SortDirection varchar(10), --ASC or DESC
@Start int,
@Take varchar(20)
AS
BEGIN

WITH project as (
	SELECT SourceDataStandardMappedSystemId
	FROM dbo.MappingProject
	WHERE MappingProjectId = @MappingProjectId 
), result as (
SELECT Definition,
		CAST(ISNULL(Definition, ' ') AS NVARCHAR(MAX)) AS DefinitionPath,
        CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
	    CAST(SystemItemId AS NVARCHAR(MAX)) as DomainItemPathIds,
	    SystemItemId as DomainId,
	    ItemTypeId,
	    MappedSystemId,
	    ParentSystemItemId,
	    SystemItemId
FROM dbo.SystemItem
JOIN project as mp on MappedSystemId = mp.SourceDataStandardMappedSystemId
WHERE ParentSystemItemId is null AND [MappedSystemId] = mp.SourceDataStandardMappedSystemId AND
		(@DomainIds = 'All' OR @DomainIds like '%;'+cast(SystemItemId as varchar(50))+';%') AND IsActive = 1
UNION all
SELECT i2.Definition,
		result.DefinitionPath + ';' + ISNULL(i2.Definition, ' '),
        result.DomainItemPath + ';' + i2.ItemName,
        result.DomainItemPathIds + ';' + CAST(i2.SystemItemId AS NVARCHAR(MAX)),
	    result.DomainId,
	    i2.ItemTypeId,
	    i2.MappedSystemId,
	    i2.ParentSystemItemId,
	    i2.SystemItemId
FROM SystemItem AS i2
inner join result
    ON result.SystemItemId = i2.ParentSystemItemId)

SELECT Definition,
		DefinitionPath,
		DomainItemPath,
		DomainItemPathIds,
		ItemTypeId,
		SystemItemId
INTO #TEMP
FROM result			
WHERE (@ItemTypeIds like '%;'+cast(ItemTypeId as varchar(20))+';%');

SELECT DefinitionPath,
        DomainItemPath,
	    DomainItemPathIds,
	    item.ItemTypeId,
		it.ItemTypeName,
	    item.SystemItemId,
	    ISNULL(map.Logic, 'Not Yet Mapped') as Logic,
	    map.Flagged,
	    ISNULL(map.WorkflowStatusTypeId, 0) as WorkflowStatusTypeId,
	    map.MappingMethodTypeId,
	    map.SystemItemMapId,
		ISNULL(map.MappedEnumerations, 0) as MappedEnumerations,
		ISNULL(sei.Count, 0) as TotalEnumerations
INTO #TEMP2
FROM #TEMP as item
	LEFT JOIN
	(SELECT CASE WHEN MappingMethodTypeId = 4 THEN 'Marked For Omission'
					WHEN MappingMethodTypeId = 3 THEN 'Marked for Extension' 
					ELSE BusinessLogic END as Logic,
			WorkflowStatusTypeId,
			MappingMethodTypeId,
			Flagged as Flagged,
			SourceSystemItemId,
			sim.SystemItemMapId,
            MappingProjectId,
			seim.Count as MappedEnumerations
	    FROM dbo.SystemItemMap as sim
		LEFT JOIN (SELECT SystemItemMapId, COUNT(*) as Count
				FROM SystemEnumerationItemMap
				GROUP BY SystemItemMapId) seim
		ON seim.SystemItemMapId = sim.SystemItemMapId
	    WHERE sim.MappingProjectId = @MappingProjectId) map ON SystemItemId = map.SourceSystemItemId
		JOIN ItemType as it on it.ItemTypeId = item.ItemTypeId

LEFT JOIN (SELECT SystemItemId, COUNT(*) as COUNT
			FROM SystemEnumerationItem
			GROUP BY SystemItemId) sei on item.SystemItemId = sei.SystemItemId

WHERE (ISNULL(map.Logic, 'Not Yet Mapped') like '%' + @SearchText + '%' OR 
		DomainItemPath like '%' + @SearchText + '%' OR 
		Definition like '%' + @SearchText + '%' OR 
		it.ItemTypeName like '%' + @SearchText + '%') AND
	    (@MethodIds = 'All' OR @MethodIds like '%;'+cast(ISNULL(map.MappingMethodTypeId, 0) as varchar(20))+';%') AND
		(@WorkflowStatusIds = 'All' OR @WorkflowStatusIds like '%;'+cast(ISNULL(map.WorkflowStatusTypeId, 0) as varchar(20))+';%') AND
	    (@Flagged = 'All' OR map.Flagged = 1);

SELECT DefinitionPath,
        DomainItemPath,
	    DomainItemPathIds,
	    ItemTypeId,
	    SystemItemId,
	    Logic,
	    Flagged,
	    WorkflowStatusTypeId,
	    MappingMethodTypeId,
	    SystemItemMapId,
		MappedEnumerations,
		TotalEnumerations
FROM #TEMP2

ORDER BY
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 0 THEN DomainItemPath END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 1 THEN ItemTypeName END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 2 THEN Logic END ASC,
	CASE WHEN @SortDirection = 'asc' AND @OrderBy = 3 THEN WorkflowStatusTypeId END ASC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 0 THEN DomainItemPath END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 1 THEN ItemTypeName END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 2 THEN Logic END DESC,
	CASE WHEN @SortDirection = 'desc' AND @OrderBy = 3 THEN WorkflowStatusTypeId END DESC
OFFSET @Start ROW
FETCH NEXT CASE WHEN @Take = 'All' THEN (Select COUNT(*) FROM #TEMP2)
			    ELSE CAST(@Take as int)
			END ROWS ONLY;

SELECT (Select COUNT(*) FROM #TEMP2) as Filtered,
		(Select COUNT(*) FROM SystemItem as si
						JOIN MappingProject as mp on MappedSystemId = mp.SourceDataStandardMappedSystemId
						WHERE mp.MappingProjectId = @MappingProjectId AND
								si.MappedSystemId = mp.SourceDataStandardMappedSystemId AND (si.ItemTypeId = 4 OR si.ItemTypeId = 5) AND si.IsActive = 1) as Total;
END