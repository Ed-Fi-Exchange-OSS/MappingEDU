// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using ClosedXML.Excel;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.MappedSystemExtension;
using MappingEdu.Service.Util;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.MappedSystems
{
    public interface IMappedSystemExtensionService
    {
        void Delete(Guid mappedSystemId, Guid mappedSystemExtensionId);

        HttpResponseMessage DownloadableReport(Guid mappedSystemId);

        HttpResponseMessage DownloadDifferences(Guid mappedSystemId, MappedSystemExtensionDownloadDetailModel model);

        MappedSystemExtensionModel[] Get(Guid mappedSystemId);

        MappedSystemExtensionModel Get(Guid mappedSystemId, Guid mappedSystemExtensionId);

        List<dynamic> GetExtensionReport(Guid mappedSystemId, Guid? parentMappedSystemId);

        List<MappedSystemExtensionSystemItemModel> GetExtensionReportDetail(Guid mappedSystemId, MappedSystemExtensionDetailRequestModel model);

        DataStandardNameViewModel[] GetLinkableStandards(Guid mappedSystemId);

        MappedSystemExtensionLinkingDetail GetLinkingDetails(Guid mappedSystemId, MappedSystemExtensionEditModel model);

        bool HasExtensions(Guid mappedSystemId);

        MappedSystemExtensionModel Post(Guid mappedSystemId, MappedSystemExtensionCreateModel model);

        MappedSystemExtensionModel Put(Guid mappedSystemId, Guid mappedSystemExtensionId, MappedSystemExtensionEditModel model);
    }

    public class MappedSystemExtensionService : IMappedSystemExtensionService
    {
        private readonly IRepository<MappedSystemExtension> _mappedSystemExtensionRepository;
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly ISystemItemRepository _systemItemRepository;

        public MappedSystemExtensionService(IRepository<MappedSystemExtension> mappedSystemExtensionRepository, IMappedSystemRepository mappedSystemRepository, ISystemItemRepository systemItemRepository)
        {
            _mappedSystemExtensionRepository = mappedSystemExtensionRepository;
            _mappedSystemRepository = mappedSystemRepository;
            _systemItemRepository = systemItemRepository;
        }

        public void Delete(Guid mappedSystemId, Guid mappedSystemExtensionId)
        {
            GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner);

            if (!_mappedSystemExtensionRepository.GetAllQueryable().Any(x => x.MappedSystemId == mappedSystemId && x.MappedSystemExtensionId == mappedSystemExtensionId))
                throw new NotFoundException(string.Format("Unable to find Mapped System Exception with id {0} and Mapped System Id {1}", mappedSystemExtensionId, mappedSystemId));

            var deleteSystemItems = _systemItemRepository.GetAllQueryable().Where(x => x.MappedSystemExtensionId == mappedSystemExtensionId);
            _systemItemRepository.RemoveRange(deleteSystemItems);

            _mappedSystemExtensionRepository.Delete(mappedSystemExtensionId);
            _mappedSystemExtensionRepository.SaveChanges();
        }

        public HttpResponseMessage DownloadableReport(Guid mappedSystemId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner, MappedSystemUser.MappedSystemUserRole.Guest);
            var elementGroups = _systemItemRepository.GetAllQueryable().Where(x => x.IsActive && !x.ParentSystemItemId.HasValue && x.MappedSystemId == mappedSystemId).ToList();
            var extensions = _mappedSystemExtensionRepository.GetAllQueryable().Where(x => x.MappedSystemId == mappedSystemId).OrderBy(x => x.ShortName).ToList();

            var workbook = new XLWorkbook();
            var coreWorksheet = workbook.Worksheets.Add("Core");
            var associationWorksheet = workbook.Worksheets.Add("Association");
            var newWorksheet = workbook.Worksheets.Add("New");

            var coreIndex = 1;
            var associationIndex = 1;
            var newIndex = 1;

            var headerColors = new List<string> { "#38B5E6", "#F8972A", "#63BAA9", "#E41564", "#0E76BD", "#FFC528", "#AC238D" };
            var cellColors1 = new List<string> { "#bbe6f7", "#fddbb5", "#cae8e2", "#f8b9d1", "#b8dffa", "#ffebb3", "#f2c0e6" };
            var cellColors2 = new List<string> { "#a4ddf4", "#fccf9c", "#b8e0d8", "#f6a2c2", "#a0d5f8", "#ffe499", "#eeaade" };

            var extensionSystemItems = _systemItemRepository
                .GetAllQueryable()
                .Where(x => x.IsActive && 
                            x.MappedSystemId == mappedSystemId && 
                            x.MappedSystemExtensionId.HasValue
                            && (x.ItemTypeId == ItemType.Element.Id || x.ItemTypeId == ItemType.Enumeration.Id)).ToList();

            var height = .1;
            foreach (var group in elementGroups.OrderBy(x => x.ItemName))
            {
                foreach (var row in _mappedSystemRepository.GetExtensionReportDetail(mappedSystemId, group.SystemItemId))
                {
                    IXLWorksheet worksheet;
                    int i;

                    if (row.ShortName != null && row.ShortName != "")
                    {
                        worksheet = newWorksheet;
                        i = newIndex;
                    }
                    else if (((string) row.ItemName).ToLower().EndsWith("association") || ((string)row.ItemName).ToLower().EndsWith("associations"))
                    {
                        worksheet = associationWorksheet;
                        i = associationIndex;
                    }
                    else
                    {
                        worksheet = coreWorksheet;
                        i = coreIndex;
                    }

                    worksheet.Cell(i, "A").Value = (row.ShortName != null && row.ShortName != "") ? row.ShortName : "Core";
                    worksheet.Cell(i, "B").Value = group.ItemName;
                    worksheet.Cell(i, "C").Value = row.ItemName;

                    var total = 0;
                    for (var j = 0; j < extensions.Count; j++)
                    {
                        var cell = worksheet.Cell(i, j + 4);

                        var value = (int) ((IDictionary<string, object>) row).First(x => x.Key == extensions[j].ShortName).Value;
                        if (value > 0)
                        {
                            cell.Value = value;
                            foreach (var item in extensionSystemItems.Where(x => x.DomainItemPathIds.ToLower().Contains(row.SystemItemId.ToString().ToLower()) && x.MappedSystemExtensionId == extensions[j].MappedSystemExtensionId))
                            {
                                var trimLength = group.ItemName.Length + 1 + ((string) row.ItemName).Length + 1;
                                var newPath = item.DomainItemPath.Substring(trimLength);
                                cell.Comment.AddText(newPath);
                                cell.Comment.AddNewLine();
                            }

                            worksheet.Cell(i, j + 4).Comment.Style.Size.SetAutomaticSize(true);
                        }

                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml(i % 2 == 1 ? cellColors1[j % 7] : cellColors2[j % 7]);

                        total += value;
                    }

                    if (total == 0)
                    {
                        worksheet.Row(i).Delete();
                    }
                    else
                    {
                        if (row.ShortName != null && row.ShortName != "")
                            newIndex++;
                        else if (((string) row.ItemName).ToLower().EndsWith("association") || ((string) row.ItemName).ToLower().EndsWith("associations"))
                            associationIndex++;
                        else
                        {
                            coreIndex++;
                            height += .1;
                        }
                    }
                }
            }

            SetUpWorksheet(newWorksheet, newIndex, extensions);
            SetUpWorksheet(associationWorksheet, associationIndex, extensions);
            SetUpWorksheet(coreWorksheet, coreIndex, extensions);

            for(var j = 0; j < extensions.Count; j++)
            {
                var worksheet = workbook.Worksheets.Add(extensions[j].ShortName);
                SetUpExtensionWorksheet(worksheet, extensions[j],
                    extensionSystemItems.Where(x => x.MappedSystemExtensionId == extensions[j].MappedSystemExtensionId),
                    headerColors[j % 7], cellColors1[j % 7], cellColors2[j % 7]);
            }

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ms)
            };

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Utility.RemoveInvalidCharacters(string.Format("{0} ({1}) Extension Report.xlsx", mappedSystem.SystemName, mappedSystem.SystemVersion))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = ms.Length;

            return result;
        }

        private void SetUpExtensionWorksheet(IXLWorksheet worksheet, MappedSystemExtension extension, IEnumerable<SystemItem> systemItems, string headerColor, string cellColor1, string cellColor2)
        {
            var i = 1;
            foreach (var item in systemItems)
            {
                var pathSegments = item.DomainItemPath.Split('.');
                var entityName = "";
                for (var p = 1; p < pathSegments.Length - 1; p++)
                {
                    entityName += pathSegments[p];
                    if (p + 2 != pathSegments.Length) entityName += ".";
                }

                worksheet.Cell(i, "A").Value = extension.ShortName;
                worksheet.Cell(i, "B").Value = pathSegments[0];
                worksheet.Cell(i, "C").Value = entityName;
                worksheet.Cell(i, "D").Value = item.ItemName;
                worksheet.Cell(i, "E").Value = item.Definition;

                worksheet.Row(i).Style.Fill.BackgroundColor = XLColor.FromHtml(i % 2 == 1 ? cellColor1 : cellColor2);

                i++;
            }
            worksheet.Row(1).InsertRowsAbove(1);

            worksheet.Cell(1, "A").Value = "System";
            worksheet.Cell(1, "B").Value = "Element Group";
            worksheet.Cell(1, "C").Value = "Entity";
            worksheet.Cell(1, "D").Value = "Item Name";
            worksheet.Cell(1, "E").Value = "Definition";

            worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml(headerColor);
            worksheet.Row(1).Style.Font.FontColor = XLColor.White;

            worksheet.Columns().AdjustToContents();
            foreach (var column in worksheet.Columns().Where(column => column.Width > 50)) column.Width = 50;
        }

        private void SetUpWorksheet(IXLWorksheet worksheet, int i, IList<MappedSystemExtension> extensions)
        {
            worksheet.Row(1).InsertRowsAbove(1);

            worksheet.Cell(1, "A").Value = "System";
            worksheet.Cell(1, "B").Value = "Element Group";
            worksheet.Cell(1, "C").Value = "Entity";

            worksheet.Columns().AdjustToContents();

            var headerColors = new List<string> { "#38B5E6", "#F8972A", "#63BAA9", "#E41564", "#0E76BD", "#FFC528", "#AC238D" };

            for (var j = 0; j < extensions.Count; j++)
            {
                var cell = worksheet.Cell(1, j + 4);

                cell.Value = extensions[j].ShortName;
                cell.WorksheetColumn().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.WorksheetColumn().Width = 10;

                cell.Style.Fill.BackgroundColor = XLColor.FromHtml(headerColors[j % 7]);
            }

            worksheet.Range("A1:C1").Style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196);
            worksheet.Range(string.Format("A1:{0}1", (char)('C' + extensions.Count))).Style.Font.FontColor = XLColor.White;
            worksheet.Range(string.Format("A1:{0}1", (char)('C' + extensions.Count))).Style.Font.Bold = true;

            for (var j = 3; j <= i; j += 2) worksheet.Range(string.Format("A{0}:C{0}", j)).Style.Fill.BackgroundColor = XLColor.LightGray;

            foreach (var column in worksheet.Columns().Where(column => column.Width > 50)) column.Width = 50;
        }

        public HttpResponseMessage DownloadDifferences(Guid mappedSystemId, MappedSystemExtensionDownloadDetailModel model)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner);
            var extensionMappedSystem = model.ExtensionMappedSystemId.HasValue ? GetMappedSystem(model.ExtensionMappedSystemId.Value) : null;

            var mappedSystemExtension = new MappedSystemExtension
            {
                ExtensionMappedSystem = extensionMappedSystem,
                ExtensionMappedSystemId = model.ExtensionMappedSystemId,
                MappedSystemExtensionId = model.MappedSystemExtensionId.HasValue ? model.MappedSystemExtensionId.Value : new Guid(),
                MappedSystem = mappedSystem,
                MappedSystemId = mappedSystemId,
                ShortName = "Test"
            };

            var newExtensions = new List<SystemItem>();
            var updatedExtensions = new List<SystemItem>();

            if (model.ExtensionMappedSystemId.HasValue)
            {
                var extensions = CreateExtensions(mappedSystemId, model.ExtensionMappedSystemId.Value, mappedSystemExtension);
                newExtensions = extensions.New.ToList();
                updatedExtensions = extensions.Updated.ToList();
            }

            var workbook = new XLWorkbook();
            if (model.IncludeMarkedExtended)
            {
                var worksheet = workbook.Worksheets.Add("Marked Extended");
                CreateWorksheet(worksheet, newExtensions.Where(x => x.IsExtended && x.ItemTypeId > 3));
            }

            if (model.IncludeNotMarkedExtended)
            {
                var worksheet = workbook.Worksheets.Add("Not Marked Extended");
                CreateWorksheet(worksheet, newExtensions.Where(x => !x.IsExtended && x.ItemTypeId > 3));
            }

            if (model.IncludeUpdated && model.MappedSystemExtensionId != null)
            {
                var worksheet = workbook.Worksheets.Add("Updated Extensions");
                CreateWorksheet(worksheet, updatedExtensions.Where(x => x.ItemTypeId > 3));
            }

            if (model.IncludeRemoved && model.MappedSystemExtensionId != null && model.ExtensionMappedSystemId.HasValue)
            {
                var worksheet = workbook.Worksheets.Add("Removed Extensions");
                CreateWorksheet(worksheet, OldExtensions(mappedSystemId, model.ExtensionMappedSystemId.Value, mappedSystemExtension).Where(x => x.ItemTypeId > 3));
            }

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ms)
            };

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Utility.RemoveInvalidCharacters(string.Format("Link Differences.{0}.xlsx", DateTime.Now.ToShortDateString().Replace("/", "-")))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = ms.Length;
            return result;
        }

        public MappedSystemExtensionModel[] Get(Guid mappedSystemId)
        {
            GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner, MappedSystemUser.MappedSystemUserRole.Guest);

            return _mappedSystemExtensionRepository.GetAllQueryable()
                .Where(x => x.MappedSystemId == mappedSystemId)
                .Select(x => new MappedSystemExtensionModel
                {
                    MappedSystemExtensionId = x.MappedSystemExtensionId,
                    ExtensionMappedSystemId = x.ExtensionMappedSystemId,
                    ExtensionMappedSystemName = x.ExtensionMappedSystem != null ? x.ExtensionMappedSystem.SystemName : "",
                    ExtensionMappedSystemVersion = x.ExtensionMappedSystem != null ? x.ExtensionMappedSystem.SystemVersion : "",
                    ShortName = x.ShortName
                }).ToArray();
        }

        public MappedSystemExtensionModel Get(Guid mappedSystemId, Guid mappedSystemExtensionId)
        {
            GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner);

            var model = _mappedSystemExtensionRepository.GetAllQueryable()
                .FirstOrDefault(x => x.MappedSystemId == mappedSystemId && x.MappedSystemExtensionId == mappedSystemExtensionId);

            if (model == null)
                throw new NotFoundException(string.Format("Unable to find Mapped System Exception with id {0} and Mapped System Id {1}", mappedSystemExtensionId, mappedSystemId));

            return new MappedSystemExtensionModel
            {
                MappedSystemExtensionId = model.MappedSystemExtensionId,
                ExtensionMappedSystemId = model.ExtensionMappedSystemId,
                ExtensionMappedSystemName = model.ExtensionMappedSystemId.HasValue ? model.ExtensionMappedSystem.SystemName : "",
                ExtensionMappedSystemVersion = model.ExtensionMappedSystemId.HasValue ? model.ExtensionMappedSystem.SystemVersion : "",
                ShortName = model.ShortName
            };
        }

        public List<dynamic> GetExtensionReport(Guid mappedSystemId, Guid? parentMappedSystemId)
        {
            GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner, MappedSystemUser.MappedSystemUserRole.Guest);
            return _mappedSystemRepository.GetExtensionReportDetail(mappedSystemId, parentMappedSystemId);
        }

        public List<MappedSystemExtensionSystemItemModel> GetExtensionReportDetail(Guid mappedSystemId, MappedSystemExtensionDetailRequestModel model)
        {
            GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner, MappedSystemUser.MappedSystemUserRole.Guest);
            var extensions = _systemItemRepository.GetAllItems().Where(x => (model.SystemItemId == Guid.Empty || x.DomainItemPathIds.Contains(model.SystemItemId.ToString())) &&
                                                                            x.IsActive && x.MappedSystemId == mappedSystemId &&
                                                                            x.MappedSystemExtensionId == model.MappedSystemExtensionId &&
                                                                            (x.ItemTypeId == ItemType.Element.Id || x.ItemTypeId == ItemType.Enumeration.Id))
                .Select(x => new MappedSystemExtensionSystemItemModel
                {
                    Definition = x.Definition,
                    DomainItemPath = x.DomainItemPath,
                    SystemItemId = x.SystemItemId
                }).OrderBy(x => x.DomainItemPath).ToList();
            return extensions;
        }

        public DataStandardNameViewModel[] GetLinkableStandards(Guid mappedSystemId)
        {
            GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner);
            var groups = _systemItemRepository.GetAllQueryable().Where(x => x.IsActive && x.ItemTypeId == ItemType.Domain.Id && x.MappedSystemId == mappedSystemId).Select(y => y.ItemName);
            var systems = _systemItemRepository.GetAllQueryable().Where(x => x.IsActive && x.ItemTypeId == ItemType.Domain.Id && groups.Contains(x.ItemName)).Select(x => x.MappedSystemId).Distinct();

            return _mappedSystemRepository.GetAll()
                .Where(x => x.ExtensionOfs.All(y => y.MappedSystemId != mappedSystemId) && x.MappedSystemId != mappedSystemId && systems.Contains(x.MappedSystemId))
                .ToList()
                .Where(x => x.HasAccess(MappedSystemUser.MappedSystemUserRole.View))
                .Select(x => new DataStandardNameViewModel
                {
                    DataStandardId = x.MappedSystemId,
                    SystemName = x.SystemName,
                    SystemVersion = x.SystemVersion
                })
                .ToArray();
        }

        public MappedSystemExtensionLinkingDetail GetLinkingDetails(Guid mappedSystemId, MappedSystemExtensionEditModel model)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner);
            var extensionMappedSystem = model.ExtensionMappedSystemId.HasValue ? GetMappedSystem(model.ExtensionMappedSystemId.Value) : null;

            var mappedSystemExtension = new MappedSystemExtension
            {
                ExtensionMappedSystem = extensionMappedSystem,
                ExtensionMappedSystemId = model.ExtensionMappedSystemId,
                MappedSystem = mappedSystem,
                MappedSystemId = mappedSystemId,
                ShortName = model.ShortName,
                MappedSystemExtensionId = model.MappedSystemExtensionId
            };

            var newExtensions = new List<SystemItem>();
            var updatedExtensions = new List<SystemItem>();
            var removedExtensions = new List<SystemItem>();

            if (model.ExtensionMappedSystemId.HasValue)
            {
                var extensions = CreateExtensions(mappedSystemId, model.ExtensionMappedSystemId.Value, mappedSystemExtension);
                newExtensions = extensions.New.ToList();
                updatedExtensions = extensions.Updated.ToList();
            }

            if (model.MappedSystemExtensionId != Guid.Empty)
                removedExtensions = OldExtensions(mappedSystemId, model.ExtensionMappedSystemId.Value, mappedSystemExtension).ToList();

            var linkingDetailModel = new MappedSystemExtensionLinkingDetail
            {
                MarkedExtensionCount = newExtensions.Count(x => x.IsExtended && x.ItemTypeId > 3),
                NotMarkedExtensionCount = newExtensions.Count(x => !x.IsExtended && x.ItemTypeId > 3),
                UpdatedCount = updatedExtensions.Count(x => x.ItemTypeId > 3),
                ToBeRemovedCount = removedExtensions.Count(x => x.ItemTypeId > 3)
            };

            return linkingDetailModel;
        }

        public bool HasExtensions(Guid mappedSystemId)
        {
            if (!Principal.Current.IsAdministrator) return false;

            return _mappedSystemExtensionRepository
                .GetAllQueryable()
                .Where(x => x.MappedSystemId == mappedSystemId)
                .ToList()
                .Any(x => x.ExtensionMappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.View));
        }

        public MappedSystemExtensionModel Post(Guid mappedSystemId, MappedSystemExtensionCreateModel model)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner);
            var extensionMappedSystem = model.ExtensionMappedSystemId.HasValue ? GetMappedSystem(model.ExtensionMappedSystemId.Value) : null;

            if (_mappedSystemExtensionRepository.GetAllQueryable().Any(x => x.MappedSystemId == mappedSystemId && x.ShortName.ToLower() == model.ShortName))
                throw new BusinessException("There is already an extension with that Short Name");

            var extension = new MappedSystemExtension
            {
                ExtensionMappedSystem = extensionMappedSystem,
                ExtensionMappedSystemId = model.ExtensionMappedSystemId,
                MappedSystem = mappedSystem,
                MappedSystemId = mappedSystemId,
                ShortName = model.ShortName,
                SystemItems = new List<SystemItem>()
            };

            if (model.ExtensionMappedSystemId.HasValue)
            {
                var systemItems = CreateExtensions(mappedSystemId, model.ExtensionMappedSystemId.Value, extension).New;

                if (!model.ImportAll)
                    systemItems = systemItems.Where(x => x.IsExtended).ToList();

                extension.SystemItems = systemItems;
            }

            foreach (var systemItem in extension.SystemItems) systemItem.IsExtended = true;

            _mappedSystemExtensionRepository.Add(extension);
            _mappedSystemExtensionRepository.SaveChanges();

            return Get(mappedSystemId, extension.MappedSystemExtensionId);
        }

        public MappedSystemExtensionModel Put(Guid mappedSystemId, Guid mappedSystemExtensionId, MappedSystemExtensionEditModel model)
        {
            GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner);
            var extension = GetMappedSystemExtension(mappedSystemExtensionId);

            if (extension.MappedSystemId != mappedSystemId)
                throw new NotFoundException(string.Format("Unable to find Mapped System Exception with id {0} and Mapped System Id {1}", mappedSystemExtensionId, mappedSystemId));

            if (_mappedSystemExtensionRepository
                .GetAllQueryable()
                .Any(x => x.MappedSystemId == mappedSystemId &&
                          x.MappedSystemExtensionId != mappedSystemExtensionId &&
                          x.ShortName.ToLower() == model.ShortName))

                throw new BusinessException("There is already an extension with that Short Name");

            extension.ShortName = model.ShortName;

            if (model.ExtensionMappedSystemId.HasValue)
            {
                var systemItems = CreateExtensions(mappedSystemId, model.ExtensionMappedSystemId.Value, extension).New;

                if (!model.ImportAll)
                    systemItems = systemItems.Where(x => x.IsExtended).ToList();

                foreach (var systemItem in systemItems) systemItem.IsExtended = true;

                _systemItemRepository.AddRange(systemItems);
            }

            if (extension.ExtensionMappedSystemId.HasValue)
                _systemItemRepository.RemoveRange(OldExtensions(mappedSystemId, extension.ExtensionMappedSystemId.Value, extension));

            _mappedSystemExtensionRepository.SaveChanges();

            return Get(mappedSystemId, extension.MappedSystemExtensionId);
        }

        private GetExtensionsModel CreateExtensions(Guid mappedSystemId, Guid extensionMappedSystemId, MappedSystemExtension mappedSystemExtension)
        {
            var coreSystemItemsWithExtensions = _systemItemRepository
                .GetAllQueryable()
                .Where(x => x.IsActive && x.MappedSystemId == mappedSystemId &&
                            (!x.MappedSystemExtensionId.HasValue || x.MappedSystemExtensionId == mappedSystemExtension.MappedSystemExtensionId))
                .ToList();

            var coreSystemItems = coreSystemItemsWithExtensions.Where(x => !x.MappedSystemExtensionId.HasValue).ToList();

            var extensionStandardSystemItems = _systemItemRepository.GetAllQueryable()
                .Where(x => x.IsActive && x.MappedSystemId == extensionMappedSystemId && !x.MappedSystemExtensionId.HasValue).ToList();

            var extensionSystemItems = (
                from ext in extensionStandardSystemItems
                join si in coreSystemItems on ext.DomainItemPath equals si.DomainItemPath into temp
                from items in temp.DefaultIfEmpty()
                where items == null
                orderby ext.DomainItemPath
                select ext).ToList();

            var newExtensions = new List<SystemItem>();
            var updatedExtensions = new List<SystemItem>();

            // Does Element Groups and Enumerations first so Enumerations can be mapped for Elements with a Enumeration SystemItemId
            foreach (var extension in extensionSystemItems.Where(x => x.ItemTypeId == ItemType.Domain.Id || x.ItemTypeId == ItemType.Enumeration.Id))
            {
                SystemItem alreadyAddedExtension;

                if (mappedSystemExtension.MappedSystemExtensionId == Guid.Empty) alreadyAddedExtension = null;
                else
                    alreadyAddedExtension = coreSystemItemsWithExtensions.FirstOrDefault(x => x.DomainItemPath == extension.DomainItemPath &&
                                                                                              x.MappedSystemExtensionId.HasValue &&
                                                                                              x.MappedSystemExtensionId == mappedSystemExtension.MappedSystemExtensionId);

                if (alreadyAddedExtension == null)
                {
                    newExtensions.Add(MapExtension(mappedSystemId, coreSystemItemsWithExtensions, newExtensions, extension, mappedSystemExtension));
                }
                else
                {
                    if (alreadyAddedExtension.ItemName != extension.ItemName ||
                        alreadyAddedExtension.Definition != extension.Definition ||
                        alreadyAddedExtension.ItemTypeId != extension.ItemTypeId ||
                        alreadyAddedExtension.ItemDataTypeId != extension.ItemDataTypeId ||
                        alreadyAddedExtension.DataTypeSource != extension.DataTypeSource ||
                        alreadyAddedExtension.FieldLength != extension.FieldLength ||
                        alreadyAddedExtension.ItemUrl != extension.ItemUrl ||
                        alreadyAddedExtension.TechnicalName != extension.TechnicalName ||
                        alreadyAddedExtension.DomainItemPath != extension.DomainItemPath)
                    {
                        alreadyAddedExtension.ItemName = extension.ItemName;
                        alreadyAddedExtension.Definition = extension.Definition;
                        alreadyAddedExtension.ItemTypeId = extension.ItemTypeId;
                        alreadyAddedExtension.ItemDataTypeId = extension.ItemDataTypeId;
                        alreadyAddedExtension.DataTypeSource = extension.DataTypeSource;
                        alreadyAddedExtension.FieldLength = extension.FieldLength;
                        alreadyAddedExtension.ItemUrl = extension.ItemUrl;
                        alreadyAddedExtension.TechnicalName = extension.TechnicalName;
                        alreadyAddedExtension.DomainItemPath = extension.DomainItemPath;

                        updatedExtensions.Add(alreadyAddedExtension);
                    }
                }
            }

            // Then Remaining Systems
            foreach (var extension in extensionSystemItems.Where(x => x.ItemTypeId != ItemType.Enumeration.Id && x.ItemTypeId != ItemType.Domain.Id))
            {
                SystemItem alreadyAddedExtension;

                if (mappedSystemExtension.MappedSystemExtensionId == Guid.Empty) alreadyAddedExtension = null;
                else
                    alreadyAddedExtension = coreSystemItemsWithExtensions.FirstOrDefault(x => x.DomainItemPath == extension.DomainItemPath &&
                                                                                              x.MappedSystemExtensionId.HasValue &&
                                                                                              x.MappedSystemExtensionId == mappedSystemExtension.MappedSystemExtensionId);

                if (alreadyAddedExtension == null)
                {
                    var newExtension = MapExtension(mappedSystemId, coreSystemItemsWithExtensions, newExtensions, extension, mappedSystemExtension);

                    if (extension.EnumerationSystemItemId.HasValue)
                    {
                        var enumerationPath = extension.EnumerationSystemItem.DomainItemPath;
                        var coreEnumeration = coreSystemItems.FirstOrDefault(x => x.DomainItemPath == enumerationPath);

                        if (coreEnumeration != null)
                        {
                            newExtension.EnumerationSystemItemId = coreEnumeration.SystemItemId;
                        }
                        else
                        {
                            var extensionEnumeration = newExtensions.FirstOrDefault(x => x.DomainItemPath == enumerationPath);
                            if (extensionEnumeration != null) newExtension.ParentSystemItem = extensionEnumeration;
                        }
                    }

                    newExtensions.Add(newExtension);
                }
                else
                {
                    if (alreadyAddedExtension.ItemName != extension.ItemName ||
                        alreadyAddedExtension.Definition != extension.Definition ||
                        alreadyAddedExtension.ItemTypeId != extension.ItemTypeId ||
                        alreadyAddedExtension.ItemDataTypeId != extension.ItemDataTypeId ||
                        alreadyAddedExtension.DataTypeSource != extension.DataTypeSource ||
                        alreadyAddedExtension.FieldLength != extension.FieldLength ||
                        alreadyAddedExtension.ItemUrl != extension.ItemUrl ||
                        alreadyAddedExtension.TechnicalName != extension.TechnicalName ||
                        alreadyAddedExtension.DomainItemPath != extension.DomainItemPath)
                    {
                        alreadyAddedExtension.ItemName = extension.ItemName;
                        alreadyAddedExtension.Definition = extension.Definition;
                        alreadyAddedExtension.ItemTypeId = extension.ItemTypeId;
                        alreadyAddedExtension.ItemDataTypeId = extension.ItemDataTypeId;
                        alreadyAddedExtension.DataTypeSource = extension.DataTypeSource;
                        alreadyAddedExtension.FieldLength = extension.FieldLength;
                        alreadyAddedExtension.ItemUrl = extension.ItemUrl;
                        alreadyAddedExtension.TechnicalName = extension.TechnicalName;
                        alreadyAddedExtension.DomainItemPath = extension.DomainItemPath;

                        updatedExtensions.Add(alreadyAddedExtension);
                    }
                }
            }

            return new GetExtensionsModel
            {
                New = newExtensions,
                Updated = updatedExtensions
            };
        }

        private void CreateWorksheet(IXLWorksheet worksheet, IEnumerable<SystemItem> extensions)
        {
            var i = 2;
            foreach (var extension in extensions)
            {
                var splitPath = extension.DomainItemPath.Split('.');

                worksheet.Cell(i, "A").Value = splitPath[0];
                worksheet.Cell(i, "B").Value = "";

                for (var j = 1; j < splitPath.Length - 1; j++)
                {
                    worksheet.Cell(i, "B").Value += splitPath[j];
                    if (j < splitPath.Length - 2) worksheet.Cell(i, "B").Value += ".";
                }

                worksheet.Cell(i, "C").Value = extension.ItemName;
                worksheet.Cell(i, "D").Value = extension.Definition;
                worksheet.Cell(i, "E").Value = extension.IsExtended ? "TRUE" : "FALSE";

                i++;
            }

            worksheet.Cell(1, "A").Value = "Element Group";
            worksheet.Cell(1, "B").Value = "Entity";
            worksheet.Cell(1, "C").Value = "Item Name";
            worksheet.Cell(1, "D").Value = "Definition";
            worksheet.Cell(1, "E").Value = "Is Extended";

            worksheet.Columns().AdjustToContents();
            foreach (var col in worksheet.Columns().Where(col => col.Width > 150)) col.Width = 150;
        }

        private MappedSystem GetMappedSystem(Guid mappedSystemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest, MappedSystemUser.MappedSystemUserRole? extensionsArePublicRole = null)
        {
            var mappedSystem = _mappedSystemRepository.Get(mappedSystemId);

            if (mappedSystem == null)
                throw new Exception(string.Format("Mapped System with id '{0}' does not exist.", mappedSystemId));

            if (!Principal.Current.IsAdministrator && !mappedSystem.HasAccess(role))
            {
                if (!extensionsArePublicRole.HasValue || !mappedSystem.AreExtensionsPublic)
                    throw new SecurityException(string.Format("User needs at least {0} Access to peform this action", role));

                if (!mappedSystem.HasAccess(extensionsArePublicRole.Value))
                    throw new SecurityException(string.Format("User needs at least {0} Access to peform this action", extensionsArePublicRole.Value));
            }

            if (!mappedSystem.IsActive)
                throw new Exception(string.Format("Mapped System with id '{0}' is marked as deleted.", mappedSystemId));

            return mappedSystem;
        }

        private MappedSystemExtension GetMappedSystemExtension(Guid mappedSystemExtensiond)
        {
            var mappedSystemExtension = _mappedSystemExtensionRepository.Get(mappedSystemExtensiond);

            if (mappedSystemExtension == null)
                throw new Exception(string.Format("Mapped System Extension with id '{0}' does not exist.", mappedSystemExtensiond));

            return mappedSystemExtension;
        }

        private static SystemItem MapExtension(Guid mappedSystemId, List<SystemItem> coreSystemItems, List<SystemItem> extensions, SystemItem extension, MappedSystemExtension mappedSystemExtension)
        {
            var newExtension = new SystemItem
            {
                MappedSystemId = mappedSystemId,
                ItemName = extension.ItemName,
                Definition = extension.Definition,
                ItemTypeId = extension.ItemTypeId,
                ItemDataTypeId = extension.ItemDataTypeId,
                DataTypeSource = extension.DataTypeSource,
                FieldLength = extension.FieldLength,
                ItemUrl = extension.ItemUrl,
                TechnicalName = extension.TechnicalName,
                IsActive = true,
                IsExtended = extension.IsExtended,
                MappedSystemExtension = mappedSystemExtension,
                DomainItemPath = extension.DomainItemPath,
                CopiedFromSystemItemId = extension.SystemItemId,
                ChildSystemItems = new List<SystemItem>()
            };
            var parentPath = extension.DomainItemPath.TrimEnd(extension.ItemName.ToArray()).TrimEnd('.');
            var coreParent = coreSystemItems.FirstOrDefault(x => x.DomainItemPath == parentPath && x.ItemTypeId < 4);

            if (coreParent != null)
            {
                newExtension.ParentSystemItemId = coreParent.SystemItemId;
                //newExtension.ParentSystemItem = coreParent;
            }
            else
            {
                var extensionParent = extensions.FirstOrDefault(x => x.DomainItemPath == parentPath && x.ItemTypeId < 4);
                if (extensionParent != null)
                    newExtension.ParentSystemItem = extensionParent;
            }

            if (extension.ItemTypeId == ItemType.Enumeration.Id && extension.SystemEnumerationItems.Any())
                newExtension.SystemEnumerationItems = extension.SystemEnumerationItems.Select(x =>
                    new SystemEnumerationItem
                    {
                        CodeValue = x.CodeValue,
                        Description = x.Description,
                        ShortDescription = x.ShortDescription
                    }).ToList();

            return newExtension;
        }

        private IEnumerable<SystemItem> OldExtensions(Guid mappedSystemId, Guid extensionMappedSystemId, MappedSystemExtension mappedSystemExtension)
        {
            var coreItems = _systemItemRepository.GetAllQueryable().Where(x => x.IsActive && x.MappedSystemId == mappedSystemId && x.MappedSystemExtensionId == mappedSystemExtension.MappedSystemExtensionId).ToList();
            var extensionItems = _systemItemRepository.GetAllQueryable().Where(x => x.IsActive && x.MappedSystemId == extensionMappedSystemId && !x.MappedSystemExtensionId.HasValue);

            var oldSystemItems = from si in coreItems
                join ext in extensionItems on si.DomainItemPath equals ext.DomainItemPath into temp
                from items in temp.DefaultIfEmpty()
                where items == null
                select si;

            return oldSystemItems;
        }

        private class GetExtensionsModel
        {
            public ICollection<SystemItem> New { get; set; }

            public ICollection<SystemItem> Updated { get; set; }
        }
    }
}