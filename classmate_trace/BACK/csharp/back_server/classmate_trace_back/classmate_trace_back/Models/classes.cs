using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static ClassmateTraceBack.Models.classes;
using Microsoft.Extensions.Configuration;

namespace ClassmateTraceBack.Models
{
    public static class classes{

        
        
        public static string connectionString = "server=localhost;database=classmate_trace;uid=root;password=123456";


        //照片
        public class Picture
        {
            public int PictureId { get; set; } //照片ID
            public string Image { get; set; } //图片url
            public int ClassId { get; set; } //班级ID
            public int ClassmateId { get; set; } // 上传同学ID
            public DateTime Date { get; set; } //时间
            public int Likes { get; set; } //点赞数
            public int? GatheringId { get; set; } //小聚ID
            public string? Address { get; set; } //地点
            public string? Description { get; set; } //描述

            public Picture(int pictureId, string image, int classId, int classmateId, DateTime date, int likes = 0) {
                this.PictureId = pictureId;
                this.Image = image;
                this.ClassId = classId;
                this.ClassmateId = classmateId;
                this.Date = date;
                this.Likes = likes;
            }

            public Picture(int pictureId, string image, int classId, int classmateId, DateTime date, int gatheringId,string address,string description, int likes)
            {
                this.PictureId = pictureId;
                this.Image = image;
                this.ClassId = classId;
                this.ClassmateId = classmateId;
                this.Date = date;
                this.Likes = likes;
                this.Description = description;
                this.Address = address;
                this.GatheringId = gatheringId;
            }
            
        }

        //上传照片
        public class UpdatePicture
        {
            public int code { get; set; }
            public Picture picture { get; set; }

            public UpdatePicture(int code,Picture picture)
            {
                this.code = code;
                this.picture = picture;
            }


        }

        //回忆
        public class Memory {
            public int MemoryId { get; set; }  //回忆ID
            public int ClassId { get; set; }  //班级ID
            public int? Theme {  get; set; }  //回忆主题
            public string? Music {  get; set; }  //音乐URL
            public List<int> Pictures { get; set; }  //照片ID列表
            
            public Memory(int memoryId, int classId)
            {
                this.MemoryId = memoryId;
                this.ClassId = classId;
                this.Pictures = new List<int>();
            }

        }

        //位置信息
        public class Address
        {
            public int ClassmateId { get; set; }  //同学ID
            public string? AddressNew { get; set; } //最新位置信息
            public string? AddressHistory { get; set; } //历史位置轨迹线

            public Address(int classmateId) { this.ClassmateId = classmateId; }
        }

        public struct UnitInform
        {
            public Classmate? Classmate { get; set; }
            public Address? Address { get; set; }
        }

        // 同学
        public class Classmate
        {
            public int ClassmateId { get; set; }  //同学ID
            public string PhoneNumber { get; set; }  //电话
            public string Password { get; set; }  //密码
            public string Name { get; set; }  //姓名

            public DateTime? Birthday { get; set; }  //生日
            public string? Mail { get; set; }  //邮箱
            public string? Sign { get; set; }  //签名
            public List<int>? Classes { get; set; }  //班级ID列表
            public DateTime? LoginTime { get; set; }  //最后登录时间
            public int? Gender { get; set; }  //性别
            public string? HeadPicture { get; set; }  //头像

            public Classmate(int classmateId, string phoneNumber, string password, string name)
            {
                this.ClassmateId = classmateId;
                this.PhoneNumber = phoneNumber;
                this.Password = password;
                this.Name = name;

                this.Birthday = null;
                this.Mail = null;
                this.Sign = null;
                this.Classes = null;
                this.LoginTime = null;
                this.Gender = null;
                this.HeadPicture = null;
            }

            public Classmate(int classmateId,string name, string password, string phoneNumber, int? gender = 0, string? mail = "", string? sign = "", string? headpicture = "", DateTime? birthday = null)
            {
                this.ClassmateId = classmateId;
                this.PhoneNumber = phoneNumber;
                this.Password = password;
                this.Name = name;

                this.Birthday = birthday;
                this.Mail = mail;
                this.Sign = sign;
                this.Classes = null;
                this.LoginTime = null;
                this.Gender = gender;
                this.HeadPicture = headpicture;
            }



        }

        public class ConnectionPC
        {
            public int pictureId;
            public int classmateId;

            public ConnectionPC(int pictureId, int classmateId)
            {
                this.pictureId = pictureId;
                this.classmateId = classmateId;
            }

        }

        public class UserInfo
        {
            public DateTime Date { get; set; }
            public int MaleNum { get; set; }
            public int FemaleNum { get; set; }
            public int OtherNum { get; set; }
            public int UnknownNum { get; set; }
            public int LoginUserNum { get; set; }

        }


        public class PersonalComment
        {
            public int comID {  get; set; }
            public int userID {  get; set; }
            public int commentator {  get; set; }
            public string comment { get; set; }
            public PersonalComment() { }
            public PersonalComment(int comID, int userID, int commentator, string comment)
            {
                this.comID = comID;
                this.userID = userID;
                this.commentator = commentator;
                this.comment = comment;
            }
        }

        public class RePersonalComment
        {
           public int comID {  get; set; }
            public int reID {  get; set; }
            public int commentator {  get; set; }
            public string comment { get; set; }
            public RePersonalComment() { }
            public RePersonalComment(int comID, int reID, int commentator, string comment)
            {
                this.comID = comID;
                this.reID = reID;
                this.commentator = commentator;
                this.comment = comment;
            }
        }

        //小聚
        public class Gathering
        {
            public int GaID {  get; set; }
            public int ClassID {  get; set; }
            public int Proposer {  get; set; }
            public DateTime Date {  get; set; }//后期改DateTime
            public string Position {  get; set; }
            public bool Read {  get; set; }//true为已读
            public string Discription {  get; set; }
            public List<int> Participant {  get; set; }
            public Gathering() { }
            public Gathering(int gaID,int classId,int proposer,DateTime date,string position,string discription, List<int> participant)
            {
                GaID = gaID;
                ClassID = classId;
                Proposer = proposer;
                Date = date;
                Position = position;
                Discription = discription;
                Participant = participant;
                Read=true;
            }
        }

        //新建小聚
        public class CreateGathering
        {
            public int code { get; set; }
            public Gathering gathering { get; set; }

            public CreateGathering(int code,Gathering gathering)
            {
                this.code = code;
                this.gathering = gathering;
            }
        }

        //更新小聚
        public class UpdateGathering
        {
            public int code { get; set;  }

            public Gathering gathering { get; set; }
            public UpdateGathering(int code, Gathering gathering)
            {
                this.code = code;
                this.gathering = gathering;
            }

        }


        //加入小聚
        public class JoiningGathering
        {
            public int code { get; set; }
            public Gathering gathering { get; set; }

            public JoiningGathering(int code, Gathering gathering)
            {
                this.code = code;
                this.gathering = gathering;
            }
        }

        //退出小聚
        public class ExitingGathering
        {
            public int code { get; set; }
            public Gathering gathering { get; set; }

            public ExitingGathering(int code, Gathering gathering)
            {
                this.code = code;
                this.gathering = gathering;
            }
        }


        //班级留言
        public class ClassComment
        {
            public int comID {  get; set; }
            public int classID {  get; set; }
            public int commentator {  get; set; }
            public string comment { get; set; }
            public int likes {  get; set; }
            public ClassComment() { likes=0; }
            public ClassComment(int comID, int classID, int commentator, string comment, int likes)
            {
                this.comID = comID;
                this.classID = classID;
                this.commentator = commentator;
                this.comment = comment;
                this.likes = likes;
            }
        }

        //班级
        public class Class
        {
            public int ClassID { get; set; }
            public string Name { get; set; }//班级名称
            public DateTime? Date { get; set; }//班级成立时间——后期改DateTime
            public int monitor {  get; set; }//班长id
            public List<int> classmates { get; set; }//同学ID列表
            public List<int> gatherings { get; set; }//小聚ID列表
            public Class() { }
            public Class(int classID, string name, DateTime? date, int monitor, List<int> classmates, List<int> gatherings)
            {
                ClassID = classID;
                Name = name;
                Date = date;
                this.monitor = monitor;
                this.classmates = classmates;
                this.gatherings = gatherings;
            }
        }

        public class Apply
        {
            public int userID { get; set; }
            public int classID { get; set; }
            public string userName { get; set; }
            public string className { get; set; }
            public Apply(int userID, int classID, string userName, string className)
            {
                this.userID = userID;
                this.classID = classID;
                this.userName = userName;
                this.className = className;
            }
        }

        public class LoginStatus
        {
            public bool Exist{ get; set; }
            public int UserId { get; set; }
            public string Message { get; set; }

            public LoginStatus()
            {
                Exist = false;
                UserId = 0;
                Message = "";
            }


            public LoginStatus(bool exist,int userId,string message)
            {
                Exist = exist;
                UserId = userId;
                Message = message;
            }
        }



    }

    
}
