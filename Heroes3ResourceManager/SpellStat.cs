using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class SpellStat
    {
        private const string TXT_FNAME = "SPTRAITS.TXT";
        private const string IMG_FNAME = "SpellBon.def";

        private static string[] allRows = null;
        public static List<SpellStat> AllSpells = null;

        public int Index { get; private set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int BaseManacost { get; set; }

        public SpellSchool MagicSchool { get; set; }

        public SpellStat(int index, string row)
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

            AllSpells = new List<SpellStat>(allRows.Length);
            int index = 0;
            for (int i = 5; i < allRows.Length; i++)
            {
                string row = allRows[i];
                if (row.StartsWith("Creature Abilities"))
                    break;
                if (string.IsNullOrEmpty(row) || row.StartsWith("\t\t") || row.StartsWith("Combat Spells") || row.StartsWith("Adventure Spells") || row.StartsWith("Name"))
                    continue;

                AllSpells.Add(new SpellStat(index++, row));
            }
        }

        public static SpellStat GetSpellByIndex(int index)
        {
            return AllSpells?.FirstOrDefault(s => s.Index == index);
        }

        public Bitmap GetImage(LodFile h3sprite)
        {
            var rec = h3sprite.GetRecord(IMG_FNAME);
            var def = rec?.GetDEFFile(h3sprite.stream);
            return def?.GetByAbsoluteNumber(Index);
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
