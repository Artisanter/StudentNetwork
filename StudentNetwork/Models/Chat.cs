using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.Models
{
    public class Chat : IEnumerable<Message>
    {
        [Key]
        public int Id { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();

        public IEnumerator<Message> GetEnumerator()
        {
            return Messages.GetEnumerator();
        }

        public void Send(Message message)
        {
            Messages.Add(message);
            message.Chat = this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Messages.GetEnumerator();
        }
    }
}
