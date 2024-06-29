using Microsoft.AspNetCore.Components.Routing;

namespace HeistAPI.Entities.Models
{
    public class MemberSkill
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Level { get; set; }
        public int MemberId { get; set; }
    }
}
