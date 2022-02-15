// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Security;
using System.Xml;
using ClosedXML.Excel;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.Import;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.Import
{
    public interface IImportExtensionsService
    {
        ImportResultModel Import(ImportSchemaModel model);
    }

    public class ImportExtensionsService : IImportExtensionsService
    {
        private static MemoryCache _memoryCache;
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly ISystemItemRepository _systemItemRepository;

        public ImportExtensionsService(IMappedSystemRepository mappedSystemRepository, ISystemItemRepository sytemItemRepository)
        {
            _mappedSystemRepository = mappedSystemRepository;
            _systemItemRepository = sytemItemRepository;
            _memoryCache = MemoryCache.Default;
        }

        public ImportResultModel Import(ImportSchemaModel model)
        {
            var standard = _mappedSystemRepository.Get(model.MappedSystemId);
            var mappedSystemId = model.MappedSystemId;

            var logs = new List<ImportLog>();

            if (!Principal.Current.IsAdministrator && !standard.HasAccess(MappedSystemUser.MappedSystemUserRole.Edit))
                throw new SecurityException("User needs at least Edit Access to import into this data standard");

            var xsd = getXSD(model.ImportData);

            var core = _systemItemRepository.GetAllQueryable()
                .First(x => x.ItemName == "Core" && x.IsActive &&
                            x.MappedSystemId == model.MappedSystemId &&
                            x.ParentSystemItemId == null);

            // Domain Entity
            foreach (var complexType in xsd.ComplexTypes.Where(x => x.TypeGroup == "Association" || x.TypeGroup == "Domain Entity" || x.TypeGroup == "Descriptor"))
            {
                // All Extensions must start with EXTENSION
                if (!complexType.Name.StartsWith("EXTENSION-"))
                {
                    logs.Add(new ImportLog {Message = string.Format("Unable to add {0} because it doesn't start with 'EXTENSION-'", complexType.Name), Status = "Warning"});
                    continue;
                }

                if (complexType.TypeGroup == "Descriptor")
                {
                    var descriptor = core.ChildSystemItems
                        .FirstOrDefault(x => x.ItemName == complexType.Name.Substring(10) && x.ItemType == ItemType.Enumeration);

                    if (descriptor == null)
                        core.ChildSystemItems.Add(new SystemItem
                        {
                            ItemName = complexType.Name.Substring(10),
                            ItemType = ItemType.Enumeration,
                            Definition = complexType.Definition,
                            MappedSystemId = mappedSystemId,
                            IsExtended = true,
                            IsActive = true
                        });
                    else
                    {
                        descriptor.Definition = complexType.Definition;
                        descriptor.IsActive = true;
                    }
                }

                if (complexType.Name.EndsWith("Extension") && !complexType.Name.EndsWith("-Extension"))
                {
                    if (complexType.Base.EndsWith("Restriction"))
                    {
                        var childComplexType = xsd.ComplexTypes.FirstOrDefault(x => x.Name == complexType.Base);
                        if (childComplexType == null)
                        {
                            logs.Add(new ImportLog {Message = string.Format("Unable to find {0}. Unable to add extention", complexType.Base), Status = "Warning"});
                            continue;
                        }
                        ;

                        var restriction = core.ChildSystemItems.FirstOrDefault(x => x.ItemName == childComplexType.Base);
                        if (restriction == null)
                        {
                            logs.Add(new ImportLog {Message = string.Format("Unable to find {0}. Unable to add extention", childComplexType.Base), Status = "Warning"});
                            continue;
                        }

                        var extentionsItem = GetItem(xsd, complexType, mappedSystemId, logs);

                        AddOrUpdate(restriction, extentionsItem);
                    }
                    else
                    {
                        var extended = core.ChildSystemItems.FirstOrDefault(x => x.ItemName == complexType.Base);
                        if (extended == null)
                        {
                            logs.Add(new ImportLog {Message = string.Format("Unable to find {0}. Unable to add extention", complexType.Base), Status = "Message"});
                            continue;
                        }

                        var extension = GetItem(xsd, complexType, mappedSystemId, logs);
                        AddOrUpdate(extended, extension);
                    }
                }
                else
                {
                    var item = GetItem(xsd, complexType, mappedSystemId, logs);
                    item.ItemType = ItemType.Entity;

                    if (complexType.Base != null && complexType.Base != "ComplexObjectType")
                    {
                        var baseType = new SystemItem();
                        if (complexType.Base.StartsWith("EXTENSION-"))
                        {
                            var childComplexType = xsd.ComplexTypes.FirstOrDefault(x => x.Name == complexType.Base);
                            if (childComplexType == null)
                            {
                                logs.Add(new ImportLog {Message = string.Format("Unable to find {0}. Unable to add extention", complexType.Base), Status = "Warning"});
                                continue;
                            }

                            baseType = GetItem(xsd, childComplexType, mappedSystemId, logs);
                        }
                        else
                        {
                            if (complexType.Base == "DescriptorType")
                            {
                                baseType = _systemItemRepository.GetAllQueryable()
                                    .First(x => x.ItemName.EndsWith("Descriptor") && x.MappedSystemId == mappedSystemId).Clone();

                                baseType.ChildSystemItems = baseType.ChildSystemItems
                                    .Where(x => x.ItemName == "CodeValue" ||
                                                x.ItemName == "ShortDescription" ||
                                                x.ItemName == "Description" ||
                                                x.ItemName == "EffectiveBeginDate" ||
                                                x.ItemName == "EffectiveEndDate" ||
                                                x.ItemName == "PriorDescriptor" ||
                                                x.ItemName == "Namespace").ToList();
                            }
                            else
                            {
                                baseType = _systemItemRepository.GetAllQueryable()
                                    .First(x => x.ItemName == complexType.Base && x.MappedSystemId == mappedSystemId).Clone();
                            }

                            if (baseType == null)
                            {
                                logs.Add(new ImportLog {Message = string.Format("Unable to find {0}. Unable to add extention", complexType.Base), Status = "Warning"});
                                continue;
                            }

                            SetValues(baseType, mappedSystemId);
                        }

                        item.ChildSystemItems = item.ChildSystemItems.Union(baseType.ChildSystemItems).ToList();
                    }

                    if (core.ChildSystemItems.Any(x => x.ItemName == item.ItemName && item.ItemType != ItemType.Enumeration))
                    {
                        var fromCore = core.ChildSystemItems.First(x => x.ItemName == item.ItemName && item.ItemType != ItemType.Enumeration);
                        AddOrUpdate(fromCore, item);
                    }
                    else core.ChildSystemItems.Add(item);
                }
            }

            _mappedSystemRepository.SaveChanges();


            if (logs.Count < 20)
                return new ImportResultModel
                {
                    Logs = logs.ToArray(),
                    IsSuccessful = true,
                };

            var token = CreateErrorFile(string.Format("{0} Extension Errors", standard.SystemName), logs);

            return new ImportResultModel
            {
                Logs = logs.Take(20).ToArray(),
                IsSuccessful = true,
                FileToken = token,
                TotalLogs = logs.Count
            };
        }

        private void AddOrUpdate(SystemItem fromCore, SystemItem extended)
        {
            foreach (var child in extended.ChildSystemItems)
            {
                var inCore = fromCore.ChildSystemItems.FirstOrDefault(x => x.ItemName == child.ItemName);
                if (inCore != null) AddOrUpdate(inCore, child);
                else fromCore.ChildSystemItems.Add(child);
            }

            if (fromCore.IsExtended)
            {
                fromCore.Definition = extended.Definition;
                fromCore.ItemType = extended.ItemType;
                fromCore.DataTypeSource = extended.DataTypeSource;
                fromCore.ItemDataTypeId = extended.ItemDataTypeId;
            }
        }

        private Guid CreateErrorFile(string fileName, ICollection<ImportLog> logs)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Errors");

            worksheet.Cell(1, "A").Value = "Status";
            worksheet.Cell(1, "B").Value = "Message";

            var i = 2;
            foreach (var log in logs)
            {
                worksheet.Cell(i, "A").Value = log.Status;
                worksheet.Cell(i, "B").Value = log.Message;
                i++;
            }

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ms)
            };

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = string.Format("{0}.xlsx", fileName)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentLength = ms.Length;

            var token = Guid.NewGuid();
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60) //After an hour the file will expire.
            };
            _memoryCache.Add(token.ToString(), response, policy);

            return token;
        }

        private SystemItem GetItem(XsdModel xsd, XSDComplexType complexType, Guid mappedSystemId, ICollection<ImportLog> logs)
        {
            var systemItem = new SystemItem
            {
                ItemName = complexType.Name.StartsWith("EXTENSION-") ? complexType.Name.Substring(10) : complexType.Name,
                Definition = complexType.Definition,
                IsActive = true,
                IsExtended = true,
                ItemType = ItemType.SubEntity,
                MappedSystemId = mappedSystemId,
                ChildSystemItems = new List<SystemItem>()
            };

            foreach (var element in complexType.Elements)
            {
                if (element.Type.StartsWith("EXTENSION-"))
                {
                    var childComplexType = xsd.ComplexTypes.FirstOrDefault(x => x.Name == element.Type);
                    if (childComplexType != null)
                    {
                        var priorDescriptor = _systemItemRepository.GetAllQueryable()
                            .First(x => (x.ItemName == "PriorDescriptor") && x.MappedSystemId == mappedSystemId).Clone();

                        SetValues(priorDescriptor, mappedSystemId);

                        var item = GetItem(xsd, childComplexType, mappedSystemId, logs);
                        item.ItemName = element.Name;

                        if (childComplexType.Base == "DescriptorReferenceType")
                            item.ChildSystemItems = item.ChildSystemItems.Union(priorDescriptor.ChildSystemItems).ToList();

                        else if (childComplexType.Base == "ReferenceType")
                            item.ChildSystemItems = item.ChildSystemItems.Union(priorDescriptor.ChildSystemItems.Where(x => x.ItemName == "id" || x.ItemName == "ref")).ToList();

                        else if (childComplexType.Base != null)
                        {
                            var baseType = _systemItemRepository.GetAllQueryable()
                                .First(x => x.ItemName == childComplexType.Base && x.MappedSystemId == mappedSystemId).Clone();

                            SetValues(baseType, mappedSystemId);

                            item.ChildSystemItems = item.ChildSystemItems.Union(baseType.ChildSystemItems).ToList();
                        }

                        systemItem.ChildSystemItems.Add(item);

                        continue;
                    }

                    var childSimpleType = xsd.SimpleTypes.FirstOrDefault(x => x.Name == element.Type);
                    if (childSimpleType != null)
                    {
                        var fieldLength = childSimpleType.Restrictions.FirstOrDefault(x => x.Type == "xs:length");

                        var simpleTypeItem = new SystemItem
                        {
                            ItemName = element.Name,
                            ItemType = ItemType.Element,
                            ItemDataTypeId = GetItemDataTypeId(childSimpleType.Base),
                            Definition = element.Definition,
                            DataTypeSource = element.Type,
                            MappedSystemId = mappedSystemId,
                            IsActive = true,
                            IsExtended = true
                        };

                        if (fieldLength != null) simpleTypeItem.FieldLength = Convert.ToInt32(fieldLength.Value);

                        systemItem.ChildSystemItems.Add(simpleTypeItem);
                        continue;
                    }

                    logs.Add(new ImportLog {Message = string.Format("Unable to find {0}. Unable to add extention", element.Type), Status = "Message"});
                }
                else
                {
                    var itemDataTypeId = 17;

                    if (element.SimpleType != null)
                        itemDataTypeId = GetItemDataTypeId(element.SimpleType.Base);
                    else if (element.Type != null)
                        itemDataTypeId = GetItemDataTypeId(element.Type);

                    if (itemDataTypeId < 17)
                    {
                        systemItem.ChildSystemItems.Add(new SystemItem
                        {
                            ItemName = element.Name,
                            ItemType = ItemType.Element,
                            ItemDataTypeId = itemDataTypeId,
                            Definition = element.Definition,
                            DataTypeSource = element.SimpleType != null ? element.SimpleType.Base : element.Type,
                            MappedSystemId = mappedSystemId,
                            IsActive = true,
                            IsExtended = true
                        });

                        continue;
                    }

                    var searchType = element.Type;
                    if (searchType.EndsWith("DescriptorReferenceType"))
                        searchType = searchType.Substring(0, searchType.Length - 23);
                    else if (searchType.EndsWith("Type"))
                        searchType = searchType.Substring(0, searchType.Length - 4);

                    var coreItem = _systemItemRepository.GetAllQueryable()
                        .FirstOrDefault(x => (x.ItemName == element.Name || x.ItemName == searchType) && x.MappedSystemId == mappedSystemId && x.ItemTypeId < 5);

                    if (coreItem == null)
                    {
                        systemItem.ChildSystemItems.Add(new SystemItem
                        {
                            ItemName = element.Name,
                            ItemType = ItemType.Element,
                            Definition = element.Definition,
                            DataTypeSource = element.Type,
                            MappedSystemId = mappedSystemId,
                            IsActive = true,
                            IsExtended = true
                        });

                        logs.Add(new ImportLog {Message = string.Format("Unable to find {0}. Adding as an Element", element.Type), Status = "Warning"});
                        continue;
                    }

                    var clone = coreItem.Clone();
                    SetValues(clone, mappedSystemId);
                    clone.Definition = element.Definition;
                    clone.ItemName = element.Name;
                    systemItem.ChildSystemItems.Add(clone);
                }
            }

            return systemItem;
        }

        private int GetItemDataTypeId(string dataType)
        {
            var sourceDataType = dataType.ToLower();

            if (sourceDataType == "xs:boolean" || sourceDataType == "true/false") return 1;
            if (sourceDataType == "tinyint" || sourceDataType == "varbinary") return 2;
            if (sourceDataType == "char" || sourceDataType == "char(" || sourceDataType == "char(1)") return 3;
            if (sourceDataType == "currency" || sourceDataType == "money") return 4;
            if (sourceDataType == "date" || sourceDataType == "xs:date" || sourceDataType == "yyyy/mm/dd" || sourceDataType == "yyyy/ mm/dd" || sourceDataType == "yyyy-mm-dd") return 5;
            if (sourceDataType == "datetime") return 6;
            if (sourceDataType == "decimal" || sourceDataType == "percent" || sourceDataType == "decimal(4,2)" || sourceDataType == "xs:decimal" || sourceDataType.StartsWith("numeric")) return 7;
            if (sourceDataType == "xs:double" || sourceDataType == "float") return 8;
            if (sourceDataType == "xs:duration") return 9;
            if (sourceDataType == "xs:int" || sourceDataType == "xs:integer" || sourceDataType == "xs:positiveinteger" || sourceDataType.StartsWith("integer")) return 10;
            if (sourceDataType == "bigint") return 11;
            if (sourceDataType == "smallint") return 12;
            if (sourceDataType.StartsWith("nvarchar") || sourceDataType.StartsWith("text") || sourceDataType == "varchar" || sourceDataType == "xs:string" || sourceDataType == "xs:sttring" || sourceDataType == "String" || sourceDataType == "URI" || sourceDataType == "xs:token" || sourceDataType == "electronicmail") return 13;
            if (sourceDataType == "xs:time" || sourceDataType == "hh:mm:ss") return 14;
            if (sourceDataType == "uniqueId" || sourceDataType == "xs:id" || sourceDataType == "xs:idref") return 15;
            if (sourceDataType == "xs:gyear") return 16;
            return 17;
        }

        private XsdModel getXSD(Stream ImportData)
        {
            var doc = new XmlDocument();
            doc.Load(ImportData);

            var manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            manager.AddNamespace("ann", "http://ed-fi.org/annotation");

            var schema = doc.GetElementsByTagName("xs:schema").Item(0);

            if (schema == null) return null;

            var xsd = new XsdModel
            {
                ComplexTypes = new List<XSDComplexType>(),
                SimpleTypes = new List<XSDSimpleType>()
            };

            for (var i = 0; i < schema.ChildNodes.Count; i++)
            {
                var child = schema.ChildNodes.Item(i);

                if (null == child) break;

                if (child.Name == "xs:complexType")
                {
                    var complexType = new XSDComplexType
                    {
                        Elements = new List<XSDElement>(),
                        IsRestriction = false
                    };

                    if (child.Attributes != null && child.Attributes["name"] != null)
                        complexType.Name = child.Attributes["name"].Value;

                    var documentation = child.SelectSingleNode("./xs:annotation/xs:documentation", manager);
                    if (documentation != null)
                        complexType.Definition = documentation.InnerText;

                    var typeGroup = child.SelectSingleNode("./xs:annotation/xs:appinfo/ann:TypeGroup", manager);
                    if (typeGroup != null)
                        complexType.TypeGroup = typeGroup.InnerText;

                    var complexContent = child.SelectSingleNode("./xs:complexContent", manager);
                    if (complexContent != null)
                    {
                        var extension = complexContent.SelectSingleNode("./xs:extension", manager);
                        if (extension != null)
                        {
                            if (extension.Attributes != null && extension.Attributes["base"] != null)
                                complexType.Base = extension.Attributes["base"].Value;

                            var elements = extension.SelectNodes("./xs:sequence/xs:element", manager);
                            if (elements != null && elements.Count > 0)
                                for (var j = 0; j < elements.Count; j++)
                                    complexType.Elements.Add(SetElement(elements[j], manager));
                        }
                        else
                        {
                            var restriction = complexContent.SelectSingleNode("./xs:restriction", manager);

                            if (restriction == null) continue;

                            complexType.IsRestriction = true;
                            if (restriction.Attributes != null && restriction.Attributes["base"] != null)
                                complexType.Base = restriction.Attributes["base"].Value;

                            var elements = restriction.SelectNodes("./xs:sequence/xs:element", manager);
                            if (elements != null && elements.Count > 0)
                                for (var j = 0; j < elements.Count; j++)
                                    complexType.Elements.Add(SetElement(elements[j], manager));
                        }
                    }
                    else
                    {
                        var elements = child.SelectNodes("./xs:sequence/xs:element", manager);
                        if (elements != null && elements.Count > 0)
                            for (var j = 0; j < elements.Count; j++)
                                complexType.Elements.Add(SetElement(elements[j], manager));
                    }

                    xsd.ComplexTypes.Add(complexType);
                }
                else if (child.Name == "xs:simpleType")
                {
                    var simpleType = SetSimpleType(child, manager);
                    xsd.SimpleTypes.Add(simpleType);
                }
            }

            return xsd;
        }

        private XSDElement SetElement(XmlNode element, XmlNamespaceManager manager)
        {
            var xsdElement = new XSDElement();

            if (element.Attributes != null && element.Attributes["name"] != null)
                xsdElement.Name = element.Attributes["name"].Value;

            if (element.Attributes != null && element.Attributes["type"] != null)
                xsdElement.Type = element.Attributes["type"].Value;
            else
            {
                xsdElement.Type = "";
                var type = element.SelectSingleNode("./xs:simpleType", manager);
                var simpleType = SetSimpleType(type, manager);
                xsdElement.SimpleType = simpleType;
            }

            var elementDocumentation = element.SelectSingleNode("./xs:annotation/xs:documentation", manager);
            if (elementDocumentation != null)
                xsdElement.Definition = elementDocumentation.InnerText;

            return xsdElement;
        }

        private XSDSimpleType SetSimpleType(XmlNode node, XmlNamespaceManager manager)
        {
            var simpleType = new XSDSimpleType {Restrictions = new List<XSDSimpleRestriction>()};

            if (node.Attributes != null && node.Attributes["name"] != null)
                simpleType.Name = node.Attributes["name"].Value;

            var documentation = node.SelectSingleNode("./xs:annotation/xs:documentation", manager);
            if (documentation != null)
                simpleType.Definition = documentation.InnerText;

            var typeGroup = node.SelectSingleNode("./xs:annotation/xs:appinfo/ann:TypeGroup", manager);
            if (typeGroup != null)
                simpleType.TypeGroup = typeGroup.InnerText;

            var restriction = node.SelectSingleNode("./xs:restriction", manager);
            if (restriction == null) return simpleType;

            if (restriction.Attributes != null && restriction.Attributes["base"] != null)
                simpleType.Base = restriction.Attributes["base"].Value;

            if (!restriction.HasChildNodes || restriction.ChildNodes.Count <= 0) return simpleType;

            for (var j = 0; j < restriction.ChildNodes.Count; j++)
            {
                var xsdSimpleRestriction = new XSDSimpleRestriction();

                var restrictionAttr = restriction.ChildNodes[j];

                xsdSimpleRestriction.Type = restrictionAttr.Name;

                if (restrictionAttr.Attributes != null && restrictionAttr.Attributes["value"] != null)
                    xsdSimpleRestriction.Value = restrictionAttr.Attributes["value"].Value;

                simpleType.Restrictions.Add(xsdSimpleRestriction);
            }

            return simpleType;
        }

        private void SetValues(SystemItem item, Guid mappedSystemId)
        {
            item.MappedSystemId = mappedSystemId;
            item.IsExtended = true;
            foreach (var child in item.ChildSystemItems)
                SetValues(child, mappedSystemId);
        }
    }
}