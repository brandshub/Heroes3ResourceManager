using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h3magic
{
    public class Spell
    {
        private const string TXT_FNAME = "SPTRAITS.TXT";
        private const string IMG_FNAME = "SpellBon.def";

        private static string[] allRows = null;
        public static List<Spell> AllSpells = null;

        private static DefFile defFile;

        public int Index { get; private set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int BaseManacost { get; set; }

        public SpellSchool MagicSchool { get; set; }

        public Spell(int index, string row)
        {
            Index = index;
            var cells = row.Split('\t');
            Name = cells[0];
            Level = int.Parse(cells[2]);

            int type = 0;
            for (int i = 3; i < 7; i++)
            {
                if (cells[i] == "x")
                {
                    type |= (1 << (i - 3));
                }
            }
            MagicSchool = (SpellSchool)type;
            BaseManacost = int.Parse(cells[7]);
        }

        public static void LoadInfo(LodFile h3bitmap)
        {
            FatRecord rec = h3bitmap[TXT_FNAME];

            string text = Encoding.Default.GetString(rec.GetRawData(h3bitmap.stream));
            allRows = text.Split(new[] { "\r\n" }, StringSplitOptions.None);

            AllSpells = new List<Spell>(allRows.Length);
            int index = 0;
            for (int i = 5; i < allRows.Length; i++)
            {
                string row = allRows[i];
                if (row.StartsWith("Creature Abilities"))
                    break;
                if (string.IsNullOrEmpty(row) || row.StartsWith("\t\t") || row.StartsWith("Combat Spells") || row.StartsWith("Adventure Spells") || row.StartsWith("Name"))
                    continue;

                AllSpells.Add(new Spell(index++, row));
            }
            defFile = null;
        }

        public static Spell GetSpellByIndex(int index)
        {
            return AllSpells.FirstOrDefault(s => s.Index == index);
        }

        public Bitmap GetImage(LodFile h3sprite)
        {
            if (defFile == null)
                defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite.stream);

            return defFile.GetByAbsoluteNumber(Index);
        }

        private static Bitmap allSpells = null;

        public static Bitmap GetAllSpells(LodFile h3sprite)
        {
            if (allSpells != null)
                return allSpells;

            if (defFile == null)
                defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite.stream);

            var sw = Stopwatch.StartNew();
            var bmp = new Bitmap((58 + 1) * 10, (64 + 1) * 7);
            using (var g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < 7; i++)
                    for (int j = 0; j < 10; j++)
                    {
                        var img = defFile.GetByAbsoluteNumber(i * 10 + j);
                        g.DrawImage(img, j * (58 + 1), i * (64 + 1));
                    }
            }
            allSpells = bmp;
            float ms = sw.ElapsedTicks * 1000.0f / Stopwatch.Frequency;
            return allSpells;
        }


        public static Bitmap GetAllSpellsParallel(LodFile h3sprite)
        {
            if (allSpells != null)
                return allSpells;

            if (defFile == null)
                defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite.stream);

            var bmp = new Bitmap((58 + 1) * 10, (64 + 1) * 7);
            var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Parallel.For(0, 70, i =>
            {
                int row = i / 10;
                int col = i % 10;

                var img = defFile.GetByAbsoluteNumber2(i);
                if (img != null)
                {
                    imageData.DrawImage24(col * (58 + 1), row * (64 + 1), 176, img);
                }
            });

            bmp.UnlockBits(imageData);
            allSpells = bmp;
            return allSpells;
        }

        private static Bitmap _allSpecSpells;
        public static int[] SpecSpellIndexes = { 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 37, 38, 39, 41, 43, 44, 45, 46, 47, 48, 51, 53, 55 };
        public static Bitmap GetAvailableSpellsForSpeciality(LodFile h3sprite)
        {
            if (_allSpecSpells != null)
                return _allSpecSpells;


            if (defFile == null)
                defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite.stream);

            var bmp = new Bitmap((58 + 1) * 6, (64 + 1) * 5);
            var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Parallel.For(0, SpecSpellIndexes.Length, i =>
            {
                int row = i / 6;
                int col = i % 6;

                var img = defFile.GetByAbsoluteNumber2(AllSpells[SpecSpellIndexes[i]].Index);
                if (img != null)
                {
                    imageData.DrawImage24(col * (58 + 1), row * (64 + 1), 176, img);
                }
            });

            bmp.UnlockBits(imageData);
            _allSpecSpells = bmp;
            return _allSpecSpells;
        }


        public override string ToString()
        {
            return Index + " " + Name + " [" + Level + "]";
        }

        [Flags]
        public enum SpellSchool
        {
            Earth = 1,
            Water = 2,
            Fire = 4,
            Air = 8,
            Any = 15
        }
    }
}
