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
            try
            {
                return new GamePlayer(id, role);
            }
            catch (ArgumentException)
            {
                return null;
            }
            
        }

        private GamePlayer(int? id, Game.Role role) : base(id) {
            this.role = role;
        }

        public Game.GameState getWonState() {
            if (role == Game.Role.CHALLENGER) return Game.GameState.CHALLENGER_WON;
            else return Game.GameState.CHALLENGEE_WON;
        }
    }
}
