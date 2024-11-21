using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mysqlx.Crud;
using System.Reflection.Metadata;
using static ClassmateTraceBack.Models.classes;
using System.Drawing.Printing;
using ClassmateTraceBack.Models.Home;
using ClassmateTraceBack.Models;
using System.Globalization;
using System.Threading;

namespace ClassmateTraceBack.Controllers.Home
{
    [ApiController]
    [Route("home/class")]
    public class ClassController : ControllerBase
    {
        private readonly ClassService classService;
        public ClassController(ClassService classService)
        {
            this.classService = classService;
        }


        //POST:/home/class/create
        public class ClassCreateModel
        {
            public string ClassName { get; set; }
            public DateTime ClassTime { get; set; }
            public int Monitor { get; set; }
        }
        [HttpPost("create")]
        public ActionResult<Class> CreateClass([FromBody] ClassCreateModel model)
        {
            try
            {
                DateTime? classTime = null;
                DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy-MM-dd";
                classTime = Convert.ToDateTime(model.ClassTime, dtFormat);
                var newClass = classService.CreateClass(model.ClassName, model.ClassTime, model.Monitor);
                return Ok(newClass);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }


        //GET:/home/class
        [HttpGet]
        public ActionResult<Class> SwitchClass(int class_id)
        {
            try
            {
                var newClass = classService.FindClass(class_id);
                return Ok(newClass);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }


        public class ClassApplyModel
        {
            public int ClassId { get; set; }
            public int UserId { get; set; }
        }
        public class ClassApplyResponse
        {
            public string response { get; set; }
        }
        //POST:/home/class/apply
        [HttpPost("apply")]
        public IActionResult Apply4Class([FromBody] ClassApplyModel model)
        {
            try
            {
                ClassApplyResponse response = new ClassApplyResponse();
                bool tmp = classService.Apply4Class(model.ClassId, model.UserId);
                if (tmp)
                {
                    response.response = "Application to join class successful.";
                    return Ok(response);
                }
                response.response = "Failed to apply to join class.";
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }

        [HttpGet("allapply")]
        public ActionResult<List<Apply>> GetApplys(int monitor)
        {
            try
            {
                var applys = classService.FindAllApplys(monitor);
                return Ok(applys);
            }
            catch (Exception ex)
            {
                Console.Write("Failed to get all applys:", ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }

        public class MonitorEesponse
        {
            public bool result { get; set; }
        }

        [HttpGet("monitor")]
        public IActionResult GetMonitor(int user_id, int class_id)
        {
            try
            {
                MonitorEesponse monitorEesponse = new MonitorEesponse();
                bool result = classService.IsMonitor(class_id, user_id);
                monitorEesponse.result = result;
                return Ok(monitorEesponse);
            }
            catch (Exception ex)
            {
                Console.Write("Failed to get monitor:", ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }


        [HttpGet("allclass")]
        public ActionResult<List<Class>> GetClasses(int user_id)
        {
            try
            {
                var applys = classService.FindAllClasses(user_id);
                return Ok(applys);
            }
            catch (Exception ex)
            {
                Console.Write("Failed to get all classes:", ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }

        public class ClassApprovalModel
        {
            public int ClassId { get; set; }
            public int ClassmateId { get; set; }
            public bool Decision { get; set; }
        }
        public class ClassApprovalResponse
        {
            public string response { get; set; }
        }
        [HttpPost("approval")]
        public IActionResult Approval2Class([FromBody] ClassApprovalModel model)
        {
            try
            {
                ClassApprovalResponse response = new ClassApprovalResponse();
                bool tmp = classService.Approval2Class(model.ClassmateId, model.ClassId, model.Decision);
                if (tmp)
                {
                    response.response = "Approval successful.";
                    return Ok(response);
                }
                response.response = "Approval failed.";
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }

    }
}
