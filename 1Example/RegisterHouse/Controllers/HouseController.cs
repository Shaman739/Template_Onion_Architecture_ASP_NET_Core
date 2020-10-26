using Shamdev.TOA.BLL;
using Core.Data.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shamdev.TOA.Web;

namespace RegisterHouse.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class HouseController : DefaultController<House>
    {
        public HouseController(ILogger<HouseController> logger, IDefaultCRUDBLL<House> defaultCRUDBLL) : base(logger, defaultCRUDBLL)
        {
        }
    }
}
