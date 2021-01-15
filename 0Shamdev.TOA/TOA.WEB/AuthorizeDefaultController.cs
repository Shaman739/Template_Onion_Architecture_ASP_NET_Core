using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Shamdev.TOA.BLL;
using Shamdev.TOA.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shamdev.TOA.Web
{
    [Authorize]
    public class AuthorizeDefaultController<TEntity> : DefaultController<TEntity>
           where TEntity : DomainObject, new()
    {
        public AuthorizeDefaultController(ILogger<DefaultController<TEntity>> logger, IDefaultCRUDBLL<TEntity> defaultCRUDBLL) : base(logger, defaultCRUDBLL)
        {
        }
    }

}
