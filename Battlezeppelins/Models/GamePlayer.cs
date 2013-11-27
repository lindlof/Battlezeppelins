using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Battlezeppelins.Models
{
    /// <summary>
    /// Player belonging for a game
    /// </summary>
    public class GamePlayer : Player
    {
        public Game.Role role { get; private set; }

        public static GamePlayer GetInstance(int? id, Game.Role role)
        {
            GamePlayer gamePlayer = new GamePlayer(id, role);
            if (gamePlayer.id == null) return null;
            return gamePlayer;
        }

        private GamePlayer(int? id, Game.Role role) : base(id) {
            this.role = role;
        }
    }
}
