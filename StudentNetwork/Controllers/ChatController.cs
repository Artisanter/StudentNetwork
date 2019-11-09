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
            if (model is null)
                return View();
			model.DateTime = DateTime.Now;
			model.Sender = await GetCurrentStudentAsync().ConfigureAwait(false);
			model.Chat.Send(model);
			Db.Messages.Add(model);
			await Db.SaveChangesAsync().ConfigureAwait(false);
			return View(model);
		}
    }
}