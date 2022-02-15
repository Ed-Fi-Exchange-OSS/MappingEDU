-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetElementListPage]
@MappedSystemId uniqueidentifier,
@SearchText varchar(MAX),
@ItemTypeIds varchar(MAX),
@DomainIds varchar(MAX),
@Start int,
@Take varchar(20),
@SortDirection varchar(20),
@OrderBy int

AS
BEGIN

with result as (
SELECT Definition,
		CAST(ISNULL(Definition, ' ') AS NVARCHAR(MAX)) AS DefinitionPath,
		CAST(ItemName AS NVARCHAR(4000)) AS DomainItemPath,
	    CAST(SystemItemId AS NVARCHAR(4000)) as DomainItemPathIds,
	    SystemItemId as DomainId,
		FieldLength,
		ItemDataTypeId,
		ItemName,
	    ItemTypeId,
	    MappedSystemId,
	    ParentSystemItemId,
	    SystemItemId
FROM dbo.SystemItem
WHERE ItemTypeId = 1 AND MappedSystemId = @MappedSystemId AND
		(@DomainIds = 'All' OR @DomainIds like '%;'+cast(SystemItemId as varchar(50))+';%') AND
		IsActive = 1
UNION all 
SELECT i2.Definition,
		result.DefinitionPath + ';' + ISNULL(i2.Definition, ' '),
		result.DomainItemPath + '.' + i2.ItemName,
        result.DomainItemPathIds + ';' + CAST(i2.SystemItemId AS VARCHAR(50)),
	    result.DomainId,
		i2.FieldLength,
		i2.ItemDataTypeId,
		i2.ItemName,
	    i2.ItemTypeId,
	    i2.MappedSystemId,
	    i2.ParentSystemItemId,
	    i2.SystemItemId
FROM SystemItem AS i2
inner join result
    ON result.SystemItemId = i2.ParentSystemItemId
WHERE i2.IsActive = 1 AND i2.ItemTypeId != 1 AND i2.MappedSystemId = @MappedSystemId)

SELECT si.Definition,
		si.DefinitionPath,
		si.DomainItemPath,
		si.DomainItemPathIds,
		si.FieldLength,
		idt.ItemDataTypeName as ItemDataType,
		it.ItemTypeName,
		si.ItemName,
		si.ItemTypeId,
		si.SystemItemId
INTO #TEMP
FROM result as si
LEFT JOIN ItemDataType as idt on si.ItemDataTypeId = idt.ItemDataTypeId
LEFT JOIN ItemType as it on it.ItemTypeId = si.ItemTypeId
WHERE (@ItemTypeIds like '%;'+cast(si.ItemTypeId as varchar(20))+';%') AND
		(si.DomainItemPath like '%' + @SearchText + '%' OR 
			si.Definition like '%' + @SearchText + '%' OR
			it.ItemTypeName like '%' + @SearchText + '%' OR
			idt.ItemDataTypeName like '%' + @SearchText + '%' OR
			si.FieldLength like '%' + @SearchText + '%');
		    
SELECT DefinitionPath,
		DomainItemPath,
		DomainItemPathIds,
		FieldLength,
		ItemDataType,
		ItemTypeId,
		SystemItemId
FROM #TEMP
ORDER BY CASE WHEN @SortDirection = 'asc' AND @OrderBy = 0 THEN DomainItemPath END ASC,
			CASE WHEN @SortDirection = 'asc' AND @OrderBy = 1 THEN ItemName END ASC,
			CASE WHEN @SortDirection = 'asc' AND @OrderBy = 2 THEN ItemTypeName END ASC,
			CASE WHEN @SortDirection = 'asc' AND @OrderBy = 3 THEN ItemDataType END ASC,
			CASE WHEN @SortDirection = 'asc' AND @OrderBy = 4 THEN FieldLength END ASC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 0 THEN DomainItemPath END DESC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 1 THEN ItemName END DESC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 2 THEN ItemTypeName END DESC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 3 THEN ItemDataType END DESC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 4 THEN FieldLength END DESC
			 
OFFSET @Start ROW
FETCH NEXT CASE WHEN @Take = 'All' THEN (Select COUNT(*) FROM #TEMP)
			    ELSE CAST(@Take as int)
			END ROWS ONLY;

SELECT (Select COUNT(*) FROM #TEMP) as Filtered,
		(Select COUNT(*) FROM SystemItem WHERE MappedSystemId = @MappedSystemId AND (ItemTypeId = 4 OR ItemTypeId = 5) ) as Total;
END