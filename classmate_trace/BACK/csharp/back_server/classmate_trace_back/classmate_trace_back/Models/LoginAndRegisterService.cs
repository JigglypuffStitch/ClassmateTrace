using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Reflection;
using System.Threading;
using static ClassmateTraceBack.Models.classes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ClassmateTraceBack.Models
{
    public class LoginAndRegisterService
    {
        //用户注册
        public Classmate UserRegister(string username, string password, string phonenumber, int? gender = 0, string? mail = "", string? sign = "", string? headpicture = "", DateTime? birthday = null)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                int success = 0;
                try
                {
                    conn.Open();

                    // 获取最大的同学ID
                    string getMaxClassmateIdQuery = "SELECT MAX(`同学ID`) AS `最大同学ID` FROM `同学`";
                    MySqlCommand getMaxCmd = new MySqlCommand(getMaxClassmateIdQuery, conn);
                    object maxClassmateIdObject = getMaxCmd.ExecuteScalar();
                    int maxClassmateId = maxClassmateIdObject == DBNull.Value ? 0 : Convert.ToInt32(maxClassmateIdObject);
                    int newClassmateId = maxClassmateId + 1;

                    List<int> classesid = null; // 班级ID为空

                    // 首先，检查电话号码是否已经存在
                    string checkPhoneQuery = "SELECT COUNT(1) FROM `同学` WHERE `电话` = @phonenumber";
                    MySqlCommand checkCmd = new MySqlCommand(checkPhoneQuery, conn);
                    checkCmd.Parameters.AddWithValue("@phonenumber", phonenumber);

                    // 执行查询
                    long phoneCount = (Int64)checkCmd.ExecuteScalar();

                    if (phoneCount == 0)
                    {
                        // 构建插入SQL语句
                        string query = @"
                INSERT INTO `同学` (`同学ID`, `电话`, `密码`, `姓名`, `生日`, `邮箱`, `签名`, `性别`, `头像`, `班级ID列表`, `最后登录时间`)
                VALUES (@classmateId, @phonenumber, @password, @username, @birthday, @mail, @sign, @gender, @headpicture, @classesId, NOW())";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@classmateId", newClassmateId);
                        cmd.Parameters.AddWithValue("@phonenumber", phonenumber);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@classesId", classesid);
                        if (birthday.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@birthday", birthday.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@birthday", DBNull.Value);
                        }
                        if (!string.IsNullOrEmpty(mail))
                        {
                            cmd.Parameters.AddWithValue("@mail", mail);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@mail", DBNull.Value);
                        }
                        if (!string.IsNullOrEmpty(sign))
                        {
                            cmd.Parameters.AddWithValue("@sign", sign);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@sign", DBNull.Value);
                        }
                        if (!string.IsNullOrEmpty(headpicture))
                        {
                            cmd.Parameters.AddWithValue("@headpicture", headpicture);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@headpicture", DBNull.Value);
                        }
                        cmd.Parameters.AddWithValue("@gender", gender);
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            success = 1;
                            return new Classmate(newClassmateId, username, password, phonenumber, gender, mail, sign, headpicture, birthday);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    // 检查结果是否为 null
                    else
                    {
                        return null;
                        //throw new Exception("电话号码已存在，无法插入新记录。");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                    return null;
                }
                finally
                {
                    conn.Close();
                }
            }
        }


        //用户登录
        public LoginStatus UserLogin(string? phonenumber = "", string? password = "")
        {
            LoginStatus loginStatus = new LoginStatus();
            int success = 0;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT `同学ID`, `密码` FROM `同学` WHERE `电话` = @phonenumber";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@phonenumber", phonenumber);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int studentId = reader.GetInt32("同学ID");
                        string storedPassword = reader.GetString("密码");

                        if (password == storedPassword)
                        {
                            loginStatus.Exist = true;
                            loginStatus.UserId = studentId;
                            loginStatus.Message = "登录成功";
                            success = 1;
                        }
                        else
                        {
                            loginStatus.Exist = true;
                            loginStatus.UserId = studentId;
                            loginStatus.Message = "密码错误";
                        }
                    }
                    else
                    {
                        loginStatus.Exist = false;
                        loginStatus.Message = "用户不存在";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                    loginStatus.Exist = false;
                    loginStatus.Message = "登录过程中发生错误";
                }
                finally
                {
                    conn.Close();
                }
                try
                {
                    conn.Open();
                    if (success == 1)
                    {
                        // 获取当前时间
                        DateTime now = DateTime.Now;
                        // 创建SQL命令
                        string query = "UPDATE 同学 SET 最后登录时间 = @LastLoginTime WHERE 电话 = @phonenumber";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        // 添加参数
                        cmd.Parameters.AddWithValue("@LastLoginTime", now);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phonenumber);
                        cmd.ExecuteNonQuery();
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
            return loginStatus;
        }

    }
}
