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

namespace ClassmateTraceBack.Models.Home
{
    public class CommentsService
    {
        public CommentsService() { }

        public List<ClassComment> GetClassComments(int class_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM 班级留言 WHERE `班级ID` = @class_id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    List<ClassComment> coms = new List<ClassComment>();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            int comID = reader.GetInt32(0);
                            int commentator = reader.GetInt32(2);
                            string comment = reader.GetString(3);
                            int likes = reader.GetInt32(4);
                            ClassComment tmp = new ClassComment(comID, class_id, commentator, comment, likes);
                            coms.Add(tmp);
                        }
                    }
                    reader.Close();
                    return coms;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("获取班级留言出错: " + ex.Message);
                    return null;
                }
                finally { connection.Close(); }
            }
        }

        public bool MakeClassComment(int user_id, int class_id, string com)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO 班级留言 (`班级ID`, `留言同学ID`, `留言信息`) VALUES (@class_id, @user_id, @com)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    cmd.Parameters.AddWithValue("@com", com);
                    int tmp = cmd.ExecuteNonQuery();
                    return tmp > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("留言出错: " + ex.Message);
                    return false;
                }
                finally { connection.Close(); }
            }
        }

        public bool LikeClassComment(int com_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE 班级留言 SET 点赞数 = 点赞数 + 1 WHERE 班级留言ID = @com_id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@com_id", com_id);
                    int tmp = cmd.ExecuteNonQuery();
                    return tmp > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("点赞留言失败: " + ex.Message);
                    return false;
                }
                finally { connection.Close(); }
            }
        }


        public bool DeleteClassComment(int com_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM 班级留言 WHERE 班级留言ID = @com_id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@com_id", com_id);
                    int tmp = cmd.ExecuteNonQuery();
                    return tmp > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("删除留言失败: " + ex.Message);
                    return false;
                }
                finally { connection.Close(); }
            }
        }
    }
}
