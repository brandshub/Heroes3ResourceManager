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
        public const string IMG_FNAME = "UN44.def";
        public const string IMG_FNAME_SMALL = "UN32.def";
        private const int BLOCK_SIZE = 40;

        public static List<Speciality> AllSpecialities;

        //0 - skill, 1 - creature, 2 - +350, 3 - spell , 4 - constant creature
        public int TypeId { get; private set; }
        public int ObjectId { get; private set; }
        public byte[] Data { get; private set; }

        public int Index { get; private set; }
        public SpecialityType Type { get { return (SpecialityType)TypeId; } }

        public static List<Speciality> LoadInfo(byte[] executableBinary, int offset)
        {
            var list = new List<Speciality>();
            int currentOffset = offset;
            int bound = HeroesManager.AllHeroes.Count;
            for (int i = 0; i < 156; i++)
            {
                var spec = new Speciality
                {
                    Index = i,
                    TypeId = BitConverter.ToInt32(executableBinary, currentOffset),
                    ObjectId = BitConverter.ToInt32(executableBinary, currentOffset + 4)
                };
                spec.Data = new byte[32];
                Buffer.BlockCopy(executableBinary, currentOffset + 8, spec.Data, 0, spec.Data.Length);
                currentOffset += BLOCK_SIZE;
                list.Add(spec);
            }
            return list;
        }

        public static void LoadInfo(byte[] executableBinary)
        {
            Unload();
            int startOffset = (int)HeroesSection.FindHeroOffset2(executableBinary);
            if (startOffset >= 0)
                AllSpecialities = LoadInfo(executableBinary, startOffset);
        }

        public static Bitmap GetImage(Heroes3Master master, int index)
        {            
            return master.ResolveWith(IMG_FNAME).GetDefFile().GetByAbsoluteNumber(index);
        }

        public static Speciality GetByIndex(int index)
        {
            return AllSpecialities[index];
        }

        public static Bitmap GetAllSpecs(Heroes3Master master)
        {
            if (BitmapCache.SpecialitiesAll != null)
                return BitmapCache.SpecialitiesAll;



            var bmp = new Bitmap(16 * (44 + 1), (44 + 1) * 9);
            using (var g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 16; j++)
                    {
                        g.DrawImage(GetImage(master, i * 16 + j), j * 45, 45 * i);
                    }
            }

            BitmapCache.SpecialitiesAll = bmp;
            return BitmapCache.SpecialitiesAll;


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
                s += Spell.GetSpellByIndex(ObjectId);
            }
            else if (TypeId == 4)
            {

                s += CreatureManager.GetByCreatureIndex(ObjectId);

                int attack, defense, damage;

                TryGetCreatureStaticBonuses(out attack, out defense, out damage);

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

        public static ProfilePropertyType ToProfilePropertyType(SpecialityType specType)
        {
            return (ProfilePropertyType)((int)specType + 4);
        }

        public static SpecialityType FromProfileProperty(ProfilePropertyType type)
        {
            int val = (int)type;
            if (val >= 4 && val <= 10)
                return (SpecialityType)(val - 4);

            return SpecialityType.Invalid;

        }

        public bool TryGetCreatureStaticBonuses(out int attack, out int defense, out int damage)
        {
            attack = 0;
            defense = 0;
            damage = 0;
            if (Type == SpecialityType.CreatureStaticBonus)
            {
                attack = BitConverter.ToInt32(Data, 0);
                defense = BitConverter.ToInt32(Data, 4);
                damage = BitConverter.ToInt32(Data, 8);
                return true;
            }
            return false;
        }

        public bool TryGetCreatureUpgrade(out int creature1, out int creature2, out int targetCreature)
        {
            creature1 = -1;
            creature2 = -1;
            targetCreature = -1;

            if (Type == SpecialityType.CreaturesUpgrade)
            {
                creature1 = ObjectId;
                creature2 = BitConverter.ToInt32(Data, 12);
                targetCreature = BitConverter.ToInt32(Data, 16);
                return true;
            }
            return false;
        }

        public static unsafe void Update(byte* ptr, int index)
        {
            var spec = AllSpecialities[index];

            long offset = HeroesSection.HeroOffset2 + index * BLOCK_SIZE;
            int* iptr = (int*)(ptr + offset);

            *iptr++ = spec.TypeId;
            *iptr++ = spec.ObjectId;
            Marshal.Copy(spec.Data, 0, new IntPtr((void*)iptr), 32);
        }


        public static void UpdateSpecialityData(SpecialityType type, int index, int arg0, int arg1, int arg2, int arg3)
        {
            var spec = new Speciality
            {
                Index = index,
                Data = new byte[32],
                TypeId = (int)type,
            };

            var temp = new int[8];

            if (type == SpecialityType.Skill || type == SpecialityType.Spell || type == SpecialityType.Resource || type == SpecialityType.CreatureLevelBonus)
            {
                spec.ObjectId = arg0;
            }
            else if (type == SpecialityType.CreaturesUpgrade)
            {
                spec.ObjectId = arg1;
                temp[3] = arg2;
                temp[4] = arg3;
                Buffer.BlockCopy(temp, 0, spec.Data, 0, 32);
            }
            else if (type == SpecialityType.CreatureStaticBonus)
            {
                spec.ObjectId = CreatureManager.IndexesOfFirstLevelCreatures[arg0];
                temp[0] = arg1;
                temp[1] = arg2;
                temp[2] = arg3;
                Buffer.BlockCopy(temp, 0, spec.Data, 0, 32);
            }
            else if (type == SpecialityType.Speed)
            {
                spec.ObjectId = 2;
            }

            AllSpecialities[index] = spec;
        }

        public override string ToString()
        {
            return GetDescription();
        }

        public bool Equals(Speciality other)
        {
            if (other == null)
                return false;

            if (TypeId != other.TypeId)
                return false;
            if (ObjectId != other.ObjectId)
                return false;

            return Data.SequenceEqual(other.Data);
        }

        public static void Unload()
        {
            AllSpecialities = null;
            BitmapCache.SpecialitiesAll = null;
        }
    }
}
