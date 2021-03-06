﻿using System;
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

        public int Index { get; set; }
        public string Name { get { return Stats[0]; } }
        public static bool AnyChanges { get; set; }
        public int Attack { get { return GetStat(2); } }
        public int Defense { get { return GetStat(3); } }
        public int MagicPower { get { return GetStat(4); } }
        public int Knowledge { get {  return GetStat(5);} }

        public int Mana { get { return Knowledge * 10; } }

        public string[] Stats;

        public HeroClass(int index, string row)
        {
            Index = index;
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
            AnyChanges = false;

            var rec = master.ResolveWith(TXT_FNAME);
            string text = Encoding.Default.GetString(rec.GetRawData());
            rows = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            AllHeroClasses = new List<HeroClass>(rows.Length);
            for (int i = 0; i < rows.Length - 2; i++)
                AllHeroClasses.Add(new HeroClass(i,rows[i + 2]));
        }


        public static void SaveLocalChanges(Heroes3Master master)
        {
            var sb = new StringBuilder();
            sb.AppendLine(rows[0]);
            sb.AppendLine(rows[1]);
            for (int i = 0; i < AllHeroClasses.Count; i++)
                sb.AppendLine(AllHeroClasses[i].GetRow());

            master.ResolveWith(TXT_FNAME).ApplyChanges(Encoding.Default.GetBytes(sb.ToString()));
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
