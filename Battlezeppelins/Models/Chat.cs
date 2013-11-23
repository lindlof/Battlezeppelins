using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;

namespace Battlezeppelins.Models
{
    public class Chat
    {
        public static void sendMessage(Player player, string message)
        {
            MySqlConnection conn = new MySqlConnection(
            ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            try
            {
                myCommand.CommandText = "INSERT INTO battlezeppelins.message (player, time, text) VALUES (@player, @time, @text)";
                myCommand.Parameters.AddWithValue("@player", player.id);
                myCommand.Parameters.AddWithValue("@time", DateTime.Now);
                myCommand.Parameters.AddWithValue("@text", message);
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        class Message
        {
            public string user;
            public string time;
            public string message;
        }

        public static string getMessages(string fromTime)
        {
            MySqlConnection conn = new MySqlConnection(
            ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            List<Message> messages = new List<Message>();

            try
            {
                myCommand.CommandText = "SELECT * FROM battlezeppelins.message WHERE TIMESTAMPDIFF(SECOND, @time, message.time) > 0";
                myCommand.Parameters.AddWithValue("@fromTime", DateTime.Parse(fromTime));
                using (MySqlDataReader reader = myCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string user = reader.GetString(reader.GetOrdinal("user"));
                        string time = reader.GetString(reader.GetOrdinal("time"));
                        string text = reader.GetString(reader.GetOrdinal("text"));

                        Message message = new Message();
                        message.user = user;
                        message.time = time;
                        message.message = text;
                        messages.Add(message);
                    }
                }
            }
            finally
            {
                conn.Close();
            }

            
            JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();
            return(jsonSerialiser.Serialize(messages));
        }
    }
}