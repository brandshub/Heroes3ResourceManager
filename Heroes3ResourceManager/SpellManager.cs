using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace h3magic
{
    static class SpellManager
    {
        private const string FNAME = "SPTRAITS.TXT";
        public static bool Loaded = false;
        private static string[] rows;
        public static List<SpellStats> stats;

        public static void LoadInfo(LodFile lodfile)
        {
            if (!Loaded)
            {
                FATRecord rec = lodfile[FNAME];

                string text = Encoding.Default.GetString(rec.GetRawData(lodfile.stream));
                rows = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                stats = new List<SpellStats>(rows.Length);

                for (int i = 0; i < 11; i++)
                    stats.Add(new SpellStats(rows[i + 5]));


                Loaded = true;
            }

        }
    }
}
