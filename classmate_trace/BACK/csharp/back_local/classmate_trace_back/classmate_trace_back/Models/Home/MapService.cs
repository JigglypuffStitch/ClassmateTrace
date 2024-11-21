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
    public class MapService
    {
        private readonly AddressService addressService;
        private readonly ILogger<MapService> _logger;

        public MapService(AddressService addressService, ILogger<MapService> logger)
        {
            this.addressService = addressService;
            _logger = logger;
        }

        public bool FindRON(int GaID, int UID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM 同学小聚关联表 WHERE `同学ID` = @uid AND `小聚ID`=@GaID";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@uid", UID);
                    cmd.Parameters.AddWithValue("@GaID", GaID);
                    long count = (long)cmd.ExecuteScalar();
                    return count == 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error querying students by class ID: " + ex.Message);
                    return false;
                }
                finally { connection.Close(); }
            }
        }
        public List<int> FindAllClassmate(int class_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    _logger.LogInformation($"Finding classmates for class_id: {class_id}");

                    string query = "SELECT 同学ID列表 FROM 班级 WHERE `班级ID` = @class_id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@class_id", class_id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string classmatesString = reader.GetString(0);
                            _logger.LogInformation($"Found classmates string: {classmatesString}");

                            if (!string.IsNullOrEmpty(classmatesString))
                            {
                                List<int> classmates = classmatesString.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                                                     .Select(int.Parse)
                                                                     .ToList();
                                _logger.LogInformation($"Parsed {classmates.Count} classmate IDs");
                                return classmates;
                            }
                        }

                        _logger.LogWarning($"No classmates found for class {class_id}");
                        return new List<int>();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error finding classmates for class {class_id}");
                    return new List<int>();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public List<Address> FindAllAddress(int class_id)
        {
            List<int> classmates = FindAllClassmate(class_id);
            List<Address> addresses = new List<Address>();

            if (classmates == null || !classmates.Any())
            {
                _logger.LogWarning($"No classmates found for class_id: {class_id}");
                return addresses;
            }

            _logger.LogInformation($"Found {classmates.Count} classmates for class {class_id}");

            foreach (int user_id in classmates)
            {
                try
                {
                    var address = addressService.QueryAddressByID(user_id);
                    if (address != null && !string.IsNullOrEmpty(address.AddressNew))
                    {
                        addresses.Add(address);
                        _logger.LogInformation($"Added address for user {user_id}: {address.AddressNew}");
                    }
                    else
                    {
                        _logger.LogInformation($"No valid address found for user {user_id}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error getting address for user {user_id}");
                }
            }

            _logger.LogInformation($"Returning {addresses.Count} addresses for class {class_id}");
            return addresses;
        }

        public List<Gathering> FindAllGatherings(int user_id, int class_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    List<Gathering> gas = new List<Gathering>();
                    connection.Open();
                    string query = "SELECT * FROM 小聚厅 WHERE `班级ID` = @class_id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            int gaID = reader.GetInt32(0);
                            int proposer = reader.GetInt32(2);
                            //修改
                            DateTime Date = reader.GetDateTime(3);
                            string position = reader.GetString(4);
                            string description = reader.GetString(5);
                            string tmp = reader.GetString(6);
                            List<int> participant = new List<int>(Array.ConvertAll(tmp.Split(' '), int.Parse));
                            Gathering ga = new Gathering(gaID, class_id, proposer, Date, position, description, participant);
                            gas.Add(ga);
                        }
                    }
                    reader.Close();
                    foreach (Gathering g in gas)
                    {
                        g.Read = FindRON(g.GaID, user_id);
                    }
                    return gas;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally { connection.Close(); }
            }
        }

        public List<object> GetClassmatesLocations(int class_id)
        {
            List<object> locations = new List<object>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    _logger.LogInformation($"Getting locations for class_id: {class_id}");

                    // 使用JOIN来获取完整的位置信息
                    string query = @"
                        SELECT w.同学ID, w.最新位置信息, w.历史位置轨迹线, s.姓名, s.最后登录时间
                        FROM 位置 w
                        JOIN 同学 s ON w.同学ID = s.同学ID
                        WHERE s.班级ID列表 LIKE @class_id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@class_id", $"%{class_id}%");  // 模糊匹配班级ID

                    _logger.LogInformation($"Executing query: {query}");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var location = new
                            {
                                同学ID = reader.GetInt32("同学ID"),
                                姓名 = reader.GetString("姓名"),
                                最新位置信息 = !reader.IsDBNull(reader.GetOrdinal("最新位置信息"))
                                    ? reader.GetString("最新位置信息")
                                    : string.Empty,
                                历史位置轨迹线 = !reader.IsDBNull(reader.GetOrdinal("历史位置轨迹线"))
                                    ? reader.GetString("历史位置轨迹线")
                                    : string.Empty,
                                最后登录时间 = reader.GetDateTime("最后登录时间").ToString().Substring(0, 10)
                            };
                            locations.Add(location);
                            _logger.LogInformation($"Added location for student {location.姓名} (ID: {location.同学ID})");
                        }
                    }

                    _logger.LogInformation($"Found {locations.Count} locations for class {class_id}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error getting locations for class {class_id}");
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                        _logger.LogInformation("Database connection closed");
                    }
                }
            }

            return locations;
        }
    }
}
