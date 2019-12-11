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
        [Route("friends")]
        public IActionResult Index()
        {
            return View(Db.Friendships
                .Include(fs => fs.Second)
                    .ThenInclude(s=>s.Image)
                .Where(fs => fs.Status == FriendshipStatus.Friend && fs.First.Login == User.Identity.Name));
        }
        [Route("friends/search")]
        public async Task<IActionResult> Search(string searchString)
        {
            IList<Student> students;
            if (String.IsNullOrEmpty(searchString))
                students = await Db.Students.Include(s => s.Image).ToListAsync().ConfigureAwait(false);
            else
                students = await Db.Students
                    .Include(s => s.Image)
                    .Where(s => s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync().ConfigureAwait(false);
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetCurrentStudentAsync().ConfigureAwait(false);
                students.Remove(user);
            }

            return View(students);
        }

        [Authorize]
        public async Task<IActionResult> Befriend(int id)
        {
            var sender = await GetCurrentStudentAsync().ConfigureAwait(false);
            var receiver = await Db.Students.FirstAsync(s => s.Id == id).ConfigureAwait(false);
            
            var friendship1 = await Db.Friendships.FirstOrDefaultAsync(f =>f.First == sender && f.Second == receiver)
                .ConfigureAwait(false);
            if (friendship1 is null)
            {
                friendship1 = new Friendship(sender, receiver);
                await Db.Friendships.AddAsync(friendship1);
            }
          
            if(friendship1.Status != FriendshipStatus.Stranger)
                return RedirectToAction("Index", "Account", new { id });

            friendship1.Status = FriendshipStatus.Subscriber;
            var friendship2 = await Db.Friendships.FirstOrDefaultAsync(f => f.First == receiver && f.Second == sender)
                .ConfigureAwait(false);
            if (friendship2 is null)
            {
                friendship2 = new Friendship(receiver, sender);
                await Db.Friendships.AddAsync(friendship2);
            }
            else if(friendship2.Status == FriendshipStatus.Subscriber)
            {
                friendship1.Status = friendship2.Status = FriendshipStatus.Friend;
            }
            await Db.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index", "Account", new { id });

        }
        [Authorize]
        public async Task<IActionResult> Unfriend(int id)
        {
            var sender = await GetCurrentStudentAsync().ConfigureAwait(false);
            var receiver = await Db.Students.FirstAsync(s => s.Id == id).ConfigureAwait(false);
            var friendship1 = await Db.Friendships.FirstOrDefaultAsync(f => f.First == sender && f.Second == receiver)
                .ConfigureAwait(false);
            if (friendship1 is null || friendship1.Status == FriendshipStatus.Stranger)
                return RedirectToAction("Index", "Account", new { id });

            if (friendship1.Status == FriendshipStatus.Friend)
            {
                var friendship2 = await Db.Friendships.FirstAsync(f => f.First == receiver && f.Second == sender)
                    .ConfigureAwait(false);
                friendship2.Status = FriendshipStatus.Subscriber;
            }

            friendship1.Status = FriendshipStatus.Stranger;
            await Db.SaveChangesAsync().ConfigureAwait(false);            
            return RedirectToAction("Index", "Account", new { id });
        }
        const int pageSize = 3;
        [Authorize]
        [Route("chat/{id}")]
        [Route("chat/{id}/{page}")]
        public IActionResult Chat(int id, int page)
        {
            var fs = Db.Friendships
                .Include(fs => fs.Chat)
                    .ThenInclude(c => c.Messages)
                        .ThenInclude(msg => msg.Sender)
                .Include(fs => fs.Second)
                    .ThenInclude(s=>s.Image)
                .First(fs => fs.Id == id);

            var messages = fs.Chat.Messages
                .OrderByDescending(m => m.DateTime)
                .Skip(page * pageSize)
                .Take(pageSize);

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ChatPartial", messages);
            }
            return View((fs, messages));
        }

    }
}