using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class Town
    {
        public const string IMG_FNAME = "ITPA.def";
        public const string TXT_FNAME = "TownType.txt";

        public static Town[] AllTownsWithNeutral;
        public static Color[] AllColors = new Color[] { Color.LightCyan, Color.PaleGreen, Color.MintCream, Color.MistyRose, Color.Gainsboro, Color.Lavender, Color.Wheat, Color.DarkSeaGreen, Color.LightSkyBlue, Color.LightYellow };
        public static string[] TownNamesWithNeutral;
        public Bitmap Image { get; private set; }
        public Bitmap LargeImage { get; private set; }

        public string Name { get; set; }

        public int Index { get; set; }
        public static void LoadInfo(Heroes3Master master)
        {
            Unload();
            var lodFile = master.Resolve(IMG_FNAME);
            var txtLodFile = master.Resolve(TXT_FNAME);

           string[] names = Encoding.Default.GetString(txtLodFile.GetRawData(TXT_FNAME)).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            AllTownsWithNeutral = new Town[10];
            var rec = lodFile.GetRecord(IMG_FNAME).GetDefFile(lodFile.stream);

            for (int i = 0; i < AllTownsWithNeutral.Length - 1; i++)
            {
                var bmp = rec.GetSprite(0, 2 + i * 2);
                AllTownsWithNeutral[i] = new Town
                {
                    Index = i,
                    Name = names[i],
                    Image = new Bitmap(bmp, new Size(36, 24)),
                    LargeImage = bmp
                };
            }

            AllTownsWithNeutral[9] = new Town
            {
                Index = 9,
                Name = "Neutral",
                LargeImage = rec.GetSprite(38)             
            };

            AllTownsWithNeutral[9].Image = new Bitmap(AllTownsWithNeutral[9].LargeImage, new Size(36, 24));

            TownNamesWithNeutral = AllTownsWithNeutral.Select(s => s.Name).ToArray();
            
        }

        public static Bitmap GetAllTownsImage(int hOff, int vOff)
        {
            if (BitmapCache.TownsGrid == null)
            {
                int imgWidth = AllTownsWithNeutral[0].LargeImage.Width;
                int imgHeight = AllTownsWithNeutral[0].LargeImage.Height;

                int width = 3 * imgWidth + 2 * hOff;
                int height = 3 * imgHeight + 2 * vOff;

                var bmp = new Bitmap(width, height);
                using (var g = Graphics.FromImage(bmp))
                {
                    for (int i = 0; i < 9; i++)
                    {
                        g.DrawImage(AllTownsWithNeutral[i].LargeImage, imgWidth * (i % 3) + hOff * (i % 3), (i / 3) * imgHeight + (i / 3) * vOff);
                    }
                }
                BitmapCache.TownsGrid = bmp;
            }
            return BitmapCache.TownsGrid;
        }

        public static void Unload()
        {
            BitmapCache.TownsGrid = null;
            if (AllTownsWithNeutral != null)
            {
                foreach (var town in AllTownsWithNeutral)
                {
                    if (town != null)
                    {
                        if (town.Image != null)
                            town.Image.Dispose();

                        if (town.LargeImage != null)
                            town.LargeImage.Dispose();
                    }

                }
            }
            AllTownsWithNeutral = null;
            TownNamesWithNeutral = null;
        }
    }
}
