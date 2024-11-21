using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using static ClassmateTraceBack.Models.classes;
using static ClassmateTraceBack.Models.AddressService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ClassmateTraceBack.Models
{
    public class PersonalService
    {
        public List<PersonalComment>? GetPersonalComment(int user_id)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    List<PersonalComment> pcoms = new List<PersonalComment>();

                    connection.Open();
                    string query = "SELECT * FROM 个人留言 WHERE 被留言同学ID=@user_id";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            int pcomID = reader.GetInt32(0);
                            int commentator = reader.GetInt32(2);
                            string comment = reader.GetString(3);
                            PersonalComment tmp = new PersonalComment(pcomID, user_id, commentator, comment);
                            pcoms.Add(tmp);
                        }
                    }
                    reader.Close();
                    return pcoms;
                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("获取个人留言错误" + ex.Message);
                    return null;
                }
                finally { connection.Close(); }
            }
        }

        public List<RePersonalComment>? GetCommentDetail(int com_id)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    List<RePersonalComment> rcoms = new List<RePersonalComment>();

                    connection.Open();
                    string query = "SELECT * FROM 留言回复 WHERE 回复留言ID=@com_id";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@com_id", com_id);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            int pcomID = reader.GetInt32(0);
                            int commentator = reader.GetInt32(2);
                            string comment = reader.GetString(3);
                            RePersonalComment tmp = new RePersonalComment(pcomID, com_id, commentator, comment);
                            rcoms.Add(tmp);
                        }
                    }
                    reader.Close();
                    return rcoms;
                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("获取留言详情错误" + ex.Message);
                    return null;
                }
                finally { connection.Close(); }
            }
        }

        public bool PostComment(int user_id, int other_id, string comment)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO 个人留言 (被留言同学ID,留言同学ID,留言信息) VALUES (@other_id,@user_id,@comment)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@other_id", other_id);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@comment", comment);
                    int tmp = cmd.ExecuteNonQuery();
                    return tmp > 0;
                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("发布个人留言错误" + ex.Message);
                    return false;
                }
                finally { connection.Close(); }
            }
        }


        public bool ReplyComment(int user_id, int rcom_id, string comment)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO 留言回复 (回复留言ID,留言同学ID,留言信息) VALUES (@rcom_id,@user_id,@comment)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@rcom_id", rcom_id);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@comment", comment);
                    int tmp = cmd.ExecuteNonQuery();
                    return tmp > 0;
                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("回复个人留言错误" + ex.Message);
                    return false;
                }
                finally { connection.Close(); }
            }
        }
    }
}
