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
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            int? id = null;
            if (Request.Cookies["userInfo"] != null)
            {
                string idStr = Server.HtmlEncode(Request.Cookies["userInfo"]["id"]);
                id = Int32.Parse(idStr);
            }

            BattlezUser user = new BattlezUser(id);

            if (!user.exists)
            {
                Response.Cookies["userInfo"].Expires = DateTime.Now.AddDays(-1d);
            }

            Response.Cookies["userInfo"]["id"] = "1";
            Response.Cookies["userInfo"].Expires = DateTime.MaxValue;

            return View(user);
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
