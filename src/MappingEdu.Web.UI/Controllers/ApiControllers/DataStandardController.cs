// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Common.Exceptions;
using MappingEdu.Service.Export;
using MappingEdu.Service.MappedSystems;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.Import;
using MappingEdu.Service.Util;
using Newtonsoft.Json.Linq;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing data standards
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class DataStandardController : ControllerBase
    {
        private static MemoryCache _exportCache;
        private readonly IDataStandardService _dataStandardService;
        private readonly IDataStandardCloneService _cloneService;
        private readonly IExportService _exportService;

        public DataStandardController(IDataStandardService dataStandardService, IExportService exportService, IDataStandardCloneService cloneService)
        {
            _dataStandardService = dataStandardService;
            _cloneService = cloneService;
            _exportCache = MemoryCache.Default;
            _exportService = exportService;
        }

        [Route("DataStandard/{id:guid}/clone")]
        [AcceptVerbs("POST")]
        public DataStandardViewModel Clone(Guid id, DataStandardCloneModel model)
        {
            return _cloneService.Clone(id, model);
        }

        [Route("DataStandard/{id:guid}/interchange")]
        [AcceptVerbs("POST")]
        public async Task<ImportResultModel> Interchange(Guid id)
        {
            var provider = new MultipartMemoryStreamProvider();
            var streamList = new List<Stream>();

            await Request.Content.ReadAsMultipartAsync(provider);

            var baseStandardId = new Guid();

            var first = true;
            foreach (var file in provider.Contents)
            {
                var dataStream = await file.ReadAsStreamAsync();
                if (!first) streamList.Add(dataStream);
                else
                {
                    first = false;
                    var buffer = new byte[2048];
                    dataStream.Read(buffer, 0, (int)dataStream.Length);
                    dynamic data = JObject.Parse(Encoding.Default.GetString(buffer));

                    baseStandardId = data.BaseStandardId;
                }
            }

            return _cloneService.CreateInterchanges(id, baseStandardId, streamList);
        }

        [Route("DataStandard/{id:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id)
        {
            _dataStandardService.Delete(id);
        }

        [AllowAnonymous]
        [Route("DataStandard/{id:guid}/export/{exportId:guid}")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage Export(Guid id, Guid exportId)
        {
            var result = (HttpResponseMessage) _exportCache.Get(exportId.ToString());
            if (result == null)
                throw new NotFoundException("Error locating export id");

            return result;
        }

        [Route("DataStandard")]
        [AcceptVerbs("GET")]
        public DataStandardViewModel[] Get()
        {
            return _dataStandardService.Get();
        }

        [Route("DataStandard/{id:guid}")]
        [AcceptVerbs("GET")]
        public DataStandardViewModel Get(Guid id)
        {
            return _dataStandardService.Get(id);
        }

        [Route("DataStandard/{id:guid}/is-extended")]
        [AcceptVerbs("GET")]
        public bool GetIsExtended(Guid id)
        {
            return _dataStandardService.IsExtended(id);
        }

        [Route("DataStandard/{id:guid}/owners")]
        [AcceptVerbs("GET")]
        public List<string> GetOwners(Guid id)
        {
            return _dataStandardService.GetOwners(id);
        }

        [Route("DataStandard/{id:guid}/creator")]
        [AcceptVerbs("GET")]
        public string GetCreator(Guid id)
        {
            return _dataStandardService.GetCreator(id);
        }



        [Route("DataStandard/without-next-versions")]
        [AcceptVerbs("GET")]
        public DataStandardViewModel[] GetAllWithoutNextVersions()
        {
            return _dataStandardService.GetAllWithoutNextVersions();
        }

        [Route("DataStandard/{id:guid}/export/token")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetExportToken(Guid id)
        {
            var token = Guid.NewGuid();
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1) //TODO Add to Configuration (App Settings)
            };

            var standard = _dataStandardService.Get(id);
            if (null == standard)
                throw new NotFoundException("Data standard not found");

            var templatePath = HttpContext.Current.Server.MapPath("~/Templates/Export/SystemExport_Master.xlsx");
            var stream = _exportService.ExportMappedSystem(id, templatePath);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Utility.RemoveInvalidCharacters(string.Format("{0}.{1}.xlsx", standard.SystemName, DateTime.Now.ToShortDateString().Replace("/", "-")))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = stream.Length;

            _exportCache.Add(token.ToString(), result, policy);

            return Request.CreateResponse(HttpStatusCode.OK, token);
        }

        [Route("DataStandard/orphaned")]
        [AcceptVerbs("GET")]
        public DataStandardViewModel[] GetOrphaned()
        {
            return _dataStandardService.Get(true);
        }

        [Route("DataStandard/public")]
        [AcceptVerbs("GET")]
        public DataStandardViewModel[] GetPublic()
        {
            return _dataStandardService.GetPublic();
        }

        [Route("DataStandard")]
        [AcceptVerbs("POST")]
        public DataStandardViewModel Post(DataStandardCreateModel model)
        {
            return _dataStandardService.Post(model);
        }

        [Route("DataStandard/{id:guid}")]
        [AcceptVerbs("PUT")]
        public DataStandardViewModel Put(Guid id, DataStandardEditModel value)
        {
            return _dataStandardService.Put(id, value);
        }

        [Route("DataStandard/{systemId:guid}/toggle-public")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage TogglePublic(Guid systemId)
        {
            _dataStandardService.TogglePublic(systemId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("DataStandard/{systemId:guid}/toggle-public-extensions")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage TogglePublicExtensions(Guid systemId)
        {
            _dataStandardService.TogglePublicExtensions(systemId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}