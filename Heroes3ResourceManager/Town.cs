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
        public static Color[] AllColors = new Color[] { Color.LightCyan, Color.PaleGreen, Color.MintCream, Color.MistyRose, Color.Gainsboro, Color.Lavender, Color.Wheat, Color.DarkSeaGreen, Color.LightSkyBlue, Color.LightYellow };

        public Bitmap Image { get; private set; }
        public Bitmap LargeImage { get; private set; }

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
            AllTowns[9].LargeImage = rec.GetSprite(38);
            AllTowns[9].Image = new Bitmap(AllTowns[9].LargeImage, new Size(36, 24));
        }

        public static Bitmap GetAllTownsImage(H3Sprite h3sprite, int hOff, int vOff)
        {
            if (AllTowns == null)
                LoadInfo(h3sprite);

            if (BitmapCache.TownsGrid == null)
            {
                int imgWidth = AllTowns[0].LargeImage.Width;
                int imgHeight = AllTowns[0].LargeImage.Height;

                int width = 3 * imgWidth + 2 * hOff;
                int height = 3 * imgHeight + 2 * vOff;

                var bmp = new Bitmap(width, height);
                using (var g = Graphics.FromImage(bmp))
                {
                    for (int i = 0; i < 9; i++)
                    {
                        g.DrawImage(AllTowns[i].LargeImage, imgWidth * (i % 3) + hOff * (i % 3), (i / 3) * imgHeight + (i / 3) * vOff);
                    }
                }
                BitmapCache.TownsGrid = bmp;
            }
            return BitmapCache.TownsGrid;
        }
    }
}
