-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[GetReviewQueuePage]
@MappingProjectId uniqueidentifier,
@SearchText varchar(MAX),
@ItemTypeIds varchar(MAX),
@WorkflowStatusIds varchar(50),
@DomainIds varchar(MAX),
@MethodIds varchar(50),
@Flagged varchar(50)
AS
BEGIN
WITH MappingProject as (
	SELECT SourceDataStandardMappedSystemId
	FROM dbo.MappingProject
	WHERE MappingProjectId = @MappingProjectId 
), result AS (
SELECT CAST(ISNULL(Definition, ' ') AS NVARCHAR(MAX)) AS DefinitionPath,
        CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
	    CAST(SystemItemId AS NVARCHAR(MAX)) as DomainItemPathIds,
	    SystemItemId as DomainId,
	    ItemTypeId,
	    MappedSystemId,
	    ParentSystemItemId,
	    SystemItemId
FROM dbo.SystemItem
JOIN MappingProject as mp on MappedSystemId = mp.SourceDataStandardMappedSystemId
WHERE ParentSystemItemId is null and [MappedSystemId] = mp.SourceDataStandardMappedSystemId
UNION all
SELECT result.DefinitionPath + ';' + ISNULL(i2.Definition, ' '),
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

SELECT DefinitionPath,
        DomainItemPath,
	    DomainItemPathIds,
	    ItemTypeId,
	    SystemItemId,
	    ISNULL(map.Logic, 'Not Yet Mapped') as Logic,
	    map.Flagged,
	    ISNULL(map.WorkflowStatusTypeId, 0) as WorkflowStatusTypeId,
	    map.MappingMethodTypeId,
	    map.SystemItemMapId
FROM result
	LEFT JOIN
	(SELECT CASE WHEN MappingMethodTypeId = 4 THEN OmissionReason ELSE BusinessLogic END as Logic,
			WorkflowStatusTypeId,
			MappingMethodTypeId,
			Flagged as Flagged,
			SourceSystemItemId,
			SystemItemMapId
	    FROM dbo.SystemItemMap as sim
	    WHERE sim.MappingProjectId = @MappingProjectId ) map on SystemItemId = map.SourceSystemItemId
WHERE @ItemTypeIds like '%;'+cast(ItemTypeId as varchar(20))+';%' AND
        (ISNULL(map.Logic, 'Not Yet Mapped') like '%' + @SearchText + '%' OR DomainItemPath like '%' + @SearchText + '%' OR DefinitionPath like '%' + @SearchText + '%') AND
	    (@WorkflowStatusIds = 'All' OR @WorkflowStatusIds like '%;'+cast(ISNULL(map.WorkflowStatusTypeId, 0) as varchar(20))+';%') AND
	    (@DomainIds = 'All' OR @DomainIds like '%;'+cast(DomainId as varchar(50))+';%') AND
	    (@MethodIds = 'All' OR @MethodIds like '%;'+cast(ISNULL(map.MappingMethodTypeId, 0) as varchar(20))+';%') AND
	    (@Flagged = 'All' OR map.Flagged = 1)
END