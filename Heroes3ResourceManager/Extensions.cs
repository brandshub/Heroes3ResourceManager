using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace h3magic
{
    public static class Extensions
    {
        public static float ElapsedMs(this Stopwatch stopwatch)
        {
            return stopwatch.ElapsedTicks * 1000.0f / Stopwatch.Frequency;
        }
        
        public static void DrawImage24(this BitmapData data, int x, int y, int stride, byte[] image)
        {
            int h = image.Length / stride;
            for (int i = 0; i < h; i++)
            {
                int offset = i * stride;
                Marshal.Copy(image, offset, IntPtr.Add(data.Scan0, (y + i) * data.Stride + x * 3), stride);
            }
        }

       
    }
}
