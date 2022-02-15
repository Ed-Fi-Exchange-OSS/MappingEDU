-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[GetElementListForDeltaPage]
@MappedSystemId uniqueidentifier,
@SearchText varchar(MAX),
@ItemTypeIds varchar(MAX),
@DomainIds varchar(MAX),
@Start int,
@Take int,
@SortDirection varchar(20),
@OrderBy int
AS
BEGIN

with result as (
SELECT CAST(ItemName AS NVARCHAR(4000)) AS DomainItemPath,
	    CAST(SystemItemId AS NVARCHAR(4000)) as DomainItemPathIds,
	    ItemTypeId,
	    ParentSystemItemId,
	    SystemItemId
FROM dbo.SystemItem
WHERE ItemTypeId = 1 AND MappedSystemId = @MappedSystemId AND
		(@DomainIds = 'All' OR @DomainIds like '%;'+cast(SystemItemId as varchar(50))+';%') AND
		IsActive = 1
UNION all 
SELECT result.DomainItemPath + ';' + i2.ItemName,
        result.DomainItemPathIds + ';' + CAST(i2.SystemItemId AS VARCHAR(50)),
	    i2.ItemTypeId,
	    i2.ParentSystemItemId,
	    i2.SystemItemId
FROM SystemItem AS i2
inner join result
    ON result.SystemItemId = i2.ParentSystemItemId
WHERE i2.IsActive = 1 AND i2.ItemTypeId != 1 AND i2.MappedSystemId = @MappedSystemId)

SELECT si.DomainItemPath,
		si.DomainItemPathIds,
		it.ItemTypeName as ItemType,
		si.SystemItemId
INTO #TEMP
FROM result as si
LEFT JOIN ItemType as it on si.ItemTypeId = it.ItemTypeId
WHERE (@ItemTypeIds = 'All' OR @ItemTypeIds like '%;'+cast(si.ItemTypeId as varchar(50))+';%') AND
		(si.DomainItemPath like '%' + @SearchText + '%' OR it.ItemTypeName like '%' + @SearchText + '%');
		    
SELECT DomainItemPath,
		DomainItemPathIds,
		ItemType,
		SystemItemId
FROM #TEMP
ORDER BY CASE WHEN @SortDirection = 'asc' AND @OrderBy = 0 THEN DomainItemPath END ASC,
			CASE WHEN @SortDirection = 'asc' AND @OrderBy = 1 THEN ItemType END ASC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 0 THEN DomainItemPath END DESC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 1 THEN ItemType END DESC
OFFSET @Start ROW
FETCH NEXT @Take ROWS ONLY;

SELECT (Select COUNT(*) FROM #TEMP) as Filtered,
		(Select COUNT(*) FROM SystemItem WHERE MappedSystemId = @MappedSystemId) as Total;
END