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
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Core.Services.SwaggerMetadataRetriever;
using MappingEdu.Service.Model.Import;
using Newtonsoft.Json;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.Import
{
    public class SwaggerAppSettings
    {
        public string webApiMetadataUrl { get; set; }
        public string adminUrl { get; set; }
        public string webApi { get; set; }
        public bool loaded { get; set; }
    }

    public interface IImportOdsApiService
    {
        Task<ICollection<JsonModelMetadata>> Import(Guid mappedSystemId, ImportSwaggerSchemaModel model);
    }

    public class ImportOdsApiService : IImportOdsApiService
    {
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly ISystemItemRepository _systemItemRepository;

        public ImportOdsApiService(IMappedSystemRepository mappedSystemRepository, ISystemItemRepository systemItemRepository)
        {
            _mappedSystemRepository = mappedSystemRepository;
            _systemItemRepository = systemItemRepository;
        }


        public async Task<ICollection<JsonModelMetadata>> Import(Guid mappedSystemId, ImportSwaggerSchemaModel model)
        {
            var mappedSystem = _mappedSystemRepository.Get(mappedSystemId);

            if (!Principal.Current.IsAdministrator && ! mappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.Edit))
                throw new SecurityException("User needs at least Edit Access to import into this data standard");

            // Ideally should upgrade to .NetFramework 4.7 or higher to support latest TLS
            // https://docs.microsoft.com/en-us/dotnet/framework/network-programming/tls
            // For now, explicitly configure all outgoing network calls to use latest version of TLS where possible
            ConfigureTls();

            var metadataUrl = CreateMetadataUrl(model.Url);

            IApiMetadataConfiguration configuration = new Configuration
            {
                Url = new UriBuilder(metadataUrl),
                ImportAll = model.ImportAll
            };

            var metadataRetriever = new SwaggerMetadataRetriever(configuration);
            var results = (await metadataRetriever.LoadMetadata())
                .Where(x => !(x.Model != null && (x.Model == "webServiceError" || x.Model == "link"))
                         && !(x.Property != null && (x.Property == "_etag" || x.Property == "link"))).ToList();

            var isRequiredDetail = mappedSystem.CustomDetailMetadata.FirstOrDefault(x => x.DisplayName == "Is Required");
            if (isRequiredDetail == null)
            {
                isRequiredDetail = new CustomDetailMetadata
                {
                    MappedSystemId = mappedSystemId,
                    DisplayName = "Is Required",
                    IsBoolean = true,
                    SystemItemCustomDetails = new List<SystemItemCustomDetail>()
                };
            }

            var systemItems = _systemItemRepository.GetAllQueryable().Where(x => x.IsActive && x.ParentSystemItemId == null && x.MappedSystemId == mappedSystemId).ToList();

            foreach (var result in results.Where(x => !x.IsReference))
            {
                var domain = systemItems.FirstOrDefault(x => string.Equals(x.ItemName, result.Category, StringComparison.CurrentCultureIgnoreCase) && x.ParentSystemItemId == null && x.ItemTypeId == ItemType.Domain.Id);
                if (domain == null)
                {
                    domain = new SystemItem
                    {
                        ItemTypeId = ItemType.Domain.Id,
                        ItemName = char.ToUpper(result.Category[0]) + result.Category.Substring(1),
                        MappedSystemId = mappedSystemId,
                        IsActive = true,
                        ChildSystemItems = new List<SystemItem>()
                    };
                    systemItems.Add(domain);
                }

                var entity = domain.ChildSystemItems.FirstOrDefault(x => x.ItemName == result.Resource);
                if (entity == null)
                {
                    entity = new SystemItem
                    {
                        ItemTypeId = ItemType.Entity.Id,
                        ItemName = result.Resource,
                        Definition = result.ResourceDescription,
                        MappedSystemId = mappedSystemId,
                        IsActive = true,
                        ChildSystemItems = new List<SystemItem>(),
                        IsExtended = result.IsResourceExtension
                    };
                   domain.ChildSystemItems.Add(entity);
                }

                entity.Definition = result.ResourceDescription;
                entity.IsExtended = result.IsResourceExtension;

                var subEntity = entity.ChildSystemItems.FirstOrDefault(x => x.ItemName == result.Model);
                if (subEntity == null)
                {
                    subEntity = new SystemItem
                    {
                        ItemTypeId = ItemType.SubEntity.Id,
                        ItemName = result.Model,
                        MappedSystemId = mappedSystemId,
                        IsActive = true,
                        ChildSystemItems = new List<SystemItem>()
                    };
                    entity.ChildSystemItems.Add(subEntity);
                }

                subEntity.IsExtended = result.IsResourceExtension;

                if (result.IsSimpleType)
                {
                    var element = subEntity.ChildSystemItems.FirstOrDefault(x => x.ItemName == result.Property);
                    if (element == null)
                    {
                        subEntity.ChildSystemItems.Add(new SystemItem
                        {
                            ItemTypeId = ItemType.Element.Id,
                            ItemName = result.Property,
                            DataTypeSource = result.Type,
                            Definition = result.Description,
                            IsActive = true,
                            MappedSystemId = mappedSystemId,
                            IsExtended = result.IsResourceExtension || result.IsExtension,
                            SystemItemCustomDetails = new List<SystemItemCustomDetail>
                            {
                                new SystemItemCustomDetail
                                {
                                    Value = result.IsRequired ? "1" : "0",
                                    CustomDetailMetadata = isRequiredDetail,
                                }
                            }
                        });
                    }
                    else
                    {
                        element.Definition = result.Description;
                        element.DataTypeSource = result.Type;
                        element.IsExtended = result.IsResourceExtension || result.IsExtension;
                    }
                }
                else GetSubEntity(mappedSystemId, subEntity, result, results, isRequiredDetail);

            }

            foreach (var item in systemItems.Where(x => x.SystemItemId == Guid.Empty))
            {
                _systemItemRepository.Add(item);
            }

            _systemItemRepository.SaveChanges();
            
            return results;
        }

        private string CreateMetadataUrl(string url)
        {
            url = url.Trim();
            url = url.TrimEnd('/');
            var trailingRoute = url.Split('/').Last();

            if (string.Equals(trailingRoute, "metadata", StringComparison.CurrentCultureIgnoreCase))
            {
                return url;
            }

            return $"{url}/metadata";
        }

        private void ConfigureTls()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        private void GetSubEntity(Guid mappedSystemId, SystemItem entity, JsonModelMetadata model, ICollection<JsonModelMetadata> models, CustomDetailMetadata isRequiredDetail)
        {
            var subEntity = entity.ChildSystemItems.FirstOrDefault(x => x.ItemName == model.Property);
            if (subEntity == null)
            {
                subEntity = new SystemItem
                {
                    ItemName = model.Property,
                    ItemTypeId = ItemType.SubEntity.Id,
                    MappedSystemId = mappedSystemId,
                    IsActive = true,
                    Definition = model.Description,
                    ChildSystemItems = new List<SystemItem>(),
                    IsExtended = model.IsExtension || model.IsResourceExtension
                };
                entity.ChildSystemItems.Add(subEntity);
            }
            else
            {
                subEntity.Definition = model.Description;
                subEntity.IsExtended = model.IsExtension || model.IsResourceExtension;
            }

            var subModels = models.Where(x => x.Category == model.Category && x.Resource == model.Resource && x.Model == model.Type);
            foreach (var subModel in subModels)
            {
                if (subModel.IsSimpleType)
                {
                    var element = subEntity.ChildSystemItems.FirstOrDefault(x => x.ItemName == subModel.Property);
                    if (element == null)
                    {
                        subEntity.ChildSystemItems.Add(new SystemItem
                        {
                            ItemTypeId = ItemType.Element.Id,
                            ItemName = subModel.Property,
                            DataTypeSource = subModel.Type,
                            Definition = subModel.Description,
                            IsActive = true,
                            MappedSystemId = mappedSystemId,
                            IsExtended = model.IsExtension || model.IsResourceExtension || subModel.IsExtension,
                            SystemItemCustomDetails = new List<SystemItemCustomDetail>
                            {
                                new SystemItemCustomDetail
                                {
                                    Value = subModel.IsRequired ? "1" : "0",
                                    CustomDetailMetadata = isRequiredDetail,
                                }
                            }
                        });
                    }
                    else
                    {
                        element.Definition = subModel.Description;
                        element.DataTypeSource = subModel.Type;
                        element.IsExtended = model.IsExtension || model.IsResourceExtension || subModel.IsExtension;
                    }
                }
                else GetSubEntity(mappedSystemId, subEntity, subModel, models, isRequiredDetail);
            }
        }

        internal class Configuration : IApiMetadataConfiguration
        {
            public bool Force
            {
                get { return true; }
                set { throw new NotImplementedException(); }
            }

            public object Url { get; set; }

            public bool ImportAll { get; set; }
        }
    }
}