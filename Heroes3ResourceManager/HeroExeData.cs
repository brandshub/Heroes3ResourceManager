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
        
        public int GenderInt;
        public int Race;
        public int ClassIndex;

        public int FirstSkillIndex;
        public int FirstSkillLevel;

        public int SecondSkillIndex;
        public int SecondSkillLevel;

        public int SpellBook;
        public int SpellIndex;

        public int Unit1Index;
        public int Unit2Index;
        public int Unit3Index;

        public int Index { get; private set; }

        public HeroStats Hero { get { if (HeroesManager.Loaded) return HeroesManager.AllHeroes[Index]; return null; } }
        public HeroClass Class { get { return HeroClassManager.AllClasses[ClassIndex]; } }

        public CreatureStats Creature1 { get { if (CreatureManager.Loaded) return CreatureManager.AllCreatures[Unit1Index]; return null; } }
        public CreatureStats Creature2 { get { if (CreatureManager.Loaded) return CreatureManager.AllCreatures[Unit2Index]; return null; } }
        public CreatureStats Creature3 { get { if (CreatureManager.Loaded) return CreatureManager.AllCreatures[Unit3Index]; return null; } }

        public SecondarySkill Skill1 { get { if (SecondarySkill.Loaded) return SecondarySkill.AllSkills[FirstSkillIndex]; return null; } }
        public SecondarySkill Skill2 { get { if (SecondarySkill.Loaded && SecondSkillIndex != -1) return SecondarySkill.AllSkills[SecondSkillIndex]; return null; } }

        public SpellStat Spell { get { return SpellStat.GetSpellByIndex(SpellIndex); } }
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
                    var hero = new HeroExeData
                    {
                        Index = i,
                        GenderInt = BitConverter.ToInt32(executableBinary, currentOffset),
                        Race = BitConverter.ToInt32(executableBinary, currentOffset + 4),
                        ClassIndex = BitConverter.ToInt32(executableBinary, currentOffset + 8),
                        FirstSkillIndex = BitConverter.ToInt32(executableBinary, currentOffset + 12),
                        FirstSkillLevel = BitConverter.ToInt32(executableBinary, currentOffset + 16),
                        SecondSkillIndex = BitConverter.ToInt32(executableBinary, currentOffset + 20),
                        SecondSkillLevel = BitConverter.ToInt32(executableBinary, currentOffset + 24),
                        SpellBook = BitConverter.ToInt32(executableBinary, currentOffset + 28),
                        SpellIndex = BitConverter.ToInt32(executableBinary, currentOffset + 32),
                        Unit1Index = BitConverter.ToInt32(executableBinary, currentOffset + 36),
                        Unit2Index = BitConverter.ToInt32(executableBinary, currentOffset + 40),
                        Unit3Index = BitConverter.ToInt32(executableBinary, currentOffset + 44)
                    };
                    Data.Add(hero);
                    currentOffset += BLOCK_SIZE_A;
                }
            }
        }

        public override string ToString()
        {
            return Hero + ", " + Class + "  " + Skill1 + "[" + FirstSkillLevel + "]" + (SecondSkillIndex != -1 ? (" | " + Skill2 + "[" + SecondSkillLevel + "]") : "");
        }
    }
}
