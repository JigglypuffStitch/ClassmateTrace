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
using Org.BouncyCastle.Ocsp;
using System.Reflection.Metadata.Ecma335;

namespace ClassmateTraceBack.Controllers.Home
{
    [ApiController]
    [Route("home/map")]
    public class MapController : ControllerBase
    {
        private readonly MapService mapService;
        private readonly AddressService addresService;
        private readonly ILogger<MapController> _logger;
        public MapController(MapService mapService, AddressService addresService, ILogger<MapController> logger)
        {
            this.mapService = mapService;
            this.addresService = addresService;
            _logger = logger;
        }

        [HttpGet("show_address")]
        public ActionResult<List<object>> GetClassmateAddress(int class_id)
        {
            try
            {
                _logger.LogInformation($"Getting locations for class_id: {class_id}");
                var locations = mapService.GetClassmatesLocations(class_id);
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting locations for class {class_id}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("show_gatherings")]
        public ActionResult<List<Gathering>> GetClassGatherings(int class_id, int user_id)
        {
            try
            {
                List<Gathering> gatherings = mapService.FindAllGatherings(user_id,class_id);
                return Ok(gatherings);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }

        [HttpPut("change")]
        public ActionResult<Address> ChangeAddress(string new_add, int user_id)
        {
            try
            {
                var address = addresService.UpdateAddress(user_id, new_add);
                return Ok(address);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }
    }
}
