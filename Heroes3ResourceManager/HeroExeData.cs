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

        public int SpecIndex;

        public int Index { get; private set; }

        public HeroStats Hero { get { if (HeroesManager.Loaded) return HeroesManager.AllHeroes[Index]; return null; } }
        public HeroClass Class { get { return HeroClass.GetByIndex(ClassIndex); } }

        public Creature Creature1 { get { return CreatureManager.GetByIndex(Unit1Index); } }
        public Creature Creature2 { get { return CreatureManager.GetByIndex(Unit2Index); } }
        public Creature Creature3 { get { return CreatureManager.GetByIndex(Unit3Index); } }

        public SecondarySkill Skill1 { get { return SecondarySkill.AllSkills[FirstSkillIndex]; } }
        public SecondarySkill Skill2 { get { if (SecondSkillIndex != -1) return SecondarySkill.AllSkills[SecondSkillIndex]; return null; } }

        public Spell Spell { get { return Spell.GetSpellByIndex(SpellIndex); } }

        public Speciality Spec { get { return Speciality.GetByIndex(SpecIndex); } }


        public static void LoadInfo(byte[] executableBinary)
        {
            if (Data == null)
            {
                Data = new List<HeroExeData>();
                int startOffset = (int)HeroesSection.FindHeroOffset1(executableBinary);
                int currentOffset = startOffset;
                int bound = HeroesManager.AllHeroes.Count;
                for (int i = 0; i < bound; i++)
                {
                    var hero = new HeroExeData
                    {
                        Index = i,
                        SpecIndex = i,
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

        public unsafe static void UpdateDataInMemory()
        {
            long offset = HeroesSection.HeroOffset1;            

            var exe = Heroes3Master.Master.Executable;
            string file = exe.Path;
            System.IO.File.WriteAllBytes(exe.Path + ".bak." + DateTime.Now.ToString("hhmmss"), exe.Data);

            for (int i = 0; i < Data.Count; i++)
            {
                var current = Data[i];
                if (current.HasChanged)
                {
                    fixed (byte* ptr = exe.Data)
                    {
                        int* iptr = (int*)(ptr + offset + i * BLOCK_SIZE_A);
                        *iptr++ = current.GenderInt;
                        *iptr++ = current.Race;
                        *iptr++ = current.ClassIndex;
                        *iptr++ = current.FirstSkillIndex;
                        *iptr++ = current.FirstSkillLevel;
                        *iptr++ = current.SecondSkillIndex;
                        *iptr++ = current.SecondSkillLevel;
                        *iptr++ = current.SpellBook;
                        *iptr++ = current.SpellIndex;
                        *iptr++ = current.Unit1Index;
                        *iptr++ = current.Unit2Index;
                        *iptr = current.Unit3Index;


                        if (current.SpecIndex != current.Index)
                            Speciality.Update(ptr, current.Index, current.SpecIndex);
                    }
                }
            }            
        }
        public override string ToString()
        {
            return Hero + ", " + Class + "  " + Skill1 + "[" + FirstSkillLevel + "]" + (SecondSkillIndex != -1 ? (" | " + Skill2 + "[" + SecondSkillLevel + "]") : "");
        }
    }
}
