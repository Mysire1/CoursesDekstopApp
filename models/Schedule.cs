using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("schedules")]
public class Schedule
{
    [Key]
    [Column("schedule_id")]
    public int ScheduleId { get; set; }

    [Column("group_id")]
    public int GroupId { get; set; }

    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;

    [Column("classroom_id")]
    public int ClassroomId { get; set; }

    [ForeignKey("ClassroomId")]
    public virtual Classroom Classroom { get; set; } = null!;

    [Column("day_of_week")]
    public DayOfWeek DayOfWeek { get; set; }

    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan EndTime { get; set; }
}