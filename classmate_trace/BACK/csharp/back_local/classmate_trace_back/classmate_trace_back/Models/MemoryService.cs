using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static ClassmateTraceBack.Models.classes;


namespace ClassmateTraceBack.Models
{
    public class MemoryService
    {
        public MemoryService() { }


        //根据回忆ID查找回忆录信息
        public Memory? QueryMemoryById(int memoryId)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    Memory? memory = null;
                    connection.Open();
                    string query = "SELECT 回忆主题,音乐URL,照片ID列表,班级ID FROM 回忆 WHERE 回忆ID=@memoryId";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@memoryId", memoryId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int? theme = reader.IsDBNull(0) ? null : reader.GetInt32(0);
                        string? music = reader.IsDBNull(1) ? null : reader.GetString(1);
                        string? pictureIDs = reader.IsDBNull(2) ? null : reader.GetString(2);
                        int classId = reader.GetInt32(3);

                        if (memory == null)
                        {
                            memory = new(memoryId, classId);
                        }

                        memory.Theme = theme;
                        memory.Music = music;
                        if (pictureIDs != null)
                        {
                            string[] picture_list = pictureIDs.Split(' ');
                            if (picture_list != null)
                            {
                                for (int i = 0; i < picture_list.Length; i++)
                                {
                                    int pictureId = Convert.ToInt32(picture_list[i]);
                                    memory.Pictures.Add(pictureId);
                                }
                            }
                        }

                    }
                    reader.Close();
                    connection.Close();

                    return memory;

                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("查找回忆录信息错误" + ex.Message);
                    return null;
                }
            }
        }

        //根据班级ID查找相关回忆录
        public List<Memory> QueryMemoryByClass(int classId)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                List<Memory> memories = new List<Memory>();
                try
                {
                    connection.Open();
                    string query = "SELECT 回忆ID,回忆主题,音乐URL,照片ID列表 FROM 回忆 WHERE 班级ID=@classId";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@classId", classId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int memoryId = reader.GetInt32(0);
                        int? theme = reader.IsDBNull(1) ? null : reader.GetInt32(1);
                        string? music = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? pictureIDs = reader.IsDBNull(3) ? null : reader.GetString(3);

                        Memory memory = new(memoryId, classId);

                        memory.Theme = theme;
                        memory.Music = music;
                        if (pictureIDs != null)
                        {
                            string[] picture_list = pictureIDs.Split(' ');
                            if (picture_list != null)
                            {
                                for (int i = 0; i < picture_list.Length; i++)
                                {
                                    int pictureId = Convert.ToInt32(picture_list[i]);
                                    memory.Pictures.Add(pictureId);
                                }
                            }
                        }

                        memories.Add(memory);
                    }
                    reader.Close();
                    connection.Close();

                    return memories;

                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("查找班级回忆录列表错误" + ex.Message);
                    return memories;
                }
            }
        }


        //找到当前最大的回忆ID
        public int getMax()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT MAX(回忆ID) AS 最大回忆ID FROM 回忆";
                    int maxMemoryId = -1;
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read() && !reader.IsDBNull(0))
                        maxMemoryId = reader.GetInt32(0);
                    reader.Close();
                    return maxMemoryId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("查找回忆ID错误" + ex);
                    return -1;
                }
                finally { connection.Close(); }
            }
        }

        //添加回忆
        public Memory? AddMemory(int classId, int? theme, string? music, string? pictures)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    //INSERT
                    connection.Open();
                    string query = "INSERT INTO 回忆 (回忆主题, 音乐URL, 照片ID列表, 班级ID) VALUES (@theme, @music, @pictures, @classId)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@theme", theme);
                    cmd.Parameters.AddWithValue("@music", music);
                    cmd.Parameters.AddWithValue("@pictures", pictures);
                    cmd.Parameters.AddWithValue("@classId", classId);
                    cmd.ExecuteNonQuery();

                    int memoryId = getMax();

                    Memory? memory = QueryMemoryById(memoryId);
                    return memory;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    Console.WriteLine("添加回忆错误" + ex.Message);
                    return null;
                }
            }
        }


    }


}
