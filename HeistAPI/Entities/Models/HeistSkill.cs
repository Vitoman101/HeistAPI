namespace HeistAPI.Entities.Models
{
    public class HeistSkill
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Level { get; set; }
        public int Members { get; set; }
        public int HeistId { get; set; }
    }
}
