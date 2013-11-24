using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Battlezeppelins.Models
{

    public class GameTable
    {
        public const int TABLE_ROWS = 10;
        public const int TABLE_COLS = 10;

        public Game.Role role { get; private set; }
        public List<OpenPoint> openPoints { get; private set; }
        public List<Zeppelin> zeppelins { get; private set; }

        public GameTable(Game.Role role)
        {
            this.role = role;

            this.openPoints = new List<OpenPoint>();
            this.zeppelins = new List<Zeppelin>();
        }

        public void removeZeppelins() {
            zeppelins = null;
        }

        public bool AddZeppelin(Zeppelin newZeppelin)
        {
            foreach (Zeppelin zeppelin in this.zeppelins)
            {
                // No multiple zeppelins of the same type
                if (zeppelin.type == newZeppelin.type)
                    return false;

                // No colliding zeppelins
                if (zeppelin.collides(newZeppelin))
                    return false;
            }

            if (newZeppelin.x < 0 ||
                newZeppelin.x + newZeppelin.getWidth()-1 > TABLE_COLS - 1 ||
                newZeppelin.y < 0 ||
                newZeppelin.y + newZeppelin.getHeight()-1 > TABLE_ROWS - 1)
            {
                return false;
            }

            this.zeppelins.Add(newZeppelin);

            return true;
        }
    }
}
