using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("enrollments")]
public class Enrollment
{
    [Key]
    [Column("enrollment_id")]
    public int EnrollmentId { get; set; }

    [Column("student_id")]
    public int StudentId { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student Student { get; set; } = null!;

    [Column("group_id")]
    public int GroupId { get; set; }

    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;

    [Column("enrollment_date")]
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;

    [Column("final_cost")]
    public decimal FinalCost { get; set; }
    
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    
    public virtual ICollection<PaymentDeferral> PaymentDeferrals { get; set; } = new List<PaymentDeferral>();
}