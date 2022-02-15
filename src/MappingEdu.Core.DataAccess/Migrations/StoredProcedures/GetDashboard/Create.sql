-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[GetDashboard]
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
	    ISNULL(map.WorkflowStatusTypeId, 0) as WorkflowStatusTypeId
INTO #TEMP
FROM result
LEFT JOIN SystemItemMap as map 
ON SystemItemId = map.SourceSystemItemId
    AND WorkflowStatusTypeId = (
            SELECT  TOP 1 ISNULL(WorkflowStatusTypeId, 0) as WorkflowStatusTypeId
            FROM    SystemItemMap
            WHERE   SystemItemId = SourceSystemItemId AND
		            map.MappingProjectId = @MappingProjectId
		    ORDER BY UpdateDate DESC)
WHERE (ItemTypeId = 4 or ItemTypeId = 5);

SELECT DomainName as GroupName,
        COUNT(*) as Count,
        CONVERT(nvarchar(50), DomainId) as Filter
FROM #TEMP
WHERE WorkflowStatusTypeId = 1 OR WorkflowStatusTypeId = 0
GROUP BY DomainId, DomainName;

SELECT ISNULL(WorkflowStatusTypeName, 'Unmapped') as GroupName,
        COUNT(*) as Count,
        CONVERT(nvarchar(50), t.WorkflowStatusTypeId) as Filter
FROM #TEMP as t
LEFT JOIN WorkflowStatusType as wst on wst.WorkflowStatusTypeId = t.WorkflowStatusTypeId
GROUP BY t.WorkflowStatusTypeId, wst.WorkflowStatusTypeName
END