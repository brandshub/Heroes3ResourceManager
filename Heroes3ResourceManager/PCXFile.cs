using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;

namespace h3magic
{
    class PcxFile
    {
        int width;
        int heigth;
        int palletteOffset;
        byte[] bts;

        bool brokenFormat = false;
        public byte[] GetBytes
        {
            get { return bts; }
        }

        public PcxFile(byte[] bytes)
        {
            bts = bytes;
            palletteOffset = BitConverter.ToInt32(bytes, 0);
            if (palletteOffset != 0x46323350)
                palletteOffset += 12;
            else
                brokenFormat = true;
            if (!brokenFormat)
            {
                width = BitConverter.ToInt32(bytes, 4);
                heigth = BitConverter.ToInt32(bytes, 8); //24
            }
            else
            {
                width = BitConverter.ToInt32(bytes, 24);
                heigth = BitConverter.ToInt32(bytes, 28);
            }
        }

        public unsafe Bitmap GetBitmap()
        {
            Bitmap bmp;
            if (!brokenFormat)
            {
                bmp = new Bitmap(width, heigth, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                if (palletteOffset == width * heigth)
                    palletteOffset += 12;

                var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                bool bpp8 = bts.Length == width * heigth + 780;

                byte* ip = (byte*)imageData.Scan0.ToPointer();
                int byteN = 12;
                int padding = (4 - ((width * 3) % 4)) % 4;

                if (bpp8)
                {
                    for (int i = 0; i < heigth; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            int off = palletteOffset + 3 * bts[byteN];
                            *ip = bts[off + 2];
                            ip++;
                            *ip = bts[off + 1];
                            ip++;
                            *ip = bts[off];
                            ip++;
                            byteN++;
                        }
                        ip += padding;
                    }
                }
                else
                {
                    if (palletteOffset == bts.Length)
                    {
                        int nstride = width * 3;
                        for (int i = 0; i < heigth; i++)
                        {
                            Marshal.Copy(bts, 12 + i * nstride, IntPtr.Add(new IntPtr(ip), i * imageData.Stride), nstride);
                        }
                    }
                    else
                    {
                        Marshal.Copy(bts, 12, new IntPtr(ip), bts.Length - 12);
                    }
                }


                bmp.UnlockBits(imageData);

            }
            else
            {
                bmp = new Bitmap(width, heigth, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                for (int i = 0; i < heigth;i++ )
                {
                    Marshal.Copy(bts, bts.Length - (i+1) * imageData.Stride, imageData.Scan0 + i * imageData.Stride, imageData.Stride);
                }
                   
                bmp.UnlockBits(imageData);
            }

            return bmp;
        }

        public unsafe byte[] GetBitmap24Bytes(out int imageWidth)
        {
            if (palletteOffset == width * heigth)
                palletteOffset += 12;

            imageWidth = width;

            int padding = (4 - ((width * 3) % 4)) % 4;
            int stride = 3 * width + padding;

            byte[] data = new byte[heigth * stride];
            bool bpp8 = bts.Length == width * heigth + 780;


            int ip = 0;
            int byteN = 12;


            if (bpp8)
            {
                for (int i = 0; i < heigth; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int off = palletteOffset + 3 * bts[byteN];
                        data[ip++] = bts[off + 2];
                        data[ip++] = bts[off + 1];
                        data[ip++] = bts[off];
                        byteN++;
                    }
                    ip += padding;
                }
            }
            else
            {
                Buffer.BlockCopy(bts, 12, data, 0, bts.Length - 12);
            }
            return data;
        }

        public static PcxFile FromBitmap(Bitmap bmp)
        {
            PcxFile pcf = new PcxFile(new byte[16]);
            pcf.LoadBitmap(bmp, false);
            return pcf;
        }



        public unsafe int LoadBitmap(Bitmap bmp, bool resizeToOriginal)
        {
            if (resizeToOriginal)
            {
                if (bmp.Width != width || bmp.Height != heigth)
                    bmp = new Bitmap(bmp, width, heigth);
            }
            else
            {
                width = bmp.Width;
                heigth = bmp.Height;
                palletteOffset = width * heigth;
            }


            bts = new byte[780 + width * heigth];

            bts[0] = (byte)(palletteOffset & 0xff);
            bts[1] = (byte)((palletteOffset >> 8) & 0xff);
            bts[2] = (byte)((palletteOffset >> 16) & 0xff);
            bts[3] = (byte)((palletteOffset >> 24) & 0xff);

            bts[4] = (byte)((width) & 0xff);
            bts[5] = (byte)((width >> 8) & 0xff);

            bts[8] = (byte)((heigth) & 0xff);
            bts[9] = (byte)((heigth >> 8) & 0xff);


            int btsN = 12;
            int padding = (4 - ((width * 3) % 4)) % 4;
            var imageData = bmp.LockBits(new Rectangle(0, 0, width, heigth), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            byte[] bitmap = new byte[imageData.Stride * imageData.Height];
            Marshal.Copy(imageData.Scan0, bitmap, 0, bitmap.Length);
            bmp.UnlockBits(imageData);


            int[] pallette = new int[256];
            for (int i = 0; i < pallette.Length; i++)
                pallette[i] = -1;


            int k = 0;
            int pn = 0;
            int val;
            int prevPixel = -1;
            int prevPalletteN = -1;
            int l;

            for (int i = 0; i < heigth; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    val = bitmap[k] | (bitmap[k + 1] << 8) | (bitmap[k + 2] << 16);
                    if (val != prevPixel)
                    {
                        for (l = 0; l <= pn; l++)
                            if (pallette[l] == -1)
                            {
                                pallette[l] = val;
                                prevPalletteN = l;
                                bts[btsN] = (byte)pn;
                                pn++;
                                break;
                            }
                            else if (pallette[l] == val)
                            {
                                bts[btsN] = (byte)l;
                                prevPalletteN = l;
                                break;
                            }
                        if (l == 256)
                        {
                            return -1;
                        }
                        prevPixel = val;
                    }
                    else
                    {
                        bts[btsN] = (byte)prevPalletteN;
                    }
                    btsN++;
                    k += 3;
                }
                k += padding;
            }


            for (int i = 0; i < pn; i++)
            {
                bts[btsN + 2] = (byte)((pallette[i]) & 0xff);
                bts[btsN + 1] = (byte)((pallette[i] >> 8) & 0xff);
                bts[btsN] = (byte)((pallette[i] >> 16) & 0xff);
                btsN += 3;
            }
            return 0;
        }


        public unsafe static void SaveBitmap24(string fileName, Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int padding = (4 - ((width * 3) % 4)) % 4;
            int realSize = (width * 3 + padding) * height;
            byte[] rawData = new byte[54 + realSize];
            rawData[0] = 66;
            rawData[1] = 77;
            rawData[2] = (byte)(rawData.Length & 0xFF);
            rawData[3] = (byte)((rawData.Length >> 8) & 0xFF);
            rawData[4] = (byte)((rawData.Length >> 16) & 0xFF);
            rawData[5] = (byte)((rawData.Length >> 24) & 0xFF);
            rawData[10] = 54;
            rawData[14] = 40;

            rawData[18] = (byte)(width & 0xFF);
            rawData[19] = (byte)((width >> 8) & 0xFF);
            rawData[20] = (byte)((width >> 16) & 0xFF);
            rawData[21] = (byte)((width >> 24) & 0xFF);
            rawData[22] = (byte)(height & 0xFF);
            rawData[23] = (byte)((height >> 8) & 0xFF);
            rawData[24] = (byte)((height >> 16) & 0xFF);
            rawData[25] = (byte)((height >> 24) & 0xFF);

            rawData[0x1A] = 1;
            rawData[0x1C] = 24;

            rawData[0x22] = (byte)(realSize & 0xFF);
            rawData[0x23] = (byte)((realSize >> 8) & 0xFF);
            rawData[0x24] = (byte)((realSize >> 16) & 0xFF);
            rawData[0x25] = (byte)((realSize >> 24) & 0xFF);


            rawData[0x26] = 0x13;
            rawData[0x27] = 0xB;

            rawData[0x2A] = 0x13;
            rawData[0x2B] = 0xB;

            BitmapData imageData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Marshal.Copy(imageData.Scan0, rawData, 54, realSize);
            bmp.UnlockBits(imageData);
            // File.WriteAllBytes(fileName, rawData);
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Write(rawData, 0, rawData.Length);
            fs.Close();
        }

    }
}
