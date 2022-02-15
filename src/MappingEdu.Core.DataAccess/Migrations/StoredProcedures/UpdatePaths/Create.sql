-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[UpdatePaths]
@SystemItemId uniqueidentifier

AS

Declare @ParentDomainItemPath NVARCHAR(MAX)
Declare @ParentDomainItemPathIds NVARCHAR(MAX)
Declare @ParentIsExtendedPath NVARCHAR(MAX)

SELECT @ParentDomainItemPath = parent.DomainItemPath,
	    @ParentDomainItemPathIds = parent.DomainItemPathIds,
	    @ParentIsExtendedPath = parent.IsExtendedPath
FROM SystemItem child
JOIN SystemItem parent on child.ParentSystemItemId = parent.SystemItemId
WHERE child.SystemItemId = @SystemItemId

BEGIN

with result as (
SELECT CASE WHEN @ParentDomainItemPath is NULL THEN CAST(ItemName as nvarchar(max))
	        ELSE @ParentDomainItemPath + '.' + ItemName END as DomainItemPath,
	    CASE WHEN @ParentDomainItemPathIds is NULL THEN CAST(SystemItemId as nvarchar(max))
	        ELSE @ParentDomainItemPathIds + '/' + CAST(SystemItemId as nvarchar(50)) END as DomainItemPathIds,
	    SystemItemId as ElementGroupId,
	    CASE WHEN @ParentIsExtendedPath is NULL THEN CAST(IsExtended as nvarchar(max))
	        ELSE @ParentIsExtendedPath + '/' + CAST(IsExtended as nvarchar(1)) END as IsExtendedPath,
	    ParentSystemItemId,
	    SystemItemId
FROM dbo.SystemItem
WHERE SystemItemId = @SystemItemId
UNION all 
SELECT  result.DomainItemPath + '.' + i2.ItemName,
        result.DomainItemPathIds + '/' + CAST(i2.SystemItemId AS VARCHAR(50)),
		result.ElementGroupId,
		result.IsExtendedPath + '/' + CAST(i2.IsExtended AS VARCHAR(1)),
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
JOIN result paths on si.SystemItemId = paths.SystemItemId;
END