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
        public int? id { get; set; }

        public static Player GetInstance(int? id)
        {
            Player player = new Player(id);
            if (player.id == null) return null;
            return player;
        }

        public static Player GetInstance(string name)
        {
            Player player = new Player(name);
            if (player.id == null) return null;
            return player;
        }

        protected Player(int? id)
        {
            if (id != null)
            {
                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                try
                {
                    myCommand.CommandText = "SELECT name FROM battlezeppelins.player WHERE id = @id";
                    myCommand.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.id = id;
                            this.name = reader.GetString(reader.GetOrdinal("name"));
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private Player(string name)
        {
            if (name != null)
            {
                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                try
                {
                    myCommand.CommandText = "SELECT id FROM battlezeppelins.player WHERE name = @name";
                    myCommand.Parameters.AddWithValue("@name", name);
                    using (MySqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.id = reader.GetInt32(reader.GetOrdinal("id"));
                            this.name = name;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void StatusUpdate()
        {
            MySqlConnection conn = new MySqlConnection(
            ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            try
            {
                myCommand.CommandText = "UPDATE battlezeppelins.player SET lastSeen=@lastSeen where id=@id";
                myCommand.Parameters.AddWithValue("@lastSeen", DateTime.Now);
                myCommand.Parameters.AddWithValue("@id", this.id);
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
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
                    myCommand.CommandText = "INSERT INTO battlezeppelins.player (name, lastSeen) VALUES (@name, @lastSeen)";
                    myCommand.Parameters.AddWithValue("@name", userName);
                    myCommand.Parameters.AddWithValue("@lastSeen", DateTime.Now);
                    myCommand.ExecuteNonQuery();


                    myCommand.CommandText = "SELECT id FROM battlezeppelins.player WHERE name = @name";
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
                        throw new Exception("MySql error " + ex.Number + ": " + ex.Message);
                    }
                }
                finally
                {
                    conn.Close();
                }
            }

            return id;
        }

        public static IEnumerable<string> GetActivePlayers()
        {
            List<string> activePlayers = new List<string>();

            MySqlConnection conn = new MySqlConnection(
            ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            try
            {
                myCommand.CommandText = "SELECT name FROM battlezeppelins.player WHERE TIMESTAMPDIFF(SECOND, player.lastSeen, @dateTime) < 600";
                myCommand.Parameters.AddWithValue("@dateTime", DateTime.Now);
                using (MySqlDataReader reader = myCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(reader.GetOrdinal("name"));
                        activePlayers.Add(name);
                    }
                }
            }
            finally
            {
                conn.Close();
            }

            return activePlayers;
        }
    }
}