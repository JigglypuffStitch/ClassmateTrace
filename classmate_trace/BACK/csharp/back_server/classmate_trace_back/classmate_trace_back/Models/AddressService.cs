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
using static ClassmateTraceBack.Models.classes;

namespace ClassmateTraceBack.Models
{


    public class AddressService
    {

        public AddressService()
        {

        }


        //根据同学ID查找位置信息
        public Address? QueryAddressByID(int ID)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    Address? address = null;

                    connection.Open();
                    string query = "SELECT 最新位置信息,历史位置轨迹线 FROM 位置 WHERE 同学ID=@ID";
                    MySqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@ID", ID);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string? addressNew = reader.IsDBNull(0) ? null : reader.GetString(0);
                        string? addressHistory = reader.IsDBNull(1) ? null : reader.GetString(1);

                        if (address == null)
                        {
                            address = new(ID);
                        }

                        address.AddressNew = addressNew;
                        address.AddressHistory = addressHistory;

                    }
                    reader.Close();
                    connection.Close();

                    return address;
                }
                catch (Exception ex)
                {

                    connection.Close();
                    Console.WriteLine("查找位置信息错误" + ex.Message);
                    return null;
                }
            }
        }

        //更新位置信息
        public Address UpdateAddress(int ID, string? addressNew)
        {
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    string? addressHistory;
                    Address? address = QueryAddressByID(ID);
                    if (address == null)
                    {
                        addressHistory = addressNew;

                        //INSERT
                        connection.Open();
                        string query = "INSERT INTO 位置 (同学ID, 最新位置信息, 历史位置轨迹线) VALUES (@ID, @addressNew, @addressHistory)";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@addressNew", addressNew);
                        cmd.Parameters.AddWithValue("@addressHistory", addressHistory);
                        cmd.ExecuteNonQuery();

                    }
                    else
                    {
                        if (addressNew != null)
                        {
                            if (address.AddressHistory == null)
                            {
                                addressHistory = addressNew;
                            }
                            else
                            {
                                addressHistory = address.AddressHistory + " " + addressNew;
                            }
                        }
                        else
                        {
                            addressHistory = address.AddressHistory;
                        }

                        //UPDATE
                        connection.Open();
                        string query = "UPDATE 位置 SET 最新位置信息=@addressNew,历史位置轨迹线=@addressHistory WHERE 同学ID=@ID";
                        MySqlCommand cmd = new(query, connection);
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@addressNew", addressNew);
                        cmd.Parameters.AddWithValue("@addressHistory", addressHistory);
                        cmd.ExecuteNonQuery();
                        connection.Close();

                    }

                    address = QueryAddressByID(ID);
                    return address;
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
