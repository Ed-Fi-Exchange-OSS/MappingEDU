-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[GetMappingProjectReportEnumerationItems]
@MappedSystemId uniqueidentifier
AS
    SELECT SystemItem.ItemName,
	SystemEnumerationItem.CodeValue,
	SystemEnumerationItem.ShortDescription,
	SystemEnumerationItem.Description
FROM dbo.SystemItem as SystemItem
	JOIN dbo.SystemEnumerationItem as SystemEnumerationItem
	on SystemItem.SystemItemId = SystemEnumerationItem.SystemItemId
WHERE SystemItem.MappedSystemId = @MappedSystemId 
	AND SystemItem.ItemTypeId = 5