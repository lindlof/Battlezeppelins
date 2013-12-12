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
        public enum StateReason { GAME_OUTCOME = 0, OPP_SURRENDER = 1, OPP_INACTIVITY = 2 }
        public enum Role { CHALLENGER, CHALLENGEE }

        private int id { get; set; }
        public GameState gameState { get; private set; }
        public StateReason? stateReason { get; private set; }
        public GamePlayer player { get; private set; }
        public GamePlayer opponent { get; private set; }

        public static Game GetCurrentInstance(Player player)
        {
            Game game = GetLatestInstance(player);
            if (game != null &&
                (game.gameState != GameState.CHALLENGER_WON && game.gameState != GameState.CHALLENGEE_WON))
            {
                return game;
            }

            return null;
        }

        public static Game GetLatestInstance(Player player)
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
                    myCommand.CommandText = 
                        "SELECT id, gameState, challenger, challengee, " +
                        "TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP, updateTime)) AS inactivity " + 
                        "FROM battlezeppelins.game " +
                        "WHERE (challenger = @playerId OR challengee = @playerId) ORDER BY updateTime DESC LIMIT 1";
                    myCommand.Parameters.AddWithValue("@playerId", player.id);

                    using (MySqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int? inactivity = reader.GetInt32(reader.GetOrdinal("inactivity"));

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

                            if (inactivity > 120)
                            {
                                TurnData turnData = game.GetTurnData();
                                GameState newState = (turnData.turn) ? game.opponent.getWonState() : game.player.getWonState();
                                game.SetState(newState, StateReason.OPP_INACTIVITY);
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
            if (challenger != null && challengee != null)
            {
                CheckGameState(challenger, challengee);

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
                        "INSERT INTO battlezeppelins.game (challenger, challengee, gameState, challengerTable, challengeeTable, challengerTurn) " +
                        "VALUES (@challenger, @challengee, @gameState, @challengerTable, @challengeeTable, false)";
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

        public static void CheckGameState(Player challenger, Player challengee)
        {
            if (GetCurrentInstance(challenger) != null)
                throw new InvalidOperationException(challenger.name + " is already in game");
            if (GetCurrentInstance(challengee) != null)
                throw new InvalidOperationException(challengee.name + " is already in game");
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

            string columnName = (role == Role.CHALLENGER) ? "challengerTable" : "challengeeTable";

            try
            {
                myCommand.CommandText = "SELECT " + columnName + " FROM battlezeppelins.game WHERE id = @gameId";
                myCommand.Parameters.AddWithValue("@gameId", this.id);
                using (MySqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string gameTablesStr = reader.GetString(reader.GetOrdinal(columnName));
                        GameTable table = GameTable.deserialize(gameTablesStr);

                        if (table.role != role) throw new System.IO.InvalidDataException(
                            "Column \"" + columnName + "\" contains a table belonging to " + table.role);

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

        public bool AddZeppelin(Zeppelin zeppelin)
        {
            GameTable table = this.GetPlayerTable();
            bool zeppelinAdded = table.AddZeppelin(zeppelin);

            if (zeppelinAdded)
            {
                this.PutTable(table);
                PutTurnData(new TurnData(false, null)); // Faster gets to start
                this.CheckGameState();
            }

            return zeppelinAdded;
        }

        public void CheckGameState()
        {
            if (gameState == GameState.PREPARATION)
            {
                GameTable table = GetTable(this.player.role);
                if (table.zeppelins.Count == 3)
                {
                    GameTable opponentTable = this.GetTable(this.opponent.role);
                    if (opponentTable.zeppelins.Count == 3)
                    {
                        this.SetState(Game.GameState.IN_PROGRESS, null);
                    }
                }
            }
            else if (gameState == GameState.IN_PROGRESS)
            {
                GameTable table = this.GetTable(this.opponent.role);
                List<Point> points = new List<Point>(table.openPoints);
                foreach (Zeppelin zeppelin in table.zeppelins)
                {
                    if (!zeppelin.fullyCollides(points)) return;
                }
                Game.GameState state = (player.role == Game.Role.CHALLENGER) ? Game.GameState.CHALLENGER_WON : Game.GameState.CHALLENGEE_WON;
                this.SetState(state, StateReason.GAME_OUTCOME);
            }
        }

        public void Surrender()
        {
            GameState newState = (player.role == Role.CHALLENGER) ? GameState.CHALLENGER_WON : GameState.CHALLENGEE_WON;
            SetState(newState, StateReason.OPP_SURRENDER);
        }

        private void SetState(GameState state, StateReason? reason)
        {
            MySqlConnection conn = new MySqlConnection(
                    ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            object reasonObj = reason;
            if (reasonObj == null)
                reasonObj = DBNull.Value;

            try
            {
                myCommand.CommandText = "UPDATE battlezeppelins.game SET gameState = @state, stateReason = @reason WHERE id = @gameId";
                myCommand.Parameters.AddWithValue("@state", state);
                myCommand.Parameters.AddWithValue("@reason", reasonObj);
                myCommand.Parameters.AddWithValue("@gameId", this.id);
                myCommand.ExecuteNonQuery();

                this.gameState = state;
                this.stateReason = reason;
            }
            finally
            {
                conn.Close();
            }
        }
        
        public TurnData GetTurnData()
        {
            MySqlConnection conn = new MySqlConnection(
                    ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();
            
            try
            {
                myCommand.CommandText = "SELECT challengerTurn, lastOpen FROM battlezeppelins.game WHERE id = @gameId";
                myCommand.Parameters.AddWithValue("@gameId", this.id);
                using (MySqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bool challengerTurn = reader.GetBoolean(reader.GetOrdinal("challengerTurn"));
                        bool challenger = player.role == Game.Role.CHALLENGER;

                        bool turn = (challengerTurn && challenger) || (!challengerTurn && !challenger);
                        OpenPoint lastOpen = null;

                        {
                            int ordinal = reader.GetOrdinal("lastOpen");
                            if (!reader.IsDBNull(ordinal))
                            {
                                string lastOpenStr = reader.GetString(ordinal);
                                lastOpen = OpenPoint.deserialize(lastOpenStr);
                            }
                        }

                        TurnData turnData = new TurnData(turn, lastOpen);
                        return turnData;
                    }
                }
            }
            finally
            {
                conn.Close();
            }

            return null;
        }

        private void PutTurnData(TurnData turnData)
        {
            bool challengerTurn = (player.role == Game.Role.CHALLENGER) ? false : true;
            string lastOpenStr = (turnData.lastOpen != null) ? turnData.lastOpen.serialize() : null;

            MySqlConnection conn = new MySqlConnection(
                    ConfigurationManager.ConnectionStrings["BattlezConnection"].ConnectionString);
            MySqlCommand myCommand = conn.CreateCommand();
            conn.Open();

            try
            {
                myCommand.CommandText = "UPDATE battlezeppelins.game SET challengerTurn = @challengerTurn, lastOpen = @lastOpen";
                myCommand.Parameters.AddWithValue("@challengerTurn", challengerTurn);
                myCommand.Parameters.AddWithValue("@lastOpen", lastOpenStr);
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        public bool Open(Point point)
        {
            if (gameState != GameState.IN_PROGRESS)
                return false;

            TurnData turnData = GetTurnData();
            if (turnData.turn == false)
                return false;

            GameTable table = GetTable(this.opponent.role);
            if (table.alreadyOpen(point))
                return false;

            bool hit = table.pointCollides(point);
            OpenPoint openPoint = new OpenPoint(point.x, point.y, hit);
            table.openPoints.Add(openPoint);
            PutTable(table);

            TurnData newTurnData = new TurnData(false, openPoint);
            PutTurnData(newTurnData);

            CheckGameState();

            return true;
        }

        public string GameStateClientString() {
            string win = "YOU_WIN";
            string lose = "YOU_LOSE";

            if (gameState == GameState.CHALLENGER_WON)
            {
                return player.role == Role.CHALLENGER ? win : lose;
            }
            else if (gameState == GameState.CHALLENGEE_WON)
            {
                return player.role == Role.CHALLENGEE ? win : lose;
            }
            else
            {
                return gameState.ToString();
            }
        }
    }
}
