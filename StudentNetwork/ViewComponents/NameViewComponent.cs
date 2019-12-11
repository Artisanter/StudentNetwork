using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentNetwork.Models;

namespace StudentNetwork.ViewComponents
{
    public class NameViewComponent : ViewComponent
    {
        public NameViewComponent(StudentContext context) => db = context;
        private readonly StudentContext db;

        [Authorize]
        public IViewComponentResult Invoke()
        {
            var name = db.Students.FirstOrDefault(s => s.Login == User.Identity.Name)?.Name;
            return Content(name);
        }
    }
}
