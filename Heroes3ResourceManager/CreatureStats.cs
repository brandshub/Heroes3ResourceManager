using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class CreatureStats
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int Attack { get; set; }
        public int Defence { get; set; }
        public int HP { get; set; }
        public int LoDamage { get; set; }
        public int HiDamage { get; set; }
        public int Speed { get; set; }
        public int Growth { get; set; }

        public int PriceLumber { get; set; }
        public int PriceOre { get; set; }
        public int PriceSulphur { get; set; }
        public int PriceMercury { get; set; }
        public int PriceCrystals { get; set; }
        public int PriceGems { get; set; }
        public int PriceGold { get; set; }

        public int Arrows { get; set; }
        public int Spells { get; set; }

        public int AIValue { get; set; }
        public int FightValue { get; set; }

        public string Plural1 { get; set; }
        public string Plural2 { get; set; }

        private string low;
        private string high;
        private string attributes;
        public string hordeGrowth;

        public int CastleIndex { get; set; }
        public int CreatureIndex { get; set; }
        public int CreatureCastleRelativeIndex { get; set; }

        public CreatureStats(string row)
        {
            //		Attack	Defense	Low	High	Shots	Spells	Low	High	Ability Text	Attributes (Reference only, do not change these values)
            string[] stats = row.Split('\t');
            Name = stats[0];
            Plural1 = stats[1];
            Plural2 = stats[2];
            PriceLumber = int.Parse(stats[3]);
            PriceMercury = int.Parse(stats[4]);
            PriceOre = int.Parse(stats[5]);
            PriceSulphur = int.Parse(stats[6]);
            PriceCrystals = int.Parse(stats[7]);
            PriceGems = int.Parse(stats[8]);
            PriceGold = int.Parse(stats[9]);

            FightValue = int.Parse(stats[10]);
            AIValue = int.Parse(stats[11]);

            Growth = int.Parse(stats[12]);
            hordeGrowth = stats[13];
            HP = int.Parse(stats[14]);
            Speed = int.Parse(stats[15]);
            Attack = int.Parse(stats[16]);
            Defence = int.Parse(stats[17]);
            LoDamage = int.Parse(stats[18]);
            HiDamage = int.Parse(stats[19]);
            Arrows = int.Parse(stats[20]);
            Spells = int.Parse(stats[21]);
            low = stats[22];
            high = stats[23];
            Description = stats[24];
            attributes = stats[25];
        }

        public string GetRow()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name); sb.Append('\t');
            sb.Append(Plural1); sb.Append('\t');
            sb.Append(Plural2); sb.Append('\t');
            sb.Append(PriceLumber.ToString()); sb.Append('\t');
            sb.Append(PriceMercury.ToString()); sb.Append('\t');
            sb.Append(PriceOre.ToString()); sb.Append('\t');
            sb.Append(PriceSulphur.ToString()); sb.Append('\t');
            sb.Append(PriceCrystals.ToString()); sb.Append('\t');
            sb.Append(PriceGems.ToString()); sb.Append('\t');
            sb.Append(PriceGold.ToString()); sb.Append('\t');
            sb.Append(FightValue.ToString()); sb.Append('\t');
            sb.Append(AIValue.ToString()); sb.Append('\t');
            sb.Append(Growth.ToString()); sb.Append('\t');
            sb.Append(hordeGrowth.ToString()); sb.Append('\t');
            sb.Append(HP.ToString()); sb.Append('\t');
            sb.Append(Speed.ToString()); sb.Append('\t');
            sb.Append(Attack.ToString()); sb.Append('\t');
            sb.Append(Defence.ToString()); sb.Append('\t');
            sb.Append(LoDamage.ToString()); sb.Append('\t');
            sb.Append(HiDamage.ToString()); sb.Append('\t');
            sb.Append(Arrows.ToString()); sb.Append('\t');
            sb.Append(Spells.ToString()); sb.Append('\t');
            sb.Append(low.ToString()); sb.Append('\t');
            sb.Append(high.ToString()); sb.Append('\t');
            sb.Append(Description.ToString()); sb.Append('\t');
            sb.Append(attributes.ToString());
            return sb.ToString();
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
