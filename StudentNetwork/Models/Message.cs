using System;
using System.ComponentModel.DataAnnotations;

namespace StudentNetwork.Models
{
	public class Message
    {
        [Key]
        public int Id { get; set; }
        public Student Sender { get; set; }
        public DateTime DateTime { get; set; }
		public Chat Chat { get; set; }
		public string Text { get; set; }
    }
}
