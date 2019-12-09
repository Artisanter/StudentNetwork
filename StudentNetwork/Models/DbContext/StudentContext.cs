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
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Role> Roles { get; set; }

        public StudentContext(DbContextOptions<StudentContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Chat);

            modelBuilder.Entity<Friendship>()
                .HasOne(fs => fs.First)
                .WithMany(u => u.Friendships)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Membership>()
                .HasOne(fs => fs.Student)
                .WithMany(u => u.Memberships)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
                .Property("ImageId")
                .HasDefaultValue(1);

            modelBuilder.Entity<Student>()
                .Property("RoleId")
                .HasDefaultValue(2);

            modelBuilder.Entity<Role>()
                .HasIndex(x => x.Name)
                .IsUnique();


            base.OnModelCreating(modelBuilder);
        }
    }
}
