using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace h3magic
{
    public class Speciality
    {
        private const string IMG_FNAME = "UN44.def";
        private const string IMG_FNAME_SMALL = "UN32.def";
        private const int BLOCK_SIZE = 40;

        //0 - skill, 1 - creature, 2 - +350, 3 - spell , 4 - constant creature
        public int TypeId { get; private set; }
        public int ObjectId { get; private set; }
        public byte[] Data { get; private set; }

        public SpecialityType Type { get { return (SpecialityType)TypeId; } }

        public static List<Speciality> AllSpecialities = new List<Speciality>();
        public static Bitmap GetImage(LodFile h3sprite, int index)
        {
            var rec = h3sprite.GetRecord(IMG_FNAME);
            if (rec != null)
            {
                var def = rec.GetDefFile(h3sprite.stream);
                if (def != null)
                    return def.GetByAbsoluteNumber(index);
            }
            return null;
        }


        public static void LoadInfo(byte[] executableBinary)
        {
            AllSpecialities = new List<Speciality>();
            int startOffset = (int)HeroesSection.FindHeroOffset2(executableBinary);
            if (startOffset >= 0)
            {
                int currentOffset = startOffset;
                int bound = HeroesManager.AllHeroes.Count;
                for (int i = 0; i < 156; i++)
                {
                    var spec = new Speciality
                    {
                        TypeId = BitConverter.ToInt32(executableBinary, currentOffset),
                        ObjectId = BitConverter.ToInt32(executableBinary, currentOffset + 4)
                    };
                    spec.Data = new byte[32];
                    Buffer.BlockCopy(executableBinary, currentOffset + 8, spec.Data, 0, spec.Data.Length);
                    currentOffset += BLOCK_SIZE;
                    AllSpecialities.Add(spec);
                }
            }
        }

        public static Speciality GetByIndex(int index)
        {
            return AllSpecialities[index];
        }

        private static Bitmap allSpecs = null;

        public static Bitmap GetAllSpecs(LodFile h3sprite)
        {
            if (allSpecs != null)
                return allSpecs;

            var bmp = new Bitmap(16 * (44 + 1), (44 + 1) * 9);
            var rec = h3sprite.GetRecord(IMG_FNAME);
            if (rec != null)
            {
                var def = rec.GetDefFile(h3sprite.stream);
                using (var g = Graphics.FromImage(bmp))
                {
                    for (int i = 0; i < 9; i++)
                        for (int j = 0; j < 16; j++)
                        {
                            g.DrawImage(GetImage(h3sprite, i * 16 + j), j * 45, 45 * i);
                        }
                }
                allSpecs = bmp;
                return allSpecs;
            }
            return null;
        }

        public string GetDescription()
        {
            string s = Type + " | ";

            if (TypeId == 0)
            {
                s += SecondarySkill.AllSkills[ObjectId];
            }
            else if (TypeId == 1)
            {
                s += CreatureManager.GetByCreatureIndex(ObjectId);
            }
            else if (TypeId == 2)
            {
                if (Enum.IsDefined(typeof(ResourceSpeciality), ObjectId))
                    s += Enum.GetName(typeof(ResourceSpeciality), ObjectId);
            }
            else if (TypeId == 3)
            {
                s += SpellStat.GetSpellByIndex(ObjectId);
            }
            else if (TypeId == 4)
            {

                s += CreatureManager.GetByCreatureIndex(ObjectId);

                int attack = BitConverter.ToInt32(Data, 0);
                int defense = BitConverter.ToInt32(Data, 4);
                int damage = BitConverter.ToInt32(Data, 8);

                if (attack != 0)
                    s += " A: +" + attack;
                if (defense != 0)
                    s += " D: +" + defense;
                if (damage != 0)
                    s += " DMG: +" + damage;

            }
            else if (TypeId == 5)
            {
                s += "+2 Speed";
            }
            else if (TypeId == 6)
            {
                int crFrom2 = BitConverter.ToInt32(Data, 12);
                int crTo = BitConverter.ToInt32(Data, 16);

                s += "Creature Upgrade: From (";
                s += CreatureManager.GetByCreatureIndex(ObjectId);
                s += " AND ";
                s += CreatureManager.GetByCreatureIndex(crFrom2);
                s += ") To";
                s += CreatureManager.GetByCreatureIndex(crTo);
            }
            else if (TypeId == 7 || TypeId == -1)
            {
                s += Enum.GetName(typeof(SpecialityType), ObjectId);
            }
            return s;
        }

        public override string ToString()
        {
            return GetDescription();

        }

        public static unsafe void Update(byte* ptr, int oldIndex, int newIndex)
        {
            var spec = AllSpecialities[newIndex];

            long offset = HeroesSection.HeroOffset2 + oldIndex * BLOCK_SIZE;
            int* iptr = (int*)(ptr + offset);

            *iptr++ = spec.TypeId;
            *iptr++ = spec.ObjectId;
            Marshal.Copy(spec.Data, 0, new IntPtr((void*)iptr), 32);

        }
    }
}
