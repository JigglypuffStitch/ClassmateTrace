using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using static ClassmateTraceBack.Models.classes;

namespace ClassmateTraceBack.Models
{
    public class ClassmateService
    {

        public ClassmateService()
        {

        }

        //根据同学ID查找同学信息
        public Classmate? QueryClassmateByID(int ID)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    Classmate? classmate = null;

                    connection.Open();
                    string query = "SELECT 电话,密码,姓名,生日,邮箱,签名,班级ID列表,最后登录时间,性别,头像 FROM 同学 WHERE 同学ID=@ID";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@ID", ID);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string phoneNumber = reader.GetString(0);
                        string password = reader.GetString(1);
                        string name = reader.GetString(2);
                        DateTime? birthday = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);
                        string? mail = reader.IsDBNull(4) ? null : reader.GetString(4);
                        string? sign = reader.IsDBNull(5) ? null : reader.GetString(5);
                        string? classesString = reader.IsDBNull(6) ? null : reader.GetString(6);
                        DateTime? loginTime = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);
                        int? gender = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8);
                        string? headPicture = reader.IsDBNull(9) ? null : reader.GetString(9);

                        if (classmate == null)
                        {
                            classmate = new(ID, phoneNumber, password, name);
                        }
                        else
                        {
                            classmate.PhoneNumber = phoneNumber;
                            classmate.Password = password;
                            classmate.Name = name;
                        }
                        if (birthday != null)
                        {
                            classmate.Birthday = birthday;
                        }
                        if (mail != null)
                        {
                            classmate.Mail = mail;
                        }
                        if (sign != null)
                        {
                            classmate.Sign = sign;
                        }

                        if (classesString != null)
                        {
                            string[] class_list = classesString.Split(' ');
                            if (class_list != null)
                            {
                                if (classmate.Classes == null)
                                {
                                    classmate.Classes = new List<int>();
                                }
                                for (int i = 0; i < class_list.Length; i++)
                                {
                                    int classId = Convert.ToInt32(class_list[i]);
                                    classmate.Classes.Add(classId);
                                }
                            }
                        }


                        if (loginTime != null)
                        {
                            classmate.LoginTime = loginTime;
                        }
                        if (gender != null)
                        {
                            classmate.Gender = gender;
                        }
                        if (headPicture != null)
                        {
                            classmate.HeadPicture = headPicture;
                        }
                    }
                    reader.Close();
                    connection.Close();


                    /*
                    if(classmate == null)
                    {
                        throw new Exception("未找到该同学ID对应的同学信息");
                    }
                    */

                    return classmate;
                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("查找同学信息错误" + ex.Message);
                    return null;
                }
            }
        }


        //更新同学信息
        public Classmate? UpdateClassmate(int ID, string? password, string username, DateTime? birthday, string? mail, string? sign, int? gender, string? headPicture)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    Classmate? classmate = QueryClassmateByID(ID);
                    if (classmate == null) { return null; }
                    if (password == null) { password = classmate.Password; }
                    connection.Open();
                    string query = "UPDATE 同学 SET 密码=@password,姓名=@username,生日=@birthday,邮箱=@mail,签名=@sign,性别=@gender,头像=@headPicture WHERE 同学ID=@ID";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@ID", ID);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@birthday", birthday);
                    cmd.Parameters.AddWithValue("@mail", mail);
                    cmd.Parameters.AddWithValue("@sign", sign);
                    cmd.Parameters.AddWithValue("@gender", gender);
                    cmd.Parameters.AddWithValue("@headPicture", headPicture);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    classmate = QueryClassmateByID(ID);
                    return classmate;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    Console.WriteLine("更新同学信息错误" + ex.Message);
                    return null;
                }
            }

        }

    }
}
