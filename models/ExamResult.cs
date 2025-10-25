using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("exam_results")]
public class ExamResult
{
    [Key]
    [Column("exam_result_id")]
    public int ExamResultId { get; set; }

    [Column("exam_id")]
    public int ExamId { get; set; }

    [ForeignKey("ExamId")]
    public virtual Exam Exam { get; set; } = null!;

    [Column("student_id")]
    public int StudentId { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student Student { get; set; } = null!;

    [Column("grade")]
    public decimal Grade { get; set; }

    [NotMapped]
    public bool IsPassed => Grade >= 50;

    [Column("certificate_issued")]
    public bool CertificateIssued { get; set; } = false;

    [Column("issue_date")]
    public DateTime? IssueDate { get; set; }
}