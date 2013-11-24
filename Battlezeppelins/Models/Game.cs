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
        public GameState gameState { get; private set; }
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
                    myCommand.CommandText = "SELECT id, gameState, challenger, challengee FROM battlezeppelins.game WHERE gameState = @gameState " + 
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
                            }
                            else if (player.id == challengeeId)
                            {
                                GamePlayer gamePlayer = new GamePlayer(challengeeId, Game.Role.CHALLENGEE);
                                GamePlayer gameOpponent = new GamePlayer(challengerId, Game.Role.CHALLENGER);
                                game = new Game(gamePlayer, gameOpponent);
                            }

                            if (game != null)
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("id"));
                                game.id = id;
                                int gameState = reader.GetInt32(reader.GetOrdinal("gameState"));
                                game.gameState = (GameState)gameState;
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
                GameTable challengerTable = new GameTable(Role.CHALLENGER);
                GameTable challengeeTable = new GameTable(Role.CHALLENGEE);

                string challengerTableStr = new JavaScriptSerializer().Serialize(challengerTable);
                string challengeeTableStr = new JavaScriptSerializer().Serialize(challengeeTable);

                MySqlConnection conn = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
                MySqlCommand myCommand = conn.CreateCommand();
                conn.Open();

                try
                {
                    myCommand.CommandText = 
                        "INSERT INTO battlezeppelins.game (challenger, challengee, gameState, challengerTable, challengeeTable) " +
                        "VALUES (@challenger, @challengee, @gameState, @challengerTable, @challengeeTable)";
                    myCommand.Parameters.AddWithValue("@challenger", challenger.id);
                    myCommand.Parameters.AddWithValue("@challengee", challengee.id);
                    myCommand.Parameters.AddWithValue("@gameState", (int)GameState.PREPARATION);
                    myCommand.Parameters.AddWithValue("@challengerTable", challengerTableStr);
                    myCommand.Parameters.AddWithValue("@challengeeTable", challengeeTableStr);
                    myCommand.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public GameTable GetPlayerTable()
        {
            return GetTable(this.player.role);
        }

        public GameTable GetOpponentTable()
        {
            GameTable table = GetTable(this.opponent.role);
            table.removeZeppelins();
            return table;
        }

        private GameTable GetTable(Role role)
        {
            MySqlConnection conn = new MySqlConnection(
            ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            string tableName = (role == Role.CHALLENGER) ? "challengerTable" : "challengeeTable";

            try
            {
                myCommand.CommandText = "SELECT " + tableName + " FROM battlezeppelins.game WHERE gameId = @gameId";
                myCommand.Parameters.AddWithValue("@gameId", this.id);
                using (MySqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string gameTablesStr = reader.GetString(reader.GetOrdinal(tableName));
                        GameTable  table = new JavaScriptSerializer().Deserialize<GameTable>(gameTablesStr);
                        return table;
                    }
                }
            }
            finally
            {
                conn.Close();
            }

            return null;
        }

        public void PutTable(GameTable table)
        {
            string tableStr = new JavaScriptSerializer().Serialize(table);

            MySqlConnection conn = new MySqlConnection(
            ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            string tableName = (table.role == Role.CHALLENGER) ? "challengerTable" : "challengeeTable";

            try
            {
                myCommand.CommandText = "UPDATE battlezeppelins.game SET " + tableName + " = @table";
                myCommand.Parameters.AddWithValue("@table", tableStr);
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        public void SetState(GameState state)
        {
            MySqlConnection conn = new MySqlConnection(
            ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            try
            {
                myCommand.CommandText = "UPDATE battlezeppelins.game SET gameState = @state";
                myCommand.Parameters.AddWithValue("@state", state);
                myCommand.ExecuteNonQuery();

                this.gameState = state;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
