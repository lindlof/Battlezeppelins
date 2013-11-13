using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Battlezeppelins.Models;

namespace Battlezeppelins.Controllers
{
    public class RequestController : Controller
    {
        public ActionResult BattleChallenge()
        {
            Player player = null;

            if (Request.Cookies["userInfo"] != null)
            {
                string idStr = Server.HtmlEncode(Request.Cookies["userInfo"]["id"]);
                int? id = Int32.Parse(idStr);
                player = new Player(id);
            }

            string challengeeName = Request.Form["challengeeSelected"];

            if (challengeeName != null)
            {
                
            }

            return null;
        }
    }
}
