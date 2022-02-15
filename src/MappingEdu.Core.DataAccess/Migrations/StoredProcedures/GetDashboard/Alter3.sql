-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetDashboard]
@MappingProjectId uniqueidentifier
AS
BEGIN

Declare @SourceDataStandardId uniqueidentifier

SELECT @SourceDataStandardId = SourceDataStandardMappedSystemId
FROM MappingProject
WHERE MappingProjectId = @MappingProjectId

SELECT eg.ItemName as DomainName,
	    si.SystemItemId,
	    si.ItemTypeId,
	    si.ElementGroupSystemItemId,
	    ISNULL(map.WorkflowStatusTypeId, 0) as WorkflowStatusTypeId
INTO #TEMP
FROM SystemItem si
JOIN SystemItem eg on si.ElementGroupSystemItemId = eg.SystemItemId
LEFT JOIN (SELECT * FROM SystemItemMap 
			WHERE MappingProjectId = @MappingProjectId) as map 
ON si.SystemItemId = map.SourceSystemItemId
WHERE (si.ItemTypeId = 4 or si.ItemTypeId = 5) AND
		(si.MappedSystemExtensionId is NULL) AND
		eg.IsActive = 1 AND si.IsActive = 1 AND
		si.MappedSystemId = @SourceDataStandardId;

SELECT DomainName as GroupName,
        COUNT(*) as Count,
        CONVERT(nvarchar(50), ElementGroupSystemItemId) as Filter
FROM #TEMP
WHERE WorkflowStatusTypeId = 1 OR WorkflowStatusTypeId = 0
GROUP BY ElementGroupSystemItemId, DomainName;

SELECT ISNULL(WorkflowStatusTypeName, 'Unmapped') as GroupName,
        COUNT(*) as Count,
        CONVERT(nvarchar(50), t.WorkflowStatusTypeId) as Filter
FROM #TEMP as t
LEFT JOIN WorkflowStatusType as wst on wst.WorkflowStatusTypeId = t.WorkflowStatusTypeId
GROUP BY t.WorkflowStatusTypeId, wst.WorkflowStatusTypeName
END