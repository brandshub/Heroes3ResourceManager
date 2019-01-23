using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace h3magic
{
    class DEFFile
    {
        private const int PALETTE_OFFSET = 0x10;
        private const int BLOCK_HEADER_OFFSET = 0x310;

        public int ID { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BlockCount { get; private set; }

        public List<SpriteBlockHeader> headers;

        private byte[,] palette = new byte[256, 3];
        private byte[] bytes;


        public DEFFile(byte[] block)
        {
            ID = BitConverter.ToInt32(block, 0);
            Width = BitConverter.ToInt32(block, 4);
            Height = BitConverter.ToInt32(block, 8);
            BlockCount = BitConverter.ToInt32(block, 12);
            bytes = block;
            headers = new List<SpriteBlockHeader>(BlockCount);
            int off = 0;
            for (int i = 0; i < BlockCount; i++)
            {
                headers.Add(new SpriteBlockHeader(block, BLOCK_HEADER_OFFSET + off));
                off += headers.Last().HeaderLength;
            }
            LoadPalette();
        }


        public Bitmap GetByName(string name)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                int index = Array.IndexOf<string>(headers[i].Names, name);
                if (index >= 0)
                {
                    return GetSprite(i, index);
                }
            }
            return null;
        }

        public Bitmap GetByAbsoluteNumber(int number)
        {

            for (int i = 0; i < headers.Count; i++)
            {
                if (number >= headers[i].SpritesCount)
                    number -= headers[i].SpritesCount;
                else
                    return GetSprite(i, number);
            }
            return null;
        }

        public void LoadPalette()
        {
            for (int i = 0; i < 256; i++)
            {
                palette[i, 0] = bytes[PALETTE_OFFSET + i * 3];
                palette[i, 1] = bytes[PALETTE_OFFSET + i * 3 + 1];
                palette[i, 2] = bytes[PALETTE_OFFSET + i * 3 + 2];
            }
        }

        public unsafe Bitmap GetSprite(int blockIndex, int spriteIndex)
        {
            SpriteBlockHeader sbh = headers[blockIndex];
            SpriteHeader sh = sbh.spriteHeaders[spriteIndex];
            int offset = sbh.Offsets[spriteIndex];

            Bitmap bmp = new Bitmap(sh.FullWidth, sh.FullHeight, PixelFormat.Format24bppRgb);
            Color c = Color.FromArgb(palette[0, 0], palette[0, 1], palette[0, 2]);
            Graphics.FromImage(bmp).FillRectangle(new SolidBrush(c), 0, 0, sh.FullWidth, sh.FullHeight);
            BitmapData imageData = bmp.LockBits(new Rectangle(0, 0, sh.FullWidth, sh.FullHeight), ImageLockMode.ReadOnly, bmp.PixelFormat);


            offset += 32;

            if (sh.Type == 1)
            {

                LoadSpriteType1(sh, imageData, offset);

            }
            else if (sh.Type == 0)
            {
                LoadSpriteType0(sh, imageData, offset);
            }
            else if (sh.Type == 3)
            {
                //   LoadSpriteType3(sh, imageData, offset);
            }
            bmp.UnlockBits(imageData);

            return bmp;
        }

        private unsafe void LoadSpriteType0(SpriteHeader sh, BitmapData data, int offset)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            byte* currentRow = (byte*)data.Scan0.ToPointer();

            int blength;
            int padding = (4 - ((sh.FullWidth * 3) % 4)) % 4;
            int pos = offset;
            for (int i = 0; i < sh.SpriteHeight; i++)
            {
                for (int j = 0; j < sh.SpriteWidth; j++)
                {
                    byte color = bytes[pos++];
                    *currentRow = palette[color, 2];
                    currentRow++;
                    *currentRow = palette[color, 1];
                    currentRow++;
                    *currentRow = palette[color, 0];
                    currentRow++;
                }
                currentRow += 3 * padding;

            }
            double result = watch.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
            Debug.WriteLine("t1: " + result);
        }



        private unsafe void LoadSpriteType1(SpriteHeader sh, BitmapData data, int offset)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            int len = sh.SpriteHeight;

            int[] ffsets = new int[len];
            for (int i = 0; i < len; i++)
                ffsets[i] = BitConverter.ToInt32(bytes, offset + i * 4);

            byte* ptr = (byte*)data.Scan0.ToPointer();
            byte* currentRow;
            byte type;
            int blength;

            for (int i = 0; i < len; i++)
            {
                int pos = ffsets[i] + offset;
                currentRow = ptr + (sh.TopMargin + i) * data.Stride + sh.LeftMargin * 3;

                int bound;
                if (i < len - 1)
                    bound = offset + ffsets[i + 1];
                else
                    bound = Math.Min(offset + ffsets[i] + sh.SpriteWidth, bytes.Length);

                while (pos < bound)
                {
                    type = bytes[pos++];
                    blength = (bytes[pos++] + 1);
                    if (type == 0xff)
                    {
                        for (int j = 0; j < blength; j++)
                        {
                            byte color = bytes[pos];
                            *currentRow = palette[color, 2];
                            currentRow++;
                            *currentRow = palette[color, 1];
                            currentRow++;
                            *currentRow = palette[color, 0];
                            currentRow++;
                            pos++;
                        }
                    }
                    else
                    {
                        currentRow += 3 * blength;
                    }
                }


            }
            double result = watch.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
            Debug.WriteLine("t2: " + result);
        }

        private unsafe void LoadSpriteType3(SpriteHeader sh, BitmapData data, int offset)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            int len = sh.SpriteHeight;

            int[] ffsets = new int[len];
            for (int i = 0; i < len; i++)
                ffsets[i] = BitConverter.ToInt16(bytes, offset + i * 2);

            byte* ptr = (byte*)data.Scan0.ToPointer();
            byte* currentRow;
            byte type;
            int blength;

            for (int i = 0; i < len; i++)
            {
                int pos = ffsets[i] + offset;
                currentRow = ptr + (sh.TopMargin + i) * data.Stride + sh.LeftMargin * 3;

                int bound;
                if (i < len - 1)
                    bound = offset + ffsets[i + 1];
                else
                    bound = Math.Min(offset + ffsets[i] + sh.SpriteWidth, bytes.Length);

                while (pos < bound)
                {
                    blength = (bytes[pos] & 0x1f) + 1;
                    type = (byte)(bytes[pos] >> 5);
                    pos++;
                    if (type == 0x1)
                    {
                        for (int j = 0; j < blength; j++)
                        {

                            *currentRow = palette[1, 2];
                            currentRow++;
                            *currentRow = palette[1, 1];
                            currentRow++;
                            *currentRow = palette[1, 0];
                            currentRow++;
                        }
                    }
                    else if (type == 4)
                    {
                        for (int j = 0; j < blength; j++)
                        {
                            *currentRow = palette[2, 2];
                            currentRow++;
                            *currentRow = palette[2, 1];
                            currentRow++;
                            *currentRow = palette[2, 0];
                            currentRow++;

                        }
                    }
                    else if (type == 7)
                    {
                        for (int j = 0; j < blength; j++)
                        {
                            byte color = bytes[pos];
                            *currentRow = palette[color, 2];
                            currentRow++;
                            *currentRow = palette[color, 1];
                            currentRow++;
                            *currentRow = palette[color, 0];
                            currentRow++;
                            pos++;
                        }
                    }
                    else if (type == 0)
                    {
                        currentRow += 3 * blength;
                    }
                }


            }
            double result = watch.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
            Debug.WriteLine("t1: " + result);
        }

    }
}
