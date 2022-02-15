-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[ExtensionReport]
@DataStandardId uniqueidentifier,
@ParentSystemItemId uniqueidentifier = '00000000-0000-0000-0000-000000000000'
AS
BEGIN

DECLARE @ColumnName nvarchar(max)
SELECT @ColumnName= ISNULL(@ColumnName + ',','') + QUOTENAME(ShortName)
FROM (SELECT DISTINCT ShortName FROM MappedSystemExtension WHERE MappedSystemId = @DataStandardId) as CustomDetails

Declare @DynamicPivotQuery NVARCHAR(MAX) = N'
	DECLARE @DataStandardId uniqueidentifier = ''' + CAST(@DataStandardId as nvarchar(50)) + '''
	DECLARE @ParentSystemItemId uniqueidentifier = ''' + CAST(@ParentSystemItemId as nvarchar(50)) + '''

	BEGIN
	WITH result AS (
		SELECT SystemItemId as EntityId,
				ItemName as EntityName,
				ItemTypeId as EntityItemTypeId,
				MappedSystemExtensionId as EntityMappedSystemExtensionId ,
				ItemTypeId,
				MappedSystemId,
				ParentSystemItemId,
				SystemItemId,
				MappedSystemExtensionId
		FROM dbo.SystemItem
		WHERE IsActive = 1 AND 
			((@ParentSystemItemId = ''00000000-0000-0000-0000-000000000000'' AND ParentSystemItemId is NULL) OR 
			(ParentSystemItemId = @ParentSystemItemId)) and [MappedSystemId] = @DataStandardId
		UNION all
		SELECT result.EntityId,
				result.EntityName,
				result.EntityItemTypeId,
				result.EntityMappedSystemExtensionId,
				i2.ItemTypeId,
				i2.MappedSystemId,
				i2.ParentSystemItemId,
				i2.SystemItemId,
				i2.MappedSystemExtensionId
		FROM SystemItem AS i2
			inner join result
				ON result.SystemItemId = i2.ParentSystemItemId AND i2.IsActive = 1)

	SELECT si.ItemName,
	       si.SystemItemId,
		   si.ItemTypeId,
		   pvt.ExtensionShortName as ShortName,
		   pvt.MappedSystemExtensionId,' 
	+ @ColumnName + 
	'   INTO #TEMP
	    FROM (SELECT EntityName as ItemName,
			     EntityId as SystemItemId,
	             EntityItemTypeId as ItemTypeId,
				 emse.MappedSystemExtensionId,
				 emse.ShortName as ExtensionShortName,
	             mse.ShortName as ShortName,
	             COUNT(si.SystemItemId) as Total
	      FROM result si
	      JOIN MappedSystemExtension mse
	        ON mse.MappedSystemExtensionId = si.MappedSystemExtensionId
		  LEFT JOIN MappedSystemExtension emse
	        ON emse.MappedSystemExtensionId = si.EntityMappedSystemExtensionId
	      WHERE (ItemTypeId = 4 or ItemTypeId = 5)
	      GROUP BY EntityId, EntityName, EntityItemTypeId, emse.MappedSystemExtensionId, emse.ShortName, mse.MappedSystemExtensionId, mse.ShortName) as ext
	PIVOT(SUM(ext.Total) for ext.ShortName in (' + @ColumnName + ')) as pvt
	RIGHT JOIN SystemItem si
	  ON si.SystemItemId = pvt.SystemItemId
	WHERE ((@ParentSystemItemId = ''00000000-0000-0000-0000-000000000000'' AND si.ParentSystemItemId is NULL) OR 
		   (si.ParentSystemItemId = @ParentSystemItemId)) and si.MappedSystemId = @DataStandardId AND si.IsActive = 1;

	SELECT * 
	FROM #TEMP 
	ORDER BY ItemName, ShortName;
	END'

EXEC sp_executesql @DynamicPivotQuery;
END