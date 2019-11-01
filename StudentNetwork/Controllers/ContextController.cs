using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.Controllers
{
    public class ContextController : Controller
    {
        protected readonly StudentContext db;
        public ContextController(StudentContext context) => db = context;

        public async Task<Student> GetCurrentStudentAsync()
        {
            return await db.Students.FirstAsync(s => s.Login == User.Identity.Name);
        }
        public Student GetCurrentStudent()
        {
            return db.Students.First(s => s.Login == User.Identity.Name);
        }

        public IActionResult GetStudentName()
        {
            return Content(GetCurrentStudent().Name);
        }
    }
}
