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
                return Json(new { 
                    playing = true, 
                    opponent = game.opponent.name,
                    gameState = game.gameState.ToString() }, JsonRequestBehavior.AllowGet);
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
            string typeStr = Request.Form["type"];
            ZeppelinType type = ZeppelinType.getByName(typeStr);
            int x = Int32.Parse(Request.Form["x"]);
            int y = Int32.Parse(Request.Form["y"]);
            bool rotDown = Boolean.Parse(Request.Form["rotDown"]);

            Player player = base.GetPlayer();
            Zeppelin zeppelin = new Zeppelin(type, x, y, rotDown);

            bool zeppelinAdded;
            lock (player)
            {
                Game game = Game.GetInstance(player);
                zeppelinAdded = game.AddZeppelin(zeppelin);
            }

            return Json(zeppelinAdded, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlayerTable()
        {
            Game game = Game.GetInstance(base.GetPlayer());
            GameTable table = game.GetPlayerTable();
            return Json(table, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOpponentTable()
        {
            Game game = Game.GetInstance(base.GetPlayer());
            GameTable table = game.GetOpponentTable();
            return Json(table, JsonRequestBehavior.AllowGet);
        }
    }
}
