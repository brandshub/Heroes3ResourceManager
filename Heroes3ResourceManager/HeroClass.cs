using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace h3magic
{
    public class HeroClass
    {
        private const string TXT_FNAME = "HCTRAITS.TXT";

        private static string[] rows;
        public static List<HeroClass> AllHeroClasses;

        public static bool HasChanges { get; set; }
        public int Attack { get { return GetStat(2); } }
        public int Defense { get { return GetStat(3); } }
        public int MagicPower { get { return GetStat(4); } }
        public int Knowledge { get {  return GetStat(5);} }

        public int Mana { get { return Knowledge * 10; } }

        public string[] Stats;

        public HeroClass(string row)
        {
            Stats = row.Split('\t');
        }

        public string GetRow()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Stats.Length - 1; i++)
            {
                sb.Append(Stats[i]);
                sb.Append('\t');
            }
            sb.Append(Stats.Last());
            return sb.ToString();
        }

        public static void LoadInfo(Heroes3Master master)
        {
            var lodFile = master.Resolve(TXT_FNAME);

            var rec = lodFile[TXT_FNAME];
            string text = Encoding.Default.GetString(rec.GetRawData(lodFile.stream));
            rows = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            AllHeroClasses = new List<HeroClass>(rows.Length);
            for (int i = 0; i < rows.Length - 2; i++)
                AllHeroClasses.Add(new HeroClass(rows[i + 2]));
        }


        public static void Save(LodFile h3Bitmap)
        {
            var sb = new StringBuilder();
            sb.AppendLine(rows[0]);
            sb.AppendLine(rows[1]);
            for (int i = 0; i < AllHeroClasses.Count; i++)
                sb.AppendLine(AllHeroClasses[i].GetRow());
            h3Bitmap[TXT_FNAME].ApplyChanges(Encoding.Default.GetBytes(sb.ToString()));
        }

        public static HeroClass GetByIndex(int index)
        {
            return AllHeroClasses[index];
        }

        public int GetStat(int index)
        {
            if (Stats != null && index < Stats.Length)
            {
                return int.Parse(Stats[index]);
            }
            return -1;
        }

        public static void Unload()
        {
            AllHeroClasses = null;
            rows = null;
        }
        public override string ToString()
        {
            return Stats == null ? "" : Stats[0];
        }
    }
}
