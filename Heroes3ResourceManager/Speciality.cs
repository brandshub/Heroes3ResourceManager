using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class Speciality
    {
        private const string IMG_FNAME = "UN44.def";
        private const string IMG_FNAME_SMALL = "UN32.def";
        private const int BLOCK_SIZE = 40;

        //0 - skill, 1 - creature, 2 - +350, 3 - spell , 4 - specific elems/devils/etc
        public int Type { get; private set; }
        public int ObjectId { get; private set; }


        public static Bitmap GetImage(LodFile h3sprite, int index)
        {
            var rec = h3sprite.GetRecord(IMG_FNAME);
            var def = rec?.GetDEFFile(h3sprite.stream);
            return def?.GetByAbsoluteNumber(index);            
        }

        private static Bitmap allSpecs = null;

        public static Bitmap GetAllSpecs(LodFile h3sprite)
        {
            if (allSpecs != null)
                return allSpecs;

            var bmp = new Bitmap(16*44, 44 * 9);
            using (var g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 16; j++)
                        g.DrawImage(GetImage(h3sprite,i*16+j), j * 44, 44 * i);
            }
            allSpecs = bmp;
            return allSpecs;
        }
    }
}
