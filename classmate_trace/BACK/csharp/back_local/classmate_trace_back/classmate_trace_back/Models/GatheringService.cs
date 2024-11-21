using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static ClassmateTraceBack.Models.classes;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ClassmateTraceBack.Models
{
    public class GatheringService
    {
        //获取当前班级ID的小聚厅列表
        public List<object> GetGatheringList(int class_id)
        {
            List<object> gatherings = new List<object>();
            List<int> gatheringIds = new List<int>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // 设置命令超时时间
                    using var cmd = new MySqlCommand("SELECT `小聚ID列表` FROM `班级` WHERE `班级ID` = @class_id", conn)
                    {
                        CommandTimeout = 30 // 设置30秒超时
                    };

                    cmd.Parameters.AddWithValue("@class_id", class_id);

                    using var reader = cmd.ExecuteReader();
                    if (reader.Read() && !reader.IsDBNull(reader.GetOrdinal("小聚ID列表")))
                    {
                        string gatheringsString = reader.GetString("小聚ID列表");
                        if (!string.IsNullOrEmpty(gatheringsString))
                        {
                            gatheringIds = gatheringsString
                                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => int.TryParse(s, out int id) ? id : -1)
                                .Where(id => id != -1)
                                .ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching gathering list: {ex.Message}");
                    return gatherings;
                }
            }

            // 使用单个连接获取所有小聚信息
            if (gatheringIds.Any())
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string idList = string.Join(",", gatheringIds);
                        string query = @"
                            SELECT `小聚ID`, `班级ID`, `发起同学ID`, `时间`, `地点`, `描述`, `加入同学ID列表`
                            FROM `小聚厅`
                            WHERE `小聚ID` IN (" + idList + ")";

                        using var cmd = new MySqlCommand(query, conn) { CommandTimeout = 30 };
                        using var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            try
                            {
                                var gathering = new Gathering(
                                    reader.GetInt32("小聚ID"),
                                    reader.GetInt32("班级ID"),
                                    reader.GetInt32("发起同学ID"),
                                    reader.GetDateTime("时间"),
                                    reader.IsDBNull(reader.GetOrdinal("地点")) ? string.Empty : reader.GetString("地点"),
                                    reader.IsDBNull(reader.GetOrdinal("描述")) ? string.Empty : reader.GetString("描述"),
                                    ParseJoinerIds(reader.IsDBNull(reader.GetOrdinal("加入同学ID列表")) ?
                                        string.Empty :
                                        reader.GetString("加入同学ID列表"))
                                );
                                gatherings.Add(gathering);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing gathering: {ex.Message}");
                                // 继续处理下一条记录
                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching gathering details: {ex.Message}");
                    }
                }
            }

            return gatherings;
        }

        private List<int> ParseJoinerIds(string joinerIdsString)
        {
            if (string.IsNullOrEmpty(joinerIdsString))
                return new List<int>();

            return joinerIdsString
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out int id) ? id : -1)
                .Where(id => id != -1)
                .ToList();
        }

        //发起小聚
        public CreateGathering GatheringCreate(int classid, DateTime datetime, int ownerid, string address, string description)
        {
            int newGatheringId = 0;
            List<int> classmatelist = new List<int>();
            classmatelist.Add(ownerid);//初始化只包含创建者自身
            //初始化，未找到小聚返回值
            Gathering noid_gathering = new Gathering(0, 0, 0, datetime, "address", "description", []);
            CreateGathering noid_createGathering = new CreateGathering(1, noid_gathering);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // 获取最大的聚会ID
                    string getMaxGatheringIdQuery = "SELECT MAX(`小聚ID`) AS `最大小聚ID` FROM `小聚厅`";
                    MySqlCommand getMaxCmd = new MySqlCommand(getMaxGatheringIdQuery, conn);

                    // 执行查询并获取结果
                    object maxGatheringIdObject = getMaxCmd.ExecuteScalar();

                    // 检查结果是否为DBNull，如果是，则设置newGatheringId为1，否则，将其设置为最大小聚ID加1
                    newGatheringId = maxGatheringIdObject == DBNull.Value ? 1 : Convert.ToInt32(maxGatheringIdObject) + 1;


                    string classmateIdsString = string.Join(" ", classmatelist); // 将列表转换为用空格分隔的字符串
                    Console.WriteLine(classmateIdsString);

                    string updateQuery = @"
                UPDATE `班级`
                SET `小聚ID列表` = CONCAT(COALESCE(`小聚ID列表`, ''), ' ', @gatheringid)
                WHERE `班级ID` = @classid";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@gatheringid", newGatheringId);
                    updateCmd.Parameters.AddWithValue("@classid", classid);
                    updateCmd.ExecuteNonQuery();


                    // 构建插入SQL语句
                    string query = @"
                INSERT INTO `小聚厅` (`小聚ID`, `班级ID`, `发起同学ID`, `时间`, `地点`, `描述`, `加入同学ID列表`)
                VALUES (@gatheringid, @classid, @ownerid, @datetime, @address, @description, @classmatelist)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@gatheringid", newGatheringId);
                    cmd.Parameters.AddWithValue("@classid", classid);
                    cmd.Parameters.AddWithValue("@ownerid", ownerid);
                    cmd.Parameters.AddWithValue("@datetime", datetime);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@classmatelist", classmateIdsString);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Gathering gathering = new Gathering(newGatheringId, classid, ownerid, datetime, address, description, classmatelist);
                        int code = 0;//插入成功
                        CreateGathering createGathering = new CreateGathering(code, gathering);
                        // 创建SQL命令
                        query = "INSERT INTO 同学小聚关联表 (同学ID, 小聚ID) VALUES (@OwnerId, @NewGatheringId)";
                        cmd = new MySqlCommand(query, conn);
                        // 添加参数
                        cmd.Parameters.AddWithValue("@OwnerId", ownerid);
                        cmd.Parameters.AddWithValue("@NewGatheringId", newGatheringId);
                        cmd.ExecuteNonQuery();
                        //object gatheringObject = createGathering;
                        return createGathering;
                    }
                    else
                    {
                        return noid_createGathering;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                    return noid_createGathering;
                }
                finally
                {
                    conn.Close();
                }

            }
        }

        //删除小聚
        public bool DeleteGathering(int gatheringid)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // 构建删除SQL语句
                    string deleteQuery = "DELETE FROM `小聚厅` WHERE `小聚ID` = @gatheringid";
                    MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@gatheringid", gatheringid);

                    // 执行删除操作
                    int rowsAffected = deleteCmd.ExecuteNonQuery();

                    // 检查是否有行被删除
                    if (rowsAffected > 0)
                    {
                        deleteQuery = "DELETE FROM `同学小聚关联表` WHERE `小聚ID` = @gatheringid";
                        deleteCmd = new MySqlCommand(deleteQuery, conn);
                        deleteCmd.Parameters.AddWithValue("@gatheringid", gatheringid);
                        deleteCmd.ExecuteNonQuery();

                        return true; // 删除成功
                    }
                    else
                    {
                        return false; // 未找到要删除的记录或删除失败
                    }
                }
                catch (Exception ex)
                {
                    // 异常处理，例如记录日志或抛出异常
                    Console.WriteLine("Exception: " + ex.ToString());
                    return false; // 删除失败
                }
                finally
                {
                    // 确保连接被关闭
                    conn.Close();
                }
            }
        }


        //加入小聚
        public JoiningGathering JoinGathering(int studentid, int gatheringid)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DateTime current = DateTime.Now;
                //初始化，未找到小聚返回值
                Gathering noid_gathering = new Gathering(0, 0, 0, current, "address", "description", []);
                JoiningGathering noid_joinGathering = new JoiningGathering(1, noid_gathering);
                bool isJoined = false;
                // 将字符串转换为整数数组
                List<int> joinerIds = new List<int>();
                int classId = 0;
                int ownerId = 0;
                DateTime datetime = current;
                string address = "address";
                string description = "description";
                string joinerIdsString = "";

                try
                {
                    conn.Open();
                    // 查询所需的属性
                    string selectQuery = @"
                SELECT `班级ID`, `发起同学ID`, `时间`, `地点`, `描述`, `加入同学ID列表`
                FROM `小聚厅`
                WHERE `小聚ID` = @gatheringid";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@gatheringid", gatheringid);
                    MySqlDataReader reader = selectCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // 获取属性值
                        classId = reader.GetInt32("班级ID");
                        ownerId = reader.GetInt32("发起同学ID");
                        DateTime time = reader.GetDateTime("时间");
                        address = reader.GetString("地点");
                        description = reader.GetString("描述");
                        joinerIdsString = reader.GetString("加入同学ID列表");

                        string[] JoinerArray = joinerIdsString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string Joiner in JoinerArray)
                        {
                            joinerIds.Add(int.Parse(Joiner));
                        }
                        // 检查是否已包含studentid
                        isJoined = joinerIds.Contains(studentid);

                    }
                    else
                    {
                        // 未找到对应的小聚
                        return noid_joinGathering;
                    }
                }
                catch (Exception ex)
                {
                    // 异常处理
                    Console.WriteLine("Exception: " + ex.ToString());
                    // 未找到对应的小聚
                    return noid_joinGathering;
                }
                finally
                {
                    conn.Close();
                }

                try
                {
                    conn.Open();
                    if (!isJoined)
                    {
                        joinerIds.Add(studentid);
                        // 添加studentid到列表中
                        string newJoinerIdsString = joinerIdsString + (joinerIdsString.Length > 0 ? " " : "") + studentid;
                        // 更新数据库中的“加入同学ID列表”
                        string updateQuery = @"
                        UPDATE `小聚厅`
                        SET `加入同学ID列表` = @newJoinerIdsString
                        WHERE `小聚ID` = @gatheringid";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@newJoinerIdsString", newJoinerIdsString);
                        updateCmd.Parameters.AddWithValue("@gatheringid", gatheringid);

                        Gathering gathering = new Gathering(gatheringid, classId, ownerId, datetime, address, description, joinerIds);

                        JoiningGathering joinGathering0 = new JoiningGathering(0, gathering);
                        JoiningGathering joinGathering1 = new JoiningGathering(1, gathering);
                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            //成功code=0
                            return joinGathering0;
                        }
                        else
                        {
                            return joinGathering1;
                        }
                    }
                    else
                    {
                        //同学已经在列表中
                        Gathering gathering = new Gathering(gatheringid, classId, ownerId, datetime, address, description, joinerIds);
                        JoiningGathering joinGathering = new JoiningGathering(1, gathering);
                        return joinGathering;

                    }
                }
                catch (Exception ex)
                {
                    // 异常处理
                    Console.WriteLine("Exception: " + ex.ToString());
                    // 未找到对应的小聚
                    return noid_joinGathering;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        //退出小聚
        public ExitingGathering ExitGathering(int gathering_id, int classmate_id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                int classId = 0;
                int ownerId = 0;
                DateTime time = DateTime.Now;
                string address = "address";
                string description = "description";
                string joinerIdsString = "";

                //初始化，未找小聚返回值
                Gathering noid_gathering = new Gathering(0, 0, 0, time, "address", "description", []);
                ExitingGathering noid_joinGathering = new ExitingGathering(1, noid_gathering);
                List<int> joinerIds = new List<int>();

                try
                {
                    conn.Open();

                    // 查询“加入同学ID列表”
                    string selectQuery = @"
                SELECT `班级ID`, `发起同学ID`, `时间`, `地点`, `描述`, `加入同学ID列表`
                FROM `小聚厅`
                WHERE `小聚ID` = @gathering_id";

                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@gathering_id", gathering_id);
                    MySqlDataReader reader = selectCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // 获取属性值
                        classId = reader.GetInt32("班级ID");
                        ownerId = reader.GetInt32("发起同学ID");
                        time = reader.GetDateTime("时间");
                        address = reader.GetString("地点");
                        description = reader.GetString("描述");

                        joinerIdsString = reader.GetString("加入同学ID列表");

                        // 将字符串转换为整数数组
                        string[] joinerArray = joinerIdsString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string joiner in joinerArray)
                        {
                            joinerIds.Add(int.Parse(joiner));
                        }

                    }
                    else
                    {
                        // 未找到对应的小聚
                        return new ExitingGathering(1, noid_gathering);
                    }
                }
                catch (Exception ex)
                {
                    // 异常处理
                    Console.WriteLine("Exception: " + ex.ToString());
                    // 返回错误信息
                    return new ExitingGathering(1, noid_gathering);
                }
                finally
                {
                    conn.Close();
                }
                try
                {
                    conn.Open();
                    // 检查是否已包含classmate_id
                    if (joinerIds.Contains(classmate_id))
                    {
                        // 如果已包含，则移除classmate_id
                        joinerIds.Remove(classmate_id);
                        // 更新数据库中的“加入同学ID列表”
                        string newJoinerIdsString = string.Join(" ", joinerIds);
                        string updateQuery = @"
                            UPDATE `小聚厅`
                            SET `加入同学ID列表` = @newJoinerIdsString
                            WHERE `小聚ID` = @gathering_id";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@newJoinerIdsString", newJoinerIdsString);
                        updateCmd.Parameters.AddWithValue("@gathering_id", gathering_id);

                        // 执行更新操作并检查结果
                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Gathering gathering = new Gathering(gathering_id, classId, ownerId, time, address, description, joinerIds);
                            // 成功code=0
                            return new ExitingGathering(0, gathering);
                        }
                        else
                        {
                            Gathering gathering = new Gathering(gathering_id, classId, ownerId, time, address, description, joinerIds);
                            // 成功code=0
                            return new ExitingGathering(1, gathering);
                        }
                    }
                    else
                    {
                        Gathering gathering = new Gathering(gathering_id, classId, ownerId, time, address, description, joinerIds);
                        // 同学已经不在列表中
                        return new ExitingGathering(1, gathering);
                    }
                }
                catch (Exception ex)
                {
                    // 异常处理
                    Console.WriteLine("Exception: " + ex.ToString());
                    // 返回错误信息
                    return new ExitingGathering(1, noid_gathering);
                }
                finally
                {
                    // 确保连接被关闭
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }



        //上传照片
        public UpdatePicture updatePicture(int classmateid, int classid, string pictureurl, string address, string description, int gatheringid)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DateTime dateTime = DateTime.Now;
                try
                {
                    conn.Open();
                    // 插入数据的SQL命令
                    string insertQuery = @"
                INSERT INTO `相册` (`图片URL`, `班级ID`, `上传同学ID`, `小聚ID`, `时间`, `地点`, `描述`, `点赞数`)
                VALUES (@pictureurl, @classid, @classmateid, @gatheringid, @date, @address, @description, @like)";

                    // 创建MySqlCommand对象
                    MySqlCommand cmd = new MySqlCommand(insertQuery, conn);

                    // 添加参数
                    cmd.Parameters.AddWithValue("@pictureurl", pictureurl);
                    cmd.Parameters.AddWithValue("@classid", classid);
                    cmd.Parameters.AddWithValue("@classmateid", classmateid);
                    cmd.Parameters.AddWithValue("@gatheringid", gatheringid);
                    cmd.Parameters.AddWithValue("@date", dateTime);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@like", 0);


                    // 执行插入操作
                    int rowsAffected = cmd.ExecuteNonQuery();
                    // 获取自增的照片ID
                    int newPhotoId = (int)cmd.LastInsertedId;

                    Picture picture = new Picture(newPhotoId, pictureurl, classid, classmateid, dateTime, gatheringid, address, description, 0);

                    // 根据影响的行数返回结果
                    if (rowsAffected > 0)
                    {
                        // 如果插入成功，返回成功的结果
                        return new UpdatePicture(0, picture);
                    }
                    else
                    {
                        // 如果没有行受影响，返回失败的结果
                        return new UpdatePicture(1, picture);
                    }
                }
                catch (Exception ex)
                {
                    // 异常处理
                    Console.WriteLine("Exception: " + ex.ToString());
                    // 返回异常信息
                    Picture picture = new Picture(0, pictureurl, classid, classmateid, dateTime, gatheringid, address, description, 0);

                    return new UpdatePicture(1, picture);
                }
                finally
                {
                    // 确保连接被关闭
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }

        //加载照片
        public List<Picture> DisplayPicture(int gathering_id)
        {
            List<Picture> pictures = new List<Picture>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // 查询所有属性“小聚ID”为gathering_id的数据
                    string selectQuery = @"
                SELECT `照片ID`, `图片URL`, `班级ID`, `上传同学ID`, `小聚ID`, `时间`, `地点`, `描述`, `点赞数`
                FROM `相册`
                WHERE `小聚ID` = @gathering_id";

                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@gathering_id", gathering_id);

                    // 执行查询
                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // 获取属性值
                            int photoId = reader.GetInt32("照片ID");
                            string pictureUrl = reader.GetString("图片URL");
                            int classId = reader.GetInt32("班级ID");
                            int uploadClassmateId = reader.GetInt32("上传同学ID");
                            int gatheringId = reader.GetInt32("小聚ID");
                            DateTime time = reader.GetDateTime("时间");
                            string? address = reader.IsDBNull("地点") ? null : reader.GetString("地点");
                            string? description = reader.IsDBNull("描述") ? null : reader.GetString("描述");
                            int likes = reader.GetInt32("点赞数");

                            Console.WriteLine(gatheringId);
                            Console.WriteLine(pictureUrl);
                            // 创建Picture对象并添加到列表中
                            Picture picture = new Picture(photoId, pictureUrl, classId, uploadClassmateId, time, gatheringId, address, description, likes);
                            pictures.Add(picture);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 异常处理
                    Console.WriteLine("Exception: " + ex.ToString());
                    // 返回空列表或处理异常
                }
                finally
                {
                    // 确保连接被关闭
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            return pictures;
        }


        //修改小聚
        public UpdateGathering ChangeGathering(int gatheringid, int classmate_id, string Datetime, string address, string description)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string format = "yyyy-MM-dd HH:mm:ss"; // 日期时间格式
                DateTime datetime = DateTime.ParseExact(Datetime, format, CultureInfo.InvariantCulture);
                //初始化，未找到小聚返回值
                Gathering noid_gathering = new Gathering(0, 0, 0, datetime, "address", "description", []);
                UpdateGathering noid_joinGathering = new UpdateGathering(1, noid_gathering);
                int classId = 0;
                int ownerId = 0;
                string addr = "";
                string desc = "";
                List<int> joinerIds = [];

                try
                {
                    conn.Open();
                    // 查询所需的属性
                    string selectQuery = @"
                SELECT `班级ID`, `发起同学ID`, `时间`, `地点`, `描述`, `加入同学ID列表`
                FROM `小聚厅`
                WHERE `小聚ID` = @gatheringid";

                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@gatheringid", gatheringid);
                    MySqlDataReader reader = selectCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // 获取属性值
                        classId = reader.GetInt32("班级ID");
                        ownerId = reader.GetInt32("发起同学ID");
                        DateTime dateTime = reader.GetDateTime("时间");
                        addr = reader.GetString("地点");
                        desc = reader.GetString("描述");
                        string joinerIdsString = reader.GetString("加入同学ID列表");

                        // 将字符串转换为整数列表
                        joinerIds = joinerIdsString.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries) // 假设ID之间由空格或逗号分隔
                            .Select(idString => int.Parse(idString)) // 将每个分割后的字符串转换为整数
                            .ToList(); // 将结果转换为List<int>
                    }
                    else
                    {
                        // 未找到对应的小聚
                        return noid_joinGathering;
                    }
                }
                catch (Exception ex)
                {
                    // 异常处理
                    Console.WriteLine("Exception: " + ex.ToString());
                    // 返回异常信息
                    return noid_joinGathering;
                }
                finally
                {
                    // 确保连接被关闭
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                try
                {
                    conn.Open();
                    // 核对发起同学ID
                    if (ownerId == classmate_id)
                    {
                        // 更新数据
                        string updateQuery = @"
                            UPDATE `小聚厅`
                            SET `时间` = @datetime, `地点` = @address, `描述` = @description
                            WHERE `小聚ID` = @gatheringid";

                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@datetime", datetime);
                        updateCmd.Parameters.AddWithValue("@address", address);
                        updateCmd.Parameters.AddWithValue("@description", description);
                        updateCmd.Parameters.AddWithValue("@gatheringid", gatheringid);

                        // 执行更新操作
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        Gathering gathering = new Gathering(gatheringid, classId, ownerId, datetime, address, description, joinerIds);
                        if (rowsAffected > 0)
                        {
                            // 更新成功
                            return new UpdateGathering(0, gathering);
                        }
                        else
                        {
                            // 没有行受影响，可能是因为没有找到匹配的小聚
                            return new UpdateGathering(1, gathering);
                        }
                    }
                    else
                    {
                        // 发起同学ID不匹配
                        return noid_joinGathering;
                    }
                }
                catch (Exception ex)
                {
                    // 异常处理
                    Console.WriteLine("Exception: " + ex.ToString());
                    // 返回异常信息
                    return noid_joinGathering;
                }
                finally
                {
                    // 确保连接被关闭
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }

    }
}
