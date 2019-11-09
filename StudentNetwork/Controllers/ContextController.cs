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
        protected StudentContext Db { get; set; }
        public ContextController(StudentContext context) => Db = context;

        public async Task<Student> GetCurrentStudentAsync()
        {
            return await Db.Students.FirstAsync(s => s.Login == User.Identity.Name).ConfigureAwait(false);
        }

        public async Task<Student> GetStudentAsync(string name)
        {
            return await Db.Students.FirstAsync(s => s.Login == name).ConfigureAwait(false);
        }

        public Student GetCurrentStudent()
        {
            return Db.Students.First(s => s.Login == User.Identity.Name);
        }

        public Student GetStudent(string name)
        {
            return Db.Students.First(s => s.Login == name);
        }

        public IActionResult GetStudentName()
        {
            return Content(GetCurrentStudent().Name);
        }
    }
}
