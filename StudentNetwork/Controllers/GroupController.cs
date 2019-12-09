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

        [Authorize]
        public IActionResult Index(int id)
        {
            var student = GetCurrentStudent();
            var group = Db.Groups
                .Include(g=>g.Image)
                .FirstOrDefault(g => g.Id == id);
            var membership = Db.Memberships
                .Include(m => m.Group)
                    .ThenInclude(g => g.Chat)
                        .ThenInclude(c => c.Messages)
                            .ThenInclude(msg => msg.Sender)
                    .ThenInclude(g => g.Image)
                .FirstOrDefault(m => m.Student == student && m.Group == group);
            if(membership is null)
            {
                membership = new Membership()
                {
                    Group = group,
                    Status = Membership.MemberStatus.NotSubscribed
                };
            }

            return View(membership);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(GroupModel model)
        {
            if (!ModelState.IsValid || model is null)
                return RedirectToAction("Index", "Group");

            var group = new Group
            {
                Name = model.Name,
                Number = model.Number,
                Image = Db.Images.First(i => i.Name == "Group Default")
            };
            var membership = new Membership()
            {
                Group = group,
                Student = GetCurrentStudent(),
                Status = Membership.MemberStatus.Approved,
                Role = Db.Roles.First(r => r.Name == "Admin")
            };
            await Db.Groups.AddAsync(group).ConfigureAwait(false);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction("Index", new { group.Id });
        }

        [Authorize]
        public async Task<IActionResult> Subscribe(int id)
        {
            var student = GetCurrentStudent();
            var group = Db.Groups.Find(id);
            var membership = Db.Memberships.FirstOrDefault(m => m.Student == student && m.Group == group);
            if (membership is null)
            {
                membership = new Membership()
                {
                    Group = group,
                    Student = student,
                    Status = Membership.MemberStatus.Subscribed,
                    Role = Db.Roles.First(r => r.Name == "User")
                };
                Db.Memberships.Add(membership);
            }
            else if (membership.Status == Membership.MemberStatus.NotSubscribed)
                membership.Status = Membership.MemberStatus.Subscribed;

            await Db.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index", new { group.Id });
        }
        [Authorize]
        public async Task<IActionResult> Unsubscribe(int id)
        {
            var student = GetCurrentStudent();
            var group = Db.Groups.Find(id);
            var membership = Db.Memberships.FirstOrDefault(m => m.Student == student && m.Group == group);
            if (membership != null && membership.Status != Membership.MemberStatus.Banned)
                membership.Status = Membership.MemberStatus.NotSubscribed;

            await Db.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index", new { group.Id });
        }
        [Authorize]
        public async Task<IActionResult> Ban(int groupId, int userId)
        {
            if (!IsAdmin(groupId))
                return RedirectToAction("Mates", new { groupId });
            var membership = GetMembership(userId, groupId);
            if (membership != null && membership.Role.Name != "Admin")
                membership.Status = Membership.MemberStatus.Banned;

            await Db.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("AdminView", new { id = groupId });
        }
        [Authorize]
        public async Task<IActionResult> GiveAdmin(int groupId, int userId)
        {
            if (!IsAdmin(groupId))
                return RedirectToAction("Mates", new { groupId });
            var membership = GetMembership(userId, groupId);
            if (membership != null)
                membership.Role = Db.Roles.First(r => r.Name == "Admin");

            await Db.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("AdminView", new { id = groupId });
        }
        [Authorize]
        public async Task<IActionResult> TakeAdmin(int groupId, int userId)
        {
            if (!IsAdmin(groupId))
                return RedirectToAction("Mates", new { groupId });
            var membership = GetMembership(userId, groupId);
            if (membership != null)
                membership.Role = Db.Roles.First(r => r.Name == "User");

            await Db.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("AdminView", new { id = groupId });
        }
        [Authorize]
        public async Task<IActionResult> Unban(int groupId, int userId)
        {
            if (!IsAdmin(groupId))
                return RedirectToAction("Mates", new { groupId });
            var membership = GetMembership(userId, groupId);
            if (membership != null && membership.Status == Membership.MemberStatus.Subscribed)
                membership.Status = Membership.MemberStatus.Subscribed;

            await Db.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("AdminView", new { id = groupId });
        }
        [Authorize]
        public async Task<IActionResult> Apply(int groupId, int userId)
        {
            ViewBag.GroupId = groupId;
            if (!IsAdmin(groupId)) 
                return RedirectToAction("Mates", new { groupId });
            var membership = GetMembership(userId, groupId);
            if (membership != null && membership.Status == Membership.MemberStatus.Subscribed)
                membership.Status = Membership.MemberStatus.Approved;

            await Db.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("AdminView", new { id = groupId });
        }
        [Authorize]
        public async Task<IActionResult> List()
        {
            var student = await GetCurrentStudentAsync().ConfigureAwait(false);
            return View(Db.Memberships
                .Include(m=>m.Group)
                    .ThenInclude(g=>g.Image)
                .Where(m=>m.Student==student)
                .Select(m=>m.Group));
        }

        public async Task<IActionResult> Search(string searchString)
        {
            ICollection<Group> groups;
            if (String.IsNullOrEmpty(searchString))
                groups = await Db.Groups
                    .Include(m => m.Image)
                    .ToListAsync().ConfigureAwait(false);
            else
                groups = await Db.Groups
                    .Where(s => s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync().ConfigureAwait(false);
            return View(groups);
        }
        [Authorize]
        public async Task<IActionResult> Mates(int id)
        {
            var student = await GetCurrentStudentAsync().ConfigureAwait(false);
            ViewBag.GroupId = id;
            ViewBag.IsAdmin = IsAdmin(id, student);
            var mates = Db.Memberships
                .Include(m => m.Student)
                    .ThenInclude(s => s.Image)
                .Where(m => m.Group.Id == id && m.Status == Membership.MemberStatus.Approved)
                .Select(m => m.Student)
                .ToList();
            return View(mates);
        }

        [Authorize]
        public async Task<IActionResult> AdminView(int id)
        {
            ViewBag.GroupId = id;
            var admin = await GetCurrentStudentAsync().ConfigureAwait(false);
            if (!IsAdmin(id, admin))
                return RedirectToAction("Mates", new { id });
            var memberships = await Db.Memberships
                .Include(m => m.Role)
                .Include(m => m.Student)
                    .ThenInclude(s => s.Image)
                .Where(m => m.Group.Id == id && m.Status != Membership.MemberStatus.NotSubscribed && m.Student.Id != admin.Id)
                .OrderBy(m => m.Status)
                .ToListAsync()
                .ConfigureAwait(true);
            return View(memberships);
        }

        private bool IsAdmin(int groupId)
            => IsAdmin(groupId, GetCurrentStudent());
        private bool IsAdmin(int groupId, Student user)
        {
            var membership = Db.Memberships
                .Include(m => m.Role)
                .FirstOrDefault(m => m.Student == user && m.Group.Id == groupId);
            return membership != null && membership.Role.Name == "Admin";
        }

        private Membership GetMembership(int userId, int groupId)
        {
            var user = Db.Students.Find(userId);
            var group = Db.Groups.Find(groupId);
            return Db.Memberships.FirstOrDefault(m => m.Student == user && m.Group == group);
        }
    }
}