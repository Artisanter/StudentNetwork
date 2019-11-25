using Microsoft.EntityFrameworkCore;
using StudentNetwork.ViewModels;

namespace StudentNetwork.Models
{
	public class StudentContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public StudentContext(DbContextOptions<StudentContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Chat>().HasMany(c => c.Messages).WithOne(m => m.Chat);

            modelBuilder.Entity<Friendship>()
                .HasOne(fs => fs.First)
                .WithMany(u => u.Friendships);
        }
    }
}
