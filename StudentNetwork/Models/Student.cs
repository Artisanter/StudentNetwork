using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentNetwork.Models
{
    public class Student
    { 
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name => $"{FirstName} {LastName}";
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public Image Image { get; set; }
        public string Password { set => PasswordHash = Hash(value); }
        public virtual ICollection<Membership> Memberships { get; set; } = new HashSet<Membership>();
        public Role Role { get; set; }
        public int? RoleId { get; set; }
        public virtual  ICollection<Friendship> Friendships { get; set; } = new HashSet<Friendship>();
        [NotMapped]
        public IEnumerable<Student> Friends => Friendships.Select(f => f.Second);



        private static readonly SHA256 sHA = SHA256.Create();
        static public string Hash(string password)
        {
            var hash = sHA.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}
