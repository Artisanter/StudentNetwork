using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNetwork.Models;

namespace StudentNetwork.Controllers
{
    public class ChatController : ContextController
    {
        public ChatController(StudentContext context, IHubContext<ChatHub> hubContext) : base(context)
        {
            _hubContext = hubContext;
        }
        private readonly IHubContext<ChatHub> _hubContext;
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var chat = await Db.Chats
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .FirstAsync(c => c.Id == id)
                .ConfigureAwait(false);
            return Redirect(Request.Headers["Referer"].ToString()); ;
        }

        [HttpPost]
        public async Task<IActionResult> Send(int id, string text)
        {
            if (String.IsNullOrEmpty(text))
                return RedirectToAction("Index", new { id });
            var chat = Db.Chats.Find(id);
            var message = new Message()
            {
                DateTime = DateTime.Now,
                Sender = await GetCurrentStudentAsync().ConfigureAwait(false),
                Chat = chat,
                Text = text                
            };
            Db.Messages.Add(message);
            await Db.SaveChangesAsync().ConfigureAwait(false);
            //await _hubContext.Clients.All.SendAsync("Send", message).ConfigureAwait(false);
            return RedirectToAction("Index", new { id }); ;
        }
        public async Task<IActionResult> Private(int id)
        {
            var sender = await GetCurrentStudentAsync().ConfigureAwait(false);
            var receiver = await Db.Students.FirstAsync(s => s.Id == id).ConfigureAwait(false);

            var friendship1 = await Db.Friendships
                .Include(fs => fs.Chat)
                .FirstOrDefaultAsync(f => f.First == sender && f.Second == receiver)
                .ConfigureAwait(false);
            if (friendship1 is null)
            {
                friendship1 = new Friendship(sender, receiver);
                await Db.Friendships.AddAsync(friendship1);
            }
            if (friendship1.Chat is null)
            {
                var friendship2 = await Db.Friendships
                    .Include(fs => fs.Chat)
                    .FirstOrDefaultAsync(f => f.First == receiver && f.Second == sender)
                    .ConfigureAwait(false);
                if (friendship2 is null)
                {
                    friendship2 = new Friendship(receiver, sender);
                    await Db.Friendships.AddAsync(friendship2);
                }
                friendship1.Chat = friendship2.Chat = new Chat();
                await Db.SaveChangesAsync().ConfigureAwait(false);
            }
            return RedirectToAction("Chat", "Friends", new { friendship1.Id });
        }
        [Route("im")]
        public async Task<IActionResult> List()
        {
            var user = await GetCurrentStudentAsync().ConfigureAwait(false);
            return View(Db.Friendships
                .Include(fs => fs.Second)
                .ThenInclude(s => s.Image)
                .Include(fs => fs.Chat)
                .Where(fs => fs.First == user && fs.Chat != null));
        }
    }
}