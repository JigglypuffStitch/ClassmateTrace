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

namespace ClassmateTraceBack.Controllers
{
    [ApiController]
    [Route("classmate")]
    public class ClassmateListController : ControllerBase
    {
        private readonly ClassService classService;
        private readonly ClassmateService classmateService;
        public ClassmateListController(ClassService classService, ClassmateService classmateService)
        {
            this.classService = classService;
            this.classmateService = classmateService;

        }


        //GET:/classmate
        [HttpGet]

        public IActionResult ClassmateList()
        {
            var headerKey = Request.Headers["class_id"].FirstOrDefault();
            int class_id = Convert.ToInt32(headerKey);
            try
            {
                // var Classmatelist = classService.GetClassmateList(class_id);

                List<Classmate> results = new List<Classmate>();
                List<int> cmIDs = classService.GetClassmateList(class_id);
                if (cmIDs.Count == 0) { return Ok(results); }
                for (int i = 0; i < cmIDs.Count; i++)
                {
                    var classmate = classmateService.QueryClassmateByID(cmIDs[i]);
                    if (classmate == null) continue;
                    results.Add(classmate);
                }
                return Ok(results);
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }

        }
    }
}
