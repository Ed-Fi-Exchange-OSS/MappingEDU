-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetMappingProjectSummary]
@MappingProjectId uniqueidentifier,
@ParentSystemItemId uniqueidentifier = '00000000-0000-0000-0000-000000000000',
@ItemTypeId int = null

AS
BEGIN
WITH project as (
	SELECT SourceDataStandardMappedSystemId
    FROM dbo.MappingProject
    WHERE MappingProjectId = @MappingProjectId), 
	
	result AS (
		SELECT SystemItemId as EntityId,
                ItemName as EntityName,
				ItemTypeId as EntityItemTypeId,
                ItemTypeId,
                MappedSystemId,
                ParentSystemItemId,
                SystemItemId
        FROM dbo.SystemItem
        JOIN project as mp on MappedSystemId = mp.SourceDataStandardMappedSystemId
            WHERE IsActive = 1 AND ((@ParentSystemItemId = '00000000-0000-0000-0000-000000000000' AND ParentSystemItemId is NULL) OR (ParentSystemItemId = @ParentSystemItemId)) and [MappedSystemId] = mp.SourceDataStandardMappedSystemId
        UNION all
		SELECT result.EntityId,
                result.EntityName,
				result.EntityItemTypeId,
                i2.ItemTypeId,
                i2.MappedSystemId,
                i2.ParentSystemItemId,
                i2.SystemItemId
        FROM SystemItem AS i2
            inner join result
                ON result.SystemItemId = i2.ParentSystemItemId AND i2.IsActive = 1)

SELECT EntityName as ItemName,
		EntityId as SystemItemId,
		EntityItemTypeId as ItemTypeId,
        COUNT(*) as Total,
		SUM(case when WorkflowStatusTypeId is NULL then 1 else 0 end) Unmapped,
		SUM(case when WorkflowStatusTypeId = 1 then 1 else 0 end) Incomplete,
		SUM(case when WorkflowStatusTypeId = 2 then 1 else 0 end) Completed,
		SUM(case when WorkflowStatusTypeId = 3 then 1 else 0 end) Reviewed,
		SUM(case when WorkflowStatusTypeId = 4 then 1 else 0 end) Approved,
		SUM(case when MappingMethodTypeId = 1 then 1 else 0 end) Mapped,
		SUM(case when MappingMethodTypeId = 3 then 1 else 0 end) Extended,
		SUM(case when MappingMethodTypeId = 4 then 1 else 0 end) Omitted
FROM result
LEFT JOIN (SELECT * FROM SystemItemMap 
			WHERE MappingProjectId = @MappingProjectId) as map 
ON SystemItemId = map.SourceSystemItemId
WHERE ((@ItemTypeId IS NULL AND (ItemTypeId = 4 or ItemTypeId = 5)) OR ItemTypeId = @ItemTypeId)
GROUP BY EntityId, EntityName, EntityItemTypeId
ORDER BY EntityName;
END