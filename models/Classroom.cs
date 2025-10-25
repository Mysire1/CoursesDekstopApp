using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultNamespace;

[Table("classrooms")]
public class Classroom
{
    [Key]
    [Column("classroom_id")]
    public int ClassroomId { get; set; }

    [Required]
    [Column("room_number")]
    [MaxLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Column("capacity")]
    public int Capacity { get; set; }
    
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}