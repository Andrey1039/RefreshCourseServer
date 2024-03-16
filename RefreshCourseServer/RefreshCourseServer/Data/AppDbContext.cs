using Microsoft.EntityFrameworkCore;
using RefreshCourseServer.Models.Database;

namespace RefreshCourseServer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<WorkLoad> WorkLoads { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<LessonType> LessonTypes { get; set; }
        public DbSet<Speciality> Specialties { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
