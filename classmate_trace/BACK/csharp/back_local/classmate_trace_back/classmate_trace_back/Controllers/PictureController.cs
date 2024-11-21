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

namespace ClassmateTraceBack.Controllers
{

    [ApiController]
    [Route("album/detail")]
    public class PictureController : ControllerBase
    {
        private readonly PictureService pictureService;
        private readonly ClassmateService classmateService;

        public PictureController(PictureService pictureService, ClassmateService classmateService)
        {
            this.pictureService = pictureService;
            this.classmateService = classmateService;
        }

        public class ConnectionPCModel
        {
            public int ClassmateId { get; set; }
            public int PictureId { get; set; }
        }
        // POST: album/detail
        [HttpPost]
        public ActionResult<ConnectionPC> CircleSelf([FromBody] ConnectionPCModel model)
        {
            try
            {
                pictureService.RecordConnectionPC(model.PictureId, model.ClassmateId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }


        // DELETE: album/detail
        [HttpDelete]
        public ActionResult<Boolean> DecircleSelf(int classmate_id, int picture_id)
        {
            try
            {
                bool result = pictureService.DeleteConnectionPC(picture_id, classmate_id);
                return result;
            }
            catch (Exception e)
            {
                return NoContent();
            }
            //return NoContent();
        }

        // GET: album/detail
        [HttpGet]
        public ActionResult<List<Classmate>> GetCircledClassmates(int picture_id)
        {
            try
            {
                List<Classmate> results = new List<Classmate>();
                int[]? cmIDs = pictureService.QueryClassmateIDByPictureID(picture_id);
                if (cmIDs == null) { return Ok(results); }
                for (int i = 0; i < cmIDs.Length; i++)
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

        public class isCircledData
        {
            public bool result { get; set; }
        }

        // GET: album/detail/conn
        [HttpGet("conn")]
        public ActionResult<Boolean> IsCircled(int user_id, int picture_id)
        {
            try
            {
                isCircledData isCircledData = new isCircledData();
                isCircledData.result = pictureService.QueryCPConnection(user_id, picture_id);
                return Ok(isCircledData);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        // PUT: album/detail
        public class LikePictureModel
        {
            public int PictureId { get; set; }
        }
        [HttpPut]
        public ActionResult<Classmate?> LikePicture([FromBody] LikePictureModel model)
        {
            try
            {
                var classmate = pictureService.UpdatePictureLikes(model.PictureId);
                return Ok(classmate);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

    }





}