-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[GetMappingProjectSummary]
@MappingProjectId uniqueidentifier
AS
BEGIN
WITH project as (
	SELECT SourceDataStandardMappedSystemId
    FROM dbo.MappingProject
    WHERE MappingProjectId = @MappingProjectId), 
	
	result AS (
		SELECT SystemItemId as DomainId,
                ItemName as DomainName,
                ItemTypeId,
                MappedSystemId,
                ParentSystemItemId,
                SystemItemId
        FROM dbo.SystemItem
        JOIN project as mp on MappedSystemId = mp.SourceDataStandardMappedSystemId
            WHERE ParentSystemItemId is null and [MappedSystemId] = mp.SourceDataStandardMappedSystemId
        UNION all
		SELECT result.DomainId,
                result.DomainName,
                i2.ItemTypeId,
                i2.MappedSystemId,
                i2.ParentSystemItemId,
                i2.SystemItemId
        FROM SystemItem AS i2
            inner join result
                ON result.SystemItemId = i2.ParentSystemItemId)

SELECT DomainName,
	    SystemItemId,
	    ItemTypeId,
	    DomainId,
	    ISNULL(map.WorkflowStatusTypeId, 0) as WorkflowStatusTypeId,
		ISNULL(map.MappingMethodTypeId, 0) as MappingMethodTypeId
INTO #TEMP
FROM result
LEFT JOIN (SELECT * FROM SystemItemMap 
			WHERE MappingProjectId = @MappingProjectId) as map 
ON SystemItemId = map.SourceSystemItemId
WHERE (ItemTypeId = 4 or ItemTypeId = 5);

SELECT DomainName,
		DomainId,
        COUNT(*) as Count,
        WorkflowStatusTypeId,
		MappingMethodTypeId
FROM #TEMP
GROUP BY DomainId, DomainName, WorkflowStatusTypeId, MappingMethodTypeId;
END;