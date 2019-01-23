using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace h3magic
{
    public class HeroStats
    {
        public string Name;
        public string Biography;
        public string Speciality;
        public int LowStack1;
        public int LowStack2;
        public int LowStack3;
        public int HighStack1;
        public int HighStack2;
        public int HighStack3;

        public int ImageIndex;
        public string CastleIndex;

        public Bitmap Large = null;
        public Bitmap Small = null;

        public HeroExeData Data1;

        public override string ToString()
        {
            return Name;
        }

    }
}
