-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[GetMappingProjectItemMaps]
@MappingProjectId uniqueidentifier
As
SELECT sim.SourceSystemItemId,
		sim.BusinessLogic,
		sim.MappingMethodTypeId,
		sim.OmissionReason,
		si.MappedSystemId as TargetSystemItem_MappedSystemId,
		si.SystemItemId as TargetSystemItem_SystemItemId,
		si.ItemTypeId as TargetSystemItem_ItemTypeId
FROM dbo.SystemItemMap as sim
JOIN dbo.SystemItemMapAssociation as sima on
	sim.SystemItemMapId = sima.SystemItemMapId
	JOIN dbo.SystemItem as si on
		sima.SystemItemId = si.SystemItemId
Where MappingProjectId = @MappingProjectId