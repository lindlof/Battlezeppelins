using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace Battlezeppelins.Models
{
    public class BattlezUser
    {
        public string name { get; set; }
        public bool exists { get; set; }

        public BattlezUser(int? id)
        {
            name = null;
            exists = false;

            if (id != null)
            {
                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                myCommand.CommandText = "SELECT name FROM battlezeppelins.player WHERE id = @ParamId";
                myCommand.Parameters.AddWithValue("@ParamId", id);
                using (MySqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        this.name = reader.GetString(reader.GetOrdinal("name"));
                        this.exists = true;
                    }
                }

                conn.Close();
            }
        }
    }
}