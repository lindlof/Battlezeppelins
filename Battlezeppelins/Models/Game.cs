using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace Battlezeppelins.Models
{
    public class Game
    {
        public enum Role { CHALLENGER, CHALLENGEE }

        public GamePlayer player { get; set; }
        public GamePlayer opponent { get; set; }

        public static Game GetInstance(Player player)
        {
            Game game = null;

            if (player != null)
            {
                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                try
                {
                    myCommand.CommandText = "SELECT * FROM battlezeppelins.game WHERE " +
                        "challenger = @playerId OR challengee = @playerId";
                    myCommand.Parameters.AddWithValue("@playerId", player.id);
                    using (MySqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int? challengerId = reader.GetInt32(reader.GetOrdinal("challenger"));
                            int? challengeeId = reader.GetInt32(reader.GetOrdinal("challengee"));

                            if (player.id == challengerId)
                            {
                                GamePlayer gamePlayer = new GamePlayer(challengerId, Game.Role.CHALLENGER);
                                GamePlayer gameOpponent = new GamePlayer(challengeeId, Game.Role.CHALLENGEE);
                                game = new Game(gamePlayer, gameOpponent);
                            }
                            else if (player.id == challengeeId)
                            {
                                GamePlayer gamePlayer = new GamePlayer(challengeeId, Game.Role.CHALLENGEE);
                                GamePlayer gameOpponent = new GamePlayer(challengerId, Game.Role.CHALLENGER);
                                game = new Game(gamePlayer, gameOpponent);
                            }
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }

            return game;
        }

        private Game(GamePlayer player, GamePlayer opponent)
        {
            this.player = player;
            this.opponent = opponent;
        }

        public static void Register(Player challenger, Player challengee)
        {
            // Insert game
        }
    }
}