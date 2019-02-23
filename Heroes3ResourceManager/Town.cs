using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class Town
    {
        private const string IMG_FNAME = "ITPA.def";

        public static Town[] AllTowns = new Town[10];
        public static Color[] AllColors = new Color[] { Color.LightCyan, Color.PaleGreen, Color.MintCream, Color.MistyRose, Color.Gainsboro, Color.Lavender, Color.Wheat, Color.DarkSeaGreen, Color.WhiteSmoke, Color.LightGray };

        public Bitmap Image;
        public Bitmap LargeImage;

        public static void LoadInfo(H3Sprite h3sprite)
        {
            var rec = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite.stream);

            for (int i = 0; i < AllTowns.Length; i++)
            {
                var bmp = rec.GetSprite(0, 2 + i * 2);
                AllTowns[i] = new Town
                {
                    Image = new Bitmap(bmp, new Size(36, 24)),
                    LargeImage = bmp
                };
            }

            AllTowns[9].Image = rec.GetSprite(0);
        }
    }
}
