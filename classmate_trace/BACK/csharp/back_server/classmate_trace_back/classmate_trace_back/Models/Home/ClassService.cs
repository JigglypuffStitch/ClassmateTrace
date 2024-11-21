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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ClassmateTraceBack.Models.Home
{
    public class ClassService
    {
        public int getMax()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT MAX(班级ID) AS 最大班级ID FROM 班级";
                    int maxClassId = -1;
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read() && !reader.IsDBNull(0))
                        maxClassId = reader.GetInt32(0);
                    reader.Close();
                    return maxClassId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return -1;
                }
                finally { connection.Close(); }
            }
        }

        public int FindMonitor(int class_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT 班长ID FROM 班级 WHERE `班级ID` =@class_id";
                    int monitor = -1;
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read() && !reader.IsDBNull(0))
                        monitor = reader.GetInt32(0);
                    reader.Close();
                    return monitor;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return -1;
                }
                finally { connection.Close(); }
            }
        }

        //判断某同学是否为某班的班长
        public bool IsMonitor(int class_id, int classmate_id)
        {
            int monitor = FindMonitor(class_id);
            if (monitor < 0) return false;
            if (monitor == classmate_id) { return true; }
            return false;
        }

        public bool DeleteApplication(int classmate_id, int class_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM 班级申请表 WHERE 班级ID= @class_id AND 同学ID = @classmate_id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    cmd.Parameters.AddWithValue("@classmate_id", classmate_id);
                    int tmp = cmd.ExecuteNonQuery();
                    return tmp > 0;
                    if (tmp > 0)
                        return true; //删除成功
                    return false; //未找到匹配项或删除操作失败
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
                finally { connection.Close(); }
            }
        }

        public Class CreateClass(string class_name, DateTime class_time, int monitor)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    List<int> classmates = new List<int> { monitor }; // 仅包含班长 ID
                    List<int> gatherings = new List<int>(); // 小聚列表为空
                    string query = "INSERT INTO 班级 (班级名称, 班级成立时间, 班长ID, 同学ID列表) VALUES (@class_name, @class_time, @monitor, @classmates)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@class_name", class_name);
                    cmd.Parameters.AddWithValue("@class_time", class_time);
                    cmd.Parameters.AddWithValue("@monitor", monitor);
                    cmd.Parameters.AddWithValue("@classmates", monitor.ToString());
                    cmd.ExecuteNonQuery();
                    int classID = getMax();
                    query = "UPDATE `同学` SET `班级ID列表` = CONCAT(COALESCE(`班级ID列表`, ''), ' ', @classID)  WHERE `同学ID` = @Monitor";
                    cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@classID", classID.ToString());
                    cmd.Parameters.AddWithValue("@Monitor", monitor);
                    cmd.ExecuteNonQuery();
                    Console.Write(query + "\n");
                    Class newClass = new Class(classID, class_name, class_time, monitor, classmates, gatherings);
                    return newClass;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally { connection.Close(); }
            }
        }


        public string GetUserName(int userID)
        {
            string userName = "";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT `姓名` FROM `同学` WHERE `同学ID` = @userID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userID", userID);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    userName = result.ToString();
                }
            }
            return userName;
        }

        public string GetClassName(int classID)
        {
            string className = "";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT `班级名称` FROM `班级` WHERE `班级ID` = @classID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    className = result.ToString();
                }
            }
            return className;
        }


        public List<Apply> FindAllApplys(int monitor)
        {

            List<Apply> applys = new List<Apply>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT `同学ID`, `班级ID` FROM `班级申请表` WHERE `班长ID` = @monitor";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@monitor", monitor);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int userID = reader.GetInt32(0);
                        int classID = reader.GetInt32(1);
                        string userName = GetUserName(userID);
                        string className = GetClassName(classID);
                        applys.Add(new Apply(userID, classID, userName, className));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
            return applys;
        }


        public List<Class> FindAllClasses(int user_id)
        {

            List<Class> classes = new List<Class>();
            List<int> classList = new List<int>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT `班级ID列表` FROM `同学` WHERE `同学ID` = @user_id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string classString = reader.GetString(0);
                        if (!string.IsNullOrEmpty(classString))
                        {
                            string[] classArray = classString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string classIdStr in classArray)
                                if (int.TryParse(classIdStr, out int classId))
                                    classList.Add(classId);
                        }
                    }
                    foreach (int classId in classList)
                        classes.Add(FindClass(classId));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
            return classes;
        }

        public Class FindClass(int class_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM `班级` WHERE `班级ID` = @class_id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    Class newClass = new Class();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            newClass.ClassID = reader.IsDBNull(reader.GetOrdinal("班级ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("班级ID"));
                            newClass.Name = reader.IsDBNull(reader.GetOrdinal("班级名称")) ? "" : reader.GetString(reader.GetOrdinal("班级名称"));
                            newClass.Date = reader.IsDBNull(reader.GetOrdinal("班级成立时间")) ? null : reader.GetDateTime(reader.GetOrdinal("班级成立时间"));
                            newClass.monitor = reader.IsDBNull(reader.GetOrdinal("班长ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("班长ID"));
                            string classmatesString = reader.IsDBNull(reader.GetOrdinal("同学ID列表")) ? "" : reader.GetString(reader.GetOrdinal("同学ID列表"));
                            newClass.classmates = new List<int>();
                            if (!string.IsNullOrEmpty(classmatesString))
                            {
                                newClass.classmates.AddRange(Array.ConvertAll(classmatesString.Split(' '), s =>
                                {
                                    int parsedInt;
                                    if (int.TryParse(s, out parsedInt))
                                    {
                                        return parsedInt;
                                    }
                                    return 0; // Or handle the conversion failure in another way
                                }));
                            }

                            string gatheringsString = reader.IsDBNull(reader.GetOrdinal("小聚ID列表")) ? "" : reader.GetString(reader.GetOrdinal("小聚ID列表"));
                            newClass.gatherings = new List<int>();
                            if (!string.IsNullOrEmpty(gatheringsString))
                            {
                                newClass.gatherings.AddRange(Array.ConvertAll(gatheringsString.Split(' '), s =>
                                {
                                    int parsedInt;
                                    if (int.TryParse(s, out parsedInt))
                                    {
                                        return parsedInt;
                                    }
                                    return 0; // Or handle the conversion failure in another way
                                }));
                            }
                        }
                    }
                    return newClass;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally { connection.Close(); }
            }
        }

        public bool Apply4Class(int class_id, int user_id)
        {
            int monitor = FindMonitor(class_id);
            if (monitor == -1)
                return false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO 班级申请表(同学ID,班级ID,班长ID) VALUES (@user_id,@class_id,@monitor)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    cmd.Parameters.AddWithValue("@monitor", monitor);
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
                finally { connection.Close(); }
            }
        }


        public bool Approval2Class(int classmate_id, int class_id, bool decision)
        {
            bool tmp = DeleteApplication(classmate_id, class_id);
            if (tmp == false)
                return false;
            if (decision == false)
                return true;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE 班级 SET 同学ID列表 = CONCAT(同学ID列表, ' ', @classmate_id) WHERE 班级ID = @class_id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@classmate_id", classmate_id);
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    cmd.ExecuteNonQuery();
                    query = "SELECT 班级ID列表 FROM 同学 WHERE 同学ID = @classmate_id";
                    cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@classmate_id", classmate_id);
                    string classIdList = cmd.ExecuteScalar().ToString();
                    if (string.IsNullOrEmpty(classIdList))
                        classIdList = class_id.ToString(); // 如果班级ID列表为空，直接将新的班级ID赋给同学
                    else
                        classIdList += " " + class_id; // 否则在列表后面添加新的班级ID
                    // 更新同学信息的班级ID列表
                    query = "UPDATE 同学 SET 班级ID列表 = @classIdList WHERE 同学ID = @classmate_id";
                    cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@classIdList", classIdList);
                    cmd.Parameters.AddWithValue("@classmate_id", classmate_id);
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
                finally { connection.Close(); }
            }
        }

        //获取同学列表
        public List<int> GetClassmateList(int class_id)
        {
            List<int> classmatesList = new List<int>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT `同学ID列表` FROM `班级` WHERE `班级ID` = @class_id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string classmatesString = reader.GetString("同学ID列表");
                        string[] classmatesArray = classmatesString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string classmate in classmatesArray)
                        {
                            classmatesList.Add(int.Parse(classmate));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }

            return classmatesList;
        }
    }
}
