-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[MappingProjectReportSourceElementMappings]
@MappingProjectId uniqueidentifier,
@MappingMethods dbo.IntId readonly,
@WorkflowStatuses dbo.IntId readonly,
@IncludeCustomDetails bit = 0
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
Declare @NumberOfMappingMethods int
Declare @NumberOfWorkflowStatuses int	

SELECT @SourceDataStandardId = SourceDataStandardMappedSystemId,
	    @TargetDataStandardId = TargetDataStandardMappedSystemId
FROM dbo.MappingProject WHERE MappingProjectId = @MappingProjectId

SELECT @NumberOfMappingMethods = COUNT(*) FROM @MappingMethods;
SELECT @NumberOfWorkflowStatuses = COUNT(*) FROM @WorkflowStatuses;

IF @IncludeCustomDetails = 1
BEGIN
	SELECT @ColumnName = ISNULL(@ColumnName + ',','') + QUOTENAME(DisplayName),
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
	'SELECT si.DomainItemPath, 
			si.ItemName,
			tsi.ItemName as TargetItemName,
			tsi.DomainItemPath as TargetDomainItemPath,
			map.MappingMethodTypeId,
			map.WorkflowStatusTypeId,
			map.BusinessLogic,
			map.OmissionReason,
			creater.FirstName + '' '' + creater.LastName as CreatedBy,
			map.CreateDate,
			updater.FirstName + '' '' + updater.LastName as UpdatedBy,
	        map.UpdateDate' +  
			CASE WHEN (@IncludeCustomDetails = 1 AND @ColumnName IS NOT NULL) THEN ',' ELSE '' END

SET @DynamicPivotQuery4 = N'
		FROM SystemItem si ' 
			    +  
				    CASE WHEN (@IncludeCustomDetails = 1 AND @ColumnName IS NOT NULL) THEN 'LEFT JOIN #CustomDetails cd on cd.SystemItemId = si.SystemItemId'
			        ELSE '' END
			    +
		' LEFT JOIN (SELECT sim.WorkflowStatusTypeId,
			                sim.MappingMethodTypeId,
						    sim.BusinessLogic,
						    sim.OmissionReason,
				            sim.CreateById,
				            sim.CreateDate,
				            sim.UpdateById,
				            sim.UpdateDate,
			                sim.SourceSystemItemId,
			                sim.SystemItemMapId,
                            sim.MappingProjectId,
						    si2.SystemItemId as TargetSystemItemId
	                FROM dbo.SystemItemMap sim
				    LEFT JOIN SystemItemMapAssociation sima on sima.SystemItemMapId = sim.SystemItemMapId
				    LEFT JOIN SystemItem si2 on si2.SystemItemId = sima.SystemItemId
	                WHERE sim.MappingProjectId = ''' + CONVERT(nvarchar(50), @MappingProjectId) + ''' ) map ON si.SystemItemId = map.SourceSystemItemId

		LEFT JOIN AspNetUsers creater on map.CreateById = creater.Id
        LEFT JOIN AspNetUsers updater on map.UpdateById = updater.Id

		LEFT JOIN SystemItem tsi on tsi.SystemItemId = map.TargetSystemItemId
		JOIN SystemItem eg on si.ElementGroupSystemItemId = eg.SystemItemId

		WHERE si.ItemTypeId = 4 AND si.MappedSystemId = '''  + CONVERT(nvarchar(50), @SourceDataStandardId) + ''' AND
				eg.IsActive = 1 and si.IsActive = 1 AND
				si.MappedSystemExtensionId is NULL AND
				(tsi.ItemTypeId is NULL OR tsi.ItemTypeId = 4) AND
			    (@NumberOfMappingMethods = 0 OR ISNULL(map.MappingMethodTypeId, 0) in (SELECT Id FROM @MappingMethods)) AND
			    (@NumberOfWorkflowStatuses = 0 OR ISNULL(map.WorkflowStatusTypeId, 0) in (SELECT Id FROM @WorkflowStatuses)) AND
				si.MappedSystemId = ''' + CONVERT(nvarchar(50), @SourceDataStandardId) + '''
		ORDER BY si.DomainItemPath;';

DECLARE @Params nvarchar(1000) = N'
@NumberOfMappingMethods int,
@MappingMethods dbo.IntId READONLY,
@NumberOfWorkflowStatuses int,
@WorkflowStatuses dbo.IntId READONLY';
		
SET @DynamicPivotQuery = CASE WHEN (@IncludeCustomDetails = 1 AND @ColumnName IS NOT NULL) 
						        THEN CAST(@DynamicPivotQuery1 as nvarchar(MAX)) + 
									 CAST(@ColumnName as nvarchar(MAX)) + 
									 CAST(@DynamicPivotQuery2 as nvarchar(MAX)) +
									 CAST(@ColumnName as nvarchar(MAX)) + 
									 CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
									 CAST(@DynamicColumnName as nvarchar(MAX)) +
							  		 CAST(@DynamicPivotQuery4 as nvarchar(MAX))
							  ELSE CAST(@DynamicPivotQuery3 as nvarchar(MAX)) +
								   CAST(@DynamicPivotQuery4 as nvarchar(MAX)) END;

EXEC sp_executesql @DynamicPivotQuery, @Params, @NumberOfMappingMethods, @MappingMethods, @NumberOfWorkflowStatuses, @WorkflowStatuses
IF @IncludeCustomDetails = 1 
BEGIN 
	DROP TABLE #TempItemCustomDetails 
END