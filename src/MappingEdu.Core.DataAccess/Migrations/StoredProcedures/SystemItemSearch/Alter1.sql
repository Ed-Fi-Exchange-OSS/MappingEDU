-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER PROCEDURE [dbo].[SystemItemSearch]
@MappedSystemId uniqueidentifier
AS
BEGIN
WITH result AS (
    SELECT DataTypeSource,
			Definition,
            CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
            SystemItemId as DomainId,
            FieldLength,
            ItemDataTypeId,
	        ItemName,
	        ItemTypeId,
			ItemUrl,
	        MappedSystemId,
	        ParentSystemItemId,
	        SystemItemId,
			TechnicalName
	FROM dbo.SystemItem
	WHERE ParentSystemItemId is null and [MappedSystemId]=@MappedSystemId

	    UNION all

	    SELECT i2.DataTypeSource,
				i2.Definition,
	            result.DomainItemPath + '.' + i2.ItemName,
		        result.DomainId,
	            i2.FieldLength,
	            i2.ItemDataTypeId,
		        i2.ItemName,
		        i2.ItemTypeId,
				i2.ItemUrl,
		        i2.MappedSystemId,
		        i2.ParentSystemItemId,
		        i2.SystemItemId,
				i2.TechnicalName
	    FROM SystemItem AS i2
		inner join result
		    ON result.SystemItemId = i2.ParentSystemItemId
)
SELECT Definition,
        DomainItemPath,
	    DomainId,
	    FieldLength,
        ItemDataTypeId,
	    ItemName,
	    ItemTypeId,
	    MappedSystemId,
	    SystemItemId
FROM result
END