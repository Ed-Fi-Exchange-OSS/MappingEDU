// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.Note;

namespace MappingEdu.Service.SystemItems
{
    public interface INoteService
    {
        NoteViewModel[] Get(Guid systemItemId);

        NoteViewModel Get(Guid systemItemId, Guid noteId);

        NoteViewModel Post(Guid systemItemId, NoteCreateModel note);

        NoteViewModel Put(Guid systemItemId, Guid noteId, NoteEditModel note);

        void Delete(Guid systemItemId, Guid noteId);
    }

    public class NoteService : INoteService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Note> _noteRepository;
        private readonly IRepository<SystemItem> _systemItemRepository;

        public NoteService(IRepository<Note> noteRepository, IRepository<SystemItem> systemItemRepository, IMapper mapper)
        {
            _noteRepository = noteRepository;
            _systemItemRepository = systemItemRepository;
            _mapper = mapper;
        }

        public NoteViewModel[] Get(Guid systemItemId)
        {
            var systemItem = GetSystemItem(systemItemId);
            var hasViewAccess = Principal.Current.IsAdministrator || systemItem.MappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.View);
            var noteViewModels = _mapper.Map<NoteViewModel[]>(systemItem.Notes);
            return (hasViewAccess) ? noteViewModels.OrderBy(x => x.CreateDate).ToArray() : new NoteViewModel[] {};
        }

        public NoteViewModel Get(Guid systemItemId, Guid noteId)
        {
            var note = GetSpecificNote(noteId, systemItemId, MappedSystemUser.MappedSystemUserRole.View);
            var noteViewModel = _mapper.Map<NoteViewModel>(note);
            return noteViewModel;
        }

        public NoteViewModel Post(Guid systemItemId, NoteCreateModel noteModel)
        {
            var systemItem = GetSystemItem(systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            var note = new Note
            {
                SystemItemId = systemItem.SystemItemId,
                SystemItem = systemItem,
                Title = noteModel.Title != null ? noteModel.Title : "",
                Notes = noteModel.Notes
            };

            _noteRepository.Add(note);
            _noteRepository.SaveChanges();

            return Get(systemItemId, note.NoteId);
        }

        public NoteViewModel Put(Guid systemItemId, Guid noteId, NoteEditModel noteModel)
        {
            var note = GetSpecificNote(noteId, systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            if (!note.CreateById.HasValue || note.CreateById.Value.ToString() != Principal.Current.UserId)
                throw new SecurityException("Cannot modify other users notes");


            note.Title = noteModel.Title != null ? noteModel.Title : "";
            note.Notes = noteModel.Notes;

            _noteRepository.SaveChanges();

            return Get(systemItemId, note.NoteId);
        }

        public void Delete(Guid systemItemId, Guid noteId)
        {
            var note = GetSpecificNote(noteId, systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            if (!note.CreateById.HasValue || note.CreateById.Value.ToString() != Principal.Current.UserId)
                throw new SecurityException("Cannot delete other users notes");

            _noteRepository.Delete(note);
            _noteRepository.SaveChanges();
        }

        private Note GetSpecificNote(Guid noteId, Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var note = _noteRepository.Get(noteId);
            if (note == null)
                throw new Exception(string.Format("The note with id '{0}' does not exist.", noteId));
            if (!Principal.Current.IsAdministrator && !note.SystemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            if (note.SystemItemId != systemItemId)
                throw new Exception(string.Format("The note with id '{0}' does not have a parent system item id of '{1}'.", noteId, systemItemId));

            return note;
        }

        private SystemItem GetSystemItem(Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var systemItem = _systemItemRepository.Get(systemItemId);
            if (systemItem == null)
                throw new Exception(string.Format("The system item with id '{0}' does not exist.", systemItemId));
            if (!Principal.Current.IsAdministrator && !systemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return systemItem;
        }
    }
}