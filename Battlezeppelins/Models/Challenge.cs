using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace Battlezeppelins.Models
{
    public class Challenge
    {
        public static void AddChallenge(Player challenger, Player challengee)
        {
            MySqlConnection conn = new MySqlConnection(
            ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            try
            {
                myCommand.CommandText = "INSERT INTO battlezeppelins.GameChallenge (challenger, challengee) VALUES (@challenger, @challengee)";
                myCommand.Parameters.AddWithValue("@challenger", challenger.id);
                myCommand.Parameters.AddWithValue("@challengee", challengee.id);
                myCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    throw new Exception("Challenge already in progress", ex);
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

        public static Player RetrieveChallenge(Player challengee)
        {
            if (challengee != null)
            {
                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                try
                {
                    myCommand.CommandText = "SELECT challenger FROM battlezeppelins.gamechallenge WHERE challengee = @id";
                    myCommand.Parameters.AddWithValue("@id", challengee.id);
                    using (MySqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string name = reader.GetString(reader.GetOrdinal("challenger"));
                            int? id = Int32.Parse(name);
                            return new Player(id);
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }

            return null;
        }

        public static void RemoveChallenge(Player challengee)
        {
            if (challengee != null)
            {
                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                try
                {
                    myCommand.CommandText = "DELETE FROM battlezeppelins.gamechallenge WHERE challengee = @id";
                    myCommand.Parameters.AddWithValue("@id", challengee.id);
                    myCommand.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}