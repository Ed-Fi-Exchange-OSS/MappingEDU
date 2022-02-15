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
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Common.Exceptions;
using MappingEdu.Service.MappedSystems;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.MappedSystemExtension;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing mapping project dashboards
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class MappedSystemExtensionController : ControllerBase
    {
        private readonly IMappedSystemExtensionService _mappedSystemExtensionService;
        private static MemoryCache _cache;

        public MappedSystemExtensionController(IMappedSystemExtensionService mappedSystemExtensionService)
        {
            _mappedSystemExtensionService = mappedSystemExtensionService;
            _cache = MemoryCache.Default;
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Extension")]
        [AcceptVerbs("GET")]
        public MappedSystemExtensionModel[] Get(Guid mappedSystemId)
        {
            return _mappedSystemExtensionService.Get(mappedSystemId);
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Has-Extensions")]
        [AcceptVerbs("GET")]
        public bool HasExtensions(Guid mappedSystemId)
        {
            return _mappedSystemExtensionService.HasExtensions(mappedSystemId);
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Extension/{mappedSystemExtensionId:guid}")]
        [AcceptVerbs("GET")]
        public MappedSystemExtensionModel Get(Guid mappedSystemId, Guid mappedSystemExtensionId)
        {
            return _mappedSystemExtensionService.Get(mappedSystemId, mappedSystemExtensionId);
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Linkable-Standards")]
        [AcceptVerbs("GET")]
        public DataStandardNameViewModel[] GetLinkableStandards(Guid mappedSystemId)
        {
            return _mappedSystemExtensionService.GetLinkableStandards(mappedSystemId);
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Extension-Report")]
        [AcceptVerbs("GET")]
        public List<dynamic> GetExtensionReport(Guid mappedSystemId, Guid? parentSystemItemId = null)
        {
            return _mappedSystemExtensionService.GetExtensionReport(mappedSystemId, parentSystemItemId);
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Download-Extension-Report")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage DownloadExtensionReport(Guid mappedSystemId)
        {
            var result = _mappedSystemExtensionService.DownloadableReport(mappedSystemId);

            var token = Guid.NewGuid();
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1) //TODO Add to Configuration (App Settings)
            };
            _cache.Add(token.ToString(), result, policy);

            return Request.CreateResponse(HttpStatusCode.OK, token);
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Download-Extension-Differences")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage DownloadDifferences(Guid mappedSystemId, MappedSystemExtensionDownloadDetailModel model)
        {
            var result = _mappedSystemExtensionService.DownloadDifferences(mappedSystemId, model);

            var token = Guid.NewGuid();
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1) //TODO Add to Configuration (App Settings)
            };
            _cache.Add(token.ToString(), result, policy);

            return Request.CreateResponse(HttpStatusCode.OK, token);
        }

        [AllowAnonymous]
        [Route("MappedSystemExtensionReport/{token:guid}")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetExport(Guid token)
        {
            var result = (HttpResponseMessage) _cache.Get(token.ToString());
            if (result == null)
                throw new NotFoundException("Error locating report id");

            _cache.Remove(token.ToString());

            return result;
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Extension-Linking-Detail")]
        [AcceptVerbs("POST")]
        public MappedSystemExtensionLinkingDetail GetExtensionLinkingDetail(Guid mappedSystemId, MappedSystemExtensionEditModel model)
        {
            return _mappedSystemExtensionService.GetLinkingDetails(mappedSystemId, model);
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Extension-Report-Detail")]
        [AcceptVerbs("POST")]
        public List<MappedSystemExtensionSystemItemModel> GetExtensionReportDetail(Guid mappedSystemId, MappedSystemExtensionDetailRequestModel model)
        {
            return _mappedSystemExtensionService.GetExtensionReportDetail(mappedSystemId, model);
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Extension")]
        [AcceptVerbs("POST")]
        public MappedSystemExtensionModel Post(Guid mappedSystemId, MappedSystemExtensionCreateModel model)
        {
            return _mappedSystemExtensionService.Post(mappedSystemId, model);
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Extension/{mappedSystemExtensionId:guid}")]
        [AcceptVerbs("PUT")]
        public MappedSystemExtensionModel Put(Guid mappedSystemId, Guid mappedSystemExtensionId, MappedSystemExtensionEditModel model)
        {
            return _mappedSystemExtensionService.Put(mappedSystemId, mappedSystemExtensionId, model);
        }

        [Route("MappedSystem/{mappedSystemId:guid}/Extension/{mappedSystemExtensionId:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid mappedSystemId, Guid mappedSystemExtensionId)
        {
            _mappedSystemExtensionService.Delete(mappedSystemId, mappedSystemExtensionId);
        }
    }
}