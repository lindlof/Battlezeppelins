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
                    myCommand.CommandText = "SELECT id, gameState, challenger, challengee FROM battlezeppelins.game " +
                        "WHERE (gameState = @gameStatePrep OR gameState = @gameStateProgress) " + 
                        "AND (challenger = @playerId OR challengee = @playerId)";
                    myCommand.Parameters.AddWithValue("@gameStatePrep", (int)GameState.PREPARATION);
                    myCommand.Parameters.AddWithValue("@gameStateProgress", (int)GameState.IN_PROGRESS);
                    myCommand.Parameters.AddWithValue("@playerId", player.id);
                    using (MySqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int? challengerId = reader.GetInt32(reader.GetOrdinal("challenger"));
                            int? challengeeId = reader.GetInt32(reader.GetOrdinal("challengee"));

                            if (player.id == challengerId)
                            {
                                GamePlayer gamePlayer = GamePlayer.GetInstance(challengerId, Game.Role.CHALLENGER);
                                GamePlayer gameOpponent = GamePlayer.GetInstance(challengeeId, Game.Role.CHALLENGEE);
                                game = new Game(gamePlayer, gameOpponent);
                            }
                            else if (player.id == challengeeId)
                            {
                                GamePlayer gamePlayer = GamePlayer.GetInstance(challengeeId, Game.Role.CHALLENGEE);
                                GamePlayer gameOpponent = GamePlayer.GetInstance(challengerId, Game.Role.CHALLENGER);
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

                string challengerTableStr = challengerTable.serialize();
                string challengeeTableStr = challengeeTable.serialize();

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
            GameTable opponentTable = GetTable(this.opponent.role);
            opponentTable.removeNonOpenZeppelins();
            return opponentTable;
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
                myCommand.CommandText = "SELECT " + tableName + " FROM battlezeppelins.game WHERE id = @gameId";
                myCommand.Parameters.AddWithValue("@gameId", this.id);
                using (MySqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string gameTablesStr = reader.GetString(reader.GetOrdinal(tableName));
                        GameTable table = GameTable.deserialize(gameTablesStr);
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

        private void PutTable(GameTable table)
        {
            string tableStr = table.serialize();

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

        private void SetState(GameState state)
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

        public bool AddZeppelin(Zeppelin zeppelin)
        {
            GameTable table = this.GetPlayerTable();
            bool zeppelinAdded = table.AddZeppelin(zeppelin);

            if (zeppelinAdded)
            {
                this.PutTable(table);
                this.CheckGameState();
            }

            return zeppelinAdded;
        }

        public void CheckGameState()
        {
            if (gameState == GameState.PREPARATION)
            {
                GameTable table = this.GetPlayerTable();
                if (table.zeppelins.Count == 3)
                {
                    GameTable opponentTable = this.GetOpponentTable();
                    if (opponentTable.zeppelins.Count == 3)
                    {
                        this.SetState(Game.GameState.IN_PROGRESS);
                    }
                }
            }
        }
    }
}
