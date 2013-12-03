using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Battlezeppelins.Models;

namespace Battlezeppelins.Controllers
{
    public abstract class BaseController : Controller
    {
        private static List<Player> playerList = new List<Player>();

        public Player GetPlayer()
        {
            if (Request.Cookies["userInfo"] != null)
            {
                string idStr = Server.HtmlEncode(Request.Cookies["userInfo"]["id"]);
                int id = Int32.Parse(idStr);

                Player player = SearchPlayer(id);
                if (player != null) return player;

                lock (playerList)
                {
                    player = SearchPlayer(id);
                    if (player != null) return player;

                    Player newPlayer = Player.GetInstance(id);
                    if (newPlayer != null) playerList.Add(newPlayer);
                    return newPlayer;
                }
            }
            return null;
        }

        private Player SearchPlayer(int id)
        {
            if (!playerList.Any()) return null;

            foreach (Player listPlayer in playerList) {
                if (listPlayer.id == id) {
                    return listPlayer;
                }
            }
            return null;
        }
    }
}
