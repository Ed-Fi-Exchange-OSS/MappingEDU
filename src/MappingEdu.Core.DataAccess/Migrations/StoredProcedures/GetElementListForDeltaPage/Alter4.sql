-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetElementListForDeltaPage]
@MappedSystemId uniqueidentifier,
@SearchText varchar(MAX),
@ItemTypeIds dbo.IntId READONLY,
@DomainIds dbo.UniqueIdentiferId READONLY,
@Start int,
@Take int,
@SortDirection varchar(20),
@OrderBy int
AS
BEGIN

DECLARE @NumberOfDomains int
DECLARE @NumberOfItemTypes int

SELECT @NumberOfDomains = COUNT(*) FROM @DomainIds;
SELECT @NumberOfItemTypes = COUNT(*) FROM @ItemTypeIds;

SELECT si.DomainItemPath,
		si.DomainItemPathIds,
		it.ItemTypeName as ItemType,
		si.SystemItemId
INTO #TEMP
FROM SystemItem as si
JOIN SystemItem eg on si.ElementGroupSystemItemId = eg.SystemItemId
LEFT JOIN ItemType as it on si.ItemTypeId = it.ItemTypeId
WHERE ((@NumberOfDomains = 0 OR eg.SystemItemId in (SELECT Id FROM @DomainIds)) AND eg.IsActive = 1) AND
        si.MappedSystemExtensionId is NULL AND
		(si.MappedSystemId = @MappedSystemId) AND (si.IsActive =1) AND
		(@NumberOfItemTypes = 0 OR si.ItemTypeId in (SELECT Id FROM @ItemTypeIds)) AND
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
		(Select COUNT(*) 
		FROM SystemItem si
		JOIN SystemItem eg on si.ElementGroupSystemItemId = eg.SystemItemId
		WHERE si.MappedSystemId = @MappedSystemId AND eg.IsActive = 1 AND si.IsActive = 1 AND si.MappedSystemExtensionId is NULL) as Total;
END