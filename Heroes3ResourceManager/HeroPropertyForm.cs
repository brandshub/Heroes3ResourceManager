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
        private ProfilePropertyType propertyType;
        private int selectedValue = 0;

        public ProfilePropertyType PropertyType
        {
            get { return propertyType; }
            set
            {
                propertyType = value;
                InitPropertyType();
            }
        }
        public int SelectedValue
        {
            get { return selectedValue; }
            set
            {
                if (propertyType == ProfilePropertyType.Creature)
                {
                    selectedValue = CreatureManager.OnlyActiveCreatures.FindIndex(c => c.CreatureIndex == value);
                }
                else
                {
                    selectedValue = value;
                }
                currentHover = selectedValue;
            }
        }
        public int CurrentIndex { get; set; }

        public HeroPropertyForm()
        {
            InitializeComponent();
        }

        public event Action<int> ItemSelected;

        public void InitPropertyType()
        {
            Image background = null;
            if (propertyType == ProfilePropertyType.Creature)
            {
                background = new Bitmap(CreatureManager.GetAllCreaturesBitmap(Heroes3Master.Master.H3Sprite));
            }
            else if (propertyType == ProfilePropertyType.SecondarySkill)
            {
                background = new Bitmap(SecondarySkill.GetSkillTree2(Heroes3Master.Master.H3Sprite));
            }
            else if (propertyType == ProfilePropertyType.Spell)
            {
                background = new Bitmap(SpellStat.GetAllSpells(Heroes3Master.Master.H3Sprite));
            }
            else if (propertyType == ProfilePropertyType.Speciality)
            {
                background = new Bitmap(Speciality.GetAllSpecs(Heroes3Master.Master.H3Sprite));
            }


            if (background != null)
            {

                Width = background.Width;
                Height = background.Height;

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
            int cellWidth, cellHeight, itemsPerRow;

            GetDimensionsForPropertyType(propertyType, out cellWidth, out cellHeight, out itemsPerRow);

            int row = e.Y / cellHeight;
            int col = e.X / cellWidth;

            int total = col + row * itemsPerRow;
            if (total != currentHover)
            {
                bool flag = false;
      
                if (propertyType == ProfilePropertyType.Creature)
                {
                    flag = total < CreatureManager.OnlyActiveCreatures.Count - 1;
                }
                else if (propertyType == ProfilePropertyType.SecondarySkill)
                {
                    flag = total < SecondarySkill.AllSkills.Count * 3;
                }
                else if (propertyType == ProfilePropertyType.Spell)
                {
                    flag = total < SpellStat.AllSpells.Count;
                }
                else if (propertyType == ProfilePropertyType.Speciality)
                {
                    flag = total < Speciality.AllSpecialities.Count;
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
            int cellWidth, cellHeight, itemsPerRow;
            GetDimensionsForPropertyType(propertyType, out cellWidth, out cellHeight, out itemsPerRow);

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

        private static void GetDimensionsForPropertyType(ProfilePropertyType propertyType, out int cellWidth, out int cellHeight, out int itemsPerRow)
        {
            if (propertyType == ProfilePropertyType.Creature)
            {
                cellWidth = 59;
                cellHeight = 65;
                itemsPerRow = 14;
            }
            else if (propertyType == ProfilePropertyType.SecondarySkill)
            {
                cellWidth = 45;
                cellHeight = 45;
                itemsPerRow = 12;
            }
            else if (propertyType == ProfilePropertyType.Spell)
            {
                cellWidth = 59;
                cellHeight = 65;
                itemsPerRow = 10;
            }
            else if (propertyType == ProfilePropertyType.Speciality)
            {
                cellWidth = 45;
                cellHeight = 45;
                itemsPerRow = 16;
            }
            else
            {
                cellWidth = 1;
                cellHeight = 1;
                itemsPerRow = 1;
            }
        }

        private static Bitmap GetImageForPropertyType(ProfilePropertyType propertyType, int index)
        {
            if (propertyType == ProfilePropertyType.Creature)
            {
                int realIndex = CreatureManager.OnlyActiveCreatures[index].CreatureIndex;
                return CreatureManager.GetImage(Heroes3Master.Master.H3Sprite, realIndex);
            }
            if (propertyType == ProfilePropertyType.SecondarySkill)
                return SecondarySkill.AllSkills[index / 3].GetImage(Heroes3Master.Master.H3Sprite, 1 + index % 3);

            if (propertyType == ProfilePropertyType.Spell)
                return SpellStat.AllSpells[index].GetImage(Heroes3Master.Master.H3Sprite);

            if (propertyType == ProfilePropertyType.Speciality)
                return Speciality.GetImage(Heroes3Master.Master.H3Sprite, index);

            return null;
        }
    }
}
