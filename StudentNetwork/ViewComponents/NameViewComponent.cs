using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNetwork.Models;

namespace StudentNetwork.ViewComponents
{
    public class NameViewComponent : ViewComponent
    {
        public NameViewComponent(StudentContext context) => db = context;
        private readonly StudentContext db;
        private string last = "Guest";

        [Authorize]
        public IViewComponentResult Invoke()
        {
            last = db.Students.FirstOrDefault(s => s.Login == User.Identity.Name)?.Name ?? last;
            return Content(last);
        }
    }
}
