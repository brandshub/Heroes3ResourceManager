using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class Speciality
    {
        private const string H_SPECSFILE = "UN44.def";
        private const string H_SPECSFILESMALL = "UN32.def";

        public static Bitmap Get(LodFile lodFile, int index)
        {
            var rec = lodFile.GetRecord(H_SPECSFILE);
            var def = rec?.GetDEFFile(lodFile.stream);
            return def?.GetByAbsoluteNumber(index);            
        }
    }
}
