-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[GetDashboardQueueFilters]
@MappingProjectId uniqueidentifier
AS
BEGIN

Declare @SourceDataStandardId uniqueidentifier

SELECT @SourceDataStandardId = SourceDataStandardMappedSystemId
FROM dbo.MappingProject
WHERE MappingProjectId = @MappingProjectId 

SELECT mpqf.Name as Name, 
	    mpqf.MappingProjectQueueFilterId as MappingProjectQueueFilterId, 
	    COUNT(*) as Total
FROM MappingProjectQueueFilter mpqf
LEFT JOIN MappingProjectQueueFilterCreatedByUser mpqfcbu on mpqfcbu.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
LEFT JOIN MappingProjectQueueFilterItemType mpqfit on mpqfit.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
LEFT JOIN MappingProjectQueueFilterMappingMethodType mpqfmmt on mpqfmmt.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
LEFT JOIN MappingProjectQueueFilterParentSystemItem mpqfpsi on mpqfpsi.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
LEFT JOIN MappingProjectQueueFilterUpdatedByUser mpqfubu on mpqfubu.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
LEFT JOIN MappingProjectQueueFilterWorkflowStatusType mpqfwst on mpqfwst.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId

CROSS JOIN SystemItem item
LEFT JOIN (SELECT CASE WHEN MappingMethodTypeId = 4 THEN 'Marked For Omission'
				        WHEN MappingMethodTypeId = 3 THEN 'Marked for Extension' 
				        ELSE BusinessLogic END as Logic,
			        WorkflowStatusTypeId,
			        MappingMethodTypeId,
			        Flagged,
				    CreateById,
				    CreateDate,
				    UpdateById,
				    UpdateDate,
				    IsAutoMapped,
			        SourceSystemItemId,
			        sim.SystemItemMapId,
                    MappingProjectId,
				    seim.Count as MappedEnumerations
	        FROM dbo.SystemItemMap as sim
		    LEFT JOIN (SELECT SystemItemMapId, COUNT(*) as Count
				        FROM SystemEnumerationItemMap
					    GROUP BY SystemItemMapId) seim
		    ON seim.SystemItemMapId = sim.SystemItemMapId
	        WHERE sim.MappingProjectId = @MappingProjectId) map ON item.SystemItemId = map.SourceSystemItemId

LEFT JOIN ItemType as it on it.ItemTypeId = item.ItemTypeId
LEFT JOIN (SELECT SystemItemId, COUNT(*) as COUNT
				FROM SystemEnumerationItem
				GROUP BY SystemItemId) sei on item.SystemItemId = sei.SystemItemId

LEFT JOIN AspNetUsers creater on map.CreateById = creater.Id
LEFT JOIN AspNetUsers updater on map.UpdateById = updater.Id
JOIN SystemItem eg on item.ElementGroupSystemItemId = eg.SystemItemId

WHERE  (item.MappedSystemId = @SourceDataStandardId) AND
		(mpqf.Search is NULL or mpqf.Search = '' OR (ISNULL(map.Logic, 'Not Yet Mapped') like '%' + mpqf.Search + '%' OR 
	    item.DomainItemPath like '%' + mpqf.Search + '%' OR 
	    item.Definition like '%' + mpqf.Search + '%' OR 
	    it.ItemTypeName like '%' + mpqf.Search + '%')) AND
		eg.IsActive = 1 AND
		item.IsActive = 1 AND
	    (mpqfmmt.MappingMethodTypeId is NULL OR map.MappingMethodTypeId = mpqfmmt.MappingMethodTypeId) AND
		((mpqfpsi.SystemItemId is NULL OR eg.SystemItemId = mpqfpsi.SystemItemId) AND eg.IsActive = 1) AND
	    (mpqfwst.WorkflowStatusTypeId is NULL OR (mpqfwst.WorkflowStatusTypeId = 0 AND map.WorkflowStatusTypeId is null) OR map.WorkflowStatusTypeId = mpqfwst.WorkflowStatusTypeId) AND
	    (mpqfcbu.CreatedByUserId is NULL OR mpqfcbu.CreatedByUserId = creater.Id) AND
	    (mpqfubu.UpdatedByUserId is NULL OR mpqfubu.UpdatedByUserId = updater.Id) AND
	    ((mpqf.Unflagged = 0 AND mpqf.Flagged  = 0) OR (mpqf.Unflagged  = 1 AND (map.Flagged = 0 OR map.Flagged is null)) OR (mpqf.Flagged = 1 AND map.Flagged = 1 AND map.SystemItemMapId is not null)) AND
	    ((mpqf.AutoMapped = 0 AND mpqf.UserMapped = 0) OR (mpqf.AutoMapped = 1 AND map.IsAutoMapped = 1) OR (mpqf.UserMapped = 1 AND map.IsAutoMapped = 0)) AND
	    ((mpqf.Extended = 0 AND mpqf.Base = 0) OR (mpqf.Extended = 1 AND item.IsExtended = 1) OR (mpqf.Base = 1 AND item.IsExtended = 0)) AND
	    ((mpqfit.ItemTypeId is NULL and (item.ItemTypeId = 4 or item.ItemTypeId = 5)) OR item.ItemTypeId = mpqfit.ItemTypeId) AND
	    mpqf.ShowInDashboard = 1 AND mpqf.MappingProjectId = @MappingProjectId

GROUP BY mpqf.MappingProjectQueueFilterId, mpqf.Name;
END