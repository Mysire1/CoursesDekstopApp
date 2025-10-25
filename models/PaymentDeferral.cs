using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("payment_deferrals")]
public class PaymentDeferral
{
    [Key]
    [Column("payment_deferral_id")]
    public int PaymentDeferralId { get; set; }

    [Column("student_id")]
    public int StudentId { get; set; }
    
    [ForeignKey("StudentId")]
    public virtual Student Student { get; set; } = null!;

    [Column("enrollment_id")]
    public int EnrollmentId { get; set; }

    [ForeignKey("EnrollmentId")]
    public virtual Enrollment Enrollment { get; set; } = null!;

    [Column("deferral_date")]
    public DateTime DeferralDate { get; set; } = DateTime.Now;

    [Column("deferred_amount")]
    public decimal DeferredAmount { get; set; }

    [Column("new_due_date")]
    public DateTime NewDueDate { get; set; }

    [Column("reason")]
    [MaxLength(500)]
    public string? Reason { get; set; }
}