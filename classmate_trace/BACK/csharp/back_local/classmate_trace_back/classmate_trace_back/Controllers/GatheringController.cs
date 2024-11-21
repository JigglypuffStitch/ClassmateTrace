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
using Microsoft.Extensions.Logging;

namespace ClassmateTraceBack.Controllers
{
    [ApiController]
    [Route("gathering")]
    public class GatheringController : ControllerBase
    {
        private readonly GatheringService gatheringService;
        private readonly ILogger<GatheringController> _logger;

        public GatheringController(GatheringService gatheringService, ILogger<GatheringController> logger)
        {
            this.gatheringService = gatheringService;
            _logger = logger;
        }

        public class GatheringListRequest
        {
            [FromQuery(Name = "class_id")]
            public int ClassId { get; set; }
        }

        //GET:/gathering
        [HttpGet]
        public ActionResult<List<object>> GatheringList([FromQuery] GatheringListRequest request)
        {
            try
            {
                _logger.LogInformation($"Received request for class_id: {request.ClassId}");
                var Gatheringlist = gatheringService.GetGatheringList(request.ClassId);
                return Ok(Gatheringlist);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in GatheringList");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("picture")]
        public ActionResult<List<Picture>> PictureList(int gathering_id)
        {
            try
            {
                _logger.LogInformation($"Querying pictures for gathering_id: {gathering_id}");
                var Picturelist = gatheringService.DisplayPicture(gathering_id);
                return Ok(Picturelist);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting pictures");
                return BadRequest(e.Message);
            }
        }

        //POST:gathering
        [HttpPost]
        public ActionResult<CreateGathering> NewGathering(int class_id, DateTime datetime, string address, string description)
        {
            try
            {
                var headerKey = Request.Headers["user_id"].FirstOrDefault();
                int ownerid = Convert.ToInt32(headerKey);
                _logger.LogInformation($"Creating new gathering for class_id: {class_id}, owner: {ownerid}");
                var newGathering = gatheringService.GatheringCreate(class_id, datetime, ownerid, address, description);
                return Ok(newGathering);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating gathering");
                return BadRequest(e.Message);
            }
        }

        //POST:gathering/uploadpicture
        [HttpPost("uploadpicture")]
        public ActionResult<UpdatePicture> NewPicture([FromBody] UpdatePictureRequest request)
        {
            Console.WriteLine($"class_id: {request.class_id}");
            Console.WriteLine($"picture_url: {request.picture_url}");
            Console.WriteLine($"address: {request.address}");
            Console.WriteLine($"description: {request.description}");
            Console.WriteLine($"gathering_id: {request.gathering_id}");

            try
            {
                var headerKey = Request.Headers["user_id"].FirstOrDefault();
                int user_id = Convert.ToInt32(headerKey);

                var newPicture = gatheringService.updatePicture(user_id, request.class_id, request.picture_url, request.address, request.description, request.gathering_id);
                return Ok(newPicture);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        public class UpdatePictureRequest
        {
            public int class_id { get; set; }
            public string picture_url { get; set; }
            public string address { get; set; }
            public string description { get; set; }
            public int gathering_id { get; set; }
        }

        // DELETE: gathering
        [HttpDelete]
        public ActionResult<Boolean> GatheringDelete(int gathering_id)
        {
            try
            {
                bool result = gatheringService.DeleteGathering(gathering_id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
            //return NoContent();
        }


        //PUT:gathering
        [HttpPut("join")]
        public ActionResult<JoiningGathering> GatheringJoining(int gathering_id)
        {
            try
            {
                var headerKey = Request.Headers["user_id"].FirstOrDefault();
                int userid = Convert.ToInt32(headerKey);
                var Gatheringjoin = gatheringService.JoinGathering(userid, gathering_id);
                return Ok(Gatheringjoin);
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }
        }

        //PUT:gathering
        [HttpPut("change")]
        public ActionResult<UpdateGathering> GatheringChange(string datetime, string address, string description)
        {
            try
            {
                var headerKey0 = Request.Headers["user_id"].FirstOrDefault();
                int userid = Convert.ToInt32(headerKey0);
                var headerKey1 = Request.Headers["gathering_id"].FirstOrDefault();
                int gatheringid = Convert.ToInt32(headerKey1);
                var Gatheringchange = gatheringService.ChangeGathering(gatheringid, userid, datetime, address, description);
                return Ok(Gatheringchange);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        [HttpPut("exit")]
        public ActionResult<ExitingGathering> GatheringExiting()
        {
            try
            {
                var headerKey0 = Request.Headers["user_id"].FirstOrDefault();
                int userid = Convert.ToInt32(headerKey0);
                var headerKey1 = Request.Headers["gathering_id"].FirstOrDefault();
                int gathering_id = Convert.ToInt32(headerKey1);
                var Gatheringexit = gatheringService.ExitGathering(gathering_id, userid);
                return Ok(Gatheringexit);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }




    }
}
