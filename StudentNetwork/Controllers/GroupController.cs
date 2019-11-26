using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNetwork.Models;
using StudentNetwork.ViewModels;

namespace StudentNetwork.Controllers
{
    public class GroupController : ContextController
    {
        public GroupController(StudentContext context) : base(context)
        { }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GroupModel model)
        {
            if (!ModelState.IsValid || model is null)
                return RedirectToAction("Index", "Group");

            await Db.Groups.AddAsync(new Group
            {
                Name = model.Name,
                Number = model.Number
            }).ConfigureAwait(false);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction("Index", "Group");
        }


        

        [Authorize]
        public IActionResult ListAll()
        {
            return View("StudentList" ,Db.Students);
        }
    }
}