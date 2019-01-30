using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace h3magic
{
    public partial class HeroPropertyForm : Form
    {
        private string propertyType = "";
        private int selectedValue = 0;

        public string PropertyType
        {
            get { return propertyType; }
            set
            {
                propertyType = value;
                InitPropertyType();
            }
        }
        public int SelectedValue { get { return selectedValue; } set { selectedValue = value; currentHover = selectedValue; } }
        public int CurrentIndex { get; set; }

        public HeroPropertyForm()
        {
            InitializeComponent();
        }

        public event Action<int> ItemSelected;

        public void InitPropertyType()
        {
            Image background = null;
            if (propertyType == "Creature")
            {
                background = new Bitmap(CreatureManager.GetAllCreaturesBitmap(Heroes3Master.Master.H3Sprite));
            }
            else if (propertyType == "SecondarySkill")
            {
                background = new Bitmap(SecondarySkill.GetSkillTree2(Heroes3Master.Master.H3Sprite));
            }
            else if (propertyType == "Spell")
            {
                background = new Bitmap(SpellStat.GetAllSpells(Heroes3Master.Master.H3Sprite));
            }

            if (background != null)
            {

                Width = background.Width;
                Height = background.Height + 25;

                using (var g = Graphics.FromImage(background))
                    g.FillRectangle(new SolidBrush(Color.FromArgb(190, Color.Gray)), pbMain.ClientRectangle);


                pbMain.Image = background;
            }
        }

        private void pbMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentHover != -1)
            {
                selectedValue = currentHover;
                
                
                if (ItemSelected != null)
                    ItemSelected(currentHover);
                Close();
            }

        }

        private int currentHover = -1;
        private void pbMain_MouseMove(object sender, MouseEventArgs e)
        {
            GetDimensionsForPropertyType(propertyType, out int cellWidth, out int cellHeight, out int itemsPerRow);

            int row = e.Y / cellHeight;
            int col = e.X / cellWidth;

            int total = col + row * itemsPerRow;
            if (total != currentHover)
            {
                bool flag = false;

                if (propertyType == "Creature")
                {
                    flag = total < CreatureManager.AllCreatures.Count - 1;
                }
                else if (propertyType == "SecondarySkill")
                {
                    flag = total < SecondarySkill.AllSkills.Count * 3;
                }
                else if(propertyType == "Spell")
                {
                    flag = total < SpellStat.AllSpells.Count;
                }

                if (flag)
                {
                    pbMain.Invalidate(new Rectangle((currentHover % itemsPerRow) * cellWidth, (currentHover / itemsPerRow) * cellHeight, cellWidth - 1, cellHeight - 1));
                    currentHover = total;
                    pbMain.Invalidate(new Rectangle((currentHover % itemsPerRow) * cellWidth, (currentHover / itemsPerRow) * cellHeight, cellWidth - 1, cellHeight - 1));
                }
            }

        }

        private void pbMain_Paint(object sender, PaintEventArgs e)
        {
            GetDimensionsForPropertyType(propertyType, out int cellWidth, out int cellHeight, out int itemsPerRow);

            if (currentHover >= 0)
            {
                var hoverImage = GetImageForPropertyType(propertyType, currentHover);
                if (hoverImage != null)
                    e.Graphics.DrawImage(hoverImage, (currentHover % itemsPerRow) * cellWidth, (currentHover / itemsPerRow) * cellHeight, cellWidth - 1, cellHeight - 1);
            }

            if (selectedValue >= 0)
            {
                var realImage = GetImageForPropertyType(propertyType, selectedValue);
                if (realImage != null)
                {
                    e.Graphics.DrawImage(realImage, (selectedValue % itemsPerRow) * cellWidth, (selectedValue / itemsPerRow) * cellHeight, cellWidth - 1, cellHeight - 1);
                    e.Graphics.DrawRectangle(new Pen(Color.Red, 4), (selectedValue % itemsPerRow) * cellWidth, (selectedValue / itemsPerRow) * cellHeight, cellWidth - 1, cellHeight - 1);
                }
            }

        }

        private static void GetDimensionsForPropertyType(string propertyType, out int cellWidth, out int cellHeight, out int itemsPerRow)
        {
            if (propertyType == "Creature")
            {
                cellWidth = 59;
                cellHeight = 65;
                itemsPerRow = 14;
            }
            else if (propertyType == "SecondarySkill")
            {
                cellWidth = 45;
                cellHeight = 45;
                itemsPerRow = 12;
            }
            else if (propertyType == "Spell")
            {
                cellWidth = 59;
                cellHeight = 65;
                itemsPerRow = 10;
            }
            else
            {
                cellWidth = 1;
                cellHeight = 1;
                itemsPerRow = 1;
            }
        }

        private static Bitmap GetImageForPropertyType(string propertyType, int index)
        {
            if (propertyType == "Creature")
                return CreatureManager.GetImageByCreatureInnerIndex(Heroes3Master.Master.H3Sprite, index);
            if (propertyType == "SecondarySkill")
                return SecondarySkill.AllSkills[index / 3].GetImage(Heroes3Master.Master.H3Sprite, 1 + index % 3);
            if (propertyType == "Spell")
                return SpellStat.AllSpells[index].GetImage(Heroes3Master.Master.H3Sprite);

            return null;
        }
    }
}
