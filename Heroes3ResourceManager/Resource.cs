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
        private const string IMG_FNAME = "Resour82.def";

        private static DefFile _defFile;
        private static Bitmap _allResources;
        public static Bitmap GetAllResources(LodFile h3sprite)
        {
            if (_allResources != null)
                return _allResources;

            if (_defFile == null)
                _defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite);

            var bmp = new Bitmap((82 + 1) * 7, 93, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Parallel.For(0, 7, i =>
            {
                var img = _defFile.GetByAbsoluteNumber2(i);
                imageData.DrawImage24(i * (82 + 1), 0, 248, img);
            });

            bmp.UnlockBits(imageData);
            _allResources = bmp;

            return _allResources;
        }

        public static Bitmap GetImage(LodFile h3sprite, int index)
        {
            if (_defFile == null)
                _defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite);

            return _defFile.GetByAbsoluteNumber(index);
        }
    }
}
