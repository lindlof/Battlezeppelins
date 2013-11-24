using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;

namespace Battlezeppelins.Models
{
    public class Game
    {
        public enum GameState { PREPARATION = 0, IN_PROGRESS = 1, CHALLENGER_WON = 2, CHALLENGEE_WON = 3 }
        public enum Role { CHALLENGER, CHALLENGEE }

        private int id { get; set; }
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
                    myCommand.CommandText = "SELECT id, challenger, challengee FROM battlezeppelins.game WHERE gameState = @gameState " + 
                        "AND (challenger = @playerId OR challengee = @playerId)";
                    myCommand.Parameters.AddWithValue("@gameState", (int)GameState.IN_PROGRESS);
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

                                int id = reader.GetInt32(reader.GetOrdinal("id"));
                                game.id = id;
                            }
                            else if (player.id == challengeeId)
                            {
                                GamePlayer gamePlayer = new GamePlayer(challengeeId, Game.Role.CHALLENGEE);
                                GamePlayer gameOpponent = new GamePlayer(challengerId, Game.Role.CHALLENGER);
                                game = new Game(gamePlayer, gameOpponent);

                                int id = reader.GetInt32(reader.GetOrdinal("id"));
                                game.id = id;
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
                myCommand.Parameters.AddWithValue("@gameState", newState);
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
                    myCommand.CommandText = "INSERT INTO battlezeppelins.game (challenger, challengee, gameState) VALUES (@challenger, @challengee, @gameState)";
                    myCommand.Parameters.AddWithValue("@challenger", challenger.id);
                    myCommand.Parameters.AddWithValue("@challengee", challengee.id);
                    myCommand.Parameters.AddWithValue("@gameState", (int)GameState.PREPARATION);
                    myCommand.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public List<GameTable> GetGame()
        {
            List<GameTable> tables = null;

            {
                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                try
                {
                    myCommand.CommandText = "SELECT gameTables FROM battlezeppelins.game WHERE gameId = @gameId";
                    myCommand.Parameters.AddWithValue("@gameId", this.id);
                    using (MySqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string gameTablesStr = reader.GetString(reader.GetOrdinal("gameTable"));
                            tables = new JavaScriptSerializer().Deserialize<List<GameTable>>(gameTablesStr);
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }

            if (tables != null)
            {
                foreach (GameTable table in tables)
                {
                    if (table.role != this.player.role)
                    {
                        table.removeZeppelins();
                    }
                }
            }

            return tables;
        }
    }
}
