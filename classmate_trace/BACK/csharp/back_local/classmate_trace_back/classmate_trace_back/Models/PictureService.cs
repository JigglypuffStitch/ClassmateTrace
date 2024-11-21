using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClassmateTraceBack.Models.Home;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using static ClassmateTraceBack.Models.classes;

namespace ClassmateTraceBack.Models
{


    public class PictureService
    {

        public PictureService()
        {

        }
        //找到当前最大的照片ID
        public int getMax()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT MAX(照片ID) AS 最大照片ID FROM 相册";
                    int maxPictureId = -1;
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read() && !reader.IsDBNull(0))
                        maxPictureId = reader.GetInt32(0);
                    reader.Close();
                    return maxPictureId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("查找照片ID错误" + ex);
                    return -1;
                }
                finally { connection.Close(); }
            }
        }

        //根据照片ID查找照片信息
        public Picture? QueryPictureByID(int pictureId)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    Picture? picture = null;

                    connection.Open();
                    string query = "SELECT 图片URL,班级ID,上传同学ID,小聚ID,时间,地点,描述,点赞数 FROM 相册 WHERE 照片ID=@pictureId";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@pictureId", pictureId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string image = reader.GetString(0);
                        int classId = reader.GetInt32(1);
                        int classmateId = reader.GetInt32(2);
                        int? gatheringId = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                        DateTime date = reader.GetDateTime(4);
                        string? address = reader.IsDBNull(5) ? null : reader.GetString(5);
                        string? description = reader.IsDBNull(6) ? null : reader.GetString(6);
                        int likes = reader.GetInt32(7);

                        if (picture == null)
                        {
                            picture = new(pictureId, image, classId, classmateId, date, likes);
                        }

                        picture.GatheringId = gatheringId;
                        picture.Address = address;
                        picture.Description = description;

                    }
                    reader.Close();
                    connection.Close();

                    return picture;
                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("查找照片信息错误" + ex.Message);
                    return null;
                }
            }
        }

        //添加照片
        public Picture? AddPicture(string image, int classId, int classmateId, DateTime? date, int? gatheringId, string? address, string? description)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    if (date == null) date = DateTime.Now;
                    int likes = 0;

                    //INSERT
                    connection.Open();
                    string query = "INSERT INTO 相册 (图片URL, 班级ID, 上传同学ID, 小聚ID, 时间, 地点, 描述, 点赞数) VALUES (@image, @classId, @classmateId, @gatheringId, @date, @address, @description, @likes)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@image", image);
                    cmd.Parameters.AddWithValue("@classId", classId);
                    cmd.Parameters.AddWithValue("@classmateId", classmateId);
                    cmd.Parameters.AddWithValue("@gatheringId", gatheringId);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@likes", likes);
                    cmd.ExecuteNonQuery();

                    int pictureId = getMax();

                    Picture? picture = QueryPictureByID(pictureId);
                    return picture;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    Console.WriteLine("添加照片错误" + ex.Message);
                    return null;
                }
            }

        }

        //删除照片
        public bool DeletePicture(int classmateId, int pictureId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Picture? picture = QueryPictureByID(pictureId);
                //照片不存在
                if (picture == null)
                {
                    return false;
                }
                //判断普通同学权限
                if (picture.ClassmateId != classmateId)
                {
                    //判断班长权限
                    ClassService classService = new ClassService();
                    if (classService.IsMonitor(picture.ClassId, classmateId) == false) { return false; }
                }

                // 删除照片
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"DELETE FROM `相册` WHERE `照片ID` = @pictureId ";
                cmd.Parameters.Clear(); // 清除参数集合
                cmd.Parameters.AddWithValue("@pictureId", pictureId);


                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    conn.Close();
                    return true;  //删除成功
                }
                else
                {
                    conn.Close();
                    return false;  //删除失败
                }
            }
        }

        //根据班级ID查找相关照片
        public List<Picture> QueryPictureByClass(int classId)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    List<Picture> pictures = new List<Picture>();

                    connection.Open();
                    string query = "SELECT 照片ID,图片URL,上传同学ID,小聚ID,时间,地点,描述,点赞数 FROM 相册 WHERE 班级ID=@classId";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@classId", classId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int pictureId = reader.GetInt32(0);
                        string image = reader.GetString(1);
                        int classmateId = reader.GetInt32(2);
                        int? gatheringId = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                        DateTime date = reader.GetDateTime(4);
                        string? address = reader.IsDBNull(5) ? null : reader.GetString(5);
                        string? description = reader.IsDBNull(6) ? null : reader.GetString(6);
                        int likes = reader.GetInt32(7);

                        Picture picture = new(pictureId, image, classId, classmateId, date, likes);

                        picture.GatheringId = gatheringId;
                        picture.Address = address;
                        picture.Description = description;

                        pictures.Add(picture);
                    }
                    reader.Close();
                    connection.Close();

                    return pictures;

                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("查找班级照片列表错误" + ex.Message);
                    return null;
                }
            }
        }

        //记录照片与同学的关联关系
        public void RecordConnectionPC(int pictureId, int classmateId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO 同学相册关联表(照片ID,同学ID) VALUES (@pictutreId,@classmateId)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@pictutreId", pictureId);
                    cmd.Parameters.AddWithValue("@classmateId", classmateId);
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                }
                finally { connection.Close(); }
            }
        }


        //删除照片与同学的关联关系
        public bool DeleteConnectionPC(int pictureId, int classmateId)
        {


            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();

                // 检查照片与同学的关联关系是否存在
                cmd.CommandText = $"SELECT * FROM `同学相册关联表` WHERE `照片ID` = @pictureId AND `同学ID` = @classmateId";
                cmd.Parameters.AddWithValue("@pictureId", pictureId);
                cmd.Parameters.AddWithValue("@classmateId", classmateId);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close(); // 关闭数据读取器
                    //MaterialMessageBox.Show("用户ID不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                reader.Close(); // 关闭数据读取器

                // 删除照片与同学的关联关系
                cmd.CommandText = $"DELETE FROM `同学相册关联表` WHERE `照片ID` = @pictureId AND `同学ID` = @classmateId";
                cmd.Parameters.Clear(); // 清除参数集合
                cmd.Parameters.AddWithValue("@pictureId", pictureId);
                cmd.Parameters.AddWithValue("@classmateId", classmateId);
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    //MaterialMessageBox.Show("删除用户成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    //MaterialMessageBox.Show("删除用户失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                conn.Close();
                return true;
            }
        }

        //根据照片ID查找相关同学ID列表
        public int[]? QueryClassmateIDByPictureID(int pictureId)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    List<int> cmIDs = new List<int>();

                    connection.Open();
                    string query = "SELECT 同学ID FROM 同学相册关联表 WHERE 照片ID=@pictureId";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@pictureId", pictureId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int calssmateId = reader.GetInt32(0);

                        cmIDs.Add(calssmateId);
                    }
                    reader.Close();
                    connection.Close();
                    if (cmIDs.Count > 0) { return cmIDs.ToArray(); }

                    return null;

                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("查找相关同学ID列表错误" + ex.Message);
                    return null;
                }
            }
        }

        //根据照片ID和同学ID查找关联情况
        public bool QueryCPConnection(int userId, int pictureId)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM 同学相册关联表 WHERE 同学ID=@userId AND 照片ID=@pictureId";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@pictureId", pictureId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    int num = 0;
                    while (reader.Read())
                    {
                        num++;
                    }
                    reader.Close();
                    connection.Close();
                    if (num > 0) { return true; }

                    return false;

                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("查找同学照片关联情况错误" + ex.Message);
                    return false;
                }
            }
        }

        //根据同学ID查找相关照片ID列表
        public int[]? QueryPictureIDByClassmateID(int classmateId)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    List<int> pIDs = new List<int>();

                    connection.Open();
                    string query = "SELECT 照片ID FROM 同学相册关联表 WHERE 同学ID=@classmateId";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@classmateId", classmateId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int pictureId = reader.GetInt32(0);

                        pIDs.Add(pictureId);
                    }
                    reader.Close();
                    connection.Close();
                    if (pIDs.Count > 0) { return pIDs.ToArray(); }

                    return null;

                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("查找相关照片ID列表错误" + ex.Message);
                    return null;
                }
            }
        }

        //照片点赞数加1
        public Picture? UpdatePictureLikes(int pictureId)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    int likes;
                    Picture? picture = QueryPictureByID(pictureId);
                    if (picture != null)
                    {
                        likes = picture.Likes + 1;
                        //UPDATE
                        connection.Open();
                        string query = "UPDATE 相册 SET 点赞数=@likes WHERE 照片ID=@pictureId";
                        MySqlCommand cmd = new(query, connection);
                        cmd.Parameters.AddWithValue("@pictureId", pictureId);
                        cmd.Parameters.AddWithValue("@likes", likes);
                        cmd.ExecuteNonQuery();
                        connection.Close();
                        picture = QueryPictureByID(pictureId);
                    }

                    return picture;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    Console.WriteLine("给照片点赞错误" + ex.Message);
                    return null;
                }
            }
        }


    }
}
