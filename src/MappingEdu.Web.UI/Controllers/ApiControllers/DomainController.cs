// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Domains;
using MappingEdu.Service.Model.Domain;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing domains
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class DomainController : ControllerBase
    {
        private readonly IDomainService _domainService;

        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        [Route("Domain/{id:guid}")]
        [AcceptVerbs("GET")]
        public DomainViewModel[] Get(Guid id)
        {
            return _domainService.Get(id);
        }

        [Route("Domain/{id:guid}/{id2:guid}")]
        [AcceptVerbs("GET")]
        public DomainViewModel Get(Guid id, Guid id2)
        {
            return _domainService.Get(id, id2);
        }

        [Route("Domain")]
        [AcceptVerbs("POST")]
        public DomainViewModel Post(DomainCreateModel createModel)
        {
            return _domainService.Post(createModel);
        }

        [Route("Domain/{id:guid}")]
        [AcceptVerbs("PUT")]
        public DomainViewModel Put(Guid id, DomainEditModel editModel)
        {
            return _domainService.Put(id, editModel);
        }

        [Route("Domain/{id:guid}/{id2:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id, Guid id2)
        {
            _domainService.Delete(id, id2);
        }
    }
}