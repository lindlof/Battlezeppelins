using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Battlezeppelins.Models;

namespace Battlezeppelins.Controllers
{
    public class PollController : Controller
    {
        public ActionResult UpdateLastSeen()
        {
            if (Request.Cookies["userInfo"] != null)
            {
                string idStr = Server.HtmlEncode(Request.Cookies["userInfo"]["id"]);
                int? id = Int32.Parse(idStr);
                Player player = new Player(id);
                player.StatusUpdate();
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }
        /*
        public ActionResult ActivePlayers()
        {
            Data data = new Data();
            data.activePlayers = Player.GetActivePlayers();

            return Json(data, JsonRequestBehavior.AllowGet);
        }*/
    }
}
