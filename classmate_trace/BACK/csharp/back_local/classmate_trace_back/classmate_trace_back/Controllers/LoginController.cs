using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mysqlx.Crud;
using System.Reflection.Metadata;
using ClassmateTraceBack.Models;
using static ClassmateTraceBack.Models.classes;
using System.Drawing.Printing;


namespace ClassmateTraceBack.Controllers
{
    [ApiController]
    [Route("entrance/login")]

    public class LoginController : ControllerBase
    {
        private readonly LoginAndRegisterService loginAndRegisterService;

        public LoginController(LoginAndRegisterService loginAndRegisterService)
        {
            this.loginAndRegisterService = loginAndRegisterService;
        }

        //GET:entrance/login
        [HttpGet]
        public ActionResult<LoginStatus> LoginUser(string phonenumber, string password)
        {
            try
            {
                var newLoginStatus = loginAndRegisterService.UserLogin(phonenumber, password);
                return Ok(newLoginStatus);
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }
        }

    }
}
