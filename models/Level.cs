using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("levels")]
public class Level
{
    [Key]
    [Column("level_id")]
    public int LevelId { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("level_order")]
    public int LevelOrder { get; set; }

    [Column("cost")]
    public decimal Cost { get; set; }

    [Column("duration_in_months")]
    public int DurationInMonths { get; set; } = 3;
    
    [Column("language_id")]
    public int LanguageId { get; set; }

    [ForeignKey("LanguageId")]
    public virtual Language Language { get; set; } = null!;
    
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}