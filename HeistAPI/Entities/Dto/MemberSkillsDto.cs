namespace HeistAPI.Entities.Dto
{
    public class MemberSkillsDto
    {
        public List<MemberSkillDto>? Skills { get; set; }
        public string? MainSkill { get; set; }
    }

    public class MemberSkillDto
    {
        public required string Name { get; set; }
        public string? Level { get; set; }
    }

}
