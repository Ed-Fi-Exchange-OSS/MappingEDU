// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using ClosedXML.Excel;
using MappingEdu.Core.DataAccess.Util;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.Datatables;
using MappingEdu.Service.Model.Logging;
using MappingEdu.Service.Providers;
using MappingEdu.Service.Util;

namespace MappingEdu.Service.Logging
{
    public interface ILoggingService
    {
        PagedResult<LoggingViewModel> GetPaging(LoggingDataTablesModel model);

        LoggingViewModel Post(LoggingCreateModel model);

        void ClearLogs(LoggingDeleteModel model);

        HttpResponseMessage Export(LoggingDataTablesModel model);

        int ExportCount(LoggingDataTablesModel model);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILoggingProvider<LoggingService> _loggingProvider;
        private readonly ILoggingRepository _loggingRepository;
        private readonly IUserRepository _userRepository;


        public LoggingService(ILoggingProvider<LoggingService> loggingProvider, ILoggingRepository loggingRepository, IUserRepository userRepository)
        {
            _loggingProvider = loggingProvider;
            _loggingRepository = loggingRepository;
            _userRepository = userRepository;
        }

        public HttpResponseMessage Export(LoggingDataTablesModel model)
        {
            if (!Principal.Current.IsAdministrator)
                throw new SecurityException("Only an Admin has access to view the logs");

            var logs = _loggingRepository.GetAllQueryable();

            if (model.StartDate != null)
                logs = logs.Where(x => x.Date >= model.StartDate);

            if (model.EndDate != null)
                logs = logs.Where(x => x.Date <= model.EndDate);

            if (model.Levels != null && model.Levels.Length > 0)
                logs = logs.Where(x => model.Levels.Contains(x.Level));

            if (model.search != null && model.search.value != null && model.search.value != "")
                logs = logs.Where(x =>
                    x.Message.ToLower().Contains(model.search.value.ToLower()) ||
                    x.User.ToLower().Contains(model.search.value.ToLower()) ||
                    x.Level.ToLower().Contains(model.search.value.ToLower()));

            var column = new DatatablesOrder() { column = 3, dir = "desc" };
            if (model.order != null && model.order.Count > 0) column = model.order[0];

            switch (column.column)
            {
                case 0: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.Level) : logs.OrderBy(x => x.Level); break;
                case 1: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.User) : logs.OrderBy(x => x.User); break;
                case 2: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.Message) : logs.OrderBy(x => x.Message); break;
                case 3: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.Date) : logs.OrderBy(x => x.Date); break;
                default: logs = logs.OrderByDescending(x => x.Date); break;
            }
            
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Logs");

            var i = 2;
            foreach (var log in logs)
            {
                worksheet.Cell(i, "A").Value = log.Level;
                worksheet.Cell(i, "B").Value = log.User;
                worksheet.Cell(i, "C").Value = log.Message;
                worksheet.Cell(i, "D").Value = log.Date.ToString("MMM dd, yyyy hh:mm tt");

                i++;
            }

            worksheet.Cell(1, "A").Value = "Level";
            worksheet.Cell(1, "B").Value = "User";
            worksheet.Cell(1, "C").Value = "Message";
            worksheet.Cell(1, "D").Value = "Date";

            worksheet.Columns().AdjustToContents();
            foreach (var col in worksheet.Columns().Where(col => col.Width > 150)) col.Width = 150;

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ms)
            };

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Utility.RemoveInvalidCharacters(string.Format("Logs.{0}.xlsx", DateTime.Now.ToShortDateString().Replace("/", "-")))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = ms.Length;
            return result;

        }

        public int ExportCount(LoggingDataTablesModel model)
        {
            var logs = _loggingRepository.GetAllQueryable();

            if (model.StartDate != null)
                logs = logs.Where(x => x.Date >= model.StartDate);

            if (model.EndDate != null)
                logs = logs.Where(x => x.Date <= model.EndDate);

            if (model.Levels != null && model.Levels.Length > 0)
                logs = logs.Where(x => model.Levels.Contains(x.Level));

            if (model.search != null && model.search.value != null && model.search.value != "")
                logs = logs.Where(x =>
                    x.Message.ToLower().Contains(model.search.value.ToLower()) ||
                    x.User.ToLower().Contains(model.search.value.ToLower()) ||
                    x.Level.ToLower().Contains(model.search.value.ToLower()));

            var column = new DatatablesOrder() { column = 3, dir = "desc" };
            if (model.order != null && model.order.Count > 0) column = model.order[0];

            switch (column.column)
            {
                case 0: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.Level) : logs.OrderBy(x => x.Level); break;
                case 1: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.User) : logs.OrderBy(x => x.User); break;
                case 2: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.Message) : logs.OrderBy(x => x.Message); break;
                case 3: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.Date) : logs.OrderBy(x => x.Date); break;
                default: logs = logs.OrderByDescending(x => x.Date); break;
            }

            return logs.Count();
        }

        public PagedResult<LoggingViewModel> GetPaging(LoggingDataTablesModel model)
        {
            if(!Principal.Current.IsAdministrator)
                throw new SecurityException("Only an Admin has access to view the logs");

            var logs = _loggingRepository.GetAllQueryable();

            var total = logs.Count();

            if (model.StartDate != null)
                logs = logs.Where(x => x.Date >= model.StartDate);

            if (model.EndDate != null)
                logs = logs.Where(x => x.Date <= model.EndDate);

            if (model.Levels != null && model.Levels.Length > 0)
                logs = logs.Where(x => model.Levels.Contains(x.Level));

            if(model.search != null && model.search.value != null)
                logs = logs.Where(x =>
                    x.Message.ToLower().Contains(model.search.value.ToLower()) ||
                    x.User.ToLower().Contains(model.search.value.ToLower()) ||
                    x.Level.ToLower().Contains(model.search.value.ToLower()));

            var filtered = logs.Count();

            var column = new DatatablesOrder() {column = 3, dir = "desc"};
            if (model.order != null && model.order.Count > 0) column = model.order[0];

            switch (column.column)
            {
                case 0: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.Level) : logs.OrderBy(x => x.Level); break;
                case 1: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.User) : logs.OrderBy(x => x.User); break;
                case 2: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.Message) : logs.OrderBy(x => x.Message); break;
                case 3: logs = column.dir == "desc" ? logs.OrderByDescending(x => x.Date) : logs.OrderBy(x => x.Date); break;
                default: logs = logs.OrderByDescending(x => x.Date); break;
            }

            logs = logs.Skip(model.start).Take(model.length);

            return new PagedResult<LoggingViewModel>()
            {
                Items = logs.Select(x =>
                    new LoggingViewModel()
                    {
                        Date = x.Date,
                        Level = x.Level,
                        Message = x.Message,
                        User = x.User
                    }).ToArray(),
                TotalFiltered = filtered,
                TotalRecords = total
            };
        }

        public LoggingViewModel Post(LoggingCreateModel model)
        {
            switch (model.Level)
            {
                case SeverityLevel.Debug:
                    _loggingProvider.LogDebug(string.Format("{0} => {1}", model.Source, model.Message));
                    break;
                case SeverityLevel.Info:
                    _loggingProvider.LogInfo(string.Format("{0} => {1}", model.Source, model.Message));
                    break;
                case SeverityLevel.Warn:
                    _loggingProvider.LogWarn(string.Format("{0} => {1}", model.Source, model.Message));
                    break;
                case SeverityLevel.Error:
                    _loggingProvider.LogError(string.Format("{0} => {1}", model.Source, model.Message));
                    break;
                case SeverityLevel.Fatal:
                    _loggingProvider.LogFatal(string.Format("{0} => {1}", model.Source, model.Message));
                    break;
                default:
                    _loggingProvider.LogInfo(string.Format("{0} => {1}", model.Source, model.Message));
                    break;
            }
            return new LoggingViewModel();
        }

        public void ClearLogs(LoggingDeleteModel model)
        {
            if (!Principal.Current.IsAdministrator)
                throw new SecurityException("Must be an admin to clear logs");

            var deleteLogs = _loggingRepository.GetAllQueryable().Where(x => x.Date <= model.DeleteDate);

            var user = _userRepository.AuthenticateUser(Principal.Current.Username, model.Password);
            if(user == null) 
                throw new SecurityException("Incorrect Password");

            _loggingRepository.DeleteRange(deleteLogs);
            _loggingRepository.SaveChanges();

        }
    }
}