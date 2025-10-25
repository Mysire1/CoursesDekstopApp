using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("teacher_languages")]
public class TeacherLanguage
{
    [Key]
    [Column("teacher_language_id")]
    public int TeacherLanguageId { get; set; }
    
    [Column("teacher_id")]
    public int TeacherId { get; set; }

    [ForeignKey("TeacherId")]
    public virtual Teacher Teacher { get; set; } = null!;

    [Column("language_id")]
    public int LanguageId { get; set; }

    [ForeignKey("LanguageId")]
    public virtual Language Language { get; set; } = null!;
}