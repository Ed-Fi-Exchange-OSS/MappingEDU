-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetElementListPage]
@MappedSystemId uniqueidentifier,
@SearchText varchar(MAX),
@ItemTypeIds dbo.IntId READONLY,
@DomainIds dbo.UniqueIdentiferId READONLY,
@MappedSystemExtensionIds dbo.UniqueIdentiferId READONLY,
@Start int,
@Take varchar(20),
@SortDirection varchar(20),
@OrderBy int,
@IsExtended bit = null

AS
BEGIN

DECLARE @NumberOfDomains int
DECLARE @NumberOfExtensions int
DECLARE @NumberOfItemTypes int

SELECT @NumberOfDomains = COUNT(*) FROM @DomainIds;
SELECT @NumberOfItemTypes = COUNT(*) FROM @ItemTypeIds;
SELECT @NumberOfExtensions = COUNT(*) FROM @MappedSystemExtensionIds

SELECT si.Definition,
		si.DomainItemPath,
		si.DomainItemPathIds,
		CASE WHEN mse.ShortName is NULL THEN 'Core'
		     ELSE mse.ShortName END as ExtensionShortName,
		si.FieldLength,
		si.IsExtended,
		si.IsExtendedPath,
		idt.ItemDataTypeName as ItemDataType,
		it.ItemTypeName,
		si.ItemName,
		si.ItemTypeId,
		si.SystemItemId
INTO #TEMP
FROM SystemItem as si
LEFT JOIN ItemDataType as idt on si.ItemDataTypeId = idt.ItemDataTypeId
LEFT JOIN ItemType as it on it.ItemTypeId = si.ItemTypeId
LEFT JOIN MappedSystemExtension as mse on si.MappedSystemExtensionId = mse.MappedSystemExtensionId
JOIN SystemItem as eg on si.ElementGroupSystemItemId = eg.SystemItemId
WHERE (si.MappedSystemId = @MappedSystemId AND si.IsActive = 1) AND
		((@NumberOfDomains = 0 OR (eg.SystemItemId in (SELECT * FROM @DomainIds))) AND eg.IsActive = 1) AND
		(@IsExtended is null OR si.IsExtended = @IsExtended) AND
		((@NumberOfItemTypes = 0 AND (si.ItemTypeId = 4 OR si.ItemTypeId = 5)) OR si.ItemTypeId in (SELECT Id FROM @ItemTypeIds)) AND
		((si.MappedSystemExtensionId is NULL AND @NumberOfExtensions = 0) OR (@NumberOfExtensions > 0 AND (si.MappedSystemExtensionId is NULL AND si.MappedSystemId in (SELECT Id FROM @MappedSystemExtensionIds)) OR si.MappedSystemExtensionId in (SELECT Id FROM @MappedSystemExtensionIds))) AND
		(si.DomainItemPath like '%' + @SearchText + '%' OR 
			si.Definition like '%' + @SearchText + '%' OR
			it.ItemTypeName like '%' + @SearchText + '%' OR
			idt.ItemDataTypeName like '%' + @SearchText + '%' OR
			si.FieldLength like '%' + @SearchText + '%' OR
			((si.MappedSystemExtensionId in (SELECT Id FROM @MappedSystemExtensionIds) AND mse.ShortName like '%' + @SearchText + '%') OR
			  (@NumberOfExtensions > 0 AND si.MappedSystemExtensionId is NULL AND si.MappedSystemId in (SELECT Id FROM @MappedSystemExtensionIds) AND 'Core' like '%' + @SearchText + '%')));
		    
SELECT Definition,
		DomainItemPath,
		DomainItemPathIds,
		ExtensionShortName,
		FieldLength,
		IsExtended,
		IsExtendedPath,
		ItemDataType,
		ItemTypeId,
		SystemItemId
FROM #TEMP
ORDER BY CASE WHEN @SortDirection = 'asc' AND @OrderBy = 0 THEN ExtensionShortName END ASC,
         CASE WHEN @SortDirection = 'asc' AND @OrderBy = 1 THEN DomainItemPath END ASC,
         CASE WHEN @SortDirection = 'asc' AND @OrderBy = 2 THEN ItemName END ASC,
	     CASE WHEN @SortDirection = 'asc' AND @OrderBy = 3 THEN ItemTypeName END ASC,
		 CASE WHEN @SortDirection = 'asc' AND @OrderBy = 4 THEN ItemDataType END ASC,
		 CASE WHEN @SortDirection = 'asc' AND @OrderBy = 5 THEN FieldLength END ASC,
         CASE WHEN @SortDirection = 'desc' AND @OrderBy = 0 THEN ExtensionShortName END DESC,
		 CASE WHEN @SortDirection = 'desc' AND @OrderBy = 1 THEN DomainItemPath END DESC,
		 CASE WHEN @SortDirection = 'desc' AND @OrderBy = 2 THEN ItemName END DESC,
		 CASE WHEN @SortDirection = 'desc' AND @OrderBy = 3 THEN ItemTypeName END DESC,
		 CASE WHEN @SortDirection = 'desc' AND @OrderBy = 4 THEN ItemDataType END DESC,
		 CASE WHEN @SortDirection = 'desc' AND @OrderBy = 5 THEN FieldLength END DESC
			 
OFFSET @Start ROW
FETCH NEXT CASE WHEN @Take = 'All' THEN (Select COUNT(*) FROM #TEMP)
			    ELSE CAST(@Take as int)
			END ROWS ONLY;

SELECT (Select COUNT(*) FROM #TEMP) as Filtered,
		(Select COUNT(*) 
		FROM SystemItem si 
		JOIN SystemItem eg on si.ElementGroupSystemItemId = eg.SystemItemId 
		WHERE eg.IsActive = 1 AND si.IsActive = 1
			AND si.MappedSystemId = @MappedSystemId 
			AND ((si.MappedSystemExtensionId is NULL AND @NumberOfExtensions = 0) OR (@NumberOfExtensions > 0 AND (si.MappedSystemExtensionId is NULL AND si.MappedSystemId in (SELECT Id FROM @MappedSystemExtensionIds)) OR si.MappedSystemExtensionId in (SELECT Id FROM @MappedSystemExtensionIds)))
			AND (si.ItemTypeId = 4 OR si.ItemTypeId = 5) ) as Total;
END