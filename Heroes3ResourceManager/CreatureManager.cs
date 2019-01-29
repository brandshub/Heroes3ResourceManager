using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace h3magic
{
    public static class CreatureManager
    {
        private const string FAT_NAME = "CRTRAITS.TXT";
        private const string IMG_FNAME = "TwCrPort.def";

        public static readonly string[] Castles = { "Замок", "Долина", "Башня", "Инферно", "Некрополис", "Подземелье", "Твердиня", "Болото", "Разное" };
        public static List<CreatureStats> AllCreatures = new List<CreatureStats>();

        private static DefFile creatureDef;
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
                    int curIndex = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 14; j++)
                        {
                            AllCreatures.Add(new CreatureStats(rows[2 + i * 17 + j]) { CastleIndex = i, CreatureIndex = curIndex++, CreatureCastleRelativeIndex = j });
                        }
                    }
                    int off = 2 + 17 * 8;

                    List<CreatureStats> misc = new List<CreatureStats>();
                    for (int i = off; i < rows.Length - 1; i++)
                    {
                        if (rows[i].Contains("\t\t\t"))
                        {
                            misc.Add(null);
                        }
                        else
                        {
                            if (!rows[i].Contains("NOT USED"))
                                misc.Add(new CreatureStats(rows[i]) { CastleIndex = 8, CreatureIndex = curIndex, CreatureCastleRelativeIndex = i - off });
                            curIndex++;
                        }
                    }
                    AllCreatures.AddRange(misc.Where(s => s != null).ToArray());
                    Loaded = true;
                }
            }

        }

        public static CreatureStats Get(int castleIndex, int relativeCreatureIndex)
        {
            return AllCreatures.FirstOrDefault(c => c.CastleIndex == castleIndex && c.CreatureCastleRelativeIndex == relativeCreatureIndex);
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


        public static Bitmap GetImage(LodFile h3sprite, int index)
        {
            if (creatureDef == null)            
                creatureDef = h3sprite.GetRecord(IMG_FNAME)?.GetDefFile(h3sprite);             

            var bmp = creatureDef.GetByAbsoluteNumber(index + 2);
            return bmp;
        }

        public static Bitmap GetImage2(LodFile h3sprite, int creatureIndex)
        {
            if (creatureDef == null)
                creatureDef = h3sprite.GetRecord(IMG_FNAME)?.GetDefFile(h3sprite);

            var bmp = creatureDef.GetByAbsoluteNumber(AllCreatures[creatureIndex].CreatureIndex + 2);
            return bmp;
        }

        private static Bitmap _allCreatures;
        public static Bitmap GetAllCreaturesBitmap(LodFile h3sprite)
        {
            var sw = Stopwatch.StartNew();
            if (_allCreatures != null)
                return _allCreatures;
            if (creatureDef == null)
                creatureDef = h3sprite.GetRecord(IMG_FNAME)?.GetDefFile(h3sprite);            

            int totalrows = AllCreatures.Count / 14 + (AllCreatures.Count % 14 == 0 ? 0 : 1);
            _allCreatures = new Bitmap((58 + 1) * 14, (64 + 1) * totalrows);
            using (var g = Graphics.FromImage(_allCreatures))
            {
                for (int i = 0; i < AllCreatures.Count; i++)
                {
                    int row = i / 14;
                    int col = i % 14;
                    var img = creatureDef.GetByAbsoluteNumber(AllCreatures[i].CreatureIndex + 2);
                    if (img != null)
                    {
                        g.DrawImage(img, col * (58 + 1), row * (64 + 1));
                    }
                    else
                    {

                    }
                }
            }
            Debug.WriteLine("allcrbmp: " + sw.ElapsedMilliseconds);
            return _allCreatures;
        }




    }
}
