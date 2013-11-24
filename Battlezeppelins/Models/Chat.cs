using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;
using System.Globalization;

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

        public class Message
        {
            public string id;
            public string user;
            public string time;
            public string message;
        }

        public static List<Message> getMessages(int fromId)
        {
            MySqlConnection conn = new MySqlConnection(
            ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            List<Message> messages = new List<Message>();
            int maxId;

            try
            {
                myCommand.CommandText = "SELECT COALESCE(MAX(id), 0) AS id FROM battlezeppelins.message";
                
                using (MySqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string maxIdStr = reader.GetString(reader.GetOrdinal("id"));
                        maxId = Int32.Parse(maxIdStr);
                    }
                    else
                    {
                        maxId = 0;
                    }
                }

                if (fromId < 0)
                {
                    fromId = maxId + fromId;
                }

                if (fromId < 0) fromId = 0;

                myCommand.CommandText = "SELECT Message.id, Player.name, Message.time, Message.text FROM battlezeppelins.message " +
                    "INNER JOIN Player ON Player.id=Message.player " +
                    "WHERE Message.id > @fromId " +
                    "ORDER BY Message.id";
                myCommand.Parameters.AddWithValue("@fromId", fromId);

                using (MySqlDataReader reader = myCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader.GetString(reader.GetOrdinal("id"));
                        string user = reader.GetString(reader.GetOrdinal("name"));
                        string time = reader.GetString(reader.GetOrdinal("time"));
                        string text = reader.GetString(reader.GetOrdinal("text"));

                        Message message = new Message();
                        message.id = id;
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
            return messages;
        }
    }
}