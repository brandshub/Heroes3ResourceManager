using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace h3magic
{
    static class HeroesManager
    {
        public const string TXT_BIOGRAPHIES_FNAME = "HeroBios.txt";
        public const string H_SPECS = "HeroSpec.txt";
        public const string H_HEROES = "HOTRAITS.TXT";
        public const string H_BACKGROUND = "HeroScr4.pcx";
        public const string H_PRIMARYSKILLS = "PSKIL42.def";

        private static string[] bio_rows, spec_rows;
        private static string[] hero_rows;

        private static string[] types = { "KN", "CL", "RN", "DR", "AL", "WZ", "HR", "DM", "DK", "NC", "OV", "WL", "BR", "BM", "BS", "WH", "PL", "EL" };
        public static List<HeroStats> AllHeroes = new List<HeroStats>();

        public static string[] HeroesOrder;
        public static bool AnyChanges { get; set; }
        public static void LoadInfo(Heroes3Master master)
        {
            Unload();
            AnyChanges = false;

            var lod1 = master.Resolve(TXT_BIOGRAPHIES_FNAME);

            var lod2 = master.Resolve(H_SPECS);
            var lod3 = master.Resolve(H_HEROES);

            var imageLodFile = master.Resolve("HPL000EL.pcx");
            //int index = lodFile.IndexOf("HPL000EL.pcx");
            int index = imageLodFile.IndexOf("HPL000EL.pcx");

            int bound = imageLodFile.FilesTable.FindLastIndex(index + types.Length * 22, fat => fat.FileName.Contains("HPL"));
            var heroes = new Dictionary<string, List<string>>(types.Length);
            for (int i = index; i < bound; i++)
            {
                List<string> list = null;
                string end = imageLodFile[i].FileName.Substring(6, 2);
                if (!heroes.TryGetValue(end, out list))
                {
                    heroes.Add(end, new List<string> { imageLodFile[i].FileName });
                }
                else
                    list.Add(imageLodFile[i].FileName);
            }
            List<string> stringList = new List<string>(types.Length * 16);
            for (int i = 0; i < types.Length; i++)
                stringList.AddRange(heroes[types[i]]);
            HeroesOrder = stringList.ToArray();

            bio_rows = Encoding.Default.GetString(lod1.GetRawData(TXT_BIOGRAPHIES_FNAME)).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            spec_rows = Encoding.Default.GetString(lod2.GetRawData(H_SPECS)).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            hero_rows = Encoding.Default.GetString(lod3.GetRawData(H_HEROES)).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

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
        public static string GetSpecDescription(int index)
        {
            return spec_rows[2 + index];
        }

        public static Bitmap GetBackground(Heroes3Master master)
        {
            if (BitmapCache.HeroesBackground != null)
                return BitmapCache.HeroesBackground;

            var bmp = new Bitmap(288, 331);
            using (var g = Graphics.FromImage(bmp))
            {

                var h3bitmap = master.Resolve(H_BACKGROUND);
                var f = h3bitmap.GetRecord(H_BACKGROUND).GetBitmap(h3bitmap.stream);
                if (f != null)
                    g.DrawImage(f, new Point(-14, -15));

                g.DrawImage(f, 5, 261, new RectangleF(18, 18, 62, 68), GraphicsUnit.Pixel);
                g.DrawImage(f, 5 + 62, 261, new RectangleF(18, 18, 62, 68), GraphicsUnit.Pixel);
                g.DrawImage(f, 5 + 62 + 62, 261, new RectangleF(18, 18, 62, 68), GraphicsUnit.Pixel);
                g.DrawImage(f, 192, 261, new RectangleF(196, 19, 93, 65), GraphicsUnit.Pixel);
                g.DrawImage(f, 0, 327, new RectangleF(14, 85, 288, 4), GraphicsUnit.Pixel);

                var h3sprite = master.Resolve(H_PRIMARYSKILLS);
                var ps = h3sprite.GetRecord(H_PRIMARYSKILLS).GetDefFile(h3sprite.stream);
                if (ps != null)
                {
                    g.DrawImage(ps.GetSprite(0), new Point(18, 97));
                    g.DrawImage(ps.GetSprite(1), new Point(88, 97));
                    g.DrawImage(ps.GetSprite(2), new Point(158, 97));
                    g.DrawImage(ps.GetSprite(5), new Point(228, 97));

                    g.DrawImage(ps.GetSprite(3), new Point(167, 167));
                }
            }

            BitmapCache.HeroesBackground = bmp;
            return BitmapCache.HeroesBackground;
        }

        public static void SaveLocalChanges(Heroes3Master master)
        {
            var lod1 = master.Resolve(TXT_BIOGRAPHIES_FNAME);
            var lod2 = master.Resolve(H_SPECS);
            var lod3 = master.Resolve(H_HEROES);

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
                    var rec = master.ResolveWith(HeroesOrder[AllHeroes[i].ImageIndex]);
                    rec.ApplyChanges(PcxFile.FromBitmap(AllHeroes[i].Large).GetBytes);
                }
                if (AllHeroes[i].Small != null)
                {
                    var rec = master.ResolveWith(HeroesOrder[AllHeroes[i].ImageIndex].Replace("HPL", "HPS"));
                    rec.ApplyChanges(PcxFile.FromBitmap(AllHeroes[i].Small).GetBytes);
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

            lod1[TXT_BIOGRAPHIES_FNAME].ApplyChanges(Encoding.Default.GetBytes(bios.ToString()));
            lod2[H_SPECS].ApplyChanges(Encoding.Default.GetBytes(spec.ToString()));
            lod3[H_HEROES].ApplyChanges(Encoding.Default.GetBytes(traits.ToString()));
        }

        public static void Unload()
        {
            BitmapCache.HeroesBackground = null;
            AllHeroes = null;

            bio_rows = null;
            spec_rows = null;
            hero_rows = null;
            HeroesOrder = null;
        }
    }
}
