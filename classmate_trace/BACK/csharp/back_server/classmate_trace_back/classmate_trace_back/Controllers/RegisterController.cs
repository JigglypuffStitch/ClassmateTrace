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
    [Route("entrance/register")]

    public class RegisterController : ControllerBase
    {
        private readonly LoginAndRegisterService loginAndRegisterService;

        public RegisterController(LoginAndRegisterService loginAndRegisterService)
        {
            this.loginAndRegisterService= loginAndRegisterService;
        }

        /*
        //POST:entrance/register
        [HttpPost]
        public ActionResult<Classmate> RegisterUser(string username, string password, string phonenumber, int? gender = 0, string? mail = "", string? sign = "", string? headpicture = "", DateTime? birthday = null)
        {
            try
            {
                var newClassmate = loginAndRegisterService.UserRegister(username, password, phonenumber, gender, mail, sign, headpicture, birthday);
                return Ok(newClassmate);
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }
        }
        */

        public class RegisterData
        {
            public string username { get; set; }
            public string password { get; set; }
            public string phonenumber { get; set; }
            public int? gender { get; set; }
            public string? mail { get; set; }
            public string? sign { get; set; }
            public string? headpicture { get; set; }
            public string? birthdayString { get; set; }
        }

        public class registerResponse
        {
            public Classmate classmate { get; set; }
            public bool state { get; set; }
        }

        //POST:entrance/register
        [HttpPost]
        public IActionResult RegisterUser([FromBody] RegisterData requestData)
        {
            try
            {
                var username = requestData.username;
                var password = requestData.password;
                var phonenumber = requestData.phonenumber;
                var gender = requestData.gender;
                var mail = requestData.mail;
                var sign = requestData.sign;
                var headpicture = requestData.headpicture;
                var birthdayString = requestData.birthdayString;

                DateTime? birthday = null;
                if (birthdayString != null)
                {
                    DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
                    dtFormat.ShortDatePattern = "yyyy-MM-dd";
                    birthday = Convert.ToDateTime(birthdayString, dtFormat);
                }
                var newClassmate = loginAndRegisterService.UserRegister(username, password, phonenumber, gender, mail, sign, headpicture, birthday);
                registerResponse response = new registerResponse();
                if(newClassmate != null) {
                    response.classmate = newClassmate;
                    response.state = true;
                }
                else
                {
                    response.state = false;
                }
                return Ok(response);

            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

    }
}
