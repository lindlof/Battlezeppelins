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
        public enum GameState { IN_PROGRESS = 0, CHALLENGER_WON = 1, CHALLENGEE_WON = 2 }
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
                    myCommand.CommandText = "SELECT * FROM battlezeppelins.game WHERE gameState = @gameState " + 
                        "AND (challenger = @playerId OR challengee = @playerId)";
                    myCommand.Parameters.AddWithValue("gameState", (int)GameState.IN_PROGRESS);
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

        public void Surrender()
        {
            MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            int newState = (player.role == Role.CHALLENGER) ? (int)GameState.CHALLENGER_WON : (int)GameState.CHALLENGEE_WON;

            try
            {
                myCommand.CommandText = "UPDATE battlezeppelins.game SET gameState = @gameState";
                myCommand.Parameters.AddWithValue("gameState", newState);
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        public static void Register(Player challenger, Player challengee)
        {
            if (challenger != null && challengee != null)
            {
                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                try
                {
                    myCommand.CommandText = "INSERT INTO battlezeppelins.game (challenger, challengee, gameState) VALUES (@challenger, @challengee, " + (int)GameState.IN_PROGRESS + ")";
                    myCommand.Parameters.AddWithValue("@challenger", challenger.id);
                    myCommand.Parameters.AddWithValue("@challengee", challengee.id);
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