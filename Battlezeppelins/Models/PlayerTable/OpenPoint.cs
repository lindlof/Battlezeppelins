using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Battlezeppelins.Models
{
    public class OpenPoint : Point
    {
        public bool hit { get; private set; }

        public OpenPoint(int x, int y, bool hit) : base(x, y)
        {
            this.hit = hit;
        }
    }
}