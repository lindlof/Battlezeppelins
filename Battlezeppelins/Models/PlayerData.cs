using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Battlezeppelins.Models
{
    public class PlayerData
    {
        public Player player { get; set; }
        public IEnumerable<string> activePlayers { get; set; }
    }
}
