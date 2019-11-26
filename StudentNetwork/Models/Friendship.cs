using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.Models
{
    public class Friendship
    {
        public Friendship() { }
        public Friendship(Student first, Student second)
        {
            First = first;
            Second = second;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Editable(false), Required]
        public virtual Student First { get; set; }
        [Editable(false), Required]
        public virtual Student Second { get; set; }
        public virtual Chat Chat { get; set; }
        public bool IsBetween(Student s1, Student s2)
            => (Second == s1 && First == s2) || (First == s1 && Second == s2);
        public virtual FriendshipStatus Status { get; set; }
    }
}
public enum FriendshipStatus
{
    Stranger = 0,
    Subscriber = 1,
    Friend = 2
}
