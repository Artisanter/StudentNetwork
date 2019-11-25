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

        public async Task<IActionResult> Send(int id, string text)
		{
            if (String.IsNullOrEmpty(text))
                return View();
            var chat = Db.Chats.Find(id);
            var message = new Message()
            {
                DateTime = DateTime.Now,
                Sender = await GetCurrentStudentAsync().ConfigureAwait(false),
                Chat = chat
            };
            chat.Messages.Add(message);
			Db.Messages.Add(message);
			await Db.SaveChangesAsync().ConfigureAwait(false);
			return View(message);
		}
    }
}