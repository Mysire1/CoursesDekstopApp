using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("groups")]
public class Group
{
    [Key]
    [Column("group_id")]
    public int GroupId { get; set; }

    [Required]
    [Column("group_name")]
    [MaxLength(150)]
    public string GroupName { get; set; } = string.Empty;

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }

    [Column("min_students")]
    public int MinStudents { get; set; } = 5;

    [Column("max_students")]
    public int MaxStudents { get; set; } = 20;

    [Column("level_id")]
    public int LevelId { get; set; }
    [ForeignKey("LevelId")]
    public virtual Level Level { get; set; } = null!;

    [Column("teacher_id")]
    public int TeacherId { get; set; }

    [ForeignKey("TeacherId")]
    public virtual Teacher Teacher { get; set; } = null!;
    
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}