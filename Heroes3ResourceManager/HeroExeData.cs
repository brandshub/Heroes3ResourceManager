using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class HeroExeData
    {
        public const int BLOCK_SIZE_A = 92;
        public static List<HeroExeData> Data { get; private set; }

        public bool HasChanged { get; set; }
        
        public int Gender;
        public int Race;
        public int ClassIndex;

        public int FirstSkill;
        public int FirstSkillLevel;

        public int SecondSkill;
        public int SecondSkillLevel;

        public int SpellBook;
        public int Spell;

        public int StartingUnit1;
        public int StartingUnit2;
        public int StartingUnit3;

        public int Index { get; private set; }

        public HeroStats Hero { get { if (HeroesManager.Loaded) return HeroesManager.AllHeroes[Index]; return null; } }
        public HeroClass Class { get { return HeroClassManager.AllClasses[ClassIndex]; } }

        public CreatureStats Creature1 { get { if (CreatureManager.Loaded) return CreatureManager.AllCreatures[StartingUnit1]; return null; } }
        public CreatureStats Creature2 { get { if (CreatureManager.Loaded) return CreatureManager.AllCreatures[StartingUnit2]; return null; } }
        public CreatureStats Creature3 { get { if (CreatureManager.Loaded) return CreatureManager.AllCreatures[StartingUnit3]; return null; } }

        public SecondarySkill Skill1 { get { if (SecondarySkill.Loaded) return SecondarySkill.AllSkills[FirstSkill]; return null; } }
        public SecondarySkill Skill2 { get { if (SecondarySkill.Loaded && SecondSkill != -1) return SecondarySkill.AllSkills[SecondSkill]; return null; } }

        public static void LoadData(byte[] executableBinary)
        {
            if (Data == null)
            {
                Data = new List<HeroExeData>();
                int startOffset = (int)HeroesSection.FindOffset2(executableBinary);
                int currentOffset = startOffset;
                int bound = HeroesManager.Loaded ? HeroesManager.AllHeroes.Count : 60;
                for (int i = 0; i < bound; i++)
                {

                    var hero = new HeroExeData();
                    hero.Index = i;
                    hero.Gender = BitConverter.ToInt32(executableBinary, currentOffset);
                    hero.Race = BitConverter.ToInt32(executableBinary, currentOffset + 4);
                    hero.ClassIndex = BitConverter.ToInt32(executableBinary, currentOffset + 8);
                    hero.FirstSkill = BitConverter.ToInt32(executableBinary, currentOffset + 12);
                    hero.FirstSkillLevel = BitConverter.ToInt32(executableBinary, currentOffset + 16);
                    hero.SecondSkill = BitConverter.ToInt32(executableBinary, currentOffset + 20);
                    hero.SecondSkillLevel = BitConverter.ToInt32(executableBinary, currentOffset + 24);
                    hero.SpellBook = BitConverter.ToInt32(executableBinary, currentOffset + 28);
                    hero.Spell = BitConverter.ToInt32(executableBinary, currentOffset + 32);
                    hero.StartingUnit1 = BitConverter.ToInt32(executableBinary, currentOffset + 36);
                    hero.StartingUnit2 = BitConverter.ToInt32(executableBinary, currentOffset + 40);
                    hero.StartingUnit3 = BitConverter.ToInt32(executableBinary, currentOffset + 44);
                    Data.Add(hero);
                    currentOffset += BLOCK_SIZE_A;
                }
            }
        }

        public override string ToString()
        {
            return Hero + ", " + Class + "  " + Skill1 + "[" + FirstSkillLevel + "]" + (SecondSkill != -1 ? (" | " + Skill2 + "[" + SecondSkillLevel + "]") : "");
        }
    }
}
