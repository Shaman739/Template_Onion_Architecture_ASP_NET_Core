using Shamdev.TOA.BLL;
using Core.Data.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shamdev.TOA.Web;
using Shamdev.TOA.BLL.Interface;

namespace RegisterHouse.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class StreetController : AuthorizeDefaultController<Street>
    {
        public StreetController(ILogger<StreetController> logger, IDefaultCRUDBLL<Street> defaultCRUDBLL, IFetchData<Street> fetchData) : base(logger, defaultCRUDBLL, fetchData)
        {
        }
    }
}
