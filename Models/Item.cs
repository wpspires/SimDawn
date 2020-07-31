using System;
namespace SimDawn
{
    public class Item
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public int LevelRequirement { get; set; }
        public int Tier { get; set; }
        public string DBRFileName { get; set; }
        
        internal void Read(GDFileReader reader)
        {
            var baseName = reader.ReadString();
            Console.WriteLine($"Item base name: {baseName}");
            var prefixName = reader.ReadString();
            var suffixName= reader.ReadString();
            var modifierName = reader.ReadString();
            var transmuteName = reader.ReadString();
            var seed = reader.ReadInt();
            var relicName = reader.ReadString();
            var relicBonus = reader.ReadString();
            var relicSeed = reader.ReadInt();
            var augmentName = reader.ReadString();
            reader.ReadInt();
            var augmentSeed = reader.ReadInt();
            reader.ReadInt();
            var stackCount = reader.ReadInt();
        }
    }
}