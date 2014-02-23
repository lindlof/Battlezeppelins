using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Battlezeppelins.Models;
using System.Net;
using System.Web.Http;

namespace Battlezeppelins.Controllers
{
    public class ChallengeController : BaseController
    {
        public ActionResult BattleChallenge()
        {
            string message = "";
			Player challenger = base.player;
            Player challengee = null;

            string challengeeName = Request.Form["challengeeSelected"];

            if (challengeeName != null)
            {
                challengee = Player.GetInstance(challengeeName);
            }

            if (challenger != null && challengee != null)
            {
                try
                {
                    Challenge.RemoveChallenge(true, challenger);
                    Challenge.AddChallenge(challenger, challengee);
                    message = "Challenge sent to " + challengee.name;
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
            }
            else
            {
                if (challenger == null) message += "Can't identify you. ";
                if (challengee == null) message += "Can't identify challenged player. ";
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Run when player accepts or rejects a challenge
        /// </summary>
        /// <returns></returns>
        public ActionResult BattleAnswer()
        {
			Player challengee = base.player;
            string playerAcceptsStr = Request.Form["PlayerAccepts"];
            bool playerAccepts = Boolean.Parse(playerAcceptsStr);
            Player challenger = Challenge.RetrieveChallenge(challengee);

            Challenge.RemoveChallenge(false, challengee);
            if (playerAccepts)
            {
                // Also remove challenges issued by challengee
                Challenge.RemoveChallenge(true, challengee);

                Game.Register(challenger, challengee);
            }

            return null;
        }
        
    }
}
