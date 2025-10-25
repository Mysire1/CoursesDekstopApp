using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("languages")]
public class Language
{
    [Key]
    [Column("language_id")]
    public int LanguageId { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public virtual ICollection<Level> Levels { get; set; } = new List<Level>();
    
    public virtual ICollection<TeacherLanguage> TeacherLanguages { get; set; } = new List<TeacherLanguage>();
}