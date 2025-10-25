using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("exams")]
public class Exam
{
    [Key]
    [Column("exam_id")]
    public int ExamId { get; set; }

    [Column("level_id")]
    public int LevelId { get; set; }

    [ForeignKey("LevelId")]
    public virtual Level Level { get; set; } = null!;

    [Column("exam_date")]
    public DateTime ExamDate { get; set; }

    [Required]
    [Column("exam_type")]
    [MaxLength(50)]
    public string ExamType { get; set; } = "Final";
    
    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
}