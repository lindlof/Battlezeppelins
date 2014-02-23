using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Battlezeppelins.Models;

namespace Battlezeppelins.Controllers
{
    public class PollController : BaseController
    {
        public ActionResult UpdateLastSeen()
        {
            if (Request.Cookies["userInfo"] != null)
            {
                Player player = base.player;
                player.StatusUpdate();
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ChallengeInbox()
        {
            if (Request.Cookies["userInfo"] != null)
            {
				Player player = base.player;
                Player challenger = Challenge.RetrieveChallenge(player);

                if (challenger != null)
                {
                    string message = challenger.name;
                    return Json(new { challenged=true, challenger=challenger.name }, 
                        JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { challenged = false },
                JsonRequestBehavior.AllowGet);
        }
    }
}
