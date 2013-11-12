using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Battlezeppelins.Models;
using MySql.Data.MySqlClient;

namespace Battlezeppelins.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string registrationName = Request.Form["registrationName"];

            if (registrationName != null)
            {
                Register(registrationName);
            }

            Player player = null;

            if (Request.Cookies["userInfo"] != null)
            {
                string idStr = Server.HtmlEncode(Request.Cookies["userInfo"]["id"]);
                int? id = Int32.Parse(idStr);
                player = new Player(id);
            }

            if (player == null)
            {
                Response.Cookies["userInfo"].Expires = DateTime.Now.AddDays(-1d);
            }
            else
            {
                player.StatusUpdate();
            }

            PlayerData playerData = new PlayerData();
            playerData.player = player;
            playerData.activePlayers = Player.GetActivePlayers();

            return View(playerData);
        }

        public void Register(string registrationName)
        {
            int? id = null;

            try
            {
                id = Player.Register(registrationName);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message + "\n";
            }

            if (id != null)
            {
                Response.Cookies["userInfo"]["id"] = id.ToString();
                Response.Cookies["userInfo"].Expires = DateTime.MaxValue;
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
