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
            Player challenger = null;
            Player challengee = null;

            if (Request.Cookies["userInfo"] != null)
            {
                string idStr = Server.HtmlEncode(Request.Cookies["userInfo"]["id"]);
                int? id = Int32.Parse(idStr);
                challenger = new Player(id);
            }

            string challengeeName = Request.Form["challengeeSelected"];

            if (challengeeName != null)
            {
                challengee = new Player(challengeeName);
            }

            if (challenger != null && challengee != null)
            {
                Challenge.AddChallenge(challenger, challengee);
            }

            return null;
        }
    }
}
