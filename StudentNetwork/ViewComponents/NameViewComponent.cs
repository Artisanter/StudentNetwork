using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNetwork.Models;

namespace StudentNetwork.ViewComponents
{
    public class NameViewComponent : ViewComponent
    {
        public NameViewComponent(StudentContext context) => db = context;
        private readonly StudentContext db;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var student = await db.Students.FirstAsync(s => s.Login == User.Identity.Name);
            return Content(student.Name);
        }
    }
}
