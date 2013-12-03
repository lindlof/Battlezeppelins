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
            this.x = x;
            this.y = y;
        }
    }
}