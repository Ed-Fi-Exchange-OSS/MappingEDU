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
using ClosedXML.Excel;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Services.Import;
using MappingEdu.Service.Model.Import;

namespace MappingEdu.Service.Import
{
    public interface IImportService
    {
        ImportResultModel Import(ImportSchemaModel model);
    }

    public class ImportService : IImportService
    {
        private static MemoryCache _memoryCache;
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly IOpenXmlImportService _openXmlImportService;

        public ImportService(IOpenXmlImportService openXmlImportService, IMappedSystemRepository mappedSystemRepository)
        {
            _openXmlImportService = openXmlImportService;
            _mappedSystemRepository = mappedSystemRepository;
            _memoryCache = MemoryCache.Default;
        }

        public ImportResultModel Import(ImportSchemaModel model)
        {
            var mappedSystem = _mappedSystemRepository.Get(model.MappedSystemId);

            if (!Principal.Current.IsAdministrator && !mappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.Edit))
                throw new SecurityException("User needs at least Edit Access to import into this data standard");

            var result = _openXmlImportService.Import(model.MappedSystemId, model.ImportData, model.OverrideData);
            
            var errors = result.Errors.Select(x => new ImportLog {Message = x, Status = "Error"});
            var warnings = result.Warnings.Select(x => new ImportLog {Message = x, Status = "Warnings"});
            var logs = errors.Union(warnings).ToList();

            if (logs.Count < 20)
                return new ImportResultModel
                {
                    Logs = logs.ToArray(),
                    IsSuccessful = result.Success,
                };

            var token = CreateErrorFile(string.Format("{0} Definition Errors", mappedSystem.SystemName), logs);

            return new ImportResultModel
            {
                Logs = logs.Take(20).ToArray(),
                IsSuccessful = result.Success,
                FileToken = token,
                TotalLogs = logs.Count
            };
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
    }
}
