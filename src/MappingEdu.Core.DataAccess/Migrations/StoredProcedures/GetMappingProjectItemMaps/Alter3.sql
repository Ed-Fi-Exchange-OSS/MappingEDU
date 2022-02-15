-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetMappingProjectItemMaps]
@MappingProjectId uniqueidentifier
As
SELECT sim.SystemItemMapId,
		sim.SourceSystemItemId,
		sim.BusinessLogic,
		sim.Flagged,
		sim.MappingMethodTypeId,
		sim.OmissionReason,
		sim.WorkflowStatusTypeId,
		si.MappedSystemId as TargetSystemItem_MappedSystemId,
		si.SystemItemId as TargetSystemItem_SystemItemId,
		si.ItemTypeId as TargetSystemItem_ItemTypeId,
		CASE WHEN creater.FirstName is NULL THEN ''
			ELSE CONCAT(creater.FirstName, ' ', creater.LastName)
			END as CreateBy,
		sim.CreateDate,
		CASE WHEN updater.FirstName is NULL THEN ''
			ELSE CONCAT(updater.FirstName, ' ', updater.LastName) 
			END as UpdateBy,
		sim.UpdateDate
FROM dbo.SystemItemMap as sim
LEFT JOIN dbo.SystemItemMapAssociation as sima on
	sim.SystemItemMapId = sima.SystemItemMapId
	LEFT JOIN dbo.SystemItem as si on
		sima.SystemItemId = si.SystemItemId

LEFT JOIN AspNetUsers creater on 
	sim.CreateById = creater.Id

LEFT JOIN AspNetUsers updater on 
	sim.UpdateById = updater.Id
Where MappingProjectId = @MappingProjectId