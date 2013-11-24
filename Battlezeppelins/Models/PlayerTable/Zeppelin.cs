using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Battlezeppelins.Models
{
    public class Zeppelin
    {
        public ZeppelinType type { get; private set; }
        public int x { get; private set; }
        public int y { get; private set; }
        public bool rotDown { get; private set; }

        public Zeppelin(ZeppelinType type, int x, int y, bool rotDown) {
            this.type = type;
            this.x = x;
            this.y = y;
            this.rotDown = rotDown;
        }

        public bool collides(Zeppelin zeppelin)
        {
            int x = zeppelin.x;
            int y = zeppelin.y;

            for (int i = 0; i < zeppelin.type.Length; i++)
            {
                if (zeppelin.rotDown) y--;
                else x++;

                if (collides(x, y)) return true;
            }

            return false;
        }

        public bool collides(int x, int y)
        {
            int thisX = this.x;
            int thisY = this.y;

            for (int i = 0; i < this.type.Length; i++)
            {
                if (this.rotDown) thisY--;
                else thisX++;

                if (thisX == x && thisY == y) return true;
            }

            return false;
        }

        public int getWidth()
        {
            if (this.rotDown) return 1;
            else return this.type.Length;
        }

        public int getHeight()
        {
            if (!this.rotDown) return 1;
            else return this.type.Length;
        }
    }
}
