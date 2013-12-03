using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Battlezeppelins.Models
{
    public class Point
    {
        [JsonProperty]
        public int x { get; private set; }
        [JsonProperty]
        public int y { get; private set; }

        public Point(int x, int y)
        {
            if (x < 0 || x > GameTable.TABLE_COLS - 1) throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y > GameTable.TABLE_ROWS - 1) throw new ArgumentOutOfRangeException("y");

            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            Point point = (Point)obj;
            return point.x == this.x && point.y == this.y;
        }
    }
}