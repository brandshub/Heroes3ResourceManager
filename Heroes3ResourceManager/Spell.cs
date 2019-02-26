using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h3magic
{
    public class Spell
    {
        public const int ALL_SPELLS_COLUMN_NUMBER = 10;

        private const string TXT_FNAME = "SPTRAITS.TXT";
        private const string IMG_FNAME = "SpellBon.def";

        private static string[] allRows = null;
        private static DefFile defFile;

        public static int[] SpecSpellIndexes = { 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 37, 38, 39, 41, 43, 44, 45, 46, 47, 48, 51, 53, 55 };
        public static Color[] MagicSchoolColors = new Color[] { Color.PaleGreen, Color.LightCyan, Color.MistyRose, Color.White };
        public static List<Spell> AllSpells = null;


        private string[] cells;
        public bool HasChanges { get; set; }
        public int Index { get; private set; }
        public string Name { get { return cells[0]; } set { cells[0] = value; } }
        public int Level { get { return getIntCell(2); } set { setIntCell(2, value); } }

        public bool IsEarthMagic { get { return cells[3] == "x"; } set { cells[3] = value ? "x" : " "; } }
        public bool IsWaterMagic { get { return cells[4] == "x"; } set { cells[4] = value ? "x" : " "; } }
        public bool IsFireMagic { get { return cells[5] == "x"; } set { cells[5] = value ? "x" : " "; } }
        public bool IsAirMagic { get { return cells[6] == "x"; } set { cells[6] = value ? "x" : " "; } }

        public int ManacostLvl0 { get { return getIntCell(7); } set { setIntCell(7, value); } }
        public int ManacostLvl1 { get { return getIntCell(8); } set { setIntCell(8, value); } }
        public int ManacostLvl2 { get { return getIntCell(9); } set { setIntCell(9, value); } }
        public int ManacostLvl3 { get { return getIntCell(10); } set { setIntCell(10, value); } }

        public int EffectPerMagicPower { get { return getIntCell(11); } set { setIntCell(11, value); } }

        public int BaseEffectLvl0 { get { return getIntCell(12); } set { setIntCell(12, value); } }
        public int BaseEffectLvl1 { get { return getIntCell(13); } set { setIntCell(13, value); } }
        public int BaseEffectLvl2 { get { return getIntCell(14); } set { setIntCell(14, value); } }
        public int BaseEffectLvl3 { get { return getIntCell(15); } set { setIntCell(15, value); } }

        public int AiValue0 { get { return getIntCell(25); } set { setIntCell(25, value); } }
        public int AiValue1 { get { return getIntCell(26); } set { setIntCell(26, value); } }
        public int AiValue2 { get { return getIntCell(27); } set { setIntCell(27, value); } }
        public int AiValue3 { get { return getIntCell(28); } set { setIntCell(28, value); } }
        public SpellSchool MagicSchool { get; set; }

        public Spell(int index, string row)
        {
            Index = index;

            cells = row.Split('\t');
            Name = cells[0];

            int type = 0;
            for (int i = 3; i < 7; i++)
            {
                if (cells[i] == "x")
                {
                    type |= (1 << (i - 3));
                }
            }
            MagicSchool = (SpellSchool)type;
        }


        public bool HasSchool(int index)
        {
            if (index < 0 || index > 3)
                throw new ArgumentException("Incorrect spell school index: " + index);

            if (index == 0)
                return IsEarthMagic;
            if (index == 1)
                return IsWaterMagic;
            if (index == 2)
                return IsFireMagic;

            return IsAirMagic;
        }

        public int GetChanceToGainForCastle(int castleIndex)
        {
            return getIntCell(16 + castleIndex);
        }

        public void SetChanceToGainForCastle(int castleIndex, int value)
        {
            setIntCell(16 + castleIndex, value);
        }

        public static void LoadInfo(LodFile h3bitmap)
        {
            var rec = h3bitmap[TXT_FNAME];

            string text = Encoding.Default.GetString(rec.GetRawData(h3bitmap.stream));
            allRows = text.Split(new[] { "\r\n" }, StringSplitOptions.None);

            AllSpells = new List<Spell>(allRows.Length);
            int index = 0;
            for (int i = 5; i < allRows.Length; i++)
            {
                string row = allRows[i];
                if (row.StartsWith("Creature Abilities"))
                    break;

                if (string.IsNullOrEmpty(row) || row.StartsWith("\t\t\t") || row.StartsWith("Combat Spells") || row.StartsWith("Adventure Spells") || row.StartsWith("Name"))
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


        public static Bitmap GetAllSpells(LodFile h3sprite)
        {
            if (BitmapCache.SpellsAll != null)
                return BitmapCache.SpellsAll;

            if (defFile == null)
                defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite.stream);

            int total = defFile.headers[0].SpritesCount;

            var baseSprite = defFile.headers[0].spriteHeaders[0];
            int imgWidth = baseSprite.SpriteWidth;
            int imgHeight = baseSprite.SpriteHeight;
            int stride = baseSprite.Stride24;

            int colCount = ALL_SPELLS_COLUMN_NUMBER;
            int rowCount = (total / colCount) + (total % colCount == 0 ? 0 : 1);

            var bmp = new Bitmap((imgWidth + 1) * colCount, (imgHeight + 1) * rowCount);
            var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            Parallel.For(0, total, i =>
            {
                int row = i / colCount;
                int col = i % colCount;

                var img = defFile.GetByAbsoluteNumber2(i);
                if (img != null)
                {
                    imageData.DrawImage24(col * (imgWidth + 1), row * (imgHeight + 1), stride, img);
                }
            });

            bmp.UnlockBits(imageData);
            BitmapCache.SpellsAll = bmp;
            return BitmapCache.SpellsAll;
        }

        public static Bitmap GetAvailableSpellsForSpeciality(LodFile h3sprite)
        {
            if (BitmapCache.SpellsForSpeciality != null)
                return BitmapCache.SpellsForSpeciality;


            if (defFile == null)
                defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite.stream);

            var bmp = new Bitmap((58 + 1) * 6, (64 + 1) * 5);
            var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

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
            BitmapCache.SpellsForSpeciality = bmp;
            return BitmapCache.SpellsForSpeciality;
        }

        private int getIntCell(int index)
        {
            return int.Parse(cells[index]);
        }
        private void setIntCell(int index, int value)
        {
            cells[index] = value.ToString();
        }

        public static void Save(LodFile lodFile)
        {
            var rec = lodFile.GetRecord(TXT_FNAME);
            if (rec != null)
            {
                var sb = new StringBuilder();
                int i;
                int index = 0;

                for (i = 0; i < 5; i++)
                    sb.AppendLine(allRows[i]);

                for (; i < allRows.Length; i++)
                {
                    string row = allRows[i];
                    if (row.StartsWith("Creature Abilities"))
                        break;

                    if (string.IsNullOrEmpty(row) || row.StartsWith("\t\t\t") || row.StartsWith("Combat Spells") || row.StartsWith("Adventure Spells") || row.StartsWith("Name"))
                    {
                        sb.AppendLine(row);
                        continue;
                    }

                    var spellCells = AllSpells[index].cells;
                    for (int j = 0; j < spellCells.Length - 1; j++)
                    {
                        sb.Append(spellCells[j]);
                        sb.Append('\t');
                    }
                    sb.AppendLine(spellCells[spellCells.Length - 1]);

                    index++;
                }

                for (; i < allRows.Length; i++)
                    sb.AppendLine(allRows[i]);
                 
                rec.ApplyChanges(Encoding.Default.GetBytes(sb.ToString()));
            }
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
