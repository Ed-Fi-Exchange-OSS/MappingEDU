-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetMappingProjectReportEnumerationItemMaps]
@MappingProjectId uniqueidentifier
AS
SELECT SystemItemMap.SourceSystemItemId as SourceSystemItemId,
		SourceSystemEnumerationItem.SystemEnumerationItemId as SourceSystemEnumerationItemId,
        SourceSystemEnumerationItem.CodeValue as SourceCodeValue,
        SourceSystemEnumerationItem.ShortDescription as SourceShortDescription,
        SourceSystemEnumerationItem.Description as SourceDescription,
        SystemEnumerationItemMap.EnumerationMappingStatusTypeId,
        SystemEnumerationItemMap.EnumerationMappingStatusReasonTypeId,
        TargetSystemEnumerationItem.CodeValue as TargetCodeValue,
        TargetSystemEnumerationItem.SystemItemId as TargetSystemItemId

FROM [dbo].[SystemItemMap] as SystemItemMap
    JOIN [dbo].[SystemItem] as SourceSystemItem on 
	    SystemItemMap.SourceSystemItemId = SourceSystemItem.SystemItemId
	JOIN [dbo].[SystemEnumerationItemMap] as SystemEnumerationItemMap on
	    SystemItemMap.SystemItemMapId = SystemEnumerationItemMap.SystemItemMapId
	JOIN [dbo].[SystemEnumerationItem] as SourceSystemEnumerationItem on 
	    SystemEnumerationItemMap.SourceSystemEnumerationItemId = SourceSystemEnumerationItem.SystemEnumerationItemId
	LEFT JOIN [dbo].[SystemEnumerationItem] as TargetSystemEnumerationItem on 
	    SystemEnumerationItemMap.TargetSystemEnumerationItemId = TargetSystemEnumerationItem.SystemEnumerationItemId
WHERE SystemItemMap.MappingProjectId = @MappingProjectId
	AND SystemITemMap.MappingMethodTypeId = 1
    AND SourceSystemItem.ItemTypeId = 5