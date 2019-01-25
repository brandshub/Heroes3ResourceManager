﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class SecondarySkill
    {
        private const string H_HEROES = "SSTRAITS.TXT";
        private const string DEF_IMAGES = "Secskill.def";

        public static bool Loaded { get; private set; }

        public static List<SecondarySkill> AllSkills = new List<SecondarySkill>();

        public int Index { get; private set; }
        public string Name { get; private set; }


        public static void LoadInfo(LodFile lodFile)
        {
            var rec = lodFile[H_HEROES];
            string text = Encoding.Default.GetString(rec.GetRawData(lodFile.stream));
            var rows = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 2; i < rows.Length; i++)
            {
                string name = rows[i].Split('\t')[0];
                AllSkills.Add(new SecondarySkill { Index = i - 2, Name = name });
            }
            Loaded = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
