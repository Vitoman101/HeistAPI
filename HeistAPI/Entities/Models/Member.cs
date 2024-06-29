using HeistAPI.Entities.Enum;

namespace HeistAPI.Entities.Models
{
    public class Member
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public Sex Sex { get; set; }
        public required string Email { get; set; }
        public ICollection<MemberSkill> Skills { get; set; } = new List<MemberSkill>();
        public string? MainSkill { get; set; }
        public Status Status { get; set; }

    }
}
