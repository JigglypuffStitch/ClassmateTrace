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
using static System.Net.Mime.MediaTypeNames;
using Picture = ClassmateTraceBack.Models.classes.Picture;

namespace ClassmateTraceBack.Controllers
{

    [ApiController]
    [Route("album")]
    public class AlbumController : ControllerBase
    {
        private readonly PictureService pictureService;
        private readonly ClassmateService classmateService;

        public AlbumController(PictureService pictureService, ClassmateService classmateService)
        {
            this.pictureService = pictureService;
            this.classmateService = classmateService;
        }

        /*
        // POST: album
        [HttpPost]
        public ActionResult<Picture?> UploadPicture(string image, int classId, string? dateString, string? address, string? description)
        {

            var headerKey = Request.Headers["user_id"].FirstOrDefault();
            int classmate_id = Convert.ToInt32(headerKey);
            DateTime date;
            if (dateString != null)
            {
                DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy-MM-dd";
                date = Convert.ToDateTime(dateString, dtFormat);
            }
            else { date = DateTime.Now; }
            try
            {
                Picture? picture = pictureService.AddPicture(image, classId, classmate_id, date, null, address, description); ;
                return Ok(picture);
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }
        }
        */

        public class AlbumData
        {
            public int classmate_id {  get; set; }
            public string image { get; set; }
            public int classId { get; set; }
            public string? dateString { get; set; }
            public string? address { get; set; }
            public string? description { get; set; }
        }


        // POST: album
        [HttpPost]
        public IActionResult UploadPicture([FromBody] AlbumData albumData)
        {
            var classmate_id = albumData.classmate_id;
            var image = albumData.image;
            var classId = albumData.classId;
            var dateString = albumData.dateString;
            var address = albumData.address;
            var description = albumData.description;

            DateTime date;
            if (dateString != null)
            {
                DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy-MM-dd";
                date = Convert.ToDateTime(dateString, dtFormat);
            }
            else { date = DateTime.Now; }
            try
            {
                Picture? picture = pictureService.AddPicture(image, classId, classmate_id, date, null, address, description); ;
                return Ok(picture);
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }
        }


        // GET: album
        [HttpGet]
        public ActionResult<List<Picture>> GetClassPictures(int class_id)
        {
            try
            {
                List<Picture> pictures = pictureService.QueryPictureByClass(class_id);
                
                return Ok(pictures);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        // DELETE: album
        [HttpDelete]
        public ActionResult<Boolean> DeletePicture(int classmate_id, int picture_id)
        {
            try
            {
                bool result = pictureService.DeletePicture(classmate_id, picture_id);
                return result;
            }
            catch (Exception e)
            {
                return NoContent();
            }
            //return NoContent();
        }


    }





}