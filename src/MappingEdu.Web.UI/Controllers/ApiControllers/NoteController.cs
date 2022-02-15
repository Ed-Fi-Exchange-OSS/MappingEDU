// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing notes
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [Route("Note/{id:guid}")]
        [AcceptVerbs("GET")]
        public NoteViewModel[] Get(Guid id)
        {
            return _noteService.Get(id);
        }

        [Route("Note/{id:guid}/{id2:guid}")]
        [AcceptVerbs("GET")]
        public NoteViewModel Get(Guid id, Guid id2)
        {
            return _noteService.Get(id, id2);
        }

        [Route("Note/{id:guid}")]
        [AcceptVerbs("POST")]
        public NoteViewModel Post(Guid id, NoteCreateModel model)
        {
            return _noteService.Post(id, model);
        }

        [Route("Note/{id:guid}/{id2:guid}")]
        [AcceptVerbs("PUT")]
        public NoteViewModel Put(Guid id, Guid id2, NoteEditModel model)
        {
            return _noteService.Put(id, id2, model);
        }

        [Route("Note/{id:guid}/{id2:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id, Guid id2)
        {
            _noteService.Delete(id, id2);
        }
    }
}