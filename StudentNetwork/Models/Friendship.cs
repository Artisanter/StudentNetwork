using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork.Models
{
    public class Friendship
    {
        public Friendship() { }
        [Key]
        public int Id { get; set; }
        public Student First { get; set; }
        public Student Second { get; set; }
        public Friendship Reversed { get; set; }
        public Friendship SetReversed()
        {
            return Reversed = new Friendship()
            {
                First = Second,
                Second = First,
                Reversed = this,
                Status = Status
            };
        }
        public bool IsBetween(Student s1, Student s2)
            => (Second == s1 && First == s2) || (First == s1 && Second == s2);
        public FriendshipStatus Status { get; set; }
        public void RaiseStatus()
        {
            switch (Status)
            {
                case FriendshipStatus.Stranger:
                    Status = FriendshipStatus.Requested;
                    Reversed.Status = FriendshipStatus.HasRequest;
                    break;
                case FriendshipStatus.HasRequest:
                    Status = Reversed.Status = FriendshipStatus.Friend;
                    break;
            }
        }
        public void LowerStatus()
        {
            switch (Status)
            {
                case FriendshipStatus.Friend:
                    Status = FriendshipStatus.HasRequest;
                    Reversed.Status = FriendshipStatus.Requested;
                    break;
                case FriendshipStatus.Requested:
                    Status = Reversed.Status = FriendshipStatus.Stranger;
                    break;
            }
        }
    }
}
public enum FriendshipStatus
{
    Stranger = 0,
    HasRequest = 1,
    Requested = 1,
    Friend = 2
}
