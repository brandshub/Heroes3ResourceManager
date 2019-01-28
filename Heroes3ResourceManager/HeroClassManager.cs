﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace h3magic
{
    public static class HeroClassManager
    {
        private const string TXT_FNAME = "HCTRAITS.TXT";

        public static bool Loaded = false;
        private static string[] rows;

        public static List<HeroClass> AllClasses;


        public static void LoadInfo(LodFile lodfile)
        {
            if (!Loaded)
            {
                FatRecord rec = lodfile[TXT_FNAME];
                string text = Encoding.Default.GetString(rec.GetRawData(lodfile.stream));
                rows = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                AllClasses = new List<HeroClass>(rows.Length);
                for (int i = 0; i < rows.Length - 2; i++)
                    AllClasses.Add(new HeroClass(rows[i + 2]));
                Loaded = true;
            }
        }


        public static void Save(LodFile lodfile)
        {
            var sb = new StringBuilder();
            sb.AppendLine(rows[0]);
            sb.AppendLine(rows[1]);
            for (int i = 0; i < AllClasses.Count; i++)
                sb.AppendLine(AllClasses[i].GetRow());
            lodfile[TXT_FNAME].ApplyChanges(Encoding.Default.GetBytes(sb.ToString()));

        }
    }
}
