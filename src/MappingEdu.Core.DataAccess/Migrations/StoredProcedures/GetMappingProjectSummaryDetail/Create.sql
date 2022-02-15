-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE Procedure [dbo].[GetMappingProjectSummaryDetail]
@MappingProjectId uniqueidentifier,
@SystemItemId uniqueidentifier = '00000000-0000-0000-000000000000',
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
                ItemTypeId,
                MappedSystemId,
                ParentSystemItemId,
                SystemItemId
        FROM dbo.SystemItem
        JOIN project as mp on MappedSystemId = mp.SourceDataStandardMappedSystemId
            WHERE ((@SystemItemId='00000000-0000-0000-0000-000000000000' AND ParentSystemItemId is NULL) 
					OR (ParentSystemItemId = @SystemItemId)
					OR ((ItemTypeId = 4 OR ItemTypeId = 5) AND SystemItemId = @SystemItemId)) 
					AND [MappedSystemId] = mp.SourceDataStandardMappedSystemId
        UNION all
		SELECT result.EntityId,
                result.EntityName,
                i2.ItemTypeId,
                i2.MappedSystemId,
                i2.ParentSystemItemId,
                i2.SystemItemId
        FROM SystemItem AS i2
            inner join result
                ON result.SystemItemId = i2.ParentSystemItemId)

SELECT EntityName,
	    SystemItemId,
	    ItemTypeId,
	    EntityId,
	    ISNULL(map.WorkflowStatusTypeId, 0) as WorkflowStatusTypeId,
		ISNULL(map.MappingMethodTypeId, 0) as MappingMethodTypeId
INTO #TEMP
FROM result
LEFT JOIN (SELECT * FROM SystemItemMap 
			WHERE MappingProjectId = @MappingProjectId) as map 
ON SystemItemId = map.SourceSystemItemId
WHERE ((@ItemTypeId IS NULL AND (ItemTypeId = 4 or ItemTypeId = 5)) OR ItemTypeId = @ItemTypeId);

SELECT  COUNT(*) as Total,
        WorkflowStatusTypeId,
		MappingMethodTypeId
FROM #TEMP
GROUP BY WorkflowStatusTypeId, MappingMethodTypeId
ORDER BY WorkflowStatusTypeId, MappingMethodTypeId;
END