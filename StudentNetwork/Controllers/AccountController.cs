using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNetwork.Models;
using StudentNetwork.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentNetwork.Controllers
{
    public class AccountController : ContextController
    {
        public AccountController(StudentContext context) : base(context)
        { }

        [Authorize]
        public IActionResult Self()
        {
            var student = Db.Students.Include(s => s.Image).First(s => s.Login == User.Identity.Name);
            ViewData["Title"] = student.Name;
            return View(student);
        }
        public IActionResult Index(int id)
        {
            var student = Db.Students.Include(s => s.Image).First(s => s.Id == id);
            ViewData["Title"] = student.Name;
            if (User.Identity.IsAuthenticated)
            {
                var fs = Db.Friendships.FirstOrDefault(f => f.First.Login == User.Identity.Name && f.Second.Id == id);
                ViewBag.FriendshipStatus = fs?.Status ?? FriendshipStatus.Stranger;
            }
            return View(student);
        }

        [HttpGet]
        public ActionResult SetProfilePicture()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SetProfilePicture(Image img, IFormFile uploadImage)
        {
            if (ModelState.IsValid && uploadImage != null)
            {
                byte[] data = null;
                using (var binaryReader = new BinaryReader(uploadImage.OpenReadStream()))
                {
                    data = binaryReader.ReadBytes((int)uploadImage.Length);
                }
                img.Bytes = data;

                Db.Images.Add(img);
                GetCurrentStudent().Image = img;
                Db.SaveChanges();

                return RedirectToAction("Self");
            }
            return View(img);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditUser()
        {
            var student = await GetCurrentStudentAsync().ConfigureAwait(false);
            var model = new UserEditModel()
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                Login = student.Login
            };
            
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditUser(UserEditModel model)
        {
            if (ModelState.IsValid && !(model is null))
            {
                var student = await GetEntireCurrentStudentAsync().ConfigureAwait(false);
                student.Login = model.Login;
                student.FirstName = model.FirstName;
                student.LastName = model.LastName;
                await Db.SaveChangesAsync().ConfigureAwait(false);
                await Authenticate(model.Login, student.Role.Name).ConfigureAwait(true);
            }
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(PasswordChangeModel model)
        {
            if (ModelState.IsValid && !(model is null))
            {
                var student = await GetCurrentStudentAsync().ConfigureAwait(false);
                student.Password = model.NewPassword;
                _ = Db.SaveChangesAsync();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid && !(model is null))
            {
                var student = await Db.Students
                    .Include(s => s.Role)
                    .FirstAsync(s => s.Login == model.Login)
                    .ConfigureAwait(false);
                if (student.PasswordHash == Student.Hash(model.Password))
                {
                    await Authenticate(model.Login, student.Role.Name).ConfigureAwait(false);
                    return RedirectToAction("Self");
                }
                ModelState.AddModelError("", "Неверный пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid || model is null)
                return View(model);

            await Db.Students.AddAsync(new Student
            {
                Login = model.Login,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = Db.Roles.First(r => r.Name == "User")
            }).ConfigureAwait(false);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            await Authenticate(model.Login, "User").ConfigureAwait(false);

            return RedirectToAction("Self");
        }

        private async Task Authenticate(string login, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id)).ConfigureAwait(false);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            return RedirectToAction("Login", "Account");
        }


        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsExist(string login)
        {
            return Json(await Db.Students.AnyAsync(s => s.Login == login).ConfigureAwait(false));
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsAbsent(string login)
        {
            return Json(await Db.Students.AllAsync(s => s.Login != login).ConfigureAwait(false));
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEditable(string login)
        {
            if ((await GetCurrentStudentAsync().ConfigureAwait(false)).Login == login)
                return Json(true);
            return Json(await Db.Students.AllAsync(s => s.Login != login).ConfigureAwait(false));
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> ConfirmPassword(string password)
        {
            return Json((await GetCurrentStudentAsync().ConfigureAwait(false)).PasswordHash == Student.Hash(password));
        }        
    }
}
