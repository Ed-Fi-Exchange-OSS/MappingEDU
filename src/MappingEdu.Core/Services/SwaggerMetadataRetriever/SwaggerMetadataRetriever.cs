// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AutoMapper.Internal;
using log4net;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable AccessToDisposedClosure

namespace MappingEdu.Core.Services.SwaggerMetadataRetriever
{
    public class SwaggerMetadataRetriever
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SwaggerMetadataRetriever).Name);

        private readonly IApiMetadataConfiguration _configuration;

        public SwaggerMetadataRetriever(IApiMetadataConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ICollection<JsonModelMetadata>> LoadMetadata()
        {

            //prime the pipeline
            var metadata = new List<Metadata>();
            try
            {
                string json;
                json = await LoadJsonString("");
                metadata = JsonConvert.DeserializeObject<Metadata[]>(json).ToList();
            }
            catch
            {
                metadata.Add(new Metadata { sectionPrefix = null, value = "Resources"});
                metadata.Add(new Metadata { sectionPrefix = null, value = "Descriptors" });
                metadata.Add(new Metadata { sectionPrefix = null, value = "Types" });
                metadata.Add(new Metadata { sectionPrefix = null, value = "Other" });
            }

            if (!_configuration.ImportAll)
            {
                var groups = new[] { "resources", "descriptors", "types" };
                metadata = metadata.Where(x => groups.Contains(x.MetadataName.ToLower())).ToList();
            }

            // swagger v2.0
            if (metadata.Any(x => x.EndpointUri != null))
                return await LoadMetadataV20(metadata);

            // swagger v1.2
            return await LoadMetadataV12(metadata);
        }

        private async Task<ICollection<JsonModelMetadata>> LoadMetadataV12(IEnumerable<Metadata> metadata)
        {
            var results = new List<JsonModelMetadata>();

            var metadataBlock = new BufferBlock<Metadata>();
            var apidocsBlock = new TransformManyBlock<Metadata, Api>(async x =>
            {
                var path = String.Format("{0}/api-docs", x.MetadataName);

                var j = await LoadJsonString(path);
                var docs = JsonConvert.DeserializeObject<ApiDoc>(j);

                try
                {
                    return docs.apis.Select(y => new Api
                    {
                        description = y.description,
                        path = String.Format("{0}{1}", path, y.path)
                    });
                }
                catch
                {
                    // ignored
                }

                path = String.Format("{0}/api-docs", x.MetadataName.ToLower());
                j = await LoadJsonString(path);
                docs = JsonConvert.DeserializeObject<ApiDoc>(j);
                return docs.apis.Select(y => new Api
                {
                    description = y.description,
                    path = String.Format("{0}{1}", path, y.path)
                });
            });
            var resourcesBlock = new TransformManyBlock<Api, JsonModelMetadata>(async x =>
            {
                var j = await LoadJsonString(x.path);
                j = j.Replace("\"$ref\"", "\"ref\"");
                var parts = x.path.Split(Path.AltDirectorySeparatorChar);
                var resources = JsonConvert.DeserializeObject<SwaggerV12Resource>(j, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

                var types = (from model in resources.models
                             from property in model.Value.properties
                             select property.Value.type == "array" ? property.Value.items.reference : property.Value.type).Distinct();

                var result = (from model in resources.models
                              from property in model.Value.properties
                              select new JsonModelMetadata
                              {
                                  Category = parts[0],
                                  Resource = parts[2],
                                  ResourceDescription = x.description,
                                  Model = model.Key,
                                  IsReference = types.Contains(model.Key),
                                  Property = property.Key,
                                  Type = property.Value.type == "array" ? property.Value.items.reference : property.Value.type,
                                  IsArray = property.Value.type == "array",
                                  IsRequired = property.Value.required,
                                  Description = property.Value.description
                              }).ToList();

                return result;
            });
            var outputBlock = new ActionBlock<JsonModelMetadata>(x =>
            {
                results.Add(x);
            });

            //link blocks
            metadataBlock.LinkTo(apidocsBlock, new DataflowLinkOptions { PropagateCompletion = true });
            apidocsBlock.LinkTo(resourcesBlock, new DataflowLinkOptions { PropagateCompletion = true });
            resourcesBlock.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });

            foreach (var m in metadata) metadataBlock.Post(m);

            metadataBlock.Complete();

            await outputBlock.Completion;
            return results;
        }

        private async Task<ICollection<JsonModelMetadata>> LoadMetadataV20(IEnumerable<Metadata> metadata)
        {
            var results = new List<JsonModelMetadata>();

            var metadataBlock = new BufferBlock<Metadata>();
            var swaggerBlock = new TransformManyBlock<Metadata, JsonModelMetadata>(async x =>
            {
                var j = await LoadJsonString(x.EndpointUri);
                j = j.Replace("\"$ref\"", "\"ref\"");

                var swaggerResource = JsonConvert.DeserializeObject<SwaggerV20Resource>(j, new JsonSerializerSettings {PreserveReferencesHandling = PreserveReferencesHandling.Objects});

                var entities = (from path in swaggerResource.paths.Where(path => !path.Key.EndsWith("{id}"))
                                from parameter in path.Value.post.parameters
                                join definition in swaggerResource.definitions
                                on parameter.schema.reference equals $"#/definitions/{definition.Key}"
                                from pathTag in path.Value.post.tags
                                join tag in swaggerResource.tags
                                on pathTag equals tag.name

                                select new
                                {
                                    parameter = parameter,
                                    definition = definition,
                                    tag = tag
                                }).ToList();

                var models = new List<JsonModelMetadata>();

                foreach (var entity in entities)
                {
                    var splitDefinition = entity.definition.Key.Split('_');
                    var model = splitDefinition.Length > 1 ? splitDefinition[1] : splitDefinition[0];
                    var isExtension = splitDefinition[0] != "edFi" && splitDefinition.Length > 1;

                    foreach (var property in entity.definition.Value.properties)
                    {
                        var type = property.Value.type;

                        var isPropertyExtension = property.Key == "_ext";

                        if (property.Value.reference != null)
                        {
                            type = property.Value.reference;
                            models.AddRange(ResolveReferences(swaggerResource, x.MetadataName, entity.tag.name, property.Value.reference, isExtension || isPropertyExtension));
                        }
                        else if (property.Value.type == "array")
                        {
                            type = property.Value.items.reference;
                            models.AddRange(ResolveReferences(swaggerResource, x.MetadataName, entity.tag.name, property.Value.items.reference, isExtension || isPropertyExtension));
                        }

                        var splitDefinitionType = type.Split(new[] { "#/definitions/" }, StringSplitOptions.None);
                        var readableType = splitDefinitionType.Length > 1 ? splitDefinitionType[1] : splitDefinitionType[0];

                        models.Add(new JsonModelMetadata
                        {
                            Category = x.MetadataName,
                            Resource = entity.tag.name,
                            ResourceDescription = entity.tag.description,
                            Model = model,
                            IsResourceExtension = isExtension,
                            IsReference = false,
                            IsExtension = isExtension || isPropertyExtension,
                            Property = property.Key,
                            Type = readableType,
                            IsArray = property.Value.type == "array",
                            IsRequired = entity.definition.Value.required != null && entity.definition.Value.required.Contains(property.Key),
                            Description = property.Value.description
                        });
                    }
                }

                return models;
            });
            var outputBlock = new ActionBlock<JsonModelMetadata>(x =>
            {
                results.Add(x);
            });

            //link blocks
            metadataBlock.LinkTo(swaggerBlock, new DataflowLinkOptions { PropagateCompletion = true });
            swaggerBlock.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });

            foreach (var m in metadata) metadataBlock.Post(m);

            metadataBlock.Complete();
            await outputBlock.Completion;

            return results;
        }

        private ICollection<JsonModelMetadata> ResolveReferences(SwaggerV20Resource swaggerResource, string category, string resource, string reference, bool isParentExtension)
        {
            var referenceDefinition = swaggerResource.definitions.First(x => reference == $"#/definitions/{x.Key}");
            var references = new List<JsonModelMetadata>();

            var model = referenceDefinition.Key;

            var splitDefinition = referenceDefinition.Key.Split('_');
            var isExtension = isParentExtension || (splitDefinition[0] != "edFi" && splitDefinition.Length > 1);

            foreach (var property in referenceDefinition.Value.properties)
            {
                var type = property.Value.type;
                var isPropertyExtension = property.Key == "_ext";

                if (property.Value.reference != null)
                {
                    type = property.Value.reference;
                    references.AddRange(ResolveReferences(swaggerResource, category, resource, property.Value.reference, isExtension || isPropertyExtension));
                }
                else if (property.Value.type == "array")
                {
                    type = property.Value.items.reference;
                    references.AddRange(ResolveReferences(swaggerResource, category, resource, property.Value.items.reference, isExtension || isPropertyExtension));
                }

                var splitDefinitionType = type.Split(new[] { "#/definitions/" }, StringSplitOptions.None);
                var readableType = splitDefinitionType.Length > 1 ? splitDefinitionType[1] : splitDefinitionType[0] ;

                references.Add(new JsonModelMetadata
                {
                    Category = category,
                    Resource = resource,
                    ResourceDescription = null,
                    Model = model,
                    IsReference = true,
                    IsExtension = isExtension || isPropertyExtension,
                    Property = property.Key,
                    Type = readableType,
                    IsArray = property.Value.type == "array",
                    IsRequired = referenceDefinition.Value.required != null && referenceDefinition.Value.required.Contains(property.Key),
                    Description = property.Value.description
                });
            }

            return references;
        }

        private async Task<string> LoadJsonString(string localUrl)
        {
            using (var client = new HttpClient { Timeout = new TimeSpan(0, 0, 5, 0) })
            {
                var url = localUrl.Contains("http") ? localUrl : string.Format("{0}/{1}", _configuration.Url, localUrl);

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
