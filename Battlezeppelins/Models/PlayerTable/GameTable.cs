using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Battlezeppelins.Models
{

    public class GameTable
    {
        public Game.Role role { get; private set; }
        public List<OpenPoint> openPoints { get; private set; }
        public List<Zeppelin> zeppelins { get; private set; }

        public GameTable(Game.Role role)
        {
            this.role = role;
        }

        public void removeZeppelins() {
            zeppelins = null;
        }
    }
}