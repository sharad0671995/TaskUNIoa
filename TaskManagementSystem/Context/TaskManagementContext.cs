using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Context
{
    public class TaskManagementContext :DbContext
    {

        public DbSet<UserTask> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        public TaskManagementContext(DbContextOptions<TaskManagementContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the relationship between User and UserTask
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.CreatedBy) // A task has one CreatedBy user
                .WithMany(u => u.Tasks) // A user can have many tasks
                .HasForeignKey(ut => ut.CreatedById) // The foreign key in UserTask
                .OnDelete(DeleteBehavior.Restrict); // Optional: Define the delete behavior (Restrict/SetNull)

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.AssignedTo) // A task has one AssignedTo user
                .WithMany(u => u.AssignedTasks) // A user can have many tasks assigned to them
                .HasForeignKey(ut => ut.AssignedToId) // The foreign key in UserTask
                .OnDelete(DeleteBehavior.Restrict); // Optional: Define the delete behavior (Restrict/SetNull)
        }
    }
}
