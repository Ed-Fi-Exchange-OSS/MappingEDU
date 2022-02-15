// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Service.Model.Admin;
using SystemConstantType = MappingEdu.Core.Domain.Enumerations.SystemConstantType;

namespace MappingEdu.Service.Admin
{
    public interface ISystemConstantService
    {
        SystemConstantModel Get(string name);

        SystemConstantModel Put(int id, SystemConstantModel constant);

        ICollection<SystemConstantModel> GetAll();
    }

    public class SystemConstantService : ISystemConstantService
    {
        private readonly EntityContext _context;

        public SystemConstantService(EntityContext context )
        {
            _context = context;
        }

        public SystemConstantModel Get(string name)
        {
            var constant = _context.Set<SystemConstant>().SingleOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (constant == null) 
                throw new NotFoundException("Unable to find system constant");

            return new SystemConstantModel(constant);
        }

        public ICollection<SystemConstantModel> GetAll()
        {
            var constants = _context.Set<SystemConstant>().ToList().Select(x => new SystemConstantModel(x)).ToList();
            return constants;
        }

        public SystemConstantModel Put(int id, SystemConstantModel model)
        {
            if(!Principal.Current.IsAdministrator) throw new BusinessException("Only Admins can change system constants");

            var constant = _context.Set<SystemConstant>().SingleOrDefault(x => x.Id == id);
            if (constant == null)
                throw new NotFoundException("Unable to find system constant");

            constant.Value = (constant.SystemConstantType == SystemConstantType.Boolean) ? model.BooleanValue.ToString() : model.Value;

            _context.SaveChanges();

            return model;
        }
    }
}