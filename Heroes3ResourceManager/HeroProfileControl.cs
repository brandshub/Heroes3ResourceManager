using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace h3magic
{
    public partial class HeroProfileControl : UserControl
    {
        private static RectangleF[] areas;

        public Image Image
        {
            get { return PictureBox.Image; }
            set { PictureBox.Image = value; }
        }

        public event Action<int, ProfilePropertyType, int, int> PropertyClicked;

        private Font font1 = new Font("Arial Unicode MS", 8);
        private float ratio = 1;
        private int lastRectIndex = -1;
        private float dx = 0;
        private float dy = 0;

        public int HeroIndex { get; set; }

        public HeroExeData Hero { get; set; }

        public HeroProfileControl()
        {
            InitializeComponent();
        }

        //331 288
        public void LoadHero(int heroIndex, Heroes3Master master)
        {
            HeroIndex = heroIndex;
            Hero = null;
            if (heroIndex > -1 && heroIndex < HeroesManager.HeroesOrder.Length && master != null)
            {
                var hs = HeroesManager.AllHeroes[heroIndex];

                var bckgImage = HeroesManager.GetBackground(master);

                var canvas = new Bitmap(bckgImage);

                var g = Graphics.FromImage(canvas);
                g.DrawImage(bckgImage, Point.Empty);

                var h3bitmap = master.Resolve(HeroesManager.HeroesOrder[hs.ImageIndex]);
                var portrait = h3bitmap[HeroesManager.HeroesOrder[hs.ImageIndex]].GetBitmap(h3bitmap.stream);
                g.DrawImage(portrait, new Point(4, 3));

                var heroData = HeroExeData.Data[heroIndex];
          
                var z = Speciality.GetImage(master, heroData.Index);
                g.DrawImage(z, new Point(4, 166));

                Hero = heroData;
                if (heroData.Skill1 != null)
                    g.DrawImage(heroData.Skill1.GetImage(master, heroData.FirstSkillLevel), new Point(5, 213));

                if (heroData.Skill2 != null)
                    g.DrawImage(heroData.Skill2.GetImage(master, heroData.SecondSkillLevel), new Point(148, 213));

                var img1 = CreatureManager.GetImage(master, heroData.Unit1Index);
                g.DrawImage(img1, new Point(5, 262));
                var img2 = CreatureManager.GetImage(master, heroData.Unit2Index);
                g.DrawImage(img2, new Point(68, 262));
                var img3 = CreatureManager.GetImage(master, heroData.Unit3Index);
                g.DrawImage(img3, new Point(129, 262));

                if (heroData.Spell != null)
                    g.DrawImage(heroData.Spell.GetImage(master), 192, 262);

                DrawData(g, heroData);


                //heroData.Class.Stats
                /*
                 *  g.DrawImage(ps.GetSprite(0), new Point(18, 97));
                    g.DrawImage(ps.GetSprite(1), new Point(88, 97));
                    g.DrawImage(ps.GetSprite(2), new Point(158, 97));
                    g.DrawImage(ps.GetSprite(5), new Point(228, 97));
                 */
                g.Dispose();
                PictureBox.Image = canvas;
                CalculateRatio();
            }
        }

        private void DrawData(Graphics g, HeroExeData hero)
        {
            var cl = hero.Class;
            var hs = HeroesManager.AllHeroes[hero.Index];

            var strData = StringsData.JKTEXT;
            Color mainColor = Color.FromArgb(255, 231, 148);
            SizeF sz;

            var nameFont = new Font(font1.FontFamily, 14, FontStyle.Bold);
            sz = g.MeasureString(hs.Name, nameFont);
            DrawShadowedString(hs.Name, g, mainColor, nameFont, 67 + (220 - sz.Width) / 2, 15);

            var classFont = new Font(font1.FontFamily, 9);
            sz = g.MeasureString(cl.Stats[0], classFont);
            DrawShadowedString(cl.Stats[0], g, Color.White, classFont, 67 + (220 - sz.Width) / 2, 40);


            sz = g.MeasureString(strData[1], font1);
            DrawShadowedString(strData[1], g, mainColor, font1, 16 + (44 - sz.Width) / 2, 77);

            sz = g.MeasureString(strData[2], font1);
            DrawShadowedString(strData[2], g, mainColor, font1, 86 + (44 - sz.Width) / 2, 77);

            sz = g.MeasureString(strData[3], font1);
            DrawShadowedString(strData[3], g, mainColor, font1, 156 + (44 - sz.Width) / 2, 77);

            sz = g.MeasureString(strData[4], font1);
            DrawShadowedString(strData[4], g, mainColor, font1, 226 + (44 - sz.Width) / 2, 77);


            DrawShadowedString(strData[5].Replace("{", "").Replace("}", ""), g, mainColor, font1, 52, 171);

            float fontSize = font1.Size;
            Font temp;
            string specName = hs.Speciality.Split('\t')[0];
            do
            {
                temp = new Font(font1.FontFamily.Name, fontSize--);
                sz = g.MeasureString(specName, temp);
            }
            while (sz.Width > 93);
            DrawShadowedString(specName, g, Color.White, temp, 52, 189);

            DrawShadowedString(string.Format("{0}/{0}", cl.Mana), g, Color.White, font1, 221 + (51 - 33) / 2, 165 + 22 - 7);

            DrawSkill(hero, 1, g, Color.White, font1, 52, 213, 44, 93);
            DrawSkill(hero, 2, g, Color.White, font1, 194, 213, 44, 93);

            DrawShadowedString(cl.Attack.ToString(), g, Color.White, font1, 18 + 22 - 6, 143);
            DrawShadowedString(cl.Defense.ToString(), g, Color.White, font1, 88 + 22 - 6, 143);
            DrawShadowedString(cl.MagicPower.ToString(), g, Color.White, font1, 158 + 22 - 6, 143);
            DrawShadowedString(cl.Knowledge.ToString(), g, Color.White, font1, 228 + 22 - 6, 143);
        }

        private void DrawSkill(HeroExeData hero, int skillIndex, Graphics g, Color color, Font font, float x, float top, float heightLimit, float widthLimit)
        {
            string skillStr = null;
            if (skillIndex == 1)
            {
                skillStr = hero.Skill1.Name + " (" + hero.FirstSkillLevel + ")";
            }
            else if (skillIndex == 2 && hero.SecondSkillIndex != -1)
            {
                skillStr = hero.Skill2.Name + " (" + hero.SecondSkillLevel + ")";
            }

            if (string.IsNullOrEmpty(skillStr))
                return;

            SizeF sz = g.MeasureString(skillStr, font);
            if (sz.Width > widthLimit)
            {
                int firstSpace = skillStr.IndexOf(' ');
                if (firstSpace >= 0)
                {
                    skillStr = skillStr.Substring(0, firstSpace) + "\r\n" + skillStr.Substring(firstSpace + 1);
                    sz = g.MeasureString(skillStr, font);
                }
            }

            float y = top + (heightLimit - sz.Height) / 2;
            DrawShadowedString(skillStr, g, Color.White, font, x, y);
        }
        private void DrawShadowedString(string text, Graphics g, Color color, Font font, float x, float y)
        {
            g.DrawString(text, font, Brushes.Black, x + 1, y + 1);
            g.DrawString(text, font, new SolidBrush(color), x, y);
        }

        private void CalculateRatio()
        {
            float kx = PictureBox.Width / 288f;
            float ky = PictureBox.Height / 331f;

            if (kx > ky)
            {
                ratio = ky;
                dx = (PictureBox.Width - 288 * ratio) / 2;
                dy = 0;
            }
            else
            {
                ratio = kx;
                dx = 0;
                dy = (PictureBox.Height - 331 * ratio) / 2;
            }
        }

        private int GetBoundingRectangleIndex(int x, int y, float ratio, float dx, float dy)
        {

            for (int i = 0; i < areas.Length; i++)
            {
                var r = Multiply(areas[i], ratio);
                r.Offset(dx, dy);
                if (x >= r.X && x <= r.X + r.Width && y >= r.Y && y <= r.Y + r.Height)
                    return i;
            }
            return -1;
        }

        private static RectangleF Multiply(RectangleF rect, float k)
        {
            return new RectangleF(rect.X * k, rect.Y * k, rect.Width * k, rect.Height * k);
        }

        static HeroProfileControl()
        {
            areas = new RectangleF[9];
            areas[0] = new RectangleF(4, 3, 58, 64);
            areas[1] = new RectangleF(65, 3, 219, 64);

            areas[2] = new RectangleF(4, 166, 44, 44);

            areas[3] = new RectangleF(5, 213, 44, 44);
            areas[4] = new RectangleF(148, 213, 44, 44);

            areas[5] = new RectangleF(5, 262, 58, 64);
            areas[6] = new RectangleF(68, 262, 58, 64);
            areas[7] = new RectangleF(129, 262, 58, 64);

            areas[8] = new RectangleF(192, 262, 58, 64);


        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (Hero != null)
            {
                if (lastRectIndex >= 0)
                {
                    var rectF = Multiply(areas[lastRectIndex], ratio);
                    rectF.Offset(dx, dy);
                    var pen = new Pen(Color.LightSkyBlue, 2);
                    //pen.Alignment = PenAlignment.Inset; //<-- this

                    e.Graphics.DrawRectangle(pen, Rectangle.Round(rectF));
                }
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (Hero != null)
            {
                int index = GetBoundingRectangleIndex(e.X, e.Y, ratio, dx, dy);
                if (index != lastRectIndex)
                {
                    lastRectIndex = index;
                    PictureBox.Invalidate();
                }
            }
        }

        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (lastRectIndex >= 0 && PropertyClicked != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    ProfilePropertyType type = ProfilePropertyType.Other;
                    int index = 0;
                    int currentValue = 0;

                    if (lastRectIndex == 0)
                    {
                        //type = "Portrait";
                        return;
                    }
                    else if (lastRectIndex == 1)
                    {
                        //type = "Name";
                        return;
                    }
                    else if (lastRectIndex == 2)
                    {
                        type = Speciality.ToProfilePropertyType(Hero.Spec.Type);
                        currentValue = Hero.Spec.ObjectId;
                    }
                    else if (lastRectIndex <= 4)
                    {
                        type = ProfilePropertyType.SecondarySkill;
                        index = lastRectIndex - 3;
                        currentValue = (index == 0 ? (3 * Hero.FirstSkillIndex + Hero.FirstSkillLevel) : (3 * Hero.SecondSkillIndex + Hero.SecondSkillLevel)) - 1;
                    }
                    else if (lastRectIndex <= 7)
                    {
                        type = ProfilePropertyType.Creature;
                        index = lastRectIndex - 5;
                        currentValue = index == 0 ? Hero.Unit1Index : (index == 1 ? Hero.Unit2Index : Hero.Unit3Index);
                    }
                    else if (lastRectIndex == 8)
                    {
                        type = ProfilePropertyType.Spell;
                        currentValue = Hero.SpellIndex;
                    }

                    PropertyClicked(Hero.Index, type, index, currentValue);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (lastRectIndex == 0)
                    {

                    }
                    else if (lastRectIndex == 1)
                    {

                    }
                    else if (lastRectIndex == 2)
                    {

                    }
                    else if (lastRectIndex <= 4)
                    {
                        if (lastRectIndex == 3)
                        {
                            if (Hero.SecondSkillIndex != -1)
                            {
                                Hero.FirstSkillIndex = Hero.SecondSkillIndex;
                                Hero.FirstSkillLevel = Hero.SecondSkillLevel;
                                Hero.SecondSkillLevel = -1;
                            }
                            else
                            {
                                Hero.FirstSkillIndex = -1;
                            }
                        }
                        else
                        {
                            Hero.SecondSkillIndex = -1;
                        }
                        LoadHero(Hero.Index, Heroes3Master.Master);

                    }
                    else if (lastRectIndex <= 7)
                    {
                        /*  type = "Creature";
                          index = lastRectIndex - 5;
                          currentValue = index == 0 ? Hero.Unit1Index : (index == 1 ? Hero.Unit2Index : Hero.Unit3Index);*/
                    }
                    else if (lastRectIndex == 8)
                    {
                        Hero.SpellBook = 0;
                        Hero.SpellIndex = -1;
                        LoadHero(Hero.Index, Heroes3Master.Master);
                    }
                }
            }
        }
    }
}
