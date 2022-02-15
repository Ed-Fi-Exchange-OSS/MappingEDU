-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[DataStandardExportEnumerationItems]
@DataStandardId uniqueidentifier
AS

DECLARE @ColumnName NVARCHAR(MAX)
DECLARE @DynamicColumnName NVARCHAR(MAX)
Declare @DynamicPivotQuery1 as NVARCHAR(MAX)
Declare @DynamicPivotQuery2 as NVARCHAR(MAX)
Declare @DynamicPivotQuery3 as NVARCHAR(MAX)
Declare @DynamicPivotQuery4 as NVARCHAR(MAX)
Declare @DynamicPivotQuery as NVARCHAR(MAX)

SELECT @ColumnName= ISNULL(@ColumnName + ',','') + QUOTENAME(DisplayName),
	   @DynamicColumnName  = ISNULL(@DynamicColumnName + ',','') + 'cd.' + QUOTENAME(DisplayName)
FROM (SELECT DISTINCT DisplayName FROM CustomDetailMetadata WHERE MappedSystemId = @DataStandardId) as CustomDetails

SELECT scid.SystemItemId, scid.Value, cdm.DisplayName
INTO #TempItemCustomDetails
FROM SystemItemCustomDetail scid
JOIN CustomDetailMetadata cdm on cdm.CustomDetailMetadataId = scid.CustomDetailMetadataId
WHERE cdm.MappedSystemId = @DataStandardId

SET @DynamicPivotQuery1 = N'SELECT SystemItemId, '

Set @DynamicPivotQuery2 = N'
	INTO #CustomDetails
	FROM #TempItemCustomDetails
	PIVOT(MAX(Value) FOR DisplayName in ('

SET @DynamicPivotQuery3 = N'' 
		+ 
			CASE WHEN @ColumnName IS NOT NULL THEN ' )) as PVT; '
			        ELSE '' END
		+
		'SELECT eg.ItemName as ElementGroup,
				si.ItemName,
				sei.CodeValue,
				sei.Description,
				sei.ShortDescription'
			    +
					CASE WHEN @ColumnName IS NOT NULL THEN ', '
				    ELSE '' END

SET @DynamicPivotQuery4 = N'
		FROM SystemEnumerationItem sei
		JOIN SystemItem si on si.SystemItemId = sei.SystemItemId '
			+
			    CASE WHEN @ColumnName IS NOT NULL THEN ' LEFT JOIN #CustomDetails cd on si.SystemItemId = cd.SystemItemId '
				    ELSE '' END
		    +
		'JOIN SystemItem eg on eg.SystemItemId = si.ElementGroupSystemItemId
		WHERE si.ItemTypeId = 5 AND si.MappedSystemId = ''' +  CONVERT(nvarchar(50), @DataStandardId) + ''' AND 
				eg.IsActive = 1 AND si.IsActive = 1 AND si.MappedSystemExtensionId is NULL
		ORDER BY eg.ItemName, si.ItemName, sei.CodeValue;'

SET @DynamicPivotQuery = CASE WHEN @ColumnName IS NOT NULL THEN CAST(@DynamicPivotQuery1 as nvarchar(MAX)) + 
						           						    CAST(@ColumnName as nvarchar(MAX)) + 
						           						    CAST(@DynamicPivotQuery2 as nvarchar(MAX)) +
						           						    CAST(@ColumnName as nvarchar(MAX)) + 
						           						    CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
						          						    CAST(@DynamicColumnName as nvarchar(MAX)) +
						          						    CAST(@DynamicPivotQuery4 as nvarchar(MAX))
					            ELSE CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
							        CAST(@DynamicPivotQuery4 as nvarchar(MAX)) END

EXEC sp_executesql @DynamicPivotQuery
DROP TABLE #TempItemCustomDetails