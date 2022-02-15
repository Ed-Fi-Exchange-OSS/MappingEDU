// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using ClosedXML.Excel;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.Export
{
    public interface IExportService
    {
        Stream ExportMappedSystem(Guid mappedSystemId, string templatePath);
    }

    public class ExportService : IExportService
    {
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly IDataStandardExportRepository _dataStandardExportRepository;

        public ExportService(IMappedSystemRepository mappedSystemRepository, IDataStandardExportRepository dataStandardExportRepository)
        {
            _mappedSystemRepository = mappedSystemRepository;
            _dataStandardExportRepository = dataStandardExportRepository;
        }

        public Stream ExportMappedSystem(Guid mappedSystemId, string templatePath)
        {
            var systemItems = _dataStandardExportRepository.GetSystemItems(mappedSystemId);
            var mappedSystem = _mappedSystemRepository.Get(mappedSystemId);

            var isExtended = systemItems.Any(x => x.IsExtended);

            if (!Principal.Current.IsAdministrator && !mappedSystem.HasAccess())
                throw new SecurityException("User needs at least View Access to export this data standard");

            var workbook = new XLWorkbook(templatePath);

            MapElementGroup(workbook, systemItems, isExtended);
            MapEntities(workbook, systemItems, isExtended);
            MapElements(mappedSystem, systemItems, workbook, isExtended);
            MapEnumerations(mappedSystem, systemItems, workbook, isExtended);
            MapEnumerationValues(workbook, mappedSystemId);

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }
      
        private void MapElementGroup(XLWorkbook workbook, List<dynamic> systemItems, bool isExtended)
        {
            var worksheet = workbook.Worksheet("ElementGroupDefinitions");
            var row = 2;

            if(isExtended)
                worksheet.Cell("C1").Value = "Is Extended";

            foreach (var domain in systemItems.Where(o => o.ItemTypeId == ItemType.Domain.Id))
            {
                // domain
                worksheet.Cell("A" + row).Value = domain.ItemName;

                // definition
                worksheet.Cell("B" + row).Value = domain.Definition;

                // is extended
                if (isExtended) worksheet.Cell("C" + row).Value = domain.IsExtended;

                row++;
            }
        }

        private void MapElements(MappedSystem mappedSystem, IEnumerable<dynamic> systemItems, XLWorkbook workbook, bool isExtended)
        {
            var worksheet = workbook.Worksheet("ElementDefinitions");

            var metaColumns = mappedSystem.CustomDetailMetadata.Select((meta, index) => new
            {
                Column = index + (isExtended ? 10 : 9),
                meta.DisplayName,
                meta.CustomDetailMetadataId,
            });

            if (isExtended)
            {
                worksheet.Cell("E1").Value = "Is Extended";

                worksheet.Cell("F1").Value = "Data Type";

                worksheet.Cell("G1").Value = "Field Length";

                worksheet.Cell("H1").Value = "Url";

                worksheet.Cell("I1").Value = "Technical Name";
            }

            foreach (var column in metaColumns)
            {
                var asciiColumnValue = (char)(column.Column + 64); //65 is A
                worksheet.Cell(1, asciiColumnValue.ToString()).Value = "EXT_" + column.DisplayName;
            }

            var row = 2;

            foreach (var element in systemItems.Where(x => x.ItemTypeId == ItemType.Element.Id))
            {
                var paths = element.DomainItemPath.Split('.');
                var domainName = paths[0];
                var enityName = "";

                for (var i = 1; i < paths.Length - 1; i++)
                {
                    enityName += paths[i];
                    if (i != paths.Length - 2) enityName += '.';
                }
                
                // domain
                worksheet.Cell("A" + row).Value = domainName;

                // entity
                worksheet.Cell("B" + row).Value = enityName;

                // name
                worksheet.Cell("C" + row).Value = element.ItemName;

                // definition
                worksheet.Cell("D" + row).Value = element.Definition;

                // is extended
                if(isExtended) worksheet.Cell("E" + row).Value = element.IsExtended;

                // data type
                worksheet.Cell((isExtended ? "F" : "E") + row).Value = element.DataTypeSource;

                // field length
                worksheet.Cell((isExtended ? "G" : "F") + row).Value = element.FieldLength;

                // url
                worksheet.Cell((isExtended ? "H" : "G") + row).Value = element.ItemUrl;

                // technical name
                worksheet.Cell((isExtended ? "I" : "H") + row).Value = element.TechnicalName;


               foreach (var column in metaColumns)
               {
                    object systemItemCustomDetailValue;
                    if (((IDictionary<string, object>)element).TryGetValue(column.DisplayName, out systemItemCustomDetailValue))
                        worksheet.Cell(row, column.Column).Value = systemItemCustomDetailValue.ToString();
                }
                row++;
            }
        }

        private void MapEntities(XLWorkbook workbook, IEnumerable<dynamic> systemItems, bool isExtended)
        {
            var worksheet = workbook.Worksheet("EntityDefinitions");
            var row = 2;

            if (isExtended)
                worksheet.Cell("D1").Value = "Is Extended";

            foreach (var entity in systemItems.Where(o => o.ItemTypeId == ItemType.Entity.Id || o.ItemTypeId == ItemType.SubEntity.Id))
            {
                var paths = entity.DomainItemPath.Split('.');
                var domainName = paths[0];
                var name = "";

                for (var i = 1; i < paths.Length; i++)
                {
                    name += paths[i];
                    if (i != paths.Length - 1) name += '.';
                }

                // domain
                worksheet.Cell("A" + row).Value = domainName;

                // name
                worksheet.Cell("B" + row).Value = name;

                // definition
                worksheet.Cell("C" + row).Value = entity.Definition;

                // is extended
                if (isExtended) worksheet.Cell("D" + row).Value = entity.IsExtended;

                row++;
            }
        }

        private void MapEnumerations(MappedSystem mappedSystem, IEnumerable<dynamic> systemItems, XLWorkbook workbook, bool isExtended)
        {
            var worksheet = workbook.Worksheet("EnumerationDefinitions");

            var metaColumns = mappedSystem.CustomDetailMetadata.Select((meta, index) => new
            {
                Column = index + (isExtended ? 5 : 4),
                meta.DisplayName,
                meta.CustomDetailMetadataId,
            });

            foreach (var column in metaColumns)
            {
                var asciiColumnValue = (char)(column.Column + 64); //65 is A
                worksheet.Cell(1, asciiColumnValue.ToString()).Value = "EXT_" + column.DisplayName;
            }

            if (isExtended)
                worksheet.Cell("D1").Value = "Is Extended";

            var row = 2;

            foreach (var enumeration in systemItems.Where(x => x.ItemTypeId == ItemType.Enumeration.Id))
            {
                var paths = enumeration.DomainItemPath.Split('.');
                var domainName = paths[0];

                // domain
                worksheet.Cell("A" + row).Value = domainName;

                // name
                worksheet.Cell("B" + row).Value = enumeration.ItemName;

                // definition
                worksheet.Cell("C" + row).Value = enumeration.Definition;

                // is extended
                if(isExtended) worksheet.Cell("D" + row).Value = enumeration.IsExtended;
                foreach (var column in metaColumns)
                {
                    object systemItemCustomDetailValue;
                    if (((IDictionary<string, object>)enumeration).TryGetValue(column.DisplayName, out systemItemCustomDetailValue))
                        worksheet.Cell(row, column.Column).Value = systemItemCustomDetailValue.ToString();
                }

                row++;
            }
        }

        private void MapEnumerationValues(XLWorkbook workbook, Guid mappedSystemId)
        {
            var enumerationItems = _dataStandardExportRepository.GetEnumerationItems(mappedSystemId);

            var worksheet = workbook.Worksheet("EnumerationItems");
            var row = 2;

            foreach (var enumerationItem in enumerationItems)
            {
                // domain
                worksheet.Cell("A" + row).Value = enumerationItem.ElementGroup;

                // name
                worksheet.Cell("B" + row).Value = enumerationItem.ItemName;

                // code value
                worksheet.Cell("C" + row).Value = enumerationItem.CodeValue;

                // short description
                worksheet.Cell("D" + row).Value = enumerationItem.ShortDescription;

                // description
                worksheet.Cell("E" + row).Value = enumerationItem.Description;

                row++;
            }
        }
    }
}