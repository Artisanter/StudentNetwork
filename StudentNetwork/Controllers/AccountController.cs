using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNetwork.Models;
using StudentNetwork.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentNetwork.Controllers
{
    public class AccountController : ContextController
    {
        public AccountController(StudentContext context) : base(context)
        { }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
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
        public async Task<IActionResult> Edit(UserEditModel model)
        {
            if (ModelState.IsValid && !(model is null))
            {
                var student = await GetCurrentStudentAsync().ConfigureAwait(false);
                student.Login = model.Login;
                student.FirstName = model.FirstName;
                student.LastName = model.LastName;
                _ = Db.SaveChangesAsync();
            }
            return View(model);
        }

        [HttpGet]
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
                var student = await GetStudentAsync(model.Login).ConfigureAwait(false);
                if (student.PasswordHash == Student.Hash(model.Password))
                {
                    await Authenticate(model.Login).ConfigureAwait(false);
                    return RedirectToAction("Index", "Home");
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
                LastName = model.LastName
            }).ConfigureAwait(false);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            await Authenticate(model.Login).ConfigureAwait(false);

            return RedirectToAction("Index", "Home");
        }

        private async Task Authenticate(string login)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login)
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
        public async Task<IActionResult> IsAvaible(string login)
        {
            return Json(!await Db.Students.AnyAsync(s => s.Login == login).ConfigureAwait(false));
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> ConfirmPassword(string password)
        {
            return Json((await GetCurrentStudentAsync().ConfigureAwait(false)).PasswordHash == Student.Hash(password));
        }        
    }
}
