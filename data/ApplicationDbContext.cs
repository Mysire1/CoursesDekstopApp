using DefaultNamespace;
using Microsoft.EntityFrameworkCore;

namespace CoursesDekstopApp.data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }
        
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentDeferral> PaymentDeferrals { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TeacherLanguage> TeacherLanguages { get; set; }
    }
}