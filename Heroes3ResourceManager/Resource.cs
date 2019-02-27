using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h3magic
{
    public class Resource
    {
        public const string IMG_FNAME = "Resour82.def";

        private static DefFile defFile;
        
        public static Bitmap GetAllResources(LodFile h3sprite)
        {
            if (BitmapCache.ResourcesAll != null)
                return BitmapCache.ResourcesAll;

            if (defFile == null)
                defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite);

            var bmp = new Bitmap((82 + 1) * 7, 93, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Parallel.For(0, 7, i =>
            {
                var img = defFile.GetByAbsoluteNumber2(i);
                imageData.DrawImage24(i * (82 + 1), 0, 248, img);
            });

            bmp.UnlockBits(imageData);
            BitmapCache.ResourcesAll = bmp;

            return BitmapCache.ResourcesAll;
        }

        public static Bitmap GetImage(LodFile h3sprite, int index)
        {
            if (defFile == null)
                defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite);

            return defFile.GetByAbsoluteNumber(index);
        }

        public static void Unload()
        {
            defFile = null;
            BitmapCache.ResourcesAll = null;
        }
    }
}
