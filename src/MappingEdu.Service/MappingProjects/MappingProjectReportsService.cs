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
using System.Security;
using System.Text;
using ClosedXML.Excel;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.Util;
using EnumerationMappingStatusReasonType = MappingEdu.Core.Domain.Enumerations.EnumerationMappingStatusReasonType;
using EnumerationMappingStatusType = MappingEdu.Core.Domain.Enumerations.EnumerationMappingStatusType;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;
using MappingMethodType = MappingEdu.Core.Domain.Enumerations.MappingMethodType;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectReportsService
    {
        HttpResponseMessage CreateCedsReport(Guid mappingProjectId);

        HttpResponseMessage CreateReport(Guid mappingProjectId, MappingProjectCreateReportModel report);

        HttpResponseMessage CreateTargetReport(Guid mappingProjectId, MappingProjectCreateReportModel report);

        MappingProjectReportsViewModel PercentageComplete(Guid mappingProjectId);
    }

    public class MappingProjectReportsService : IMappingProjectReportsService
    {
        private readonly IMappingProjectReportRepository _mappingProjectReportRepository;
        private readonly IRepository<MappingProject> _mappingProjectRepository;
        private readonly ISystemItemRepository _systemItemRepository;

        public MappingProjectReportsService(ISystemItemRepository systemItemRepository,
            IRepository<MappingProject> mappingProjectRepository,
            IMappingProjectReportRepository mappingProjectReportRepository)
        {
            _systemItemRepository = systemItemRepository;
            _mappingProjectRepository = mappingProjectRepository;
            _mappingProjectReportRepository = mappingProjectReportRepository;
        }

        public HttpResponseMessage CreateCedsReport(Guid mappingProjectId)
        {
            // Base on CEDS version 5 Upload Template

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("CEDS Export");

            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.View))
                throw new SecurityException("User needs at least View Access to peform this action");

            if (!mappingProject.SourceDataStandard.SystemName.Contains("CEDS"))
                throw new Exception("Target Data Standard must be CEDS");

            var i = 1;
            foreach (var map in mappingProject.SystemItemMaps.Where(map => Equals(map.WorkflowStatusType, WorkflowStatusType.Approved)))
            {
                foreach (var target in map.TargetSystemItems.Where(x => x.ItemTypeId == 4))
                {
                    //System Name (Column 1)
                    worksheet.Cell(i, "A").Value = mappingProject.TargetDataStandard.SystemName + " v" + mappingProject.TargetDataStandard.SystemVersion;

                    //Element/Field Name (Column 4)
                    worksheet.Cell(i, "D").Value = target.ItemName;

                    if (target.ParentSystemItem != null)
                    {
                        //Table Name (Column 3)
                        var tableName = new StringBuilder();
                        var targetElement = target.ParentSystemItem;
                        do
                        {
                            tableName.Insert(0, "." + targetElement.ItemName);
                            targetElement = targetElement.ParentSystemItem;
                        } while (targetElement.ParentSystemItem != null && targetElement.ParentSystemItem.ParentSystemItem != null);
                        tableName.Remove(0, 1);
                        worksheet.Cell(i, "C").Value = tableName;

                        //Database Name (Column 2)
                        if (targetElement != null && targetElement.ParentSystemItem != null)
                            worksheet.Cell(i, "B").Value = targetElement.ItemName;
                    }

                    //Element Id (Column 5)

                    //Element Definition (Column 6)
                    worksheet.Cell(i, "F").Value = target.Definition;

                    //Data Type (Column 7)
                    worksheet.Cell(i, "G").Value = target.DataTypeSource;

                    //Length (Column 8)
                    worksheet.Cell(i, "H").Value = target.FieldLength;

                    //Valid Values/Option Set (Column 9)

                    //Option Description (Column 10)

                    //CEDS Element Data Model Id (Column 11)
                    var systemItemCustomDetail = map.SourceSystemItem.SystemItemCustomDetails.SingleOrDefault(x => x.CustomDetailMetadata.DisplayName == "Item Version Id");
                    if (systemItemCustomDetail != null) worksheet.Cell(i, "K").Value = systemItemCustomDetail.Value;

                    //Definition Response Id (Column 12)
                    if (map.SourceSystemItem.Definition == target.Definition)
                        worksheet.Cell(i, "L").Value = 1;
                    else if (target.TargetSystemItemMaps.Count(x => x.MappingProjectId == mappingProjectId) <= 1)
                        worksheet.Cell(i, "L").Value = 2;
                    else
                        worksheet.Cell(i, "L").Value = 3;

                    //Option Set Response Id (Column 13)
                    worksheet.Cell(i, "M").Value = target.DataTypeSource != null && target.DataTypeSource.Contains("token") ? 3 : 5;

                    //Complete Name (Column 14)
                    var completeName = new StringBuilder();
                    var element = map.SourceSystemItem;
                    while (element != null)
                    {
                        completeName.Insert(0, "." + element.ItemName);
                        element = element.ParentSystemItem;
                    }
                    completeName.Remove(0, 1);
                    worksheet.Cell(i, "N").Value = completeName;

                    i++;
                }
            }

            worksheet.SortRows.Add("K", XLSortOrder.Ascending);
            worksheet.Row(1).InsertRowsAbove(1);
            worksheet.Cell(1, "A").Value = "System Name";
            worksheet.Cell(1, "B").Value = "Database Name";
            worksheet.Cell(1, "C").Value = "Table Name";
            worksheet.Cell(1, "D").Value = "Element/Field Name";
            worksheet.Cell(1, "E").Value = "Element Id";
            worksheet.Cell(1, "F").Value = "Element Definition";
            worksheet.Cell(1, "G").Value = "Data Type";
            worksheet.Cell(1, "H").Value = "Length";
            worksheet.Cell(1, "I").Value = "Valid Values";
            worksheet.Cell(1, "J").Value = "Option Description";
            worksheet.Cell(1, "K").Value = "CEDS Element Data Model ID";
            worksheet.Cell(1, "L").Value = "Definitions Response ID";
            worksheet.Cell(1, "M").Value = "Option Set Response ID";
            worksheet.Cell(1, "N").Value = "CEDS Element Complete Name";

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ms)
            };

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Utility.RemoveInvalidCharacters(string.Format("{0} CEDS Export.xlsx", mappingProject.ProjectName))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = ms.Length;
            return result;
        }

        public HttpResponseMessage CreateReport(Guid mappingProjectId, MappingProjectCreateReportModel report)
        {
            var workbook = new XLWorkbook();

            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.View))
                throw new SecurityException("User needs at least View Access to peform this action");

            if (report.ElementList.Show) ElementList(workbook, mappingProject.SourceDataStandard);
            if (report.ElementMappingList.Show) SourceElementMappingList(workbook, mappingProject, report.ElementMappingList);
            if (report.EnumerationList.Show) EnumerationList(workbook, mappingProject.SourceDataStandard);
            if (report.EnumrationMappingList.Show) SourceEnumerationMappingList(workbook, mappingProject, report.EnumrationMappingList);

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ms)
            };

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Utility.RemoveInvalidCharacters(string.Format("{0} Report.xlsx", mappingProject.ProjectName))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = ms.Length;

            return result;
        }

        public HttpResponseMessage CreateTargetReport(Guid mappingProjectId, MappingProjectCreateReportModel report)
        {
            var workbook = new XLWorkbook();

            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.View))
                throw new SecurityException("User needs at least View Access to peform this action");

            var targetDataStandard = mappingProject.TargetDataStandard;

            if (report.ElementList.Show) ElementList(workbook, targetDataStandard, false);
            if (report.ElementMappingList.Show) TargetElementMappingList(workbook, mappingProject, report.ElementMappingList);
            if (report.EnumerationList.Show) EnumerationList(workbook, targetDataStandard, false);
            if (report.EnumrationMappingList.Show) TargetEnumerationMappingList(workbook, mappingProject, report.EnumrationMappingList);

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ms)
            };

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Utility.RemoveInvalidCharacters(string.Format("{0} Target Report.xlsx", mappingProject.ProjectName))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = ms.Length;

            return result;
        }

        public MappingProjectReportsViewModel PercentageComplete(Guid mappingProjectId)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.View))
                throw new SecurityException("User needs at least View Access to peform this action");

            var approvedMappings = _systemItemRepository.GetAllItems()
                .Where(si => si.MappedSystemId == mappingProject.SourceDataStandardMappedSystemId)
                .Count(
                    si => si.SourceSystemItemMaps.Any(
                        sim => sim.MappingProjectId == mappingProjectId &&
                               sim.WorkflowStatusTypeId == WorkflowStatusType.Approved.Id));

            var totalItems = mappingProject.SourceDataStandard.SystemItems.Count(
                si => si.ItemTypeId == ItemType.Element.Id ||
                      si.ItemTypeId == ItemType.Enumeration.Id);

            return new MappingProjectReportsViewModel
            {
                PercentComplete = totalItems > 0 ? decimal.Divide(approvedMappings, totalItems) : 0
            };
        }

        private void ElementList(XLWorkbook workbook, MappedSystem standard, bool showNotes = true)
        {
            var systemName = Utility.RemoveInvalidCharacters(string.Format("{0} ({1})", standard.SystemName, standard.SystemVersion));
            var worksheet = workbook.Worksheets.Add(string.Format("{0} Element List", systemName.Length > 14 ? systemName.Substring(0, 11) + "..." : systemName));
            var metaColumns = standard.CustomDetailMetadata.Select((meta, index) => new
            {
                Column = index + 6,
                meta.DisplayName,
                meta.CustomDetailMetadataId
            });

            var systemItems = _mappingProjectReportRepository.GetElementList(standard.MappedSystemId);

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

                worksheet.Cell(i, "A").Value = pathSegments[0];
                worksheet.Cell(i, "B").Value = entityName;
                worksheet.Cell(i, "C").Value = item.ItemName;
                worksheet.Cell(i, "D").Value = item.DataType;
                worksheet.Cell(i, "E").Value = item.FieldLength;
                worksheet.Cell(i, "F").Value = item.Definition;

                foreach (var column in metaColumns)
                {
                    object systemItemCustomDetailValue;
                    if (!((IDictionary<string, object>) item).TryGetValue(column.DisplayName, out systemItemCustomDetailValue)) continue;

                    var asciiColumnValue = (char) (column.Column + 65); //65 is A
                    worksheet.Cell(i, asciiColumnValue.ToString()).Value = systemItemCustomDetailValue.ToString();
                }
                i++;
            }

            worksheet.Row(1).InsertRowsAbove(1);
            worksheet.Cell(1, "A").Value = "Element Group";
            worksheet.Cell(1, "B").Value = "Entity";
            worksheet.Cell(1, "C").Value = "Element";
            worksheet.Cell(1, "D").Value = "Data Type";
            worksheet.Cell(1, "E").Value = "Field Length";
            worksheet.Cell(1, "F").Value = "Definition";

            var maxValue = 6;
            foreach (var column in metaColumns)
            {
                if (column.Column + 1 > maxValue) maxValue = column.Column + 1;
                var asciiColumnValue = (char) (column.Column + 65); //65 is A
                worksheet.Cell(1, asciiColumnValue.ToString()).Value = "EXT_" + column.DisplayName;
            }

            var farthestColumn = ((char) (maxValue + 64)).ToString();
            var notesColumn = ((char) (maxValue + 65)).ToString();
            var notesToColumn = ((char) (maxValue + 66)).ToString();

            if (showNotes)
            {
                worksheet.Cell(1, notesColumn).Value = string.Format("{0}: Notes", standard.SystemName);
                worksheet.Cell(1, notesToColumn).Value = string.Format("Comment to {0}: Notes", standard.SystemName);
                farthestColumn = notesToColumn;
            }

            worksheet.Range(string.Format("A1:{0}1", farthestColumn)).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196);
            worksheet.Range(string.Format("A1:{0}1", farthestColumn)).Style.Font.FontColor = XLColor.White;
            worksheet.Range(string.Format("A1:{0}1", farthestColumn)).Style.Font.Bold = true;
            worksheet.SheetView.Freeze(1, 0);

            for (var j = 3; j <= i; j += 2) worksheet.Range(string.Format("A{0}:{1}{0}", j, farthestColumn)).Style.Fill.BackgroundColor = XLColor.LightGray;
            if (showNotes) worksheet.Range(string.Format("{1}2:{1}{0}", i, notesColumn)).Style.Fill.BackgroundColor = XLColor.FromArgb(254, 242, 203);

            worksheet.Columns().AdjustToContents();
            foreach (var column in worksheet.Columns().Where(column => column.Width > 50)) column.Width = 50;
        }

        private void EnumerationList(XLWorkbook workbook, MappedSystem standard, bool showNotes = true)
        {
            var systemName = Utility.RemoveInvalidCharacters(string.Format("{0} ({1})", standard.SystemName, standard.SystemVersion));
            var worksheet = workbook.Worksheets.Add(string.Format("{0} Enumeration List", systemName.Length > 14 ? systemName.Substring(0, 11) + "..." : systemName));
            var metaColumns = standard.CustomDetailMetadata.Select((meta, index) => new
            {
                Column = index + 3,
                meta.DisplayName,
                meta.CustomDetailMetadataId
            });

            var enumerationColumn = metaColumns.Any() ? metaColumns.Max(x => x.Column) + 1 : 3;

            var enumerationItems = _mappingProjectReportRepository.GetEnumerationItems(standard.MappedSystemId);

            var i = 1;
            foreach (var item in enumerationItems)
            {
                worksheet.Cell(i, 1).Value = item.ElementGroup;
                worksheet.Cell(i, 2).Value = item.ItemName;

                foreach (var column in metaColumns)
                {
                    object systemItemCustomDetailValue;
                    if (!((IDictionary<string, object>) item).TryGetValue(column.DisplayName, out systemItemCustomDetailValue)) continue;
                    worksheet.Cell(i, column.Column).Value = systemItemCustomDetailValue.ToString();
                }

                worksheet.Cell(i, enumerationColumn).Value = item.CodeValue;
                worksheet.Cell(i, enumerationColumn + 1).Value = item.ShortDescription;
                worksheet.Cell(i, enumerationColumn + 2).Value = item.Description;

                i++;
            }

            worksheet.Row(1).InsertRowsAbove(1);
            worksheet.Cell(1, "A").Value = "Element Group";
            worksheet.Cell(1, "B").Value = "Item Name";

            foreach (var column in metaColumns)
                worksheet.Cell(1, column.Column).Value = "EXT_" + column.DisplayName;

            worksheet.Cell(1, enumerationColumn + 1).Value = "Short Description";
            worksheet.Cell(1, enumerationColumn + 2).Value = "Description";

            if (showNotes)
            {
                worksheet.Cell(1, enumerationColumn + 3).Value = string.Format("{0}: Notes", standard.SystemName);
                worksheet.Cell(1, enumerationColumn + 4).Value = string.Format("Comment to {0}: Notes", standard.SystemName);
            }

            var notesColumn = (char) (enumerationColumn + 65 + 2); //65 is A
            var commentToColumn = (char) (enumerationColumn + 65 + 3); //65 is A

            worksheet.Range(string.Format("A1:{0}1", commentToColumn)).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196);
            worksheet.Range(string.Format("A1:{0}1", commentToColumn)).Style.Font.FontColor = XLColor.White;
            worksheet.Range(string.Format("A1:{0}1", commentToColumn)).Style.Font.Bold = true;

            worksheet.SheetView.Freeze(1, 0);

            for (var j = 3; j <= i; j += 2) worksheet.Range(string.Format("A{0}:{1}{0}", j, commentToColumn)).Style.Fill.BackgroundColor = XLColor.LightGray;
            if (showNotes)
                worksheet.Range(string.Format("{0}2:{0}{1}", notesColumn, i)).Style.Fill.BackgroundColor = XLColor.FromArgb(254, 242, 203);

            worksheet.Columns().AdjustToContents();
            foreach (var column in worksheet.Columns().Where(column => column.Width > 50)) column.Width = 50;
        }

        private void SourceElementMappingList(XLWorkbook workbook, MappingProject mappingProject, ElementMappingListPage page)
        {
            var sourceDataStandard = mappingProject.SourceDataStandard;
            var targetDataStandard = mappingProject.TargetDataStandard;
            var customDetails = sourceDataStandard.CustomDetailMetadata;
            var viewOnly = !mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.Edit);

            var worksheet = workbook.Worksheets.Add("Element Mappings");
            var index = 1;
            var columns = page.Columns;
            var c = 'A';

            foreach (var column in columns)
            {
                column.Column = c.ToString();
                c++;
            }

            var includeCustomDetails = columns.Any(x => x.IsCustomDetail);
            var includeTargetItems = columns.Any(x => x.DisplayText == "Destination Element" || x.DisplayText == "Destination Complete Element Name");
            var rows = _mappingProjectReportRepository.GetSourceMappings(mappingProject.MappingProjectId, page.MappingMethods, page.WorkflowStatuses, includeCustomDetails, includeTargetItems);

            foreach (var item in rows)
            {
                var sourceSegments = item.DomainItemPath.Split('.');
                var entityName = "";
                for (var p = 1; p < sourceSegments.Length - 1; p++)
                {
                    entityName += sourceSegments[p];
                    if (p + 2 != sourceSegments.Length) entityName += ".";
                }

                int mappingMethodTypeId;
                int workflowStatusTypeId;

                bool retrievedMappingMethod = int.TryParse(item.MappingMethodTypeId, out mappingMethodTypeId);
                bool retrievedWorkflowStatus = int.TryParse(item.WorkflowStatusTypeId, out workflowStatusTypeId);

                var hasTargetElements = mappingMethodTypeId == MappingMethodType.EnterMappingBusinessLogic.Id && ((List<dynamic>) item.TargetSystemItems).Any();

                for (var loopIndex = 0; loopIndex < (hasTargetElements ? ((List<dynamic>) item.TargetSystemItems).Count : 1); loopIndex++)
                {
                    dynamic target = null;
                    if (hasTargetElements) target = item.TargetSystemItems[loopIndex];
                    foreach (var column in columns)
                    {
                        switch (column.DisplayText)
                        {
                            case "System":
                                worksheet.Cell(index, column.Column).Value = string.Format("{0} {1}", sourceDataStandard.SystemName, sourceDataStandard.SystemVersion);
                                break;
                            case "Element Group":
                                worksheet.Cell(index, column.Column).Value = sourceSegments[0];
                                break;
                            case "Entity":
                                worksheet.Cell(index, column.Column).Value = entityName;
                                break;
                            case "Element":
                                worksheet.Cell(index, column.Column).Value = item.ItemName;
                                break;
                            case "Element Mapping Method":
                                if (retrievedWorkflowStatus)
                                {
                                    if (mappingMethodTypeId == MappingMethodType.EnterMappingBusinessLogic.Id)
                                    {
                                        worksheet.Cell(index, column.Column).Value = "Mapped";
                                    }
                                    else
                                    {
                                        worksheet.Cell(index, column.Column).Value = MappingMethodType.GetById(mappingMethodTypeId).Name;
                                    }
                                }
                                break;
                            case "Element Workflow Status":
                                worksheet.Cell(index, column.Column).Value = retrievedWorkflowStatus ? WorkflowStatusType.GetById(workflowStatusTypeId).Name : "Unmapped";
                                break;
                            case "Omission Reason":
                                if (mappingMethodTypeId == MappingMethodType.MarkForOmission.Id) worksheet.Cell(index, column.Column).Value = item.OmissionReason;
                                break;
                            case "Business Logic":
                                if (mappingMethodTypeId == MappingMethodType.EnterMappingBusinessLogic.Id || mappingMethodTypeId == MappingMethodType.MarkForExtension.Id) worksheet.Cell(index, column.Column).Value = item.BusinessLogic;
                                break;
                            case "Destination Version":
                                if (target != null) worksheet.Cell(index, column.Column).Value = targetDataStandard.SystemVersion;
                                break;
                            case "Destination Element":
                                if (target != null) worksheet.Cell(index, column.Column).Value = target.ItemName;
                                break;
                            case "Destination Complete Element Name":
                                if (target != null) worksheet.Cell(index, column.Column).Value = target.DomainItemPath;
                                break;
                            case "Created By":
                                if (!viewOnly) worksheet.Cell(index, column.Column).Value = item.CreatedBy;
                                break;
                            case "Creation Date":
                                if (!viewOnly) worksheet.Cell(index, column.Column).Value = item.CreateDate;
                                break;
                            case "Updated By":
                                if (!viewOnly) worksheet.Cell(index, column.Column).Value = item.UpdatedBy;
                                break;
                            case "Update Date":
                                if (!viewOnly) worksheet.Cell(index, column.Column).Value = item.UpdateDate;
                                break;
                            default:
                                if (column.IsCustomDetail)
                                {
                                    var detail = customDetails.FirstOrDefault(x => x.DisplayName == column.DisplayText);
                                    if (detail == null) continue;

                                    object systemItemCustomDetailValue;
                                    if (((IDictionary<string, object>) item).TryGetValue(detail.DisplayName, out systemItemCustomDetailValue))
                                        worksheet.Cell(index, column.Column).Value = systemItemCustomDetailValue.ToString();
                                }
                                break;
                        }
                    }

                    index++;
                }
            }

            worksheet.Row(1).InsertRowsAbove(1);

            foreach (var column in columns)
            {
                switch (column.DisplayText)
                {
                    case "Notes":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0}: Notes", mappingProject.SourceDataStandard.SystemName);
                        break;
                    case "Notes To":
                        worksheet.Cell(1, column.Column).Value = string.Format("Comment to {0}: Notes", mappingProject.SourceDataStandard.SystemName);
                        break;
                    case "Destination Version":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Version", mappingProject.TargetDataStandard.SystemName);
                        break;
                    case "Destination Element":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Element", mappingProject.TargetDataStandard.SystemName);
                        break;
                    case "Destination Complete Element Name":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Element Complete Name", mappingProject.TargetDataStandard.SystemName);
                        break;
                    case "Created By":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Creation Date":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Updated By":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Update Date":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    default:
                        worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                }
            }

            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196);
            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Font.FontColor = XLColor.White;
            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Font.Bold = true;

            worksheet.SheetView.Freeze(1, columns.Count(x => x.DisplayText == "System" || x.DisplayText == "Element Group" || x.DisplayText == "Entity" || x.DisplayText == "Element"));

            for (var j = 3; j <= index; j += 2) worksheet.Range(string.Format("A{0}:{1}{0}", j, columns.Last().Column)).Style.Fill.BackgroundColor = XLColor.LightGray;
            if (columns.Any(x => x.DisplayText == "Notes")) worksheet.Range(string.Format("{1}2:{1}{0}", index, columns.First(x => x.DisplayText == "Notes").Column)).Style.Fill.BackgroundColor = XLColor.FromArgb(254, 242, 203);

            worksheet.Columns().AdjustToContents();
            foreach (var column in worksheet.Columns().Where(column => column.Width > 50)) column.Width = 50;
        }

        private void SourceEnumerationMappingList(XLWorkbook workbook, MappingProject mappingProject, EnumerationMappingListPage page)
        {
            var columns = page.Columns;
            var customDetails = mappingProject.SourceDataStandard.CustomDetailMetadata;

            var c = 'A';
            foreach (var column in columns)
            {
                column.Column = c.ToString();
                c++;
            }

            var worksheet = workbook.Worksheets.Add("Enumeration Mappings");
            var viewOnly = !mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.Edit);

            var enumerationItemMaps = _mappingProjectReportRepository.GetSourceEnumerationItemMaps(mappingProject.MappingProjectId, page.EnumerationMappingStatusTypes, page.EnumerationMappingStatusReasonTypes, columns.Any(x => x.IsCustomDetail));

            var i = 1;

            foreach (var enumerationMap in enumerationItemMaps)
            {
                int statusTypeId;
                int reasonTypeId;

                int.TryParse(enumerationMap.EnumerationMappingStatusTypeId, out statusTypeId);
                int.TryParse(enumerationMap.EnumerationMappingStatusReasonTypeId, out reasonTypeId);

                foreach (var column in columns)
                {
                    switch (column.DisplayText)
                    {
                        case "Element Group":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.SourceElementGroup;
                            break;
                        case "Element":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.SourceItemName;
                            break;
                        case "Enumeration":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.SourceCodeValue;
                            break;
                        case "Short Description":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.SourceShortDescription;
                            break;
                        case "Description":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.SourceDescription;
                            break;
                        case "Mapping Status":
                            if (statusTypeId > 0)
                                worksheet.Cell(i, column.Column).Value = EnumerationMappingStatusType.GetById(statusTypeId).Name;
                            else if (enumerationMap.SystemEnumerationItemMapId == "")
                                worksheet.Cell(i, column.Column).Value = "Unmapped";
                            break;
                        case "Mapping Status Reason":
                            if (reasonTypeId > 0) worksheet.Cell(i, column.Column).Value = EnumerationMappingStatusReasonType.GetById(reasonTypeId).Name;
                            break;
                        case "Destination Enumeration":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.TargetCodeValue;
                            break;
                        case "Destination Element":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.TargetItemName;
                            break;
                        case "Destination Complete Element Name":
                            if (enumerationMap.TargetElementGroup != "") worksheet.Cell(i, column.Column).Value = enumerationMap.TargetElementGroup + "." + enumerationMap.TargetItemName;
                            break;
                        case "Created By":
                            if (!viewOnly) worksheet.Cell(i, column.Column).Value = enumerationMap.CreatedBy;
                            break;
                        case "Creation Date":
                            if (!viewOnly) worksheet.Cell(i, column.Column).Value = enumerationMap.CreateDate;
                            break;
                        case "Updated By":
                            if (!viewOnly) worksheet.Cell(i, column.Column).Value = enumerationMap.UpdatedBy;
                            break;
                        case "Update Date":
                            if (!viewOnly) worksheet.Cell(i, column.Column).Value = enumerationMap.UpdateDate;
                            break;
                        default:
                            if (column.IsCustomDetail)
                            {
                                var detail = customDetails.FirstOrDefault(x => x.DisplayName == column.DisplayText);
                                if (detail == null) continue;

                                object systemItemCustomDetailValue;
                                if (((IDictionary<string, object>) enumerationMap).TryGetValue(detail.DisplayName, out systemItemCustomDetailValue))
                                    worksheet.Cell(i, column.Column).Value = systemItemCustomDetailValue.ToString();
                            }
                            break;
                    }
                }
                i++;
            }

            worksheet.Row(1).InsertRowsAbove(1);

            foreach (var column in columns)
            {
                switch (column.DisplayText)
                {
                    case "Notes":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0}: Notes", mappingProject.SourceDataStandard.SystemName);
                        break;
                    case "Notes To":
                        worksheet.Cell(1, column.Column).Value = string.Format("Comment to {0}: Notes", mappingProject.SourceDataStandard.SystemName);
                        break;
                    case "Destination Enumeration":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Enumeration", mappingProject.TargetDataStandard.SystemName);
                        break;
                    case "Destination Element":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Element", mappingProject.TargetDataStandard.SystemName);
                        break;
                    case "Destination Complete Element Name":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Element Complete Name", mappingProject.TargetDataStandard.SystemName);
                        break;
                    case "Created By":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Creation Date":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Updated By":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Update Date":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    default:
                        worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                }
            }

            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196);
            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Font.FontColor = XLColor.White;
            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Font.Bold = true;

            worksheet.SheetView.Freeze(1, columns.Count(x => x.DisplayText == "Element Group" || x.DisplayText == "Entity" || x.DisplayText == "Element"));

            for (var j = 3; j <= i; j += 2) worksheet.Range(string.Format("A{0}:{1}{0}", j, columns.Last().Column)).Style.Fill.BackgroundColor = XLColor.LightGray;
            if (columns.Any(x => x.DisplayText == "Notes")) worksheet.Range(string.Format("{1}2:{1}{0}", i, columns.First(x => x.DisplayText == "Notes").Column)).Style.Fill.BackgroundColor = XLColor.FromArgb(254, 242, 203);

            worksheet.Columns().AdjustToContents();
            foreach (var column in worksheet.Columns().Where(column => column.Width > 50)) column.Width = 50;
        }

        private void TargetElementMappingList(XLWorkbook workbook, MappingProject mappingProject, ElementMappingListPage page)
        {
            var sourceDataStandard = mappingProject.SourceDataStandard;
            var targetDataStandard = mappingProject.TargetDataStandard;
            var customDetails = targetDataStandard.CustomDetailMetadata;
            var viewOnly = !mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.Edit);

            var worksheet = workbook.Worksheets.Add("Element Mappings");
            var index = 1;
            var columns = page.Columns;
            var c = 'A';

            foreach (var column in columns)
            {
                column.Column = c.ToString();
                c++;
            }

            var includeCustomDetails = columns.Any(x => x.IsCustomDetail);
            var rows = _mappingProjectReportRepository.GetTargetMappings(mappingProject.MappingProjectId, page.MappingMethods, page.WorkflowStatuses, includeCustomDetails);

            foreach (var item in rows)
            {
                var targetSegments = item.DomainItemPath.Split('.');
                var entityName = "";
                for (var p = 1; p < targetSegments.Length - 1; p++)
                {
                    entityName += targetSegments[p];
                    if (p + 2 != targetSegments.Length) entityName += ".";
                }

                var hasSourceMappings = ((List<dynamic>) item.SourceMappings).Any();

                for (var loopIndex = 0; loopIndex < (hasSourceMappings ? ((List<dynamic>) item.SourceMappings).Count : 1); loopIndex++)
                {
                    dynamic mapping = null;
                    if (hasSourceMappings) mapping = item.SourceMappings[loopIndex];

                    var workflowStatusTypeId = 0;

                    if (mapping != null)
                        int.TryParse(mapping.WorkflowStatusTypeId, out workflowStatusTypeId);

                    foreach (var column in columns)
                    {
                        switch (column.DisplayText)
                        {
                            case "System":
                                worksheet.Cell(index, column.Column).Value = string.Format("{0} {1}", targetDataStandard.SystemName, targetDataStandard.SystemVersion);
                                break;
                            case "Element Group":
                                worksheet.Cell(index, column.Column).Value = targetSegments[0];
                                break;
                            case "Entity":
                                worksheet.Cell(index, column.Column).Value = entityName;
                                break;
                            case "Element":
                                worksheet.Cell(index, column.Column).Value = item.ItemName;
                                break;
                            case "Workflow Status":
                                worksheet.Cell(index, column.Column).Value = workflowStatusTypeId > 0 ? WorkflowStatusType.GetById(workflowStatusTypeId).Name : "Unmapped";
                                break;
                            case "Business Logic":
                                if (mapping != null) worksheet.Cell(index, column.Column).Value = mapping.BusinessLogic;
                                break;
                            case "Source Version":
                                if (mapping != null) worksheet.Cell(index, column.Column).Value = sourceDataStandard.SystemVersion;
                                break;
                            case "Source Element":
                                if (mapping != null) worksheet.Cell(index, column.Column).Value = mapping.ItemName;
                                break;
                            case "Source Complete Element Name":
                                if (mapping != null) worksheet.Cell(index, column.Column).Value = mapping.DomainItemPath;
                                break;
                            case "Created By":
                                if (!viewOnly && mapping != null) worksheet.Cell(index, column.Column).Value = mapping.CreatedBy;
                                break;
                            case "Creation Date":
                                if (!viewOnly && mapping != null) worksheet.Cell(index, column.Column).Value = mapping.CreateDate;
                                break;
                            case "Updated By":
                                if (!viewOnly && mapping != null) worksheet.Cell(index, column.Column).Value = mapping.UpdatedBy;
                                break;
                            case "Update Date":
                                if (!viewOnly & mapping != null) worksheet.Cell(index, column.Column).Value = mapping.UpdateDate;
                                break;
                            default:
                                if (column.IsCustomDetail)
                                {
                                    var detail = customDetails.FirstOrDefault(x => x.DisplayName == column.DisplayText);
                                    if (detail == null) continue;

                                    object systemItemCustomDetailValue;
                                    if (((IDictionary<string, object>) item).TryGetValue(detail.DisplayName, out systemItemCustomDetailValue))
                                        worksheet.Cell(index, column.Column).Value = systemItemCustomDetailValue.ToString();
                                }
                                break;
                        }
                    }

                    index++;
                }
            }

            worksheet.Row(1).InsertRowsAbove(1);

            foreach (var column in columns)
            {
                switch (column.DisplayText)
                {
                    case "Source Version":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Version", sourceDataStandard.SystemName);
                        break;
                    case "Source Element":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Element", sourceDataStandard.SystemName);
                        break;
                    case "Source Complete Element Name":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Element Complete Name", sourceDataStandard.SystemName);
                        break;
                    case "Created By":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Creation Date":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Updated By":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Update Date":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    default:
                        worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                }
            }

            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196);
            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Font.FontColor = XLColor.White;
            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Font.Bold = true;

            worksheet.SheetView.Freeze(1, columns.Count(x => x.DisplayText == "System" || x.DisplayText == "Element Group" || x.DisplayText == "Entity" || x.DisplayText == "Element"));

            for (var j = 3; j <= index; j += 2) worksheet.Range(string.Format("A{0}:{1}{0}", j, columns.Last().Column)).Style.Fill.BackgroundColor = XLColor.LightGray;

            worksheet.Columns().AdjustToContents();
            foreach (var column in worksheet.Columns().Where(column => column.Width > 50)) column.Width = 50;
        }

        private void TargetEnumerationMappingList(XLWorkbook workbook, MappingProject mappingProject, EnumerationMappingListPage page)
        {
            var columns = page.Columns;
            var customDetails = mappingProject.TargetDataStandard.CustomDetailMetadata;

            var c = 'A';
            foreach (var column in columns)
            {
                column.Column = c.ToString();
                c++;
            }

            var worksheet = workbook.Worksheets.Add("Enumeration Mappings");
            var viewOnly = !mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.Edit);

            var enumerationItemMaps = _mappingProjectReportRepository.GetTargetEnumerationItemMaps(mappingProject.MappingProjectId, page.EnumerationMappingStatusTypes, page.EnumerationMappingStatusReasonTypes, columns.Any(x => x.IsCustomDetail));

            var i = 1;

            foreach (var enumerationMap in enumerationItemMaps)
            {
                int statusTypeId;
                int reasonTypeId;

                int.TryParse(enumerationMap.EnumerationMappingStatusTypeId, out statusTypeId);
                int.TryParse(enumerationMap.EnumerationMappingStatusReasonTypeId, out reasonTypeId);

                foreach (var column in columns)
                {
                    switch (column.DisplayText)
                    {
                        case "Element Group":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.TargetElementGroup;
                            break;
                        case "Element":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.TargetItemName;
                            break;
                        case "Enumeration":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.TargetCodeValue;
                            break;
                        case "Short Description":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.TargetShortDescription;
                            break;
                        case "Description":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.TargetDescription;
                            break;
                        case "Mapping Status":
                            if (statusTypeId > 0)
                                worksheet.Cell(i, column.Column).Value = EnumerationMappingStatusType.GetById(statusTypeId).Name;
                            else if (enumerationMap.SystemEnumerationItemMapId == "")
                                worksheet.Cell(i, column.Column).Value = "Unmapped";
                            break;
                        case "Mapping Status Reason":
                            if (reasonTypeId > 0) worksheet.Cell(i, column.Column).Value = EnumerationMappingStatusReasonType.GetById(reasonTypeId).Name;
                            break;
                        case "Source Enumeration":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.SourceCodeValue;
                            break;
                        case "Source Element":
                            worksheet.Cell(i, column.Column).Value = enumerationMap.SourceItemName;
                            break;
                        case "Source Complete Element Name":
                            if (enumerationMap.SourceElementGroup != "") worksheet.Cell(i, column.Column).Value = enumerationMap.SourceElementGroup + "." + enumerationMap.SourceItemName;
                            break;
                        case "Created By":
                            if (!viewOnly) worksheet.Cell(i, column.Column).Value = enumerationMap.CreatedBy;
                            break;
                        case "Creation Date":
                            if (!viewOnly) worksheet.Cell(i, column.Column).Value = enumerationMap.CreateDate;
                            break;
                        case "Updated By":
                            if (!viewOnly) worksheet.Cell(i, column.Column).Value = enumerationMap.UpdatedBy;
                            break;
                        case "Update Date":
                            if (!viewOnly) worksheet.Cell(i, column.Column).Value = enumerationMap.UpdateDate;
                            break;
                        default:
                            if (column.IsCustomDetail)
                            {
                                var detail = customDetails.FirstOrDefault(x => x.DisplayName == column.DisplayText);
                                if (detail == null) continue;

                                object systemItemCustomDetailValue;
                                if (((IDictionary<string, object>)enumerationMap).TryGetValue(detail.DisplayName, out systemItemCustomDetailValue))
                                    worksheet.Cell(i, column.Column).Value = systemItemCustomDetailValue.ToString();
                            }
                            break;
                    }
                }
                i++;
            }

            worksheet.Row(1).InsertRowsAbove(1);

            foreach (var column in columns)
            {
                switch (column.DisplayText)
                {
                    case "Source Enumeration":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Enumeration", mappingProject.SourceDataStandard.SystemName);
                        break;
                    case "Source Element":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Element", mappingProject.SourceDataStandard.SystemName);
                        break;
                    case "Source Complete Element Name":
                        worksheet.Cell(1, column.Column).Value = string.Format("{0} Element Complete Name", mappingProject.SourceDataStandard.SystemName);
                        break;
                    case "Created By":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Creation Date":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Updated By":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    case "Update Date":
                        if (!viewOnly) worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                    default:
                        worksheet.Cell(1, column.Column).Value = column.DisplayText;
                        break;
                }
            }

            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196);
            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Font.FontColor = XLColor.White;
            worksheet.Range(string.Format("A1:{0}1", columns.Last().Column)).Style.Font.Bold = true;

            worksheet.SheetView.Freeze(1, columns.Count(x => x.DisplayText == "Element Group" || x.DisplayText == "Entity" || x.DisplayText == "Element"));

            for (var j = 3; j <= i; j += 2) worksheet.Range(string.Format("A{0}:{1}{0}", j, columns.Last().Column)).Style.Fill.BackgroundColor = XLColor.LightGray;

            worksheet.Columns().AdjustToContents();
            foreach (var column in worksheet.Columns().Where(column => column.Width > 50)) column.Width = 50;
        }
    }
}
