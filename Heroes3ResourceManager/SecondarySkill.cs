﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class SecondarySkill
    {
        private const string TXT_FNAME = "SSTRAITS.TXT";
        private const string IMG_FNAME = "Secskill.def";

        public static bool Loaded { get; private set; }

        public static List<SecondarySkill> AllSkills = new List<SecondarySkill>();

        public int Index { get; private set; }
        public string Name { get; private set; }

        public static void LoadInfo(LodFile lodFile)
        {
            var rec = lodFile[TXT_FNAME];
            string text = Encoding.Default.GetString(rec.GetRawData(lodFile.stream));
            var rows = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 2; i < rows.Length; i++)
            {
                string name = rows[i].Split('\t')[0];
                AllSkills.Add(new SecondarySkill { Index = i - 2, Name = name });
            }
            Loaded = true;
        }

        public static Bitmap GetImage(LodFile lodFile, int skillIndex, int level)
        {
            return AllSkills[skillIndex].GetImage(lodFile, level);
        }

        public Bitmap GetImage(LodFile lodFile, int level)
        {
            var def = lodFile?.GetRecord(IMG_FNAME)?.GetDefFile(lodFile);
            return def.GetByAbsoluteNumber(Index * 3 + level + 2);
        }

        public override string ToString()
        {
            return Name;
        }

        private static Bitmap skillTree = null;
        private static Bitmap skillTree2 = null;

        public static Bitmap GetSkillTree(LodFile h3sprite)
        {
            var sw = Stopwatch.StartNew();
            if (skillTree != null)
                return skillTree;

            var def = h3sprite?.GetRecord(IMG_FNAME)?.GetDefFile(h3sprite.stream);
            var bmp = new Bitmap((44 + 60) * 4, 44 * 7);
            using (var g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 7; j++)
                        g.DrawImage(def.GetByAbsoluteNumber(3 + (i * 7 + j) * 3), i * 104, 44 * j);
            }
            skillTree = bmp;
            float ms = sw.ElapsedTicks * 1000.0f / Stopwatch.Frequency;
            Debug.WriteLine("skilltree: " + ms);
            return skillTree;
        }


        public static Bitmap GetSkillTree2(LodFile h3sprite)
        {
            if (skillTree2 != null)
                return skillTree2;

            var def = h3sprite?.GetRecord(IMG_FNAME)?.GetDefFile(h3sprite.stream);
            var sw = Stopwatch.StartNew();
            var bmp = new Bitmap((44 + 1) * 12, (44 + 1) * 7);
            using (var g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < 7; i++)
                    for (int j = 0; j < 12; j++)
                    {
                        var img = def.GetByAbsoluteNumber(3 + i * 12 + j);
                        g.DrawImage(img, j * (44 + 1), i * (44 + 1));
                    }
            }
            skillTree2 = bmp;
            float ms = sw.ElapsedTicks * 1000.0f / Stopwatch.Frequency;
            return skillTree2;
        }
    }
}
