using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNetwork.Models;

namespace StudentNetwork.Controllers
{
    public class ChatController : ContextController
    {
        public ChatController(StudentContext context) : base(context)
        { }

        public async Task<IActionResult> Send(Message model)
		{
			model.DateTime = DateTime.Now;
			model.Sender = await db.Students.FirstAsync(s => s.Login == User.Identity.Name);
			model.Chat.Send(model);
			db.Messages.Add(model);
			await db.SaveChangesAsync();
			return View(model);
		}
    }
}