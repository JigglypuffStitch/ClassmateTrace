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
using Microsoft.AspNetCore.Cors;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ClassmateTraceBack.Controllers
{

    [ApiController]
    [EnableCors(PolicyName = "AllowSpecificOrigin")]
    [Route("setting")]
    public class SettingController : ControllerBase
    {
        private readonly ClassmateService classmateService;
        private readonly AddressService addresService;

        public SettingController(ClassmateService classmateService, AddressService addresService)
        {
            this.classmateService = classmateService;
            this.addresService = addresService;
        }


        // GET: setting
        [HttpGet]
        public ActionResult<Classmate?> GetClassmateInfo(int classmate_id)
        {
            try
            {
                var classmate = classmateService.QueryClassmateByID(classmate_id);
                return Ok(classmate);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }



        /*
        // PUT: setting
        [HttpPut]
        public ActionResult<UnitInform> EditClassmateInfo(int classmate_id, string password, string username, string? birthdayString, string? mail, int? gender, string? headPicture, string? addressInfo)
        {

            UnitInform unitInform = new UnitInform();
            DateTime? birthday = null;
            if (birthdayString != null)
            {
                DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy-MM-dd";
                birthday = Convert.ToDateTime(birthdayString, dtFormat);
            }
            try
            {
                var classmate = classmateService.UpdateClassmate(classmate_id, password, username, birthday, mail, null, gender, headPicture);
                unitInform.Classmate = classmate;
                var address = addresService.UpdateAddress(classmate_id, addressInfo);
                unitInform.Address = address;
                return Ok(unitInform);
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }
        }*/
        public class SettingsData
        {
            public int classmate_id { get; set; }
            public string password { get; set; }
            public string username { get; set; }
            public string? birthdayString { get; set; }
            public string? mail { get; set; }
            public int gender { get; set; }
            public string? headPicture { get; set; }
        }
        [HttpPut]
        public IActionResult UpdateSettings([FromBody] SettingsData requestData)
        {
            try
            {
                var classmate_id = requestData.classmate_id;
                var password = requestData.password;
                var username = requestData.username;
                var birthdayString = requestData.birthdayString;
                var mail = requestData.mail;
                var gender = requestData.gender;
                var headPicture = requestData.headPicture;
                UnitInform unitInform = new UnitInform();
                DateTime? birthday = null;
                if (birthdayString != null)
                {
                    DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
                    dtFormat.ShortDatePattern = "yyyy-MM-dd";
                    birthday = Convert.ToDateTime(birthdayString, dtFormat);
                }
                var classmate = classmateService.UpdateClassmate(classmate_id, password, username, birthday, mail, null, gender, headPicture);
                unitInform.Classmate = classmate;
                return Ok(unitInform);

            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }
    }





}