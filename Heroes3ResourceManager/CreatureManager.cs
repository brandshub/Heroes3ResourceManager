using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace h3magic
{
    static class CreatureManager
    {
        public static readonly string FAT_NAME = "CRTRAITS.TXT";
        public static readonly string[] Castles = { "Замок", "Долина", "Башня", "Инферно", "Некрополис", "Подземелье", "Твердиня", "Болото", "Разное" };

        private const string H_IMAGES = "TwCrPort.def";

        public static List<CreatureStats> AllCreatures = new List<CreatureStats>();

        private static string[] rows;

        public static bool Loaded { get; private set; }
        public static bool HasChanges = false;
        private static string lastLodName = "";


        public static void LoadInfo(LodFile file)
        {
            if (lastLodName == file.Name)
                Loaded = true;
            else
                lastLodName = file.Name;

            if (!Loaded)
            {
                var rec = file.GetRecord(FAT_NAME);
                if (rec != null)
                {
                    string text = Encoding.Default.GetString(rec.GetRawData(file.stream));
                    rows = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 14; j++)
                        {
                            AllCreatures.Add(new CreatureStats(rows[2 + i * 17 + j]) { CastleIndex = i, CreatureIndex = i * 17 + j, CreatureCastleRelativeIndex = j });
                        }
                    }
                    int off = 2 + 17 * 8;
                    List<CreatureStats> misc = new List<CreatureStats>();
                    for (int i = off; i < rows.Length - 1; i++)
                        if (!rows[i].Contains("\t\t"))
                            misc.Add(new CreatureStats(rows[i]) { CastleIndex = 8, CreatureIndex = i, CreatureCastleRelativeIndex = i - off });
                        else
                            misc.Add(null);
                    AllCreatures.AddRange(misc.Where(s => s != null).ToArray());
                    Loaded = true;
                }
            }

        }

        public static CreatureStats Get(int castleIndex, int relativeCreatureIndex)
        {
            return AllCreatures.FirstOrDefault(c => c.CastleIndex == castleIndex && c.CreatureCastleRelativeIndex == relativeCreatureIndex);
        }

        public static Bitmap GetImage(LodFile h3sprite, int index)
        {            
            var rec = h3sprite.GetRecord(H_IMAGES);              
            var def = rec?.GetDefFile(h3sprite);            
            var bmp = def?.GetByAbsoluteNumber(index + 2);            
            return bmp;
        }

        public static void Save(LodFile lodfile)
        {
            FatRecord rec = lodfile.GetRecord(FAT_NAME);
            if (rec != null)
            {
                string val = GetAllStats();
                rec.ApplyChanges(Encoding.Default.GetBytes(val));
            }
        }

        public static string GetAllStats()
        {
            if (rows != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine(rows[0]);
                sb.AppendLine(rows[1]);
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 14; j++)
                        sb.AppendLine(AllCreatures[i * 14 + j].GetRow());
                    sb.AppendLine(rows[16]);
                    sb.AppendLine(rows[16]);
                    sb.AppendLine(rows[16]);
                }
                int off = 2 + 17 * 8;
                int k = 0;

                for (int i = off; i < rows.Length - 1; i++)
                    if (!rows[i].Contains("\t\t"))
                        sb.AppendLine(AllCreatures[8 * 14 + k++].GetRow());
                    else
                        sb.AppendLine(rows[i]);


                return sb.ToString();
            }
            return "";
        }






    }
}
