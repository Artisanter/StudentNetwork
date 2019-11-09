using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        public ICollection<Message> Messages { get; } = new List<Message>();

        public IEnumerator<Message> GetEnumerator()
        {
            return Messages.GetEnumerator();
        }

        public void Send(Message message)
        {
            if (message is null)
                return;
            Messages.Add(message);
            message.Chat = this;
        }

    }
}
