using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace h3magic
{
    public class CreatureAnimationLoop : IDisposable
    {

        private static string[] backgroundNames = { "CRBKGCAS.pcx", "CRBKGRAM.pcx", "CRBKGTOW.pcx", "CRBKGINF.pcx", "CRBKGNEC.pcx", "CRBKGDUN.pcx", "CRBKGSTR.pcx", "CRBKGFOR.pcx", "CrBkgEle.pcx", "CRBKGNEU.pcx" };
        private static Bitmap[] backgrounds = new Bitmap[10];

        private const int TIMER_INTERVAL = 200;
        private const int SPRITES_INDEX = 2;
        private Timer timer;

        private Bitmap[] frames;
        private DefFile creatureAnimation;
        public event EventHandler TimerTick;

        public bool Enabled
        {
            get { return timer.Enabled; }
            set { timer.Enabled = value; }
        }

        public int FrameCount { get; private set; }
        public int CurrentFrame { get; private set; }
        public int CreatureIndex { get; private set; }
        public CreatureAnimationLoop(int creatureIndex, DefFile def)
        {
            timer = new Timer();
            timer.Interval = TIMER_INTERVAL;
            timer.Tick += timer_Tick;
            if (def.headers.Count <= SPRITES_INDEX)
                throw new Exception("not a creature");

            creatureAnimation = def;
            CreatureIndex = creatureIndex;
            FrameCount = def.headers[SPRITES_INDEX].SpritesCount;
            frames = new Bitmap[FrameCount];
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            CurrentFrame++;
            if (CurrentFrame >= FrameCount)
                CurrentFrame = 0;

            if (TimerTick != null)
                TimerTick(sender, e);
        }

        public Bitmap GetFrame(Heroes3Master master)
        {
            if (frames == null || CurrentFrame >= frames.Length)
                return null;

            if (frames[CurrentFrame] == null)
            {
                var bmp = creatureAnimation.GetSprite(SPRITES_INDEX, CurrentFrame);
                var spriteData = creatureAnimation.headers[SPRITES_INDEX].spriteHeaders[CurrentFrame];
                int castleIndex = GetBackgroundIndex(CreatureIndex);

                if (backgrounds[castleIndex] == null)
                    backgrounds[castleIndex] = master.H3Bitmap.GetRecord(backgroundNames[castleIndex]).GetBitmap(master.H3Bitmap.stream);

                Point pt;
                Size size;
                ComputeSpriteParameters(creatureAnimation.headers[SPRITES_INDEX], out pt, out size);

                /*if (spriteData.TopMargin > 0 || spriteData.LeftMargin > 0)
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.DrawRectangle(Pens.Red, new Rectangle(spriteData.LeftMargin, spriteData.TopMargin, spriteData.SpriteWidth, spriteData.SpriteHeight));
                    }
                }*/

                // frames[CurrentFrame] = DrawTransparent(backgrounds[castleIndex], bmp, spriteData);
                //  frames[CurrentFrame] = bmp;
                frames[CurrentFrame] = DrawTransparent(backgrounds[castleIndex], bmp, pt, size);
                using (var g = Graphics.FromImage(frames[CurrentFrame]))
                    g.DrawRectangle(Pens.Black, 0, 0, 100-1, 130-1);

            }

            return frames[CurrentFrame];
        }

        private static int GetBackgroundIndex(int creatureIndex)
        {
            if (creatureIndex < 8 * 14)
                return creatureIndex / 14;
            if (creatureIndex <= 131)
                return creatureIndex == 116 || creatureIndex == 117 || creatureIndex == 122 || creatureIndex == 124 || creatureIndex == 126 ? 9 : 8;
            return 9;
        }

        /* private unsafe static Bitmap DrawTransparent(Bitmap background, Bitmap creatureFrame, SpriteHeader spriteData)
         {
             var result = new Bitmap(background);
             var backData = result.LockBits(new Rectangle(0, 0, background.Width, background.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
             var imgData = creatureFrame.LockBits(new Rectangle(0, 0, creatureFrame.Width, creatureFrame.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

             int offX = 0;// backData.Width - spriteData.SpriteWidth;


             byte* backPtr = (byte*)backData.Scan0.ToPointer();
             byte* crPtr = (byte*)imgData.Scan0.ToPointer();
             byte* backOffset, sprOffset;

             for (int i = 0; i < spriteData.SpriteHeight; i++)
             {
                 backOffset = backPtr + i * backData.Stride + 3 * offX;
                 sprOffset = crPtr + (i + spriteData.TopMargin) * imgData.Stride + 3 * spriteData.LeftMargin;

                 for (int j = 0; j < spriteData.SpriteWidth; j++)
                 {
                     byte b = *(sprOffset++);
                     byte g = *(sprOffset++);
                     byte r = *(sprOffset++);

                     if (!(r == 0 && g == 255 && b == 255) && !(r == 255 && g == 255 && b == 0))
                     {
                         if ((r == 255 && b == 255 && (g == 0 || g == 150)) || (r == 180 && g == 0 && b == 255) || (r == 0 && b == 0 && g == 255))
                         {
                             *backOffset = 0;
                             *(backOffset + 1) = 0;
                             *(backOffset + 2) = 0;
                         }
                         else
                         {
                             *backOffset = b;
                             *(backOffset + 1) = g;
                             *(backOffset + 2) = r;
                         }
                     }

                     backOffset += 3;
                 }
             }


             creatureFrame.UnlockBits(imgData);
             result.UnlockBits(backData);
             return result;
         }*/

        private void ComputeSpriteParameters(SpriteBlockHeader block, out Point offset, out Size size)
        {
            int x = block.spriteHeaders.Min(z => z.LeftMargin);
            int y = block.spriteHeaders.Min(z => z.TopMargin);
            int w = block.spriteHeaders.Max(z => z.SpriteWidth);
            int h = block.spriteHeaders.Max(z => z.SpriteHeight);

            offset = new Point(x, y);
            size = new Size(w, h);

        }


        private static byte ColorBlend(byte c1, float a1, byte c2, float a2, float af)
        {
            double c = Math.Round((c2 * a2 + c1 * a1 * (1 - a2)) / af);
            if (c < 0)
                return 0;
            if (c > 255)
                return 255;
            return (byte)c;
        }

        private static byte ColorBlend(byte c1, float a1)
        {
            double c = Math.Round(c1 * (1 - a1));
            if (c < 0)
                return 0;
            if (c > 255)
                return 255;
            return (byte)c;
        }

        private static float AlphaBlendFactor(float alphaImage, float alphaBackground)
        {
            return alphaBackground + (1.0f - alphaBackground) * alphaImage;
        }


        private unsafe static void AlphBlackBlend(byte* ptr, float alpha)
        {
            /*float af = AlphaBlendFactor(alpha, 1);

            *(ptr) = ColorBlend(0, alpha, *(ptr), 1, af);
            *(ptr + 1) = ColorBlend(0, alpha, *(ptr + 1), 1, af);
            *(ptr + 2) = ColorBlend(0, alpha, *(ptr + 2), 1, af);*/


            //ALphaB
            *(ptr) = ColorBlend(*(ptr), alpha);
            *(ptr + 1) = ColorBlend(*(ptr + 1), alpha);
            *(ptr + 2) = ColorBlend(*(ptr + 1), alpha);
        }

        private unsafe static Bitmap DrawTransparent(Bitmap background, Bitmap creatureFrame, Point computedOffset, Size computedSize)
        {
            var result = new Bitmap(background);
            var backData = result.LockBits(new Rectangle(0, 0, background.Width, background.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var imgData = creatureFrame.LockBits(new Rectangle(0, 0, creatureFrame.Width, creatureFrame.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int offX = (backData.Width - computedSize.Width) / 2;
            int offY = Math.Max(0, backData.Height - 15 - computedSize.Height);


            byte* backPtr = (byte*)backData.Scan0.ToPointer();
            byte* crPtr = (byte*)imgData.Scan0.ToPointer();
            byte* backOffset, sprOffset;

            for (int i = 0; i < computedSize.Height; i++)
            {
                backOffset = backPtr + (i + offY) * backData.Stride + 3 * offX;
                sprOffset = crPtr + (i + computedOffset.Y) * imgData.Stride + 3 * computedOffset.X;

                for (int j = 0; j < computedSize.Width; j++)
                {
                    byte b = *(sprOffset++);
                    byte g = *(sprOffset++);
                    byte r = *(sprOffset++);

                    if (!(r == 0 && g == 255 && b == 255) && !(r == 255 && g == 255 && b == 0))
                    {
                        if ((r == 180 && g == 0 && b == 255) || (r == 0 && b == 0 && g == 255))
                        {
                            *backOffset = 0;
                            *(backOffset + 1) = 0;
                            *(backOffset + 2) = 0;
                        }
                        else if (r == 255 && g == 0 && b == 255)
                        {
                            *backOffset = 5;
                            *(backOffset + 1) = 5;
                            *(backOffset + 2) = 5;
                        }
                        else if (r == 255 && b == 255 && g == 150)
                        {
                            *backOffset = 15;
                            *(backOffset + 1) = 15;
                            *(backOffset + 2) = 15;
                        }
                        else
                        {
                            *backOffset = b;
                            *(backOffset + 1) = g;
                            *(backOffset + 2) = r;
                        }
                    }

                    backOffset += 3;
                }
            }


            creatureFrame.UnlockBits(imgData);
            result.UnlockBits(backData);
            return result;
        }

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Dispose();
            }
            if (frames != null)
            {
                foreach (var bmp in frames)
                    if (bmp != null)
                        bmp.Dispose();
            }
        }
    }
}
