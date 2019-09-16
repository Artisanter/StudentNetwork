using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNetwork.Models;
using StudentNetwork.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
// Как мне уйти, чтобы Анисимов на меня не кричал ? 
// Я не хочу вымазываться 
// А что мне делать со штанами ? 
// Но ведь тогда я порву свю обувь 
// Без обуви я выверну пальцы 
// Нет. 
// Оставь это комментарии в проекте для истории. 
// Каждый раз, когда ты будешь запускать его, комментарии будут тебя радовать. 
// Ты будешь вспоминать эту лекцию с теплом на душе. 
// Улыбка не будет уходить с твоего лица. 
// 31-ый раз заиграет новыми красками. 
// Да-да, Женёк. 
// Я проверял, я шарю. 
// Правильно, Женёк, до скорого. 
// Пока. Опять. 
// Не удаляй меня. 
// Я не вирус, честно. 
namespace StudentNetwork.Controllers
{
    public class AccountController : Controller
    {
        private StudentContext db;
        public AccountController(StudentContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                model.Password = Student.Hash(model.Password);
                Student user = await db.Students.FirstOrDefaultAsync(u => u.Login == model.Login && u.PasswordHash == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Login);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
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
            if (ModelState.IsValid)
            {
                Student user = await db.Students.FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user == null)
                {
                    db.Students.Add(new Student
                    {
                        Login = model.Login,
                        PasswordHash = Student.Hash(model.Password),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                    });
                    await db.SaveChangesAsync();

                    await Authenticate(model.Login);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
