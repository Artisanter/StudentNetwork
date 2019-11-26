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
        public AccessType Type { get; set; }
        public ICollection<Message> Messages { get; } = new List<Message>();

        public enum AccessType
        {
            Private,
            Public
        }        
    }
}
