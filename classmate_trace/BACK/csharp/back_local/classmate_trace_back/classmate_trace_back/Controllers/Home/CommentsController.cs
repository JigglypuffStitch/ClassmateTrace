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
    [Route("home/comments")]

    public class CommentsController : ControllerBase
    {
        private readonly CommentsService commentsServices;
        public CommentsController(CommentsService commentsService)
        {
            this.commentsServices = commentsService;
        }


        [HttpGet]
        public ActionResult<List<ClassComment>> GetClassComments(int class_id)
        {
            try
            {
                List<ClassComment> coms = commentsServices.GetClassComments(class_id);
                return Ok(coms);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }


        public class ClassCommentModel
        {
            public int ClassId { get; set; }
            public string Comment { get; set; }
            public int UserId { get; set; }
        }
        public class ClassCommentResponse
        {
            public string response { get; set; }
        }
        [HttpPost]
        public IActionResult MakeClassComment([FromBody] ClassCommentModel model)
        {
            ClassCommentResponse classCommentResponse = new ClassCommentResponse();
            try
            {
                bool tmp = commentsServices.MakeClassComment(model.UserId, model.ClassId, model.Comment);
                if (tmp)
                {
                    classCommentResponse.response = "Comment has been made successfully";
                    return Ok(classCommentResponse);
                }
                classCommentResponse.response = "Failed to make comment";
                return BadRequest(classCommentResponse);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }



        public class LikeClassCommentModel
        {
            public int CommentId { get; set; }
        }
        public class LikeClassCommentResponse
        {
            public string response { get; set; }
        }
        [HttpPut]
        public IActionResult LikeClassComment([FromBody] LikeClassCommentModel model)
        {
            LikeClassCommentResponse likeClassCommentResponse = new LikeClassCommentResponse();
            try
            {
                bool tmp = commentsServices.LikeClassComment(model.CommentId);
                if (tmp)
                {
                    likeClassCommentResponse.response = "You have successfully liked the message.";
                    return Ok(likeClassCommentResponse);
                }
                likeClassCommentResponse.response = "Failed to like the message";
                return BadRequest(likeClassCommentResponse);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }

        public class DeleteResponse
        {
            public string response { get; set; }
        }
        [HttpDelete]
        public IActionResult DeleteClassComment(int com_id)
        {
            DeleteResponse deleteResponse = new DeleteResponse();
            try
            {
                bool tmp = commentsServices.DeleteClassComment(com_id);
                if (tmp)
                {
                    deleteResponse.response = "You have successfully deleted the message.";
                    return Ok(deleteResponse);
                }
                deleteResponse.response = "Failed to delete the message";
                return BadRequest(deleteResponse);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return BadRequest(ex.InnerException.Message);
            }
        }
    }
}