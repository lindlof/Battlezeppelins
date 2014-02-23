using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Battlezeppelins.Models;

namespace Battlezeppelins.Controllers
{
    public class ChatController : BaseController
    {

        public ActionResult SendMessage()
        {
            Player player = base.player;
            string message = Request.Form["message"];

            Chat.sendMessage(player, message);

            return null;
        }

        public ActionResult GetMessages()
        {
            string fromIdStr = Request.Form["fromId"];
            int fromId = Int32.Parse(fromIdStr);
            return Json(Chat.getMessages(fromId), JsonRequestBehavior.AllowGet);
        }

    }
}
