namespace SimDawn
{
    public class Character
    {
        public string Name { get; set; }
        public bool isMale { get; set; } //Sex is a bit with 0 being female and 1 being male, so here we go
        public int Level { get; set; }
        public int SkillPoints { get; set; }
        public int DevotionPoints { get; set; }
        public int Physique { get; set; }
        public int Cunning { get; set; }
        public int Spirit { get; set; }
        public int Health { get; set; }
        public int Energy { get; set; }
        public int OffensiveAbility { get; set; }
        public int DefensiveAbility { get; set; }
    }
}