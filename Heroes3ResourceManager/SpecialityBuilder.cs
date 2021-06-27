using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class SpecialityBuilder
    {
        private static List<Speciality> OriginalSpecs { get; set; }

        private static DefFile def32, def44;
        private static FatRecord h_specs;
        private static string[] spec_rows;

        public static void LoadFromMaster(Heroes3Master master)
        {
            FatRecord un32, un44;


            un32 = master.ResolveWith(BackupManager.GetBackupFileName(Speciality.IMG_FNAME_SMALL)) ?? master.ResolveWith(Speciality.IMG_FNAME_SMALL);
            un44 = master.ResolveWith(BackupManager.GetBackupFileName(Speciality.IMG_FNAME)) ?? master.ResolveWith(Speciality.IMG_FNAME);
            h_specs = master.ResolveWith(BackupManager.GetBackupFileName(HeroesManager.H_SPECS)) ?? master.ResolveWith(HeroesManager.H_SPECS);

            spec_rows = Encoding.Default.GetString(h_specs.GetRawData()).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            LoadOriginalSpecs(Properties.Resources.allspecs, 0);
            LoadDefsUncompressed(un32.GetRawData(), un44.GetRawData());
        }

        private static void LoadOriginalSpecs(byte[] raw, int offset)
        {
            OriginalSpecs = Speciality.LoadInfo(raw, offset);
        }


        private static void LoadDefsUncompressed(byte[] un32, byte[] un44)
        {
            def32 = new DefFile(null, un32);
            def44 = new DefFile(null, un44);
        }

        private static int FindOriginalSpecIndexFromSpeciality(Speciality newSpec)
        {

            for (int i = 0; i < OriginalSpecs.Count; i++)
            {
                if (OriginalSpecs[i].Equals(newSpec))
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindSameIconIndexForSpeciality(Speciality spec)
        {
            for (int i = 0; i < OriginalSpecs.Count; i++)
            {
                if (OriginalSpecs[i].SameIcon(spec))
                {
                    return i;
                }
            }
            return -1;
        }

        public static string OriginalSpecText(int index)
        {
            return spec_rows[index + 2];
        }

        private static int TryUpdateSpec(HeroExeData hero, DefFile un32, DefFile un44)
        {
            if (OriginalSpecs == null)
                return -1;

            int index = FindOriginalSpecIndexFromSpeciality(hero.Spec);
            if (index >= 0)
            {
                un44.RetargetSprite(def44, 0, hero.Index, index);
                un32.RetargetSprite(def32, 0, hero.Index, index);

                var hs = HeroesManager.AllHeroes[hero.Index];
                hs.Speciality = SpecialityBuilder.OriginalSpecText(index);
                return index;
            }
            else
            {
                index = FindSameIconIndexForSpeciality(hero.Spec);
                if (index >= 0)
                {
                    un44.RetargetSprite(def44, 0, hero.Index, index);
                    un32.RetargetSprite(def32, 0, hero.Index, index);
                }
                else
                {
                    CreateNewSpecImage();
                }
                var hs = HeroesManager.AllHeroes[hero.Index];
                hs.Speciality = GenerateCustomSpecText(hero);
            }
            return index;
        }

        private static void CreateNewSpecImage()
        {
        }

        private static string GetCreatureSiblingsText(Creature creature)
        {
            if (creature.TownIndex <= 8)
                return creature.Plural1.ChangeFirstCharCase() + " or " + CreatureManager.GetByCreatureIndex(creature.CreatureIndex + 1).Plural1.ChangeFirstCharCase();
            return creature.Plural1.ChangeFirstCharCase();
        }

        private static string GenerateCustomSpecText(HeroExeData hero)
        {
            string title = "", shortDescription = "", longDescription = "";
            var spec = hero.Spec;
            // if spec is secondary skill, it would be found in original data;

            if (spec.Type == SpecialityType.CreatureLevelBonus)
            {
                var creature = CreatureManager.GetByCreatureIndex(spec.ObjectId);
                title = creature.Plural1.ChangeFirstCharCase();
                shortDescription = "Creature Bonus: " + creature.Plural1.ChangeFirstCharCase();
                if (creature.TownIndex <= 8)
                {
                    longDescription = string.Format("Increases Speed of any {0} by 1 and their Attack and Defense skills by 5% for every {1} (rounded up)."
                        , GetCreatureSiblingsText(creature),
                        creature.CreatureTownLevel == 1 ? "level" : (creature.CreatureTownLevel + " levels"));
                }
                else
                {
                    longDescription = string.Format("Increases Speed of any {0} by 1 and their Attack and Defense skills by 5% for every 4 levels (rounded up).", creature.Plural1.ChangeFirstCharCase());
                }
            }
            else if (spec.Type == SpecialityType.Resource)
            {
                int d = 1;
                if (spec.ObjectId == (int)ResourceSpeciality.Gold350)
                    d = 350;

                string resourceType = Enum.GetName(typeof(ResourceSpeciality), spec.ObjectId);
                title = "+ " + d + " " + resourceType;
                shortDescription = title + " each day.";
                longDescription = "Increases kingdom's " + resourceType.ChangeFirstCharCase(false) + " production by " + d + " per day.";

            }
            else if (spec.Type == SpecialityType.Spell)
            {
                var spell = Spell.GetSpellByIndex(spec.ObjectId);

                title = spell.Name;
                shortDescription = "Spell Bonus: " + spell.Name;
                longDescription = "Casts " + spell.Name + "with effect increased by 3% for every n hero levels, where n is the level of the targeted creature.";

            }
            else if (spec.Type == SpecialityType.CreatureStaticBonus)
            {
                var creature = CreatureManager.GetByCreatureIndex(spec.ObjectId);

                int attack, defense, damage;
                spec.TryGetCreatureStaticBonuses(out attack, out defense, out damage);

                title = creature.Plural1.ChangeFirstCharCase();
                shortDescription = "Creature Bonus: " + title;

                longDescription = GetCreatureSiblingsText(creature);
                longDescription += " receive ";

                if (attack != 0)
                    longDescription += " +" + attack + " Attack";
                if (defense != 0)
                    longDescription += " +" + defense + " Attack";
                if (damage != 0)
                    longDescription += " +" + damage + " Damage";
            }
            else if (spec.Type == SpecialityType.CreaturesUpgrade)
            {

                int crFrom2 = BitConverter.ToInt32(spec.Data, 12);
                int crTo = BitConverter.ToInt32(spec.Data, 16);

                title = CreatureManager.GetByCreatureIndex(crTo).Plural1.ChangeFirstCharCase();
                shortDescription = "Creature Bonus: " + title;
                if (crFrom2 >= 0 || spec.ObjectId >= 0)
                {
                    longDescription = "Can upgrade ";
                    if (spec.ObjectId >= 0)
                        longDescription += GetCreatureSiblingsText(CreatureManager.GetByCreatureIndex(spec.ObjectId));               

                    if (crFrom2 >= 0 && spec.ObjectId >= 0)
                        longDescription += " and ";

                    if (crFrom2 >= 0)
                        longDescription += GetCreatureSiblingsText(CreatureManager.GetByCreatureIndex(crFrom2));                  

                    longDescription += " to " + title;
                }
                else
                {
                    longDescription = shortDescription;
                }                
            }
            return string.Format("{0}\t{1}\t{2}", title, shortDescription, longDescription);

        }


        public static int TryUpdateSpecialityImageAndText(Heroes3Master master, HeroExeData hero)
        {
            var un32 = master.Resolve(Speciality.IMG_FNAME_SMALL);
            var un44 = master.Resolve(Speciality.IMG_FNAME);

            var un32Def = un32.GetRecord(Speciality.IMG_FNAME_SMALL).GetDefFile();
            var un44Def = un44.GetRecord(Speciality.IMG_FNAME).GetDefFile();

            return TryUpdateSpec(hero, un32Def, un44Def);

        }
    }
}
