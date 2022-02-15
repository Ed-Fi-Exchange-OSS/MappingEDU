-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetMatchmakerSearchPage]
@MappedSystemId uniqueidentifier,
@SearchText varchar(MAX),
@ItemTypeIds varchar(MAX),
@ItemDataTypeIds varchar(MAX),
@DomainIds varchar(MAX),
@EntityIds varchar(MAX),
@Start int,
@Take varchar(20),
@SortDirection varchar(20),
@OrderBy int,
@IsExtended bit = null

AS
BEGIN

with result as (
SELECT Definition,
	    SystemItemId as DomainId,
		CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
		FieldLength,
		IsExtended,
		ItemDataTypeId,
		ItemName,
	    ItemTypeId,
		0 as Level,
	    MappedSystemId,
	    ParentSystemItemId,
	    SystemItemId
FROM dbo.SystemItem
WHERE ItemTypeId = 1 AND MappedSystemId = @MappedSystemId AND
		(@DomainIds = 'All' OR @DomainIds like '%;'+cast(SystemItemId as varchar(50))+';%') AND
		IsActive = 1
UNION all 
SELECT i2.Definition,
		result.DomainId,
		result.DomainItemPath + '.' + i2.ItemName,
		i2.FieldLength,
		i2.IsExtended,
		i2.ItemDataTypeId,
		i2.ItemName,
	    i2.ItemTypeId,
		result.Level + 1 as Level,
	    i2.MappedSystemId,
	    i2.ParentSystemItemId,
	    i2.SystemItemId
FROM SystemItem AS i2
inner join result
    ON result.SystemItemId = i2.ParentSystemItemId
WHERE ((result.Level = 0 AND (@EntityIds = 'All' OR @EntityIds like '%;'+cast(i2.SystemItemId as varchar(50))+';%')) OR result.Level != 0))
				
SELECT si.Definition,
		si.DomainItemPath,
		si.FieldLength,
		si.IsExtended,
		si.ItemDataTypeId,
		idt.ItemDataTypeName as ItemDataType,
		it.ItemTypeName,
		si.ItemName,
		si.ItemTypeId,
		si.SystemItemId
INTO #TEMP
FROM result as si
LEFT JOIN ItemDataType as idt on si.ItemDataTypeId = idt.ItemDataTypeId
LEFT JOIN ItemType as it on it.ItemTypeId = si.ItemTypeId
WHERE (@IsExtended is null OR si.IsExtended = @IsExtended) AND
		(@ItemTypeIds = 'All' OR @ItemTypeIds like '%;'+cast(si.ItemTypeId as varchar(20))+';%') AND
		(@ItemDataTypeIds = 'All' OR @ItemDataTypeIds like '%;'+cast(si.ItemDataTypeId as varchar(20))+';%') AND
		(si.DomainItemPath like '%' + @SearchText + '%' OR 
			si.Definition like '%' + @SearchText + '%' OR
			it.ItemTypeName like '%' + @SearchText + '%' OR
			idt.ItemDataTypeName like '%' + @SearchText + '%' OR
			si.FieldLength like '%' + @SearchText + '%');
		    
SELECT Definition,
		DomainItemPath,
		FieldLength,
		IsExtended,
		ItemDataTypeId,
		ItemTypeId,
		SystemItemId
FROM #TEMP
ORDER BY CASE WHEN @SortDirection = 'asc' AND @OrderBy = 0 THEN DomainItemPath END ASC,
			CASE WHEN @SortDirection = 'asc' AND @OrderBy = 1 THEN ItemTypeName END ASC,
			CASE WHEN @SortDirection = 'asc' AND @OrderBy = 2 THEN ItemDataType END ASC,
			CASE WHEN @SortDirection = 'asc' AND @OrderBy = 3 THEN FieldLength END ASC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 0 THEN DomainItemPath END DESC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 1 THEN ItemTypeName END DESC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 2 THEN ItemDataType END DESC,
			CASE WHEN @SortDirection = 'desc' AND @OrderBy = 3 THEN FieldLength END DESC
			 
OFFSET @Start ROW
FETCH NEXT CASE WHEN @Take = 'All' THEN (Select COUNT(*) FROM #TEMP)
			    ELSE CAST(@Take as int)
			END ROWS ONLY;

SELECT (Select COUNT(*) FROM #TEMP) as Filtered,
		(Select COUNT(*) FROM SystemItem WHERE MappedSystemId = @MappedSystemId AND (@ItemTypeIds = 'All' OR @ItemTypeIds like '%;'+cast(ItemTypeId as varchar(20))+';%')) as Total;
END;