using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace Battlezeppelins.Models
{
    public class Player
    {
        public string name { get; set; }

        public Player(int? id)
        {
            name = null;

            if (id != null)
            {
                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                try
                {
                    myCommand.CommandText = "SELECT name FROM battlezeppelins.player WHERE id = @ParamId";
                    myCommand.Parameters.AddWithValue("@ParamId", id);
                    using (MySqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.name = reader.GetString(reader.GetOrdinal("name"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString(), ex);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static int? Register(string userName) {
            int? id = null;

            if (userName == "")
            {
                throw new Exception("Username empty");
            }

            if (userName != null)
            {
                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                try
                {
                    myCommand.CommandText = "INSERT INTO battlezeppelins.player (name) VALUES (@ParamName)";
                    myCommand.Parameters.AddWithValue("@ParamName", userName);
                    myCommand.ExecuteNonQuery();


                    myCommand.CommandText = "SELECT id FROM battlezeppelins.player WHERE name = @ParamName";
                    using (MySqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string idStr = reader.GetString(reader.GetOrdinal("id"));
                            id = Int32.Parse(idStr);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062)
                    {
                        throw new Exception("Username already taken", ex);
                    }
                    else
                    {
                        throw new Exception("MySql error " + ex.Number);
                    }
                }
                finally
                {
                    conn.Close();
                }
            }

            return id;
        }
    }
}