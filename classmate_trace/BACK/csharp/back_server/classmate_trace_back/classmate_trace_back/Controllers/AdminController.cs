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
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService adminService;

        public AdminController(AdminService adminService)
        {
            this.adminService = adminService;
        }



        // POST: admin
        [HttpPost]
        public ActionResult<Boolean> UpdateUserInfo()
        {
            try
            {
                adminService.CheckUsersInfo();
                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }
        }

        [HttpGet]
        public ActionResult<List<UserInfo>> GetDisplayUserInfo()
        {
            try
            {
                var stats = adminService.DisplayUserInfo();
                return Ok(stats);
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }
        }
    }
}
