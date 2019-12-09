using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.Models
{
    public class Membership
    {
        [Key]
        public int Id { get; set; }
        public Student Student { get; set; }
        public Group Group { get; set; }
        [Required]
        public Role Role { get; set; }
        public int? RoleId { get; set; }
        public MemberStatus Status { get; set; } = MemberStatus.NotSubscribed;
        public enum MemberStatus
        {
            Banned = 0,
            NotSubscribed,
            Subscribed,
            Approved
        }
    }
}
