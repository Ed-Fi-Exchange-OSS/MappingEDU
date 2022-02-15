-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[GetDashboardElementGroups]
@MappingProjectId uniqueidentifier
AS
BEGIN
WITH MappingProject as (
    SELECT SourceDataStandardMappedSystemId
    FROM dbo.MappingProject
    WHERE MappingProjectId = @MappingProjectId
), result AS (
SELECT SystemItemId as DomainId,
        ItemName as DomainName,
        ItemTypeId,
        MappedSystemId,
        ParentSystemItemId,
        SystemItemId
FROM dbo.SystemItem
JOIN MappingProject as mp on MappedSystemId = mp.SourceDataStandardMappedSystemId
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

SELECT DomainName as GroupName,
        COUNT(*) as Count,
        convert(nvarchar(50), DomainId) as Filter
FROM result
    LEFT JOIN 
    (SELECT sim.SourceSystemItemId,
        COUNT(*) as Maps
        FROM dbo.SystemItemMap as sim
        WHERE sim.MappingProjectId = @MappingProjectId AND
            sim.WorkflowStatusTypeId != 1
        GROUP BY sim.SourceSystemItemId) map on SystemItemId = map.SourceSystemItemId
WHERE map.Maps IS NULL AND (ItemTypeId = 4 OR ItemTypeId = 5)
GROUP BY DomainId, DomainName
END