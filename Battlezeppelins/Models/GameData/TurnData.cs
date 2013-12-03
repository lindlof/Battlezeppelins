using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Battlezeppelins.Models
{
    public class TurnData
    {
        public bool turn { get; private set; }
        public OpenPoint lastOpen { get; private set; }

        public TurnData(bool turn, OpenPoint lastOpen)
        {
            this.turn = turn;
            this.lastOpen = lastOpen;
        }
    };
}