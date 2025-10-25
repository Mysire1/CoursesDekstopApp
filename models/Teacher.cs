using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("teachers")]
public class Teacher
{
        [Key]
        [Column("teacher_id")]
        public int TeacherId { get; set; }

        [Required]
        [Column("first_name")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Column("last_name")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Column("phone")]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [Column("email")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Column("hire_date")]
        public DateTime HireDate { get; set; } = DateTime.Now;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        
        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
        public virtual ICollection<TeacherLanguage> TeacherLanguages { get; set; } = new List<TeacherLanguage>();
}