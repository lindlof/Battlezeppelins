using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Battlezeppelins.Models;

namespace Battlezeppelins.Controllers
{
    public class GameController : BaseController
    {
        public ActionResult Metadata()
        {
            Game game = Game.GetInstance(base.GetPlayer());

            if (game != null)
            {
                return Json(new { playing = true, opponent = game.opponent.name }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { playing = false }, JsonRequestBehavior.AllowGet);
            }
        }
<<<<<<< HEAD

        public ActionResult Surrender()
        {
            Game game = Game.GetInstance(base.GetPlayer());

            if (game != null)
            {
                game.Surrender();
            }

            return null;
        }

=======
>>>>>>> origin/layout
    }
}
