-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE PROCEDURE [dbo].[GetDashboardQueueFilters]
@MappingProjectId uniqueidentifier
AS
BEGIN
WITH project as (
	SELECT SourceDataStandardMappedSystemId
	FROM dbo.MappingProject
	WHERE MappingProjectId = @MappingProjectId 
), result as (
	SELECT Definition,
            CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
	        SystemItemId as DomainId,
		    IsExtended,
	        ItemTypeId,
	        MappedSystemId,
	        ParentSystemItemId,
	        SystemItemId
    FROM dbo.SystemItem
    JOIN project as mp on MappedSystemId = mp.SourceDataStandardMappedSystemId
    WHERE ParentSystemItemId is null AND [MappedSystemId] = mp.SourceDataStandardMappedSystemId AND IsActive = 1
    UNION all
    SELECT i2.Definition,
            result.DomainItemPath + '.' + i2.ItemName,
	        result.DomainId,
		    i2.IsExtended,
	        i2.ItemTypeId,
	        i2.MappedSystemId,
	        i2.ParentSystemItemId,
	        i2.SystemItemId
    FROM SystemItem AS i2
    inner join result
        ON result.SystemItemId = i2.ParentSystemItemId)

SELECT mpqf.Name as Name, 
	    mpqf.MappingProjectQueueFilterId as MappingProjectQueueFilterId, 
	    COUNT(*) as Total
FROM result as item
JOIN MappingProjectQueueFilter mpqf on @MappingProjectId = mpqf.MappingProjectId
LEFT JOIN MappingProjectQueueFilterCreatedByUser mpqfcbu on mpqfcbu.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
LEFT JOIN MappingProjectQueueFilterItemType mpqfit on mpqfit.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
LEFT JOIN MappingProjectQueueFilterMappingMethodType mpqfmmt on mpqfmmt.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
LEFT JOIN MappingProjectQueueFilterParentSystemItem mpqfpsi on mpqfpsi.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
LEFT JOIN MappingProjectQueueFilterUpdatedByUser mpqfubu on mpqfubu.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
LEFT JOIN MappingProjectQueueFilterWorkflowStatusType mpqfwst on mpqfwst.MappingProjectQueueFilterId = mpqf.MappingProjectQueueFilterId
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

JOIN ItemType as it on it.ItemTypeId = item.ItemTypeId
LEFT JOIN (SELECT SystemItemId, COUNT(*) as COUNT
				FROM SystemEnumerationItem
				GROUP BY SystemItemId) sei on item.SystemItemId = sei.SystemItemId

LEFT JOIN AspNetUsers creater on map.CreateById = creater.Id
LEFT JOIN AspNetUsers updater on map.UpdateById = updater.Id

WHERE (mpqf.Search is NULL or mpqf.Search = '' OR (ISNULL(map.Logic, 'Not Yet Mapped') like '%' + mpqf.Search + '%' OR 
	    DomainItemPath like '%' + mpqf.Search + '%' OR 
	    Definition like '%' + mpqf.Search + '%' OR 
	    it.ItemTypeName like '%' + mpqf.Search + '%')) AND
	    (mpqfmmt.MappingMethodTypeId is NULL OR map.MappingMethodTypeId = mpqfmmt.MappingMethodTypeId) AND
        (mpqfpsi.SystemItemId is NULL OR item.DomainId = mpqfpsi.SystemItemId) AND
	    (mpqfwst.WorkflowStatusTypeId is NULL OR (mpqfwst.WorkflowStatusTypeId = 0 AND map.WorkflowStatusTypeId is null) OR map.WorkflowStatusTypeId = mpqfwst.WorkflowStatusTypeId) AND
	    (mpqfcbu.CreatedByUserId is NULL OR mpqfcbu.CreatedByUserId = creater.Id) AND
	    (mpqfubu.UpdatedByUserId is NULL OR mpqfubu.UpdatedByUserId = updater.Id) AND
	    ((mpqf.Unflagged = 0 AND mpqf.Flagged  = 0) OR (mpqf.Unflagged  = 1 AND (map.Flagged = 0 OR map.Flagged is null)) OR (mpqf.Flagged = 1 AND map.Flagged = 1 AND map.SystemItemMapId is not null)) AND
	    ((mpqf.AutoMapped = 0 AND mpqf.UserMapped = 0) OR (mpqf.AutoMapped = 1 AND map.IsAutoMapped = 1) OR (mpqf.UserMapped = 1 AND map.IsAutoMapped = 0)) AND
	    ((mpqf.Extended = 0 AND mpqf.Base = 0) OR (mpqf.Extended = 1 AND IsExtended = 1) OR (mpqf.Base = 1 AND IsExtended = 0)) AND
	    ((mpqfit.ItemTypeId is NULL and (item.ItemTypeId = 4 or item.ItemTypeId = 5)) OR item.ItemTypeId = mpqfit.ItemTypeId) AND
	    mpqf.ShowInDashboard = 1

GROUP BY mpqf.MappingProjectQueueFilterId, mpqf.Name;
END