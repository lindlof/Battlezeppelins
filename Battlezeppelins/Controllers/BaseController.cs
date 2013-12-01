using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.CompilerServices;
using Battlezeppelins.Models;

namespace Battlezeppelins.Controllers
{
    public abstract class BaseController : Controller
    {
        private static List<Player> playerList = new List<Player>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Player GetPlayer()
        {
            if (Request.Cookies["userInfo"] != null)
            {
                string idStr = Server.HtmlEncode(Request.Cookies["userInfo"]["id"]);
                int id = Int32.Parse(idStr);

                Player player = null;
                foreach (Player listPlayer in playerList) {
                    if (listPlayer.id == id) {
                        player = listPlayer;
                    }
                }

                if (player == null)
                {
                    Player newPlayer = Player.GetInstance(id);
                    playerList.Add(newPlayer);
                    return newPlayer;
                } else {
                    return player;
                }
            }
            return null;
        }
    }
}
