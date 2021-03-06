-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[MappingProjectReportSourceEnumerationItemMappings]
@MappingProjectId uniqueidentifier,
@EnumerationStatuses dbo.IntId readonly,
@EnumerationStatusReasons dbo.IntId readonly,
@IncludeCustomDetails bit
AS

Declare @ColumnName NVARCHAR(MAX)
Declare @DynamicColumnName NVARCHAR(MAX)
Declare @SourceDataStandardId uniqueidentifier
Declare @TargetDataStandardId uniqueidentifier
Declare @DynamicPivotQuery1 as NVARCHAR(MAX)
Declare @DynamicPivotQuery2 as NVARCHAR(MAX)
Declare @DynamicPivotQuery3 as NVARCHAR(MAX)
Declare @DynamicPivotQuery4 as NVARCHAR(MAX)
Declare @DynamicPivotQuery as NVARCHAR(MAX)
Declare @NumberOfEnumerationStatuses int
Declare @NumberOfEnumerationStatusReasons int

SELECT @SourceDataStandardId = SourceDataStandardMappedSystemId,
	    @TargetDataStandardId = TargetDataStandardMappedSystemId
FROM dbo.MappingProject WHERE MappingProjectId = @MappingProjectId;

SELECT @NumberOfEnumerationStatuses = COUNT(*) FROM @EnumerationStatuses;
SELECT @NumberOfEnumerationStatusReasons = COUNT(*) FROM @EnumerationStatusReasons;

IF @IncludeCustomDetails = 1
BEGIN
	SELECT @ColumnName= ISNULL(@ColumnName + ',','') + QUOTENAME(DisplayName),
	       @DynamicColumnName  = ISNULL(@DynamicColumnName + ',','') + 'cd.' + QUOTENAME(DisplayName)
	FROM (SELECT DISTINCT DisplayName FROM CustomDetailMetadata WHERE MappedSystemId = @SourceDataStandardId) as CustomDetails

	SELECT scid.SystemItemId, scid.Value, cdm.DisplayName
	INTO #TempItemCustomDetails
	FROM SystemItemCustomDetail scid
	JOIN CustomDetailMetadata cdm on cdm.CustomDetailMetadataId = scid.CustomDetailMetadataId
	WHERE cdm.MappedSystemId = @SourceDataStandardId
END

SET @DynamicPivotQuery1 = N'SELECT SystemItemId, '

Set @DynamicPivotQuery2 = N'
	INTO #CustomDetails
	FROM #TempItemCustomDetails
	PIVOT(MAX(Value) FOR DisplayName in ('

SET @DynamicPivotQuery3 = N''
		+  
				CASE WHEN (@IncludeCustomDetails = 1 AND @ColumnName IS NOT NULL) THEN ')) as PVT; '
				ELSE '' END
		+
	'	
	SELECT  elementGroup.ItemName as SourceElementGroup,
			SystemItem.ItemName as SourceItemName,
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
			TargetSystemEnumerationItem.SystemItemId as TargetSystemItemId,
			TargetSystemItem.ItemName as TargetItemName,
		    TargetElementGroup.ItemName as TargetElementGroup,
			updater.FirstName + '' '' + updater.LastName as UpdatedBy,
			SystemEnumerationItemMap.UpdateDate as UpdateDate,
			creater.FirstName + '' '' + creater.LastName as CreatedBy,
			SystemEnumerationItemMap.CreateDate as CreateDate' +  
			CASE WHEN (@IncludeCustomDetails = 1 AND @ColumnName IS NOT NULL) THEN ',' ELSE '' END

SET @DynamicPivotQuery4 = N'
	FROM SystemItem as systemItem '
	+  
		CASE WHEN (@IncludeCustomDetails = 1 AND @ColumnName IS NOT NULL) THEN 'LEFT JOIN #CustomDetails cd on cd.SystemItemId = systemItem.SystemItemId'
		ELSE '' END
	+
	' JOIN [dbo].[SystemEnumerationItem] as SourceSystemEnumerationItem on 
		SystemItem.SystemItemId = SourceSystemEnumerationItem.SystemItemId
	JOIN [dbo].[SystemItem] elementGroup on 
			elementGroup.SystemItemId = systemItem.ElementGroupSystemItemId

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
		SystemEnumerationItemMap.SourceSystemEnumerationItemId = SourceSystemEnumerationItem.SystemEnumerationItemId

	LEFT JOIN [dbo].[SystemEnumerationItem] as TargetSystemEnumerationItem on 
		SystemEnumerationItemMap.TargetSystemEnumerationItemId = TargetSystemEnumerationItem.SystemEnumerationItemId

	LEFT JOIN AspNetUsers creater on SystemEnumerationItemMap.CreateById = creater.Id
	LEFT JOIN AspNetUsers updater on SystemEnumerationItemMap.UpdateById = updater.Id

	LEFT JOIN SystemItem TargetSystemItem on TargetSystemItem.SystemItemId = TargetSystemEnumerationItem.SystemItemId
	LEFT JOIN SystemItem TargetElementGroup on TargetSystemItem.ElementGroupSystemItemId = TargetElementGroup.SystemItemId
	WHERE systemItem.ItemTypeId = 5 AND elementGroup.IsActive = 1 AND 
	systemItem.MappedSystemExtensionId is NULL AND
	TargetSystemItem.MappedSystemExtensionId is NULL AND
	systemItem.MappedSystemId = ''' + CAST(@SourceDataStandardId as nvarchar(36)) + ''' AND
	(@NumberOfEnumerationStatuses = 0 OR ISNULL(SystemEnumerationItemMap.EnumerationMappingStatusTypeId, CASE WHEN SystemEnumerationItemMap.SystemEnumerationItemMapId is NULL THEN 0 ELSE -1 END) in (SELECT Id FROM @EnumerationStatuses)) AND
    (@NumberOfEnumerationStatusReasons = 0 OR ISNULL(SystemEnumerationItemMap.EnumerationMappingStatusReasonTypeId, CASE WHEN SystemEnumerationItemMap.SystemEnumerationItemMapId is NULL THEN 0 ELSE -1 END) in (SELECT Id FROM @EnumerationStatusReasons))
	ORDER BY elementGroup.ItemName, systemItem.ItemName, SourceSystemEnumerationItem.CodeValue;'

DECLARE @Params nvarchar(1000) = N'
    @NumberOfEnumerationStatuses int,
	@EnumerationStatuses dbo.IntId READONLY,
	@NumberOfEnumerationStatusReasons int,
	@EnumerationStatusReasons dbo.IntId READONLY';

SET @DynamicPivotQuery = CASE WHEN (@IncludeCustomDetails = 1 AND @ColumnName IS NOT NULL) 
                                THEN CAST(@DynamicPivotQuery1 as nvarchar(MAX)) + 
									 CAST(@ColumnName as nvarchar(MAX)) + 
									 CAST(@DynamicPivotQuery2 as nvarchar(MAX)) +
									 CAST(@ColumnName as nvarchar(MAX)) + 
									 CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
									 CAST(@DynamicColumnName as nvarchar(MAX)) +
							  		 CAST(@DynamicPivotQuery4 as nvarchar(MAX))
							  ELSE CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
								   CAST(@DynamicPivotQuery4 as nvarchar(MAX)) END

EXEC sp_executesql @DynamicPivotQuery, @Params, @NumberOfEnumerationStatuses, @EnumerationStatuses, @NumberOfEnumerationStatusReasons, @EnumerationStatusReasons
IF @IncludeCustomDetails = 1 
BEGIN 
	DROP TABLE #TempItemCustomDetails 
END