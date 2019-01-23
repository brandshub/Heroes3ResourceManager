using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace h3magic
{
    static class HeroesManager
    {
        private const string H_BIOGRAPHIES = "HeroBios.txt";
        private const string H_SPECS = "HeroSpec.txt";
        private const string H_HEROES = "HOTRAITS.TXT";

        private static string[] bio_rows, spec_rows;
        private static string[] hero_rows;

        private static string[] types = { "KN", "CL", "RN", "DR", "AL", "WZ", "HR", "DM", "DK", "NC", "OV", "WL", "BR", "BM", "BS", "WH", "PL", "EL" };
        public static List<HeroStats> AllHeroes = new List<HeroStats>();

        public static string[] HeroesOrder;
        public static bool Loaded { get; private set; }

        public static void LoadInfo(LodFile lodFile)
        {
            if (!Loaded)
            {
                int index = lodFile.IndexOf("HPL000EL.pcx");
                int bound = lodFile.FilesTable.FindLastIndex(index + types.Length * 22, fat => fat.FileName.Contains("HPL"));
                Dictionary<string, List<string>> heroes = new Dictionary<string, List<string>>(types.Length);
                List<string> list;
                for (int i = index; i < bound; i++)
                {
                    string end = lodFile[i].FileName.Substring(6, 2);
                    if (!heroes.TryGetValue(end, out list))
                    {
                        heroes.Add(end, new List<string> { lodFile[i].FileName });
                    }
                    else
                        list.Add(lodFile[i].FileName);
                }
                List<string> stringList = new List<string>(types.Length * 16);
                for (int i = 0; i < types.Length; i++)
                    stringList.AddRange(heroes[types[i]]);
                HeroesOrder = stringList.ToArray();
                bio_rows = Encoding.Default.GetString(lodFile.GetRawData(H_BIOGRAPHIES)).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                spec_rows = Encoding.Default.GetString(lodFile.GetRawData(H_SPECS)).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                hero_rows = Encoding.Default.GetString(lodFile.GetRawData(H_HEROES)).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                AllHeroes = new List<HeroStats>(HeroesOrder.Length);
                for (int i = 0; i < HeroesOrder.Length; i++)
                {
                    string[] traits = hero_rows[2 + i].Split('\t');
                    AllHeroes.Add(new HeroStats()
                    {
                        Biography = bio_rows[i],
                        Speciality = spec_rows[2 + i],
                        CastleIndex = types[i / 16],
                        Name = traits[0],
                        ImageIndex = i,
                        LowStack1 = int.Parse(traits[1]),
                        HighStack1 = int.Parse(traits[2]),
                        LowStack2 = int.Parse(traits[4]),
                        HighStack2 = int.Parse(traits[5]),
                        LowStack3 = int.Parse(traits[7]),
                        HighStack3 = int.Parse(traits[8])

                    });


                }
            }
            Loaded = true;
        }

        public static void Save(LodFile lodfile)
        {
            StringBuilder bios, spec, traits;
            bios = new StringBuilder();
            spec = new StringBuilder();
            traits = new StringBuilder();
            spec.AppendLine(spec_rows[0] + "\r\n" + spec_rows[1]);
            traits.AppendLine(hero_rows[0] + "\r\n" + hero_rows[1]);

            for (int i = 0; i < HeroesOrder.Length; i++)
            {
                bios.AppendLine(AllHeroes[i].Biography);
                spec.AppendLine(AllHeroes[i].Speciality);
                traits.Append(AllHeroes[i].Name);
                traits.Append('\t');
                traits.Append(AllHeroes[i].LowStack1);
                traits.Append('\t');
                traits.Append(AllHeroes[i].HighStack1);
                traits.Append('\t');
                traits.Append("cr1");
                traits.Append('\t');
                traits.Append(AllHeroes[i].LowStack2);
                traits.Append('\t');
                traits.Append(AllHeroes[i].HighStack2);
                traits.Append('\t');
                traits.Append("cr2");
                traits.Append('\t');
                traits.Append(AllHeroes[i].LowStack3);
                traits.Append('\t');
                traits.Append(AllHeroes[i].HighStack3);
                traits.Append('\t');
                traits.AppendLine("cr3");
                if (AllHeroes[i].Large != null)
                {
                    lodfile[HeroesOrder[AllHeroes[i].ImageIndex]].ApplyChanges(PCXFile.FromBitmap(AllHeroes[i].Large).GetBytes);
                }
                if (AllHeroes[i].Small != null)
                {
                    lodfile[HeroesOrder[AllHeroes[i].ImageIndex].Replace("HPL", "HPS")].ApplyChanges(PCXFile.FromBitmap(AllHeroes[i].Small).GetBytes);
                }
            }
            for (int i = HeroesOrder.Length; i < bio_rows.Length; i++)
            {
                bios.AppendLine(bio_rows[i]);
                if (i + 2 < spec_rows.Length)
                    spec.AppendLine(spec_rows[2 + i]);
                if (i + 2 < hero_rows.Length)
                    traits.AppendLine(hero_rows[2 + i]);
            }
            lodfile[H_BIOGRAPHIES].ApplyChanges(Encoding.Default.GetBytes(bios.ToString()));
            lodfile[H_SPECS].ApplyChanges(Encoding.Default.GetBytes(spec.ToString()));
            lodfile[H_HEROES].ApplyChanges(Encoding.Default.GetBytes(traits.ToString()));

        }
    }
}
