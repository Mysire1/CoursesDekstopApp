using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("students")]
public class Student
{
    [Key]
    [Column("student_id")]
    public int StudentId { get; set; }

    [Required]
    [Column("first_name")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Column("last_name")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Column("date_of_birth")]
    public DateTime DateOfBirth { get; set; }

    [Column("phone")]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [Column("email")]
    [MaxLength(100)]
    public string? Email { get; set; }

    [Column("registration_date")]
    public DateTime RegistrationDate { get; set; } = DateTime.Now;

    [Column("has_discount")]
    public bool HasDiscount { get; set; } = false;

    [Column("discount_percentage")]
    public decimal DiscountPercentage { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [NotMapped]
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
    
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}