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
        private static Dictionary<int, Player> players = new Dictionary<int, Player>();

		private BaseData baseData;
		public Player player { get; private set; }

		protected override void OnActionExecuting(ActionExecutingContext aec)
		{
			base.OnActionExecuting(aec);
			baseData = new BaseData();
			GetPlayer();

			baseData.navigationShowGame = (Game.GetCurrentInstance(player) != null);

			ViewData["BaseData"] = baseData;
		}

		public Player GetPlayer()
		{
			this.player = GetLocalPlayer();
			return this.player;
		}

        private Player GetLocalPlayer()
        {
            if (Request.Cookies["userInfo"] != null)
            {
                string idStr = Server.HtmlEncode(Request.Cookies["userInfo"]["id"]);
                int id = Int32.Parse(idStr);

                Player player = SearchPlayer(id);
                if (player != null) return player;

                lock (players)
                {
                    player = SearchPlayer(id);
                    if (player != null) return player;

                    Player newPlayer = Player.GetInstance(id);
                    if (newPlayer != null) players.Add(newPlayer.id, newPlayer);
                    return newPlayer;
                }
            }
            return null;
        }

        private Player SearchPlayer(int id)
        {
            Player player;
            if (players.TryGetValue(id, out player))
            {
                return player;
            }
            return null;
        }
    }
}
