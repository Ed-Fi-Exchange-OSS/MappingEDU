-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[UpdateAllPaths]
@MappedSystemIds dbo.UniqueIdentiferId READONLY

AS
BEGIN

with result as (
SELECT CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
	    CAST(SystemItemId AS NVARCHAR(MAX)) as DomainItemPathIds,
	    SystemItemId as ElementGroupId,
	    CAST(IsExtended AS NVARCHAR(MAX)) as IsExtendedPath,
	    ParentSystemItemId,
	    SystemItemId
FROM dbo.SystemItem
WHERE ParentSystemItemId IS NULL AND (MappedSystemId in (SELECT * FROM @MappedSystemIds))
UNION all 
SELECT result.DomainItemPath + '.' + i2.ItemName,
        result.DomainItemPathIds + '/' + CAST(i2.SystemItemId AS VARCHAR(50)),
	    result.ElementGroupId,
	    result.IsExtendedPath + '/' + CAST(i2.IsExtended AS VARCHAR(50)),
	    i2.ParentSystemItemId,
	    i2.SystemItemId
FROM SystemItem AS i2
inner join result
    ON result.SystemItemId = i2.ParentSystemItemId)

UPDATE si
SET si.DomainItemPath = paths.DomainItemPath,
    si.DomainItemPathIds = paths.DomainItemPathIds,
	si.IsExtendedPath = paths.IsExtendedPath,
	si.ElementGroupSystemItemId = paths.ElementGroupId
FROM SystemItem si
JOIN result paths on si.SystemItemId = paths.SystemItemId
END