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

        public Zeppelin(ZeppelinType zeppelin, int x, int y, bool rotDown) {
            this.type = type;
            this.x = x;
            this.y = y;
            this.rotDown = rotDown;
        }
    }
}