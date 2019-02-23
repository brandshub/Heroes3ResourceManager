using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace h3magic
{
    public class DefFile
    {
        private const int PALETTE_OFFSET = 0x10;
        private const int BLOCK_HEADER_OFFSET = 0x310;

        public int ID { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BlockCount { get; private set; }

        public List<SpriteBlockHeader> headers;

        private byte[,] palette = new byte[256, 3];
        private int[] palette2 = new int[256];
        private byte[] bytes;

        private bool hasChanged = false;
        public bool HasChanges
        {
            get
            {
                if (Parent == null)
                    return hasChanged;

                return Parent.HasChanged;
            }
            set
            {
                hasChanged = value;
                if (Parent != null && value)
                    Parent.HasChanged = true;
            }
        }

        public FatRecord Parent { get; set; }

        public DefFile(FatRecord parent, byte[] block)
        {
            Parent = parent;

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

        public byte[] GetByAbsoluteNumber2(int number)
        {

            for (int i = 0; i < headers.Count; i++)
            {
                if (number >= headers[i].SpritesCount)
                    number -= headers[i].SpritesCount;
                else
                    return GetSprite2(i, number);
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

                palette2[i] = palette[i, 2] | (palette[i, 1] << 8) | (palette[i, 0] << 16);
            }
        }

        public Bitmap GetSprite(int frame)
        {
            int blockIndex = 0;
            while (frame > headers[blockIndex].spriteHeaders.Count)
            {
                frame -= headers[blockIndex].spriteHeaders.Count;
                blockIndex++;
            }
            return GetSprite(blockIndex, frame);
        }

        public unsafe Bitmap GetSprite(int blockIndex, int spriteIndex)
        {
            SpriteBlockHeader sbh = headers[blockIndex];

            spriteIndex = spriteIndex % sbh.spriteHeaders.Count;

            var sh = sbh.spriteHeaders[spriteIndex];
            int offset = sbh.Offsets[spriteIndex];

            var bmp = new Bitmap(sh.FullWidth, sh.FullHeight, PixelFormat.Format24bppRgb);
            var color = Color.FromArgb(palette[0, 0], palette[0, 1], palette[0, 2]);
            using (var g = Graphics.FromImage(bmp))
                g.FillRectangle(new SolidBrush(color), 0, 0, sh.FullWidth, sh.FullHeight);

            var imageData = bmp.LockBits(new Rectangle(0, 0, sh.FullWidth, sh.FullHeight), ImageLockMode.ReadWrite, bmp.PixelFormat);

            offset += 32;
            //TODO apply same byte logic as for loadsprite12
            if (sh.Type == 0)
            {
                LoadSpriteType0(sh, imageData, offset);
            }
            else if (sh.Type == 1)
            {
                /*  var d = LoadSpriteType12(sh, bytes, offset);
                  Marshal.Copy(d, 0, imageData.Scan0, d.Length);*/
                LoadSpriteType1(sh, imageData, offset);
            }
            else if (sh.Type == 2)
            {
                LoadSpriteType2(sh, imageData, offset);
            }
            else if (sh.Type == 3)
            {
                LoadSpriteType3(sh, imageData, offset);
            }
            bmp.UnlockBits(imageData);
            return bmp;
        }

        public unsafe byte[] GetSprite2(int blockIndex, int spriteIndex)
        {
            SpriteBlockHeader sbh = headers[blockIndex];
            spriteIndex = spriteIndex % sbh.spriteHeaders.Count;

            var sh = sbh.spriteHeaders[spriteIndex];
            int offset = sbh.Offsets[spriteIndex];
            offset += 32;

            if (sh.Type == 1)
                return LoadSpriteType12(sh, bytes, offset);

            throw new Exception("wrong type");
        }


        private unsafe void LoadSpriteType0(SpriteHeader sh, BitmapData data, int offset)
        {
            var watch = Stopwatch.StartNew();

            byte* currentRow = (byte*)data.Scan0.ToPointer();

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
            double result = watch.ElapsedTicks / (double)Stopwatch.Frequency;
            //Debug.WriteLine("t0: " + result);
        }

        private byte[,] LoadSpriteType0(SpriteHeader sh, byte[] data, int offset)
        {
            var sw = Stopwatch.StartNew();
            var imageBytes = new byte[sh.FullHeight, sh.FullWidth * 3];
            int padding = ((4 - ((sh.FullWidth * 3) % 4)) % 4) * 3;

            int pos = offset;
            for (int i = 0; i < sh.SpriteHeight; i++)
            {
                int currentCol = 0;
                for (int j = 0; j < sh.SpriteWidth; j++)
                {
                    byte color = bytes[pos++];
                    imageBytes[i, currentCol++] = palette[color, 2];
                    imageBytes[i, currentCol++] = palette[color, 1];
                    imageBytes[i, currentCol++] = palette[color, 0];
                }

                for (int j = 0; j < padding; j++)
                {
                    imageBytes[i, currentCol++] = palette[0, 2];
                    imageBytes[i, currentCol++] = palette[0, 1];
                    imageBytes[i, currentCol++] = palette[0, 0];
                }
            }
            double result = sw.ElapsedTicks / (double)Stopwatch.Frequency;
            Debug.WriteLine("LoadSpriteType0: " + result);
            return imageBytes;
        }

        private unsafe void LoadSpriteType1(SpriteHeader sh, BitmapData data, int offset)
        {
            var watch = Stopwatch.StartNew();
            int len = sh.SpriteHeight;
            int tm = sh.TopMargin;
            int lm = sh.LeftMargin;
            int sw = sh.SpriteWidth;

            if (sh.SpriteHeight > sh.FullHeight)
            {
                len = sh.FullHeight;
                tm = 0;
                lm = 0;
                sw = sh.FullWidth;
                offset -= 16;
            }

            int[] offB = new int[len];
            for (int i = 0; i < len; i++)
                offB[i] = BitConverter.ToInt32(bytes, offset + i * 4);

            byte* ptr = (byte*)data.Scan0.ToPointer();
            byte* currentRow;
            byte type;
            int blength;

            for (int i = 0; i < len; i++)
            {
                int pos = offB[i] + offset;
                currentRow = ptr + (tm + i) * data.Stride + lm * 3;

                int currentWidth = 0;
                while (currentWidth < sw)
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
                        for (int j = 0; j < blength; j++)
                        {
                            *currentRow = palette[type, 2];
                            currentRow++;
                            *currentRow = palette[type, 1];
                            currentRow++;
                            *currentRow = palette[type, 0];
                            currentRow++;
                        }

                    }
                    currentWidth += blength;
                }
            }
            double result = watch.ElapsedTicks / (double)Stopwatch.Frequency;
            // Debug.WriteLine("t1: " + result);
        }


        private unsafe byte[] LoadSpriteType12(SpriteHeader sh, byte[] data, int offset)
        {
            int len = sh.SpriteHeight;
            int tm = sh.TopMargin;
            int lm = sh.LeftMargin;
            int sw = sh.SpriteWidth;

            if (sh.SpriteHeight > sh.FullHeight)
            {
                len = sh.FullHeight;
                tm = 0;
                lm = 0;
                sw = 0;
                offset -= 16;
            }

            int type, blength;
            int bWidth = sh.FullWidth * 3;
            if (bWidth % 4 != 0)
                bWidth += 4 - bWidth % 4;


            int bw4 = bWidth / 4;

            var imageBytes = new byte[sh.FullHeight * bWidth];

            int[] ffsets = new int[len];
            for (int i = 0; i < len; i++)
                ffsets[i] = BitConverter.ToInt32(bytes, offset + i * 4);

            int currentCol = 0;
            int boundary = imageBytes.Length / 3;


            fixed (byte* ptr = imageBytes)
            {

                /* for (int i = 0; i < len; i++)
                 {
                     int pos = ffsets[i] + offset;
                     currentCol = (tm+i) * bWidth + lm * 3;

                     int bound;
                     if (i < len - 1)
                         bound = offset + ffsets[i + 1];
                     else
                         bound = Math.Min(offset + ffsets[i] + sw, bytes.Length);

                     while (pos < bound)
                     {
                         type = bytes[pos++];
                         blength = (bytes[pos++] + 1);
                         if (type == 0xff)
                         {
                             for (int j = 0; j < blength; j++)
                             {
                                 *((int*)(ptr + currentCol)) = palette2[bytes[pos]];
                                 currentCol += 3;
                                 pos++;
                             }
                         }
                         else
                         {
                             for (int j = 0; j < blength; j++)
                             {
                                 *((int*)(ptr + currentCol)) = palette2[type];
                                 currentCol += 3;
                             }
                         }
                     }
                 }*/

                int pos;
                for (int i = 0; i < len - 1; i++)
                {
                    pos = ffsets[i] + offset;
                    currentCol = (tm + i) * bWidth + lm * 3;

                    int bound;
                    if (i < len - 1)
                        bound = offset + ffsets[i + 1];
                    else
                        bound = Math.Min(offset + ffsets[i] + sw, bytes.Length);

                    while (pos < bound)
                    {
                        type = bytes[pos++];
                        blength = (bytes[pos++] + 1);
                        if (type == 0xff)
                        {
                            for (int j = 0; j < blength; j++)
                            {
                                *((int*)(ptr + currentCol)) = palette2[bytes[pos]];
                                currentCol += 3;
                                pos++;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < blength; j++)
                            {
                                *((int*)(ptr + currentCol)) = palette2[type];
                                currentCol += 3;
                            }
                        }
                    }
                }
                // last row

                pos = ffsets[len - 1] + offset;
                byte* cb = ptr + (tm + len - 1) * bWidth + lm * 3;

                int currentWidth = 0;
                while (currentWidth < sw)
                {
                    type = bytes[pos++];
                    blength = (bytes[pos++] + 1);
                    if (type == 0xff)
                    {
                        for (int j = 0; j < blength; j++)
                        {
                            byte color = bytes[pos];
                            *cb = palette[color, 2];
                            cb++;
                            *cb = palette[color, 1];
                            cb++;
                            *cb = palette[color, 0];
                            cb++;
                            pos++;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < blength; j++)
                        {
                            *cb = palette[type, 2];
                            cb++;
                            *cb = palette[type, 1];
                            cb++;
                            *cb = palette[type, 0];
                            cb++;
                        }

                    }
                    currentWidth += blength;
                }
            }
            return imageBytes;
        }



        private unsafe void LoadSpriteType2(SpriteHeader sh, BitmapData data, int offset)
        {
            var watch = Stopwatch.StartNew();
            int len = sh.SpriteHeight;

            int[] ffsets = new int[len];
            for (int i = 0; i < len; i++)
                ffsets[i] = BitConverter.ToUInt16(bytes, offset + i * 2 * ((sh.SpriteWidth + sh.LeftMargin) / 32));
            //ffsets[i] = BitConverter.ToUInt16(bytes, offset + i * 2 * (sh.SpriteWidth / 32));

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
                    bound = Math.Min(offset + ffsets[i] - sh.SpriteWidth, bytes.Length);

                do
                {

                    blength = (bytes[pos] & 0x1f) + 1;
                    type = (byte)(bytes[pos] >> 5);
                    pos++;
                    if (type == 0)
                    {
                        currentRow += 3 * blength;
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
                    else
                    {
                        int pIndex = 0;
                        if (type == 1)
                            pIndex = 1;
                        else if (type == 4)
                            pIndex = 2;
                        else if (type == 5)
                            pIndex = 5;
                        else if (type == 2)
                        {
                            pIndex = 2;
                        }
                        else if (type == 3)
                        {
                            pIndex = 2;
                        }
                        else
                        {
                            pIndex = 2;
                        }

                        for (int j = 0; j < blength; j++)
                        {
                            *currentRow = palette[pIndex, 2];
                            currentRow++;
                            *currentRow = palette[pIndex, 1];
                            currentRow++;
                            *currentRow = palette[pIndex, 0];
                            currentRow++;
                        }
                    }

                }
                while (pos < bound);


            }
            double result = watch.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
            //Debug.WriteLine("t2: " + result);
        }

        private unsafe void LoadSpriteType3(SpriteHeader sh, BitmapData data, int offset)
        {
            var watch = Stopwatch.StartNew();
            int len = sh.SpriteHeight;

            int[] ffsets = new int[len];
            for (int i = 0; i < len; i++)
                ffsets[i] = BitConverter.ToUInt16(bytes, offset + i * 2 * (sh.SpriteWidth / 32));

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
                    bound = Math.Min(offset + ffsets[i] - sh.SpriteWidth, bytes.Length);

                while (pos < bound)
                {

                    blength = (bytes[pos] & 0x1f) + 1;
                    type = (byte)(bytes[pos] >> 5);
                    pos++;
                    if (type == 0)
                    {
                        currentRow += 3 * blength;
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
                    else
                    {
                        int pIndex = 0;
                        if (type == 1)
                            pIndex = 1;
                        else if (type == 4 || type == 2)
                            pIndex = 2;
                        else if (type == 5)
                            pIndex = 5;
                        else
                        {
                            pIndex = type;
                        }

                        for (int j = 0; j < blength; j++)
                        {
                            *currentRow = palette[pIndex, 2];
                            currentRow++;
                            *currentRow = palette[pIndex, 1];
                            currentRow++;
                            *currentRow = palette[pIndex, 0];
                            currentRow++;
                        }
                    }

                }


            }
            double result = watch.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
            //Debug.WriteLine("t3: " + result);
        }


        public void RetargetSprite(int blockIndex, int oldSpriteIndex, int newSpriteIndex)
        {
            RetargetSprite(this, blockIndex, oldSpriteIndex, newSpriteIndex);

        }

        public void RetargetSprite(DefFile original, int blockIndex, int oldSpriteIndex, int newSpriteIndex)
        {
            HasChanges = true;

            int offset = BLOCK_HEADER_OFFSET;
            for (int i = 0; i < blockIndex; i++)
                offset += headers[i].HeaderLength;

            offset += 16;
            offset += 13 * headers[blockIndex].SpritesCount;
            offset += 4 * oldSpriteIndex;


            int newOffset = original.headers[blockIndex].Offsets[newSpriteIndex];
            var newBytes = BitConverter.GetBytes(newOffset);

            headers[blockIndex].Offsets[oldSpriteIndex] = newOffset;
            Buffer.BlockCopy(newBytes, 0, bytes, offset, 4);

        }


    }
}
