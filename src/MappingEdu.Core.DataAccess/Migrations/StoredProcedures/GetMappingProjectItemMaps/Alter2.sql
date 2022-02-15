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
		sim.CreateBy,
		sim.CreateDate,
		sim.UpdateBy,
		sim.UpdateDate
FROM dbo.SystemItemMap as sim
LEFT JOIN dbo.SystemItemMapAssociation as sima on
	sim.SystemItemMapId = sima.SystemItemMapId
	LEFT JOIN dbo.SystemItem as si on
		sima.SystemItemId = si.SystemItemId
Where MappingProjectId = @MappingProjectId