using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Battlezeppelins.Models
{
    public class OpenPoint
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public bool hit { get; private set; }

        public OpenPoint(int x, int y, bool hit)
        {
            this.x = x;
            this.y = y;
            this.hit = hit;
        }
    }
}