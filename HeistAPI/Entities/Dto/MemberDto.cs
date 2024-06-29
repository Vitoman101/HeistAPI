namespace HeistAPI.Entities.Dto
{
    public class MemberDto
    {
        public required string Name { get; set; }
        public List<MemberSkillDto>?  Skills { get; set; }
    }
}
