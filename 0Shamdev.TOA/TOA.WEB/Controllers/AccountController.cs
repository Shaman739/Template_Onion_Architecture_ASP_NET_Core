using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shamdev.ERP.Core.Data.Domain;
using Shamdev.TOA.BLL;
using Shamdev.TOA.Web;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Service.Interface;
using Shamdev.TOA.BLL.Service.DTO;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;

namespace RegisterHouse.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {

        IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [HttpPost]
       // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(DefaultParamOfCRUDOperation<UserDTO> param)
        {

            BaseResultType result = await _accountService.LoginAllowCheckAsync(param);
            if(result.Status == ResultStatus.Success)
            {
                await Authenticate(param.Item.Email); // аутентификация
            }

            return Json(result);
        }

        [HttpPost]
        [Route("Registration")]
     //   [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(DefaultParamOfCRUDOperation<UserDTO> param)
        {
            BaseResultType result = await _accountService.RegisterAsync(param);
            if (result.Status == ResultStatus.Success)
            {
                await Authenticate(param.Item.Email); // аутентификация
            }

            return Json(result);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            BaseResultType result = new BaseResultType() { Status = ResultStatus.Success };
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            }
            catch(Exception e)
            {
                result.AddError(e.Message);
            }
            return Json(result);
        }
    }

}
