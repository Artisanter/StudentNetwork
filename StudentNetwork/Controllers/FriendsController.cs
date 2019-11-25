using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNetwork.Models;

namespace StudentNetwork.Controllers
{
    public class FriendsController : ContextController
    {
        public FriendsController(StudentContext context) : base(context)
        { }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View((await GetCurrentStudentAsync().ConfigureAwait(false)).Friends);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchString)
        {
            IList<Student> students;
            if (String.IsNullOrEmpty(searchString))
                students = await Db.Students.ToListAsync().ConfigureAwait(false);
            students = await Db.Students
                .Where(s => s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToListAsync().ConfigureAwait(false);
            return View(students);
        }

        [Authorize]
        public async void Befriend(int id)
        {
            var sender = await GetCurrentStudentAsync().ConfigureAwait(false);
            var receiver = await Db.Students.FindAsync(id);
            var friendship = await Db.Friendships
                .FirstOrDefaultAsync(f => f.First == sender && f.Second == receiver).ConfigureAwait(false);
            if (friendship is null)
            {
                friendship = new Friendship()
                {
                    First = sender,
                    Second = receiver
                };
                await Db.Friendships.AddAsync(friendship);
                await Db.Friendships.AddAsync(friendship.SetReversed());
            }
            friendship.RaiseStatus();
            await Db.SaveChangesAsync().ConfigureAwait(false);
        }
        [Authorize]
        public async void Unfriend(int id)
        {
            var sender = await GetCurrentStudentAsync().ConfigureAwait(false);
            var receiver = await Db.Students.FindAsync(id);
            var friendship = await Db.Friendships
                .FirstOrDefaultAsync(f => f.First == sender && f.Second == receiver).ConfigureAwait(false);
            if (friendship != null)
            {
                friendship.LowerStatus();
                await Db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}