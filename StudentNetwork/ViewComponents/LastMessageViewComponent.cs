using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentNetwork.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace StudentNetwork.ViewComponents
{
    public class LastMessageViewComponent : ViewComponent
    {
        private readonly IStringLocalizer<LastMessageViewComponent> _localizer;
        public LastMessageViewComponent(StudentContext context, IStringLocalizer<LastMessageViewComponent> localizer)
        {
            db = context;
            _localizer = localizer;
        }
        private readonly StudentContext db;

        [Authorize]
        public IViewComponentResult Invoke(int chatId)
        {
            var messages = db.Messages
                .Include(m => m.Chat)
                .Include(m => m.Sender)
                .Where(m => m.Chat.Id == chatId);
            if(!messages.Any())
                return Content(_localizer["No messages"]);

            var message = messages.OrderByDescending(m => m.DateTime).First();
            if (message.Sender.Login == User.Identity.Name)
                return Content($"{_localizer["You"]}: {message.Text}");
            else
                return Content($"{message.Sender.FirstName}: {message.Text}");
        }
    }
}
