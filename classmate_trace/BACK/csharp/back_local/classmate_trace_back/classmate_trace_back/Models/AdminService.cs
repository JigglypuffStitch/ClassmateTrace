using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using static ClassmateTraceBack.Models.classes;


namespace ClassmateTraceBack.Models
{
    public class AdminService
    {
        //用户数量统计更新
        public bool CheckUsersInfo()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // 获取当前日期
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                // 检索“同学”表中的性别属性，并计算数量
                string genderQuery = "SELECT `性别`, COUNT(*) AS Count FROM `同学` WHERE `最后登录时间` = @date GROUP BY `性别`";
                MySqlCommand genderCmd = new MySqlCommand(genderQuery, conn);
                genderCmd.Parameters.AddWithValue("@date", currentDate);
                MySqlDataReader genderReader = genderCmd.ExecuteReader();

                int femaleCount = 0;
                int maleCount = 0;
                int otherCount = 0;
                int unknownCount = 0;

                while (genderReader.Read())
                {
                    int gender = genderReader.GetInt32("性别");
                    int count = genderReader.GetInt32("Count");
                    switch (gender)
                    {
                        case 0:
                            maleCount = count;
                            break;
                        case 1:
                            femaleCount = count;
                            break;
                        case 2:
                            otherCount = count;
                            break;
                        case 3:
                            unknownCount = count;
                            break;
                    }
                }
                genderReader.Close();

                // 检索“同学”表中的“最后登录时间”属性，并计算当天登录用户数量
                string loginQuery = "SELECT COUNT(*) AS LoginCount FROM `同学` WHERE `最后登录时间` = @date";
                MySqlCommand loginCmd = new MySqlCommand(loginQuery, conn);
                loginCmd.Parameters.AddWithValue("@date", currentDate);
                int loginCount = Convert.ToInt32(loginCmd.ExecuteScalar());

                //更新（插入）新数据
                // 检查“用户统计量”表中是否已存在当前日期
                string checkDateQuery = "SELECT COUNT(*) FROM `用户统计量` WHERE `日期` = @date";
                MySqlCommand checkDateCmd = new MySqlCommand(checkDateQuery, conn);
                checkDateCmd.Parameters.AddWithValue("@date", currentDate);
                int dateCount = Convert.ToInt32(checkDateCmd.ExecuteScalar());

                if (dateCount > 0)
                {
                    // 更新操作
                    string updateStatsQuery = "UPDATE `用户统计量` SET `女性数量` = @femaleCount, `男性数量` = @maleCount, `其他数量` = @otherCount, `未知数量` = @unknownCount, `当天用户登录数量` = @loginCount WHERE `日期` = @date";
                    MySqlCommand updateStatsCmd = new MySqlCommand(updateStatsQuery, conn);
                    updateStatsCmd.Parameters.AddWithValue("@femaleCount", femaleCount);
                    updateStatsCmd.Parameters.AddWithValue("@maleCount", maleCount);
                    updateStatsCmd.Parameters.AddWithValue("@unknownCount", unknownCount);
                    updateStatsCmd.Parameters.AddWithValue("@otherCount", otherCount);
                    updateStatsCmd.Parameters.AddWithValue("@loginCount", loginCount);
                    updateStatsCmd.Parameters.AddWithValue("@date", currentDate);
                    updateStatsCmd.ExecuteNonQuery();
                }
                else
                {
                    // 插入操作
                    string insertStatsQuery = "INSERT INTO `用户统计量` (日期, 女性数量, 男性数量, 其他数量, 未知数量, 当天用户登录数量) VALUES (@date, @femaleCount, @maleCount, @otherCount, @unknownCount, @loginCount)";
                    MySqlCommand insertStatsCmd = new MySqlCommand(insertStatsQuery, conn);
                    insertStatsCmd.Parameters.AddWithValue("@date", currentDate);
                    insertStatsCmd.Parameters.AddWithValue("@femaleCount", femaleCount);
                    insertStatsCmd.Parameters.AddWithValue("@maleCount", maleCount);
                    insertStatsCmd.Parameters.AddWithValue("@unknownCount", unknownCount);
                    insertStatsCmd.Parameters.AddWithValue("@otherCount", otherCount);
                    insertStatsCmd.Parameters.AddWithValue("@loginCount", loginCount);
                    insertStatsCmd.ExecuteNonQuery();
                }
                conn.Close();
                return true;
            }
        }

        public List<UserInfo> DisplayUserInfo()
        {
            List<UserInfo> weeklyStats = new List<UserInfo>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // 获取当前日期
                    DateTime currentDate = DateTime.Now.Date;
                    // 获取当前日期向前推7天的日期
                    DateTime startDate = currentDate.AddDays(-7);

                    // 准备SQL查询语句，根据需要查询从startDate到currentDate的数据
                    string query = "SELECT * FROM 用户统计量 WHERE 日期 >= @startDate AND 日期 <= @endDate ORDER BY 日期 DESC LIMIT 7";

                    // 创建 MySqlCommand 对象
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    // 添加参数
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", currentDate);

                    // 执行查询
                    MySqlDataReader reader = cmd.ExecuteReader();

                    // 读取查询结果
                    while (reader.Read())
                    {
                        UserInfo stats = new UserInfo
                        {
                            Date = reader.GetDateTime("日期"), // 使用 GetDateTime 获取日期类型
                            MaleNum = reader.GetInt32("男性数量"),
                            FemaleNum = reader.GetInt32("女性数量"),
                            OtherNum = reader.GetInt32("其他数量"),
                            UnknownNum = reader.GetInt32("未知数量"),
                            LoginUserNum = reader.GetInt32("当天用户登录数量")
                            // 添加其他字段的映射
                        };
                        weeklyStats.Add(stats);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return weeklyStats;
        }

    }
}
