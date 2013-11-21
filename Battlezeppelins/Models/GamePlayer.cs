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
        Game.Role role;

        public GamePlayer(int? id, Game.Role role) : base(id) {
            this.role = role;
        }
    }
}