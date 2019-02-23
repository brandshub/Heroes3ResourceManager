using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h3magic
{
    public static class CreatureManager
    {
        private const string FAT_NAME = "CRTRAITS.TXT";
        private const string IMG_FNAME = "TwCrPort.def";
        private const string IMG_SMALL_FNAME = "CPRSMALL.def";

        public static readonly string[] Castles = { "Castle", "Rampart", "Tower", "Inferno", "Necropolis", "Dungeon", "Stronghold", "Fortress", "Other" };

        public static List<Creature> OnlyActiveCreatures = new List<Creature>();
        public static Bitmap[] SmallImages = null;
        public static Creature[] AllCreatures2 = null;
        public static int[] IndexesOfFirstLevelCreatures;

        private static DefFile creatureDef;
        private static DefFile smallCreatureDef;

        private static string[] rows;

        public static bool Loaded { get; private set; }
        public static bool HasChanges = false;



        public static void LoadInfo(LodFile file)
        {
            var rec = file.GetRecord(FAT_NAME);
            if (rec != null)
            {
                string text = Encoding.Default.GetString(rec.GetRawData(file.stream));
                rows = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                AllCreatures2 = new Creature[rows.Length];
                OnlyActiveCreatures.Clear();

                int curIndex = 0;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 14; j++)
                    {
                        var stat = new Creature(rows[2 + i * 17 + j]) { TownIndex = i, CreatureIndex = curIndex++, CreatureCastleRelativeIndex = j };

                        AllCreatures2[stat.CreatureIndex] = stat;
                        OnlyActiveCreatures.Add(stat);
                    }
                }
                int off = 2 + 17 * 8;

                int ccri = 0;
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
                            var stat = new Creature(rows[i]) { TownIndex = 8, CreatureIndex = curIndex, CreatureCastleRelativeIndex = ccri };
                            AllCreatures2[stat.CreatureIndex] = stat;
                            misc.Add(stat);
                            ccri++;
                        }
                        curIndex++;
                    }
                }
                OnlyActiveCreatures.AddRange(misc.Where(s => s != null).ToArray());

                var indexes = new List<int>(100);
                for (int i = 0; i < OnlyActiveCreatures.Count; i++)
                {
                    if (i < 112 && i % 2 == 1)
                    {
                        continue;
                    }
                    if (i == 119 || (i >= 121 && i <= 125) || i == 127)
                    {
                        continue;
                    }
                    indexes.Add(OnlyActiveCreatures[i].CreatureIndex);
                }
                IndexesOfFirstLevelCreatures = indexes.ToArray();

                Loaded = true;
                SmallImages = new Bitmap[AllCreatures2.Length];
            }


        }

        public static Creature Get(int castleIndex, int relativeCreatureIndex)
        {
            return OnlyActiveCreatures.FirstOrDefault(c => c.TownIndex == castleIndex && c.CreatureCastleRelativeIndex == relativeCreatureIndex);
        }

        public static Creature GetByIndex(int index)
        {
            return OnlyActiveCreatures[index];
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
                creatureDef = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite);

            var bmp = creatureDef.GetByAbsoluteNumber(index + 2);
            return bmp;
        }

        public unsafe static Bitmap GetSmallImage(LodFile h3sprite, int creatureIndex)
        {
            if (smallCreatureDef == null)
                smallCreatureDef = h3sprite.GetRecord(IMG_SMALL_FNAME).GetDefFile(h3sprite);

            Bitmap bmp;
            if (SmallImages[creatureIndex] == null)
            {

                bmp = smallCreatureDef.GetByAbsoluteNumber(creatureIndex + 2);
                var creature = AllCreatures2[creatureIndex];
                var clr = Town.AllColors[creature.TownIndex];

                var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int i = 0; i < imageData.Height; i++)
                {
                    byte* offset = (byte*)imageData.Scan0 + i * imageData.Stride;
                    for (int j = 0; j < imageData.Width; j++)
                    {
                        byte b = *offset;
                        byte g = *(offset + 1);
                        byte r = *(offset + 2);
                        if (r == 0 && g == 0xff && b == 0xff)
                        {
                            *(offset++) = clr.B;
                            *(offset++) = clr.G;
                            *(offset++) = clr.R;
                        }
                        else
                        {
                            offset += 3;
                        }
                    }

                }
                bmp.UnlockBits(imageData);
                SmallImages[creatureIndex] = bmp;
            }

            return SmallImages[creatureIndex];
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
            if (_allCreatures != null)
                return _allCreatures;

            if (creatureDef == null)
                creatureDef = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite);

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
            }
            _allCreatures.UnlockBits(imageData);
            return _allCreatures;
        }


        public static Bitmap GetAllCreaturesBitmapParallel(LodFile h3sprite)
        {
            var sw = Stopwatch.StartNew();
            if (_allCreatures != null)
                return _allCreatures;

            if (creatureDef == null)
                creatureDef = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite);

            int totalrows = OnlyActiveCreatures.Count / 14 + (OnlyActiveCreatures.Count % 14 == 0 ? 0 : 1);
            _allCreatures = new Bitmap((58 + 1) * 14, (64 + 1) * totalrows);
            var imageData = _allCreatures.LockBits(new Rectangle(0, 0, _allCreatures.Width, _allCreatures.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Parallel.For(0, OnlyActiveCreatures.Count, i =>
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
                });

            _allCreatures.UnlockBits(imageData);
            return _allCreatures;
        }

        private static Bitmap _allUnUpgradedCreatures;
        public static Bitmap GetAllBasicCreatures(LodFile h3sprite)
        {
            if (_allUnUpgradedCreatures != null)
                return _allUnUpgradedCreatures;

            int totalrows = IndexesOfFirstLevelCreatures.Length / 14 + (IndexesOfFirstLevelCreatures.Length % 14 == 0 ? 0 : 1);
            _allUnUpgradedCreatures = new Bitmap((58 + 1) * 14, (64 + 1) * totalrows);
            var imageData = _allUnUpgradedCreatures.LockBits(new Rectangle(0, 0, _allUnUpgradedCreatures.Width, _allUnUpgradedCreatures.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Parallel.For((int)0, IndexesOfFirstLevelCreatures.Length, i =>
            {
                int row = i / 14;
                int col = i % 14;
                var img = creatureDef.GetByAbsoluteNumber2(IndexesOfFirstLevelCreatures[i] + 2);
                if (img != null)
                {
                    imageData.DrawImage24(col * (58 + 1), row * (64 + 1), 176, img);
                }
            });
            _allUnUpgradedCreatures.UnlockBits(imageData);
            return _allUnUpgradedCreatures;
        }

        //
    }
}
