using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentNetwork.Models;

namespace StudentNetwork.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentContext db;

        public HomeController(StudentContext context)
        {
            db = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewBag.Student = db.Students.First(s => s.Login == User.Identity.Name);
            return View();
        }
        
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
