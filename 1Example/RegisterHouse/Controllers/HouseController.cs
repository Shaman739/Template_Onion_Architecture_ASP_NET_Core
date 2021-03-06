﻿using Shamdev.TOA.BLL;
using Core.Data.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shamdev.TOA.Web;
using Shamdev.TOA.BLL.Interface;

namespace RegisterHouse.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class HouseController : DefaultController<House>
    {
        public HouseController(ILogger<HouseController> logger, IDefaultCRUDBLL<House> defaultCRUDBLL, IFetchData<House> fetchData) : base(logger, defaultCRUDBLL, fetchData)
        {
        }
    }
}
