using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                return View(model);

            await Db.Groups.AddAsync(new Group
            {
                Name = model.Name,
                Number = model.Number
            }).ConfigureAwait(false);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction("Index", "Group");
        }

        public async Task<IActionResult> Enter(GroupModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var group = await Db.Groups.FirstAsync(g => g.Number == model.Number).ConfigureAwait(false);
            var student = GetCurrentStudent();
            group.Students.Add(student);
            student.Group = group;
            await Db.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction("Index", "Group");
        }

    }
}