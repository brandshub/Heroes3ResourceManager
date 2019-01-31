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

        public static List<Creature> OnlyActiveCreatures = new List<Creature>();
        public static Creature[] AllCreatures2 = null;

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
                    AllCreatures2 = new Creature[rows.Length];

                    int curIndex = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 14; j++)
                        {
                            var stat = new Creature(rows[2 + i * 17 + j]) { CastleIndex = i, CreatureIndex = curIndex++, CreatureCastleRelativeIndex = j };

                            AllCreatures2[stat.CreatureIndex] = stat;
                            OnlyActiveCreatures.Add(stat);
                        }
                    }
                    int off = 2 + 17 * 8;

                    List<Creature> misc = new List<Creature>();
                    for (int i = off; i < rows.Length - 1; i++)
                    {
                        if (rows[i].Contains("\t\t\t"))
                        {
                            misc.Add(null);
                        }
                        else
                        {
                            if (!rows[i].Contains("NOT USED"))
                            {
                                var stat = new Creature(rows[i]) { CastleIndex = 8, CreatureIndex = curIndex, CreatureCastleRelativeIndex = i - off };
                                AllCreatures2[stat.CreatureIndex] = stat;
                                misc.Add(stat);
                            }
                            curIndex++;
                        }
                    }
                    OnlyActiveCreatures.AddRange(misc.Where(s => s != null).ToArray());
                    Loaded = true;
                }
            }

        }

        public static Creature Get(int castleIndex, int relativeCreatureIndex)
        {
            return OnlyActiveCreatures.FirstOrDefault(c => c.CastleIndex == castleIndex && c.CreatureCastleRelativeIndex == relativeCreatureIndex);
        }

        public static Creature GetByIndex(int index)
        {
            return OnlyActiveCreatures?[index];
        }

        public static Creature GetByCreatureIndex(int index)
        {
            return OnlyActiveCreatures.FirstOrDefault(c => c.CreatureIndex == index);
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
                        sb.AppendLine(OnlyActiveCreatures[i * 14 + j].GetRow());
                    sb.AppendLine(rows[16]);
                    sb.AppendLine(rows[16]);
                    sb.AppendLine(rows[16]);
                }
                int off = 2 + 17 * 8;
                int k = 0;

                for (int i = off; i < rows.Length - 1; i++)
                    if (!rows[i].Contains("\t\t"))
                        sb.AppendLine(OnlyActiveCreatures[8 * 14 + k++].GetRow());
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

        public static void Save(LodFile lodfile)
        {
            FatRecord rec = lodfile.GetRecord(FAT_NAME);
            if (rec != null)
            {
                string val = GetAllStats();
                rec.ApplyChanges(Encoding.Default.GetBytes(val));
            }
        }



        private static Bitmap _allCreatures;
        public static Bitmap GetAllCreaturesBitmap(LodFile h3sprite)
        {
            var sw = Stopwatch.StartNew();
              if (_allCreatures != null)
                  return _allCreatures;

            if (creatureDef == null)
                creatureDef = h3sprite.GetRecord(IMG_FNAME)?.GetDefFile(h3sprite);

            int totalrows = OnlyActiveCreatures.Count / 14 + (OnlyActiveCreatures.Count % 14 == 0 ? 0 : 1);
            _allCreatures = new Bitmap((58 + 1) * 14, (64 + 1) * totalrows);
            var imageData = _allCreatures.LockBits(new Rectangle(0, 0, _allCreatures.Width, _allCreatures.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            for (int i = 0; i < OnlyActiveCreatures.Count; i++)
            {
                int row = i / 14;
                int col = i % 14;
                var img = creatureDef.GetByAbsoluteNumber2(OnlyActiveCreatures[i].CreatureIndex + 2);
                if (img != null)
                {
                    imageData.DrawImage24(col * (58 + 1), row * (64 + 1), 176, img);
                }
                else
                {

                }
            }
            _allCreatures.UnlockBits(imageData);
            return _allCreatures;
        }
    }
}
