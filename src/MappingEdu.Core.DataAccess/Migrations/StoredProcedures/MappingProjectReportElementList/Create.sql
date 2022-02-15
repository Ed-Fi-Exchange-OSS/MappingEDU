-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[MappingProjectReportElementList]
@DataStandardId uniqueidentifier
AS

Declare @ColumnName NVARCHAR(MAX)
Declare @DynamicPivotQuery1 as NVARCHAR(MAX)
Declare @DynamicPivotQuery2 as NVARCHAR(MAX)
Declare @DynamicPivotQuery3 as NVARCHAR(MAX)
Declare @DynamicPivotQuery4 as NVARCHAR(MAX)
Declare @DynamicPivotQuery as NVARCHAR(MAX)

SELECT @ColumnName= ISNULL(@ColumnName + ',','') + QUOTENAME(DisplayName)
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
			CASE WHEN @ColumnName is not NULL THEN ' )) as PVT; '
			        ELSE '' END
		+
		'WITH result as (
	    SELECT Definition,
                CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
	            SystemItemId as DomainId,
			    FieldLength,
			    ItemDataTypeId,
			    ItemName,
	            ItemTypeId,
	            MappedSystemId,
	            ParentSystemItemId,
	            SystemItemId
        FROM dbo.SystemItem
        WHERE ParentSystemItemId is null AND 
		        [MappedSystemId] = ''' + CONVERT(nvarchar(50), @DataStandardId) + ''' AND 
			    IsActive = 1
        UNION all
        SELECT i2.Definition,
                result.DomainItemPath + ''.'' + i2.ItemName,
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
            ON result.SystemItemId = i2.ParentSystemItemId)

		SELECT si.DomainItemPath, 
			    si.ItemName, 
			    si.FieldLength,
			    si.Definition,
			    idt.ItemDataTypeName as DataType'
			    +
					CASE WHEN @ColumnName is not NULL THEN ', '
				    ELSE '' END

SET @DynamicPivotQuery4 = N'
		FROM result si '
			+
			    CASE WHEN @ColumnName is not NULL THEN ' LEFT JOIN #CustomDetails cd on si.SystemItemId = cd.SystemItemId '
				    ELSE '' END
		    +
		'LEFT JOIN ItemDataType idt on si.ItemDataTypeId = idt.ItemDataTypeId
		
		WHERE si.ItemTypeId = 4
		ORDER BY DomainItemPath;'

SET @DynamicPivotQuery = CASE WHEN @ColumnName is not NULL THEN CAST(@DynamicPivotQuery1 as nvarchar(MAX)) + 
						           						    CAST(@ColumnName as nvarchar(MAX)) + 
						           						    CAST(@DynamicPivotQuery2 as nvarchar(MAX)) +
						           						    CAST(@ColumnName as nvarchar(MAX)) + 
						           						    CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
						          						    CAST(@ColumnName as nvarchar(MAX)) +
						          						    CAST(@DynamicPivotQuery4 as nvarchar(MAX))
					            ELSE CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
							        CAST(@DynamicPivotQuery4 as nvarchar(MAX)) END

EXEC sp_executesql @DynamicPivotQuery
DROP TABLE #TempItemCustomDetails 