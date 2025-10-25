using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("payments")]
public class Payment
{
    [Key]
    [Column("payment_id")]
    public int PaymentId { get; set; }

    [Column("student_id")]
    public int StudentId { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student Student { get; set; } = null!;

    [Column("enrollment_id")]
    public int EnrollmentId { get; set; }

    [ForeignKey("EnrollmentId")]
    public virtual Enrollment Enrollment { get; set; } = null!;

    [Column("amount")]
    public decimal Amount { get; set; }
    [Column("payment_date")]
    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [Column("payment_method")]
    [MaxLength(50)]
    public string? PaymentMethod { get; set; }
}