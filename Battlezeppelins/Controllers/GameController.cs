﻿using System;
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

        public ActionResult Surrender()
        {
            Game game = Game.GetInstance(base.GetPlayer());

            if (game != null)
            {
                game.Surrender();
            }

            return null;
        }

        public ActionResult AddZeppelin()
        {
            Game game = Game.GetInstance(base.GetPlayer());

            string typeStr = Request.Form["type"];
            ZeppelinType type = (ZeppelinType)Enum.Parse(typeof(ZeppelinType), typeStr, true);
            int x = Int32.Parse(Request.Form["x"]);
            int y = Int32.Parse(Request.Form["y"]);
            bool rotDown = Boolean.Parse(Request.Form["rotDown"]);

            Zeppelin zeppelin = new Zeppelin(type, x, y, rotDown);

            GameTable table = game.GetPlayerTable();
            bool zeppelinAdded = table.AddZeppelin(zeppelin);

            if (zeppelinAdded) {
                game.PutTable(table);
            }

            return Json(zeppelinAdded, JsonRequestBehavior.AllowGet);
        }
    }
}
