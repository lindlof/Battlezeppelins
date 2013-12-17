using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Battlezeppelins.Models;

namespace Battlezeppelins.Controllers
{
    public class PlayerController : Controller
    {
        //
        // GET: /Player/

        public ActionResult GetOnline()
        {
            return Json(Player.GetActivePlayers(), JsonRequestBehavior.AllowGet);
        }

    }
}
