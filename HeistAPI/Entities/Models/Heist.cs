using HeistAPI.Entities.Enum;

namespace HeistAPI.Entities.Models
{
    public class Heist
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ICollection<HeistSkill> Skills { get; set; } = new List<HeistSkill>();
        public ICollection<Member>? Members { get; set; } = new List<Member>();
        public Status? Status { get; set; }
    }
}
