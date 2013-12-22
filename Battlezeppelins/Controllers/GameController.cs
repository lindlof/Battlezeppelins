using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Battlezeppelins.Models;

namespace Battlezeppelins.Controllers
{
    public class GameController : BaseController
    {
        class MetadataResult
        {
            public bool playing;
            public string opponent;
            public string gameState;
            public string stateReason;
        }

        public ActionResult Metadata()
        {
            Player player = base.GetPlayer();
            Game game = Game.GetCurrentInstance(player);

            MetadataResult metadata = new MetadataResult();

            if (game != null) {
                metadata.playing = true;
            } else {
                metadata.playing = false;
                game = Game.GetLatestInstance(player);
            }

            if (game != null)
            {
                metadata.opponent = game.opponent.name;
                metadata.gameState = game.GameStateClientString();
                if (game.stateReason != null) 
                    metadata.stateReason = game.stateReason.ToString();
            }

            return Json(metadata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Surrender()
        {
            Player player = base.GetPlayer();
            Game game = Game.GetCurrentInstance(player);

            if (game != null)
            {
                lock (player)
                {
                    game.Surrender();
                }
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
            Zeppelin zeppelin = new Zeppelin(type, new Point(x, y), rotDown);

            bool zeppelinAdded;
            lock (player)
            {
                Game game = Game.GetCurrentInstance(player);
                zeppelinAdded = game.AddZeppelin(zeppelin);
            }

            return Json(zeppelinAdded, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpenPoint()
        {
            int x = Int32.Parse(Request.Form["x"]);
            int y = Int32.Parse(Request.Form["y"]);
            Point point = new Point(x, y);
            bool success = false;

            Player player = base.GetPlayer();
            lock (player)
            {
                Game game = Game.GetCurrentInstance(player);
								if (game != null)
									success = game.Open(point);
            }

            return Json(success, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlayerTable()
        {
            Game game = Game.GetCurrentInstance(base.GetPlayer());
            GameTable table = (game != null) ? game.GetPlayerTable() : null;
            return Json(table, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOpponentTable()
        {
            Game game = Game.GetCurrentInstance(base.GetPlayer());
            GameTable table = (game != null) ? game.GetOpponentTable() : null;
            return Json(table, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTurn()
        {
            Game game = Game.GetLatestInstance(base.GetPlayer());
						TurnData turn = null;
					  if (game != null) turn = game.GetTurnData();
            return Json(turn, JsonRequestBehavior.AllowGet);
        }
    }
}
