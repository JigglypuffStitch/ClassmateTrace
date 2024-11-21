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
using System.Globalization;
using static ClassmateTraceBack.Controllers.RegisterController;
using Memory = ClassmateTraceBack.Models.classes.Memory;

namespace ClassmateTraceBack.Controllers
{

    [ApiController]
    [Route("album/memoirs")]
    public class MemoryController : ControllerBase
    {
        private readonly MemoryService memoryService;

        public MemoryController(MemoryService memoryService)
        {
            this.memoryService = memoryService;
        }


        /*
        // POST: album/memoirs
        [HttpPost]
        public ActionResult<Memory?> RecordMemory(int? theme, string? music, string? pictures)
        {

            var headerKey = Request.Headers["class_id"].FirstOrDefault();
            int class_id = Convert.ToInt32(headerKey);
            try
            {
                Memory? memory = memoryService.AddMemory(class_id, theme, music, pictures);
                return Ok(memory);
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }
        }
        */

        public class MemoryData
        {
            public int class_id { get; set; }
            public int? theme { get; set; }
            public string? music { get; set; }
            public string? pictures { get; set; }
            
        }

        // POST: album/memoirs
        [HttpPost]
        public IActionResult RecordMemory([FromBody] MemoryData memoryData)
        {
            var class_id = memoryData.class_id;
            var theme = memoryData.theme;
            var music = memoryData.music;
            var pictures = memoryData.pictures;

            try
            {
                Memory? memory = memoryService.AddMemory(class_id, theme, music, pictures);
                return Ok(memory);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        // GET: album/memoirs
        [HttpGet]
        public ActionResult<List<Memory>> GetClassMemories(int class_id)
        {
            try
            {
                List<Memory> memories = memoryService.QueryMemoryByClass(class_id);

                return Ok(memories);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

    }





}