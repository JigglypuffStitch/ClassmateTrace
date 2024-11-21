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
using static ClassmateTraceBack.Controllers.PersonalController;
using System.Diagnostics.Eventing.Reader;

namespace ClassmateTraceBack.Controllers
{
    [ApiController]
    [Route("personalspace")]
    public class PersonalController : ControllerBase
    {
        private readonly PersonalService personalService;
        private readonly AddressService addressService;
        private readonly PictureService pictureService;
        private readonly ClassmateService classmateService;
        public PersonalController(PersonalService personalService, AddressService addressService, PictureService pictureService, ClassmateService classmateServic)
        {
            this.personalService = personalService;
            this.addressService = addressService;
            this.pictureService = pictureService;
            this.classmateService = classmateServic;
        }

        //获取个人照片
        [HttpGet("picture")]
        public ActionResult<List<Picture>> GetPersonalPicturess(int user_id)
        {
            try
            {
                List<Picture> pictures = new List<Picture>();
                int[]? pIDs = pictureService.QueryPictureIDByClassmateID(user_id);
                if (pIDs == null) { return Ok(pictures); }
                for (int i = 0; i < pIDs.Length; i++)
                {
                    var picture = pictureService.QueryPictureByID(pIDs[i]);
                    if (picture == null) continue;
                    pictures.Add(picture);
                }
                return Ok(pictures);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        //获取个人信息
        [HttpGet("info")]
        public ActionResult<Classmate?> GetPersonalInfo(int user_id)
        {
            try
            {
                var classmate = classmateService.QueryClassmateByID(user_id);
                return Ok(classmate);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        public class InfoData
        {
            public int classmate_id { get; set; }
            public string username { get; set; }
            public string? sign { get; set; }
            public string? birthdayString { get; set; }
            public string? mail { get; set; }
            public int? gender { get; set; }
            public string? headPicture { get; set; }
        }

        //修改个人信息
        [HttpPut("info")]
        public IActionResult EditPersonalInfo([FromBody] InfoData infoData)
        {
            var classmate_id = infoData.classmate_id;
            var username = infoData.username;
            var sign = infoData.sign;
            var birthdayString = infoData.birthdayString;
            var mail = infoData.mail;
            var gender = infoData.gender;
            var headPicture = infoData.headPicture;

            DateTime? birthday = null;
            if (birthdayString != null)
            {
                DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy-MM-dd";
                birthday = Convert.ToDateTime(birthdayString, dtFormat);
            }
            try
            {
                var classmate = classmateService.UpdateClassmate(classmate_id, null, username, birthday, mail, sign, gender, headPicture);

                return Ok(classmate);
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);
            }
        }


        //获取个人位置
        [HttpGet("address")]
        public ActionResult<Address> GetPersonalAdd(int user_id)
        {
            try
            {
                var add = addressService.QueryAddressByID(user_id);
                return Ok(add);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        //获取个人留言
        [HttpGet("personalcom")]
        public ActionResult<PersonalComment> GetPersonalComment(int user_id)
        {
            try
            {
                var pcom = personalService.GetPersonalComment(user_id);
                return Ok(pcom);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        //获取留言详情
        [HttpGet("comdetail")]
        public ActionResult<PersonalComment> GetCommentDetail(int com_id)
        {
            try
            {
                var pcom = personalService.GetCommentDetail(com_id);
                return Ok(pcom);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        public class CommentData
        {
            public string comment { get; set; }
            public int others_id { get; set; }
            public int user_id { get; set; }
        }

        public class CommentResponse
        {
            public string message { get; set; }

            public CommentResponse(string message)
            {
                this.message = message;
            }
        }

        //发布留言
        [HttpPost("postcom")]
        public IActionResult PostComment([FromBody] CommentData commentData)
        {
            try
            {
                var comment = commentData.comment;
                var others_id = commentData.others_id;
                var user_id = commentData.user_id;
                var commentResponse = new CommentResponse("Failed to post comment");
                bool tmp = personalService.PostComment(user_id, others_id, comment);
                if (tmp)
                    commentResponse.message = "Comment has been posted successfully.";             
                return Ok(commentResponse);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }

        public class ReplyData
        {
            public string comment { get; set; }
            public int rcom_id { get; set; }
            public int user_id { get; set; }
        }

        //回复留言
        [HttpPost("recom")]
        public IActionResult ReplyComment([FromBody] ReplyData replyData)
        {
            try
            {
                var comment = replyData.comment;
                var rcom_id = replyData.rcom_id;
                var user_id = replyData.user_id;
                var commentResponse = new CommentResponse("Failed to reply comment");
                bool tmp = personalService.ReplyComment(user_id, rcom_id, comment);
                if (tmp)
                    commentResponse.message = "Comment has been replied successfully.";
                return Ok(commentResponse);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
        }
    }
}
