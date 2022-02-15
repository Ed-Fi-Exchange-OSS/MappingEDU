// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.MapNote;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing map notes
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class MapNoteController : ControllerBase
    {
        private readonly IMapNoteService _mapNoteService;

        public MapNoteController(IMapNoteService mapNoteService)
        {
            _mapNoteService = mapNoteService;
        }

        [Route("MapNotes/{id:guid}")]
        [AcceptVerbs("GET")]
        public MapNoteViewModel[] Get(Guid id)
        {
            return _mapNoteService.Get(id);
        }

        [Route("MapNotes/{id:guid}/{id2:guid}")]
        [AcceptVerbs("GET")]
        public MapNoteViewModel Get(Guid id, Guid id2)
        {
            return _mapNoteService.Get(id, id2);
        }

        [Route("MapNotes/{id:guid}")]
        [AcceptVerbs("POST")]
        public MapNoteViewModel Post(Guid id, MapNoteCreateModel model)
        {
            return _mapNoteService.Post(id, model);
        }

        [Route("MapNotes/{id:guid}/{id2:guid}")]
        [AcceptVerbs("PUT")]
        public MapNoteViewModel Put(Guid id, Guid id2, MapNoteEditModel model)
        {
            return _mapNoteService.Put(id, id2, model);
        }

        [Route("MapNotes/{id:guid}/{id2:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id, Guid id2)
        {
            _mapNoteService.Delete(id, id2);
        }
    }
}