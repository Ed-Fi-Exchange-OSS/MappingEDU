// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Common.Exceptions;
using MappingEdu.Service.Import;
using MappingEdu.Service.Model.Import;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing element group uploads
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class ImportController : ControllerBase
    {
        private static MemoryCache _memoryCache;
        private readonly IImportService _importService;
        private readonly IImportExtensionsService _extensionsService;
        private readonly IImportOdsApiService _importOdsApiService;

        public ImportController(IImportService importService, IImportExtensionsService extensionsService, IImportOdsApiService odsApiService)
        {
            _importService = importService;
            _extensionsService = extensionsService;
            _importOdsApiService = odsApiService;
            _memoryCache = MemoryCache.Default;
        }

        [Route("Import/{mappedSystemId:guid}/Definition")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> Post(Guid mappedSystemId, bool? overrideData = null)
        {
            var provider = new MultipartMemoryStreamProvider();
            var streamList = new List<Stream>();

            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var file in provider.Contents)
            {
                var dataStream = await file.ReadAsStreamAsync();
                streamList.Add(dataStream);
            }

            var model = new ImportSchemaModel
            {
                ImportData = streamList[0],
                OverrideData = overrideData.HasValue && overrideData.Value,
                MappedSystemId = mappedSystemId
            };

            var resultModel = _importService.Import(model);
            return Request.CreateResponse(HttpStatusCode.OK, resultModel);
        }

        [Route("Import/{mappedSystemId:guid}/Extension")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> ExtensionUpload(Guid mappedSystemId)
        {
            var provider = new MultipartMemoryStreamProvider();
            var streamList = new List<Stream>();

            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var file in provider.Contents)
            {
                var dataStream = await file.ReadAsStreamAsync();
                streamList.Add(dataStream);
            }


            var model = new ImportSchemaModel
            {
                ImportData = streamList[1],
                MappedSystemId = mappedSystemId
            };

            var result = _extensionsService.Import(model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [Route("Import/{mappedSystemId:guid}/OdsApi")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> ODSApiUpload(Guid mappedSystemId, ImportSwaggerSchemaModel model)
        {
            var result = await _importOdsApiService.Import(mappedSystemId, model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [AllowAnonymous]
        [Route("Import/{token:guid}/LogFile")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage DownloadErrorFile(Guid token)
        {
            var result = (HttpResponseMessage)_memoryCache.Get(token.ToString());
            if (result == null)
                throw new NotFoundException("Error locating file. The file either doesn't exist or has expired.");

            return Request.CreateResponse();
        }
    }
}
