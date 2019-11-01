using Microsoft.EntityFrameworkCore;
using StudentNetwork.ViewModels;

namespace StudentNetwork.Models
{
	public class StudentContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public StudentContext(DbContextOptions<StudentContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<StudentNetwork.ViewModels.GroupModel> GroupModel { get; set; }
    }
}
