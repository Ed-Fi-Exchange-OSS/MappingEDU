-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[MappingProjectReportSourceElementMappings]
@MappingProjectId uniqueidentifier,
@MappingMethods nvarchar(50),
@WorkflowStatuses nvarchar(50),
@IncludeCustomDetails bit = 0
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
FROM dbo.MappingProject WHERE MappingProjectId = @MappingProjectId

IF @IncludeCustomDetails = 1
BEGIN
	SELECT @ColumnName= ISNULL(@ColumnName + ',','') + QUOTENAME(DisplayName)
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
				CASE WHEN @IncludeCustomDetails = 1 THEN ')) as PVT; '
				ELSE '' END
		+
	'WITH result as (
	    SELECT Definition,
                CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
	            SystemItemId as DomainId,
		        IsExtended,
			    ItemName,
	            ItemTypeId,
	            MappedSystemId,
	            ParentSystemItemId,
	            SystemItemId
        FROM dbo.SystemItem
        WHERE ParentSystemItemId is null AND 
		        [MappedSystemId] = ''' + CONVERT(nvarchar(50), @SourceDataStandardId) + ''' AND 
			    IsActive = 1
        UNION all
        SELECT i2.Definition,
                result.DomainItemPath + ''.'' + i2.ItemName,
	            result.DomainId,
		        i2.IsExtended,
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
			    map.TargetItemName,
			    map.TargetSystemItemId,map.MappingMethodTypeId,
			    map.WorkflowStatusTypeId,
			    map.BusinessLogic,
			    map.OmissionReason,
			    creater.FirstName + '' '' + creater.LastName as CreatedBy,
			    map.CreateDate,
			    updater.FirstName + '' '' + updater.LastName as UpdatedBy,
			    map.UpdateDate' +  
			    CASE WHEN @IncludeCustomDetails = 1 THEN ',' ELSE '' END

SET @DynamicPivotQuery4 = N'
		INTO #TEMP
		FROM result si ' 
			    +  
				    CASE WHEN @IncludeCustomDetails = 1 THEN 'LEFT JOIN #CustomDetails cd on cd.SystemItemId = si.SystemItemId'
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
						    si2.ItemName as TargetItemName,
						    si2.SystemItemId as TargetSystemItemId
	                FROM dbo.SystemItemMap sim
				    LEFT JOIN SystemItemMapAssociation sima on sima.SystemItemMapId = sim.SystemItemMapId
				    LEFT JOIN SystemItem si2 on si2.SystemItemId = sima.SystemItemId
	                WHERE sim.MappingProjectId = ''' + CONVERT(nvarchar(50), @MappingProjectId) + ''' ) map ON si.SystemItemId = map.SourceSystemItemId

		LEFT JOIN AspNetUsers creater on map.CreateById = creater.Id
        LEFT JOIN AspNetUsers updater on map.UpdateById = updater.Id

		WHERE si.ItemTypeId = 4 AND si.MappedSystemId = '''  + CONVERT(nvarchar(50), @SourceDataStandardId) + ''' AND
			    (''' + @MappingMethods + ''' = ''All'' OR + ''' + @MappingMethods + ''' like ''%;''+ cast(ISNULL(map.MappingMethodTypeId, 0) as varchar(20))+'';%'') AND
			    (''' + @WorkflowStatuses + ''' = ''All'' OR + ''' + @WorkflowStatuses + ''' like ''%;''+cast(ISNULL(map.WorkflowStatusTypeId, 0) as varchar(20))+'';%'');
		
		WITH result2 as (
			SELECT CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
				    ItemTypeId,
				    MappedSystemId,
				    ParentSystemItemId,
				    SystemItemId
			FROM dbo.SystemItem
			WHERE ParentSystemItemId is null AND 
				    [MappedSystemId] = ''' + CONVERT(nvarchar(50), @TargetDataStandardId) + ''' AND 
				    IsActive = 1
			UNION all
			SELECT  result2.DomainItemPath + ''.'' + i2.ItemName,
					i2.ItemTypeId,
					i2.MappedSystemId,
					i2.ParentSystemItemId,
					i2.SystemItemId
			FROM SystemItem AS i2
			inner join result2
				ON result2.SystemItemId = i2.ParentSystemItemId)

		SELECT si.DomainItemPath,
		        si.ItemName,
			    si.TargetItemName,
			    tsi.DomainItemPath as TargetDomainItemPath,
			    si.MappingMethodTypeId,
			    si.WorkflowStatusTypeId,
			    si.BusinessLogic,
			    si.OmissionReason,
			    si.CreatedBy,
			    si.CreateDate,
			    si.UpdatedBy,
			    si.UpdateDate' + 
			    CASE WHEN @IncludeCustomDetails = 1 THEN ',' ELSE '' END

SET @DynamicPivotQuery5 = N'
		FROM #TEMP si
		LEFT JOIN result2 tsi on tsi.SystemItemId = si.TargetSystemItemId
		ORDER BY DomainItemPath;'

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