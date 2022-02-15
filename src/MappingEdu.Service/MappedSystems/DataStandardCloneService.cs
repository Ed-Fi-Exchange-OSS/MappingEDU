// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.Import;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.MappedSystems
{
    public interface IDataStandardCloneService
    {
        DataStandardViewModel Clone(Guid id, DataStandardCloneModel model);

        ImportResultModel CreateInterchanges(Guid standardId, Guid baseStandardId, ICollection<Stream> streams);
    }

    public class DataStandardCloneService : IDataStandardCloneService
    {
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly IMapper _mapper;

        public DataStandardCloneService(IMappedSystemRepository mappedSystemRepository, IMapper mapper, ISystemItemRepository systemItemRepository)
        {
            _mappedSystemRepository = mappedSystemRepository;
            _mapper = mapper;
            _systemItemRepository = systemItemRepository;
        }

        public DataStandardViewModel Clone(Guid id, DataStandardCloneModel model)
        {
            // Only Admins can clone with Extensions
            if (!Principal.Current.IsAdministrator) model.WithExtensions = false;

            var standard = GetMappedSystem(id, MappedSystemUser.MappedSystemUserRole.View).Clone(model.WithExtensions);

            standard.ClonedFromMappedSystemId = id;
            standard.SystemName = model.SystemName;
            standard.SystemVersion = model.SystemVersion;
            standard.PreviousMappedSystemId = model.PreviousDataStandardId;
            standard.Users.Add(new MappedSystemUser(standard.MappedSystemId, Principal.Current.UserId, MappedSystemUser.MappedSystemUserRole.Owner));

            _mappedSystemRepository.Add(standard);
            _mappedSystemRepository.SaveChanges();

            var returnModel = _mapper.Map<DataStandardCloningViewModel>(standard);
            if (!model.PreviousDataStandardId.HasValue) return returnModel;

            var previousStandard = _mappedSystemRepository.Get(model.PreviousDataStandardId.Value);
            var clonedFrom = _mappedSystemRepository.Get(id);
            if (clonedFrom.PreviousMappedSystemId.HasValue && previousStandard.ClonedFromMappedSystemId.HasValue
                && clonedFrom.PreviousMappedSystemId.Value == previousStandard.ClonedFromMappedSystemId.Value)
            {
                returnModel.SimilarVersioning = true;
            }
            return returnModel;
        }

        public ImportResultModel CreateInterchanges(Guid standardId, Guid baseStandardId, ICollection<Stream> streams)
        {
            var baseStandard = GetMappedSystem(baseStandardId, MappedSystemUser.MappedSystemUserRole.View);
            var standard = GetMappedSystem(standardId, MappedSystemUser.MappedSystemUserRole.Edit);

            var interchanges = new List<XSDComplexType>();
            var logs = new List<ImportLog>();

            foreach (var stream in streams)
            {
                try
                {
                    interchanges.Add(GetInterchange(stream));
                }
                catch (Exception ex)
                {
                    var fs = stream as FileStream;
                    if (ex.Message.Contains("{FileName}"))
                    {
                        logs.Add(new ImportLog
                        {
                            Message = ex.Message.Replace("{FileName}", fs.Name),
                            Status = "Warning"
                        });
                    }
                    else
                    {
                        logs.Add(new ImportLog
                        {
                            Message = String.Format("Unknown Error with {0}. Unable to add Interchange", fs.Name),
                            Status = "Warning"
                        });
                    }
                }
            }

            var core = baseStandard.SystemItems.FirstOrDefault(x => x.IsActive && x.ItemName.ToLower() == "core");
            if (core == null)
            {
                logs.Add(new ImportLog
                {
                    Message = "Unable to find Core. Not able to create interchange standard.",
                    Status = "Error"
                });

                return null;
            }

            foreach (var interchange in interchanges)
            {
                var elementGroup = standard.SystemItems.FirstOrDefault(x => x.IsActive && x.ItemName == interchange.Name && x.ParentSystemItem == null);
                if (elementGroup == null)
                {
                    elementGroup = new SystemItem
                    {
                        ItemName = interchange.Name,
                        IsActive = true,
                        ItemType = ItemType.Domain,
                        MappedSystem = standard,
                        ChildSystemItems = new List<SystemItem>()
                    };

                    standard.SystemItems.Add(elementGroup);
                }
                elementGroup.Definition = interchange.Definition;

                foreach (var element in interchange.Elements)
                {
                    var entity = elementGroup.ChildSystemItems.FirstOrDefault(x => x.ItemName == element.Name);
                    var baseEntity = FindSystemItem(core, element.Name);

                    if (entity == null)
                    {
                        if (baseEntity == null)
                        {
                            logs.Add(new ImportLog
                            {
                                Message = String.Format("Unable to find {0}. {0} was not added to interchange {1}", element.Name, interchange.Name),
                                Status = "Warning"
                            });
                            continue;
                        }

                        var newEntity = new SystemItem
                        {
                            ItemName = baseEntity.ItemName,
                            IsActive = true,
                            ItemTypeId = ItemType.Entity.Id,
                            Definition = baseEntity.Definition,
                            IsExtended = baseEntity.IsExtended,
                            MappedSystem = standard,
                            ParentSystemItem = elementGroup,
                            ChildSystemItems = SetItems(baseEntity.ChildSystemItems, standard)
                        };

                        elementGroup.ChildSystemItems.Add(newEntity);
                    }
                    else CompareSystemItems(baseEntity, entity, standard);
                }
            }

            _mappedSystemRepository.SaveChanges();

            return new ImportResultModel()
            {
                Logs = logs.ToArray(),
                IsSuccessful = true,
                TotalLogs = logs.Count
            };
        }

        private void CompareSystemItems(SystemItem baseItem, SystemItem item, MappedSystem standard)
        {
            item.Definition = baseItem.Definition;
            item.FieldLength = baseItem.FieldLength;
            item.DataTypeSource = baseItem.DataTypeSource;
            item.IsExtended = baseItem.IsExtended;
            item.ItemUrl = baseItem.ItemUrl;
            item.ItemDataTypeId = baseItem.ItemDataTypeId;
            item.ItemTypeId = baseItem.ItemTypeId;
            item.TechnicalName = baseItem.TechnicalName;

            foreach (var baseChild in baseItem.ChildSystemItems)
            {
                var child = item.ChildSystemItems.FirstOrDefault(x => x.ItemName == baseChild.ItemName);
                if (child == null)
                {
                    var newElement = new SystemItem
                    {
                        ItemName = baseChild.ItemName,
                        Definition = baseChild.Definition,
                        DataTypeSource = baseChild.DataTypeSource,
                        FieldLength = baseChild.FieldLength,
                        IsActive = true,
                        IsExtended = baseChild.IsExtended,
                        ItemUrl = baseChild.ItemUrl,
                        ItemDataTypeId = baseChild.ItemDataTypeId,
                        ItemTypeId = baseChild.ItemTypeId,
                        MappedSystem = standard,
                        TechnicalName = baseChild.TechnicalName
                    };

                    if (baseChild.ChildSystemItems != null && baseChild.ChildSystemItems.Any())
                        newElement.ChildSystemItems = SetItems(baseChild.ChildSystemItems, standard).ToList();

                    item.ChildSystemItems.Add(newElement);
                }
                else CompareSystemItems(baseChild, child, standard);
            }
        }

        private ICollection<SystemItem> SetItems(ICollection<SystemItem> systemItems, MappedSystem standard)
        {
            var newChildSystemItems = new List<SystemItem>();

            foreach (var systemItem in systemItems)
            {
                var newElement = new SystemItem
                {
                    ItemName = systemItem.ItemName,
                    Definition = systemItem.Definition,
                    DataTypeSource = systemItem.DataTypeSource,
                    FieldLength = systemItem.FieldLength,
                    IsActive = true,
                    IsExtended = systemItem.IsExtended,
                    ItemUrl = systemItem.ItemUrl,
                    ItemDataTypeId = systemItem.ItemDataTypeId,
                    ItemTypeId = systemItem.ItemTypeId,
                    MappedSystem = standard,
                    TechnicalName = systemItem.TechnicalName
                };

                if (systemItem.ChildSystemItems != null && systemItem.ChildSystemItems.Any())
                    newElement.ChildSystemItems = SetItems(systemItem.ChildSystemItems, standard).ToList();

                newChildSystemItems.Add(newElement);
            }

            return newChildSystemItems;
        }

        private SystemItem FindSystemItem(SystemItem item, string itemName)
        {
            foreach (var child in item.ChildSystemItems)
            {
                if (child.ItemName == itemName) return child;

                if (child.ItemTypeId != 2 && child.ItemTypeId != 3) continue;

                var returnItem = FindSystemItem(child, itemName);
                if (returnItem != null) return returnItem;
            }

            return null;
        }

        private XSDComplexType GetInterchange(Stream stream)
        {
            var doc = new XmlDocument();
            doc.Load(stream);

            var manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            manager.AddNamespace("ann", "http://ed-fi.org/annotation");

            var schema = doc.GetElementsByTagName("xs:schema").Item(0);

            if (schema == null) 
                throw new Exception("Unable to add {FileName}: Missing xs:schema");
            
            var interchange = schema.SelectSingleNode("./xs:element", manager);
            if (interchange == null)
                throw new Exception("Unable to add {FileName}: Missing xs:schema/xs:element");

            var complexType = new XSDComplexType()
            {
                Elements = new List<XSDElement>()
            };

            if (interchange.Attributes != null && interchange.Attributes["name"] != null)
                complexType.Name = interchange.Attributes["name"].Value;

            var documentation = interchange.SelectSingleNode("./xs:annotation/xs:documentation", manager);
            if (documentation != null)
                complexType.Definition = documentation.InnerText;

            var entities = interchange.SelectNodes("./xs:complexType/xs:choice/xs:element", manager);
            if (entities != null && entities.Count > 0)
            {
                for (var i = 0; i < entities.Count; i++)
                {
                    var simple = new XSDElement();

                    if (entities[i].Attributes != null && entities[i].Attributes["name"] != null)
                        simple.Name = entities[i].Attributes["name"].Value;

                    if (entities[i].Attributes != null && entities[i].Attributes["type"] != null)
                        simple.Type = entities[i].Attributes["type"].Value;

                    complexType.Elements.Add(simple);

                }
            }
            else throw new Exception("Unable to add {FileName}: Missing xs:schema/xs:element/xs:complexType/xs:choice/xs:element");

            return complexType;
        }

        private MappedSystem GetMappedSystem(Guid mappedSystemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var mappedSystem = _mappedSystemRepository.Get(mappedSystemId);

            if (mappedSystem == null)
                throw new Exception(string.Format("Mapped System with id '{0}' does not exist.", mappedSystemId));

            if (!Principal.Current.IsAdministrator && !mappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            if (!mappedSystem.IsActive)
                throw new Exception(string.Format("Mapped System with id '{0}' is marked as deleted.", mappedSystemId));

            return mappedSystem;
        }
    }
}