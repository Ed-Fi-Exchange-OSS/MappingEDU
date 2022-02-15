-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[MappingProjectReportTargetEnumerationItemMappings]
@MappingProjectId uniqueidentifier,
@EnumerationStatuses nvarchar(max),
@EnumerationStatusReasons nvarchar(max),
@IncludeCustomDetails bit
AS

Declare @ColumnName NVARCHAR(MAX)
Declare @SourceDataStandardId uniqueidentifier
Declare @TargetDataStandardId uniqueidentifier
Declare @DynamicPivotQuery1 as NVARCHAR(MAX)
Declare @DynamicPivotQuery2 as NVARCHAR(MAX)
Declare @DynamicPivotQuery3 as NVARCHAR(MAX)
Declare @DynamicPivotQuery4 as NVARCHAR(MAX)
Declare @DynamicPivotQuery5 as NVARCHAR(MAX)
Declare @DynamicPivotQuery as NVARCHAR(MAX)

SELECT @SourceDataStandardId = SourceDataStandardMappedSystemId,
	    @TargetDataStandardId = TargetDataStandardMappedSystemId
FROM dbo.MappingProject WHERE MappingProjectId = @MappingProjectId;

IF @IncludeCustomDetails = 1
BEGIN
	SELECT @ColumnName= ISNULL(@ColumnName + ',','') + QUOTENAME(DisplayName)
	FROM (SELECT DISTINCT DisplayName FROM CustomDetailMetadata WHERE MappedSystemId = @TargetDataStandardId) as CustomDetails

	SELECT scid.SystemItemId, scid.Value, cdm.DisplayName
	INTO #TempItemCustomDetails
	FROM SystemItemCustomDetail scid
	JOIN CustomDetailMetadata cdm on cdm.CustomDetailMetadataId = scid.CustomDetailMetadataId
	WHERE cdm.MappedSystemId = @TargetDataStandardId
END

SET @DynamicPivotQuery1 = N'SELECT SystemItemId, '

Set @DynamicPivotQuery2 = N'
	INTO #CustomDetails
	FROM #TempItemCustomDetails
	PIVOT(MAX(Value) FOR DisplayName in ('

SET @DynamicPivotQuery3 = N''
		+  
				CASE WHEN @IncludeCustomDetails = 1 THEN ')) as PVT; '
				ELSE '' END
		+
	'	WITH result as (
		SELECT ItemName as ElementGroup,
				ItemName,
				ItemTypeId,
				SystemItemId
		FROM dbo.SystemItem
		WHERE ParentSystemItemId is null AND 
				[MappedSystemId] = ''' + CONVERT(nvarchar(50), @TargetDataStandardId) + ''' AND 
				IsActive = 1
		UNION all
		SELECT result.ElementGroup,
				i2.ItemName,
				i2.ItemTypeId,
				i2.SystemItemId
		FROM SystemItem AS i2
		inner join result
			ON result.SystemItemId = i2.ParentSystemItemId
		WHERE i2.ItemTypeId = 5)
			
	SELECT  SystemItem.ElementGroup,
			SystemItem.ItemName,
			SourceSystemEnumerationItem.SystemItemId as SourceSystemItemId,
			SourceSystemEnumerationItem.SystemEnumerationItemId as SourceSystemEnumerationItemId,
			SourceSystemEnumerationItem.CodeValue as SourceCodeValue,
			SourceSystemEnumerationItem.ShortDescription as SourceShortDescription,
			SourceSystemEnumerationItem.Description as SourceDescription,
			SystemEnumerationItemMap.SystemEnumerationItemMapId,
			SystemEnumerationItemMap.EnumerationMappingStatusTypeId,
			SystemEnumerationItemMap.EnumerationMappingStatusReasonTypeId,
			TargetSystemEnumerationItem.CodeValue as TargetCodeValue,
			TargetSystemEnumerationItem.ShortDescription as TargetShortDescription,
			TargetSystemEnumerationItem.Description as TargetDescription,
			TargetSystemEnumerationItem.SystemEnumerationItemId as TargetSystemEnumerationItemId,
			updater.FirstName + '' '' + updater.LastName as UpdatedBy,
			SystemEnumerationItemMap.UpdateDate as UpdateDate,
			creater.FirstName + '' '' + creater.LastName as CreatedBy,
			SystemEnumerationItemMap.CreateDate as CreateDate' +  
			CASE WHEN @IncludeCustomDetails = 1 THEN ',' ELSE '' END

SET @DynamicPivotQuery4 = N'
	INTO #TEMP
	FROM result as systemItem '
	+  
		CASE WHEN @IncludeCustomDetails = 1 THEN 'LEFT JOIN #CustomDetails cd on cd.SystemItemId = systemItem.SystemItemId'
		ELSE '' END
	+
	' JOIN [dbo].[SystemEnumerationItem] as TargetSystemEnumerationItem on 
		SystemItem.SystemItemId = TargetSystemEnumerationItem.SystemItemId
	LEFT JOIN [dbo].[SystemEnumerationItemMap] as SystemEnumerationItemMap on
		SystemEnumerationItemMap.TargetSystemEnumerationItemId = TargetSystemEnumerationItem.SystemEnumerationItemId
	LEFT JOIN [dbo].[SystemEnumerationItem] as SourceSystemEnumerationItem on 
		SystemEnumerationItemMap.SourceSystemEnumerationItemId = SourceSystemEnumerationItem.SystemEnumerationItemId

	LEFT JOIN AspNetUsers creater on SystemEnumerationItemMap.CreateById = creater.Id
	LEFT JOIN AspNetUsers updater on SystemEnumerationItemMap.UpdateById = updater.Id
	WHERE systemItem.ItemTypeId = 5 AND
	(''' + @EnumerationStatuses + ''' = ''All'' OR + ''' + @EnumerationStatuses + ''' like ''%;''+ cast(ISNULL(SystemEnumerationItemMap.EnumerationMappingStatusTypeId, CASE WHEN SystemEnumerationItemMap.SystemEnumerationItemMapId is NULL THEN 0 ELSE -1 END) as varchar(20))+'';%'') AND
	(''' + @EnumerationStatusReasons + ''' = ''All'' OR + ''' + @EnumerationStatusReasons + ''' like ''%;''+cast(ISNULL(SystemEnumerationItemMap.EnumerationMappingStatusReasonTypeId, CASE WHEN SystemEnumerationItemMap.SystemEnumerationItemMapId is NULL THEN 0 ELSE -1 END) as varchar(20))+'';%'');

	WITH result as (
			SELECT ItemName as ElementGroup,
					ItemName,
					ItemTypeId,
					SystemItemId
			FROM dbo.SystemItem
			WHERE ParentSystemItemId is null AND 
					[MappedSystemId] = ''' +  CONVERT(nvarchar(50), @SourceDataStandardId) + ''' AND 
					IsActive = 1
			UNION all
			SELECT result.ElementGroup,
					i2.ItemName,
					i2.ItemTypeId,
					i2.SystemItemId
			FROM SystemItem AS i2
			inner join result
				ON result.SystemItemId = i2.ParentSystemItemId
			WHERE i2.ItemTypeId = 5)

	SELECT tsi.ElementGroup as TargetElementGroup,
		    tsi.ItemName as TargetItemName,
		    tsi.SourceCodeValue,
		    tsi.SourceShortDescription,
		    tsi.SourceDescription,
		    tsi.SystemEnumerationItemMapId as SystemEnumerationItemMapId,
		    tsi.EnumerationMappingStatusTypeId,
		    tsi.EnumerationMappingStatusReasonTypeId,
		    tsi.TargetCodeValue,
		    tsi.TargetShortDescription,
		    tsi.TargetDescription,
		    si.ItemName as SourceItemName,
		    si.ElementGroup as SourceElementGroup,
		    tsi.CreatedBy,
		    tsi.CreateDate,
		    tsi.UpdatedBy,
		    tsi.UpdateDate' + 
		    CASE WHEN @IncludeCustomDetails = 1 THEN ',' ELSE '' END

SET @DynamicPivotQuery5 = N'
		FROM #TEMP tsi
		LEFT JOIN result si on si.SystemItemId = tsi.SourceSystemItemId
		ORDER BY tsi.ElementGroup, tsi.ItemName, TargetCodeValue;
		DROP TABLE #TEMP;'

SET @DynamicPivotQuery = CASE WHEN @IncludeCustomDetails = 1 THEN CAST(@DynamicPivotQuery1 as nvarchar(MAX)) + 
															    CAST(@ColumnName as nvarchar(MAX)) + 
															    CAST(@DynamicPivotQuery2 as nvarchar(MAX)) +
															    CAST(@ColumnName as nvarchar(MAX)) + 
															    CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
															    CAST(@ColumnName as nvarchar(MAX)) +
							  								    CAST(@DynamicPivotQuery4 as nvarchar(MAX)) + 
							 								    CAST(@ColumnName as nvarchar(MAX)) +
							 								    CAST(@DynamicPivotQuery5 as nvarchar(MAX))
							    ELSE CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
								    CAST(@DynamicPivotQuery4 as nvarchar(MAX)) + 
								    CAST(@DynamicPivotQuery5 as nvarchar(MAX)) END

EXEC sp_executesql @DynamicPivotQuery
IF @IncludeCustomDetails = 1 
BEGIN 
	DROP TABLE #TempItemCustomDetails 
END