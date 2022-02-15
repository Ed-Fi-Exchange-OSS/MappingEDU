-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[MappingProjectReportTargetEnumerationItemMappings]
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
	'
			
	SELECT  TargetElementGroup.ItemName as TargetElementGroup,
			TargetSystemItem.ItemName as TargetItemName,
			SourceSystemItem.ItemName as SourceItemName,
			SourceElementGroup.ItemName as SourceElementGroup,
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
	FROM SystemItem as TargetSystemItem '
	+  
		CASE WHEN @IncludeCustomDetails = 1 THEN 'LEFT JOIN #CustomDetails cd on cd.SystemItemId = TargetSystemItem.SystemItemId'
		ELSE '' END
	+
	' JOIN [dbo].[SystemEnumerationItem] as TargetSystemEnumerationItem on 
		TargetSystemItem.SystemItemId = TargetSystemEnumerationItem.SystemItemId
				 
		JOIN [dbo].[SystemItem] TargetElementGroup on
		TargetSystemItem.ElementGroupSystemItemId = TargetElementGroup.SystemItemId

		LEFT JOIN (
		SELECT SystemEnumerationItemMap.SystemEnumerationItemMapId,
				SystemEnumerationItemMap.SourceSystemEnumerationItemId,
				SystemEnumerationItemMap.TargetSystemEnumerationItemId,
				SystemEnumerationItemMap.DeferredMapping,
				SystemEnumerationItemMap.EnumerationMappingStatusTypeId,
				SystemEnumerationItemMap.EnumerationMappingStatusReasonTypeId,
				SystemEnumerationItemMap.CreateDate,
				SystemEnumerationItemMap.UpdateDate,
				SystemEnumerationItemMap.SystemItemMapId,
				SystemEnumerationItemMap.CreateBy,
				SystemEnumerationItemMap.UpdateBy,
				SystemEnumerationItemMap.CreateById,
				SystemEnumerationItemMap.UpdateById
		FROM [dbo].[SystemEnumerationItemMap] SystemEnumerationItemMap
		JOIN [dbo].[SystemItemMap] SystemItemMap on
			SystemItemMap.SystemItemMapId = SystemEnumerationItemMap.SystemItemMapId
		WHERE SystemItemMap.MappingProjectId = ''' + CAST(@MappingProjectId as nvarchar(36)) + '''
	) as SystemEnumerationItemMap on
		SystemEnumerationItemMap.TargetSystemEnumerationItemId = TargetSystemEnumerationItem.SystemEnumerationItemId

	LEFT JOIN [dbo].[SystemEnumerationItem] as SourceSystemEnumerationItem on 
		SystemEnumerationItemMap.SourceSystemEnumerationItemId = SourceSystemEnumerationItem.SystemEnumerationItemId

	LEFT JOIN AspNetUsers creater on SystemEnumerationItemMap.CreateById = creater.Id
	LEFT JOIN AspNetUsers updater on SystemEnumerationItemMap.UpdateById = updater.Id

	LEFT JOIN SystemItem SourceSystemItem on
			SourceSystemEnumerationItem.SystemItemId = SourceSystemItem.SystemItemId
	LEFT JOIN SystemItem SourceElementGroup on
			SourceSystemItem.ElementGroupSystemItemId = SourceElementGroup.SystemItemId
				  
	WHERE TargetSystemItem.ItemTypeId = 5 AND
	TargetElementGroup.IsActive = 1 AND TargetSystemItem.IsActive = 1 AND
	TargetSystemItem.MappedSystemExtensionId is NULL AND
	SourceSystemItem.MappedSystemExtensionId is NULL AND
	TargetSystemItem.MappedSystemId = ''' + CAST(@TargetDataStandardId as nvarchar(36)) +++ ''' AND
	(''' + @EnumerationStatuses + ''' = ''All'' OR + ''' + @EnumerationStatuses + ''' like ''%;''+ cast(ISNULL(SystemEnumerationItemMap.EnumerationMappingStatusTypeId, CASE WHEN SystemEnumerationItemMap.SystemEnumerationItemMapId is NULL THEN 0 ELSE -1 END) as varchar(20))+'';%'') AND
	(''' + @EnumerationStatusReasons + ''' = ''All'' OR + ''' + @EnumerationStatusReasons + ''' like ''%;''+cast(ISNULL(SystemEnumerationItemMap.EnumerationMappingStatusReasonTypeId, CASE WHEN SystemEnumerationItemMap.SystemEnumerationItemMapId is NULL THEN 0 ELSE -1 END) as varchar(20))+'';%'')
	ORDER BY TargetElementGroup.ItemName, TargetSystemItem.ItemName, TargetSystemEnumerationItem.CodeValue, SourceSystemItem.DomainItemPath;';


SET @DynamicPivotQuery = CASE WHEN @IncludeCustomDetails = 1 THEN CAST(@DynamicPivotQuery1 as nvarchar(MAX)) + 
															    CAST(@ColumnName as nvarchar(MAX)) + 
															    CAST(@DynamicPivotQuery2 as nvarchar(MAX)) +
															    CAST(@ColumnName as nvarchar(MAX)) + 
															    CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
															    CAST(@ColumnName as nvarchar(MAX)) +
							  								    CAST(@DynamicPivotQuery4 as nvarchar(MAX))
							    ELSE CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
								    CAST(@DynamicPivotQuery4 as nvarchar(MAX)) END

EXEC sp_executesql @DynamicPivotQuery
IF @IncludeCustomDetails = 1 
BEGIN 
	DROP TABLE #TempItemCustomDetails 
END