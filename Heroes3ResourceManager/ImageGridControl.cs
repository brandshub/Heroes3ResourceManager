using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace h3magic
{
    public partial class ImageGridControl : UserControl
    {

        private int selectedIndex;
        private int hoverIndex;

        public int HeroIndex { get; set; }
        public int CurrentIndex { get; set; }
        public bool ForceAllCreatures { get; set; }

        private ProfilePropertyType propertyType;
        private int selectedValue = 0;


        public ImageGridControl()
        {
            InitializeComponent();
        }

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
                if (value >= 0)
                {
                    try
                    {
                        if (propertyType == ProfilePropertyType.Creature)
                        {
                            selectedValue = CreatureManager.OnlyActiveCreatures.FindIndex(c => c.CreatureIndex == value);
                        }
                        else if (propertyType == ProfilePropertyType.SpecCreature)
                        {
                            selectedValue = Array.IndexOf<int>(CreatureManager.IndexesOfFirstLevelCreatures, value);
                        }
                        else if (propertyType == ProfilePropertyType.SpecCreatureStatic)
                        {
                            selectedValue = Array.IndexOf<int>(CreatureManager.IndexesOfFirstLevelCreatures, value);
                        }
                        else if (propertyType == ProfilePropertyType.SpecSecondarySkill)
                        {
                            selectedValue = Array.IndexOf<int>(SecondarySkill.IndexesOfAllSpecSkills, value);
                        }
                        else if (propertyType == ProfilePropertyType.SpecSpell)
                        {
                            selectedValue = Array.IndexOf<int>(Spell.specSpellIndexes, value);
                        }
                        else if (propertyType == ProfilePropertyType.SpecCreatureUpgrade)
                        {
                            if (ForceAllCreatures)
                                selectedValue = selectedValue = CreatureManager.OnlyActiveCreatures.FindIndex(c => c.CreatureIndex == value);
                            else
                                selectedValue = Array.IndexOf<int>(CreatureManager.IndexesOfFirstLevelCreatures, value);
                        }
                        else
                        {
                            selectedValue = value;
                        }
                    }

                    catch
                    {

                    }
                    currentHover = selectedValue;
                }
            }
        }



        public event Action<int> ItemSelected;

        public void InitPropertyType()
        {
            var temp = GetBackgroundImageForPropertyType(propertyType, ForceAllCreatures);
            if (temp != null)
            {
                Image background = new Bitmap(temp);

                if (background != null)
                {
                    Width = background.Width;
                    Height = background.Height;

                    using (var g = Graphics.FromImage(background))
                        g.FillRectangle(new SolidBrush(Color.FromArgb(190, Color.Gray)), pbMain.ClientRectangle);

                    pbMain.Image = background;
                }
            }
        }



        private void pbMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentHover != -1)
            {
                selectedValue = currentHover;

                pbMain.Invalidate();
                if (ItemSelected != null)
                    ItemSelected(currentHover);
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
                    flag = total < Spell.AllSpells.Count;
                }
                else if (propertyType == ProfilePropertyType.SpecCreature || propertyType == ProfilePropertyType.SpecCreatureStatic)
                {
                    flag = total < CreatureManager.IndexesOfFirstLevelCreatures.Length;
                }
                else if (propertyType == ProfilePropertyType.SpecCreatureUpgrade)
                {
                    if (ForceAllCreatures)
                        flag = total < CreatureManager.OnlyActiveCreatures.Count - 1;
                    else
                        flag = total < CreatureManager.IndexesOfFirstLevelCreatures.Length;
                }
                else if (propertyType == ProfilePropertyType.SpecSecondarySkill)
                {
                    flag = total < SecondarySkill.IndexesOfAllSpecSkills.Length;
                }
                else if (propertyType == ProfilePropertyType.SpecSpell)
                {
                    flag = total < Spell.specSpellIndexes.Length;
                }
                else if (propertyType == ProfilePropertyType.SpecResource)
                {
                    flag = total < 7;
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
                var hoverImage = GetImageForPropertyType(propertyType, currentHover,ForceAllCreatures);
                if (hoverImage != null)
                    e.Graphics.DrawImage(hoverImage, (currentHover % itemsPerRow) * cellWidth, (currentHover / itemsPerRow) * cellHeight, cellWidth - 1, cellHeight - 1);
            }

            if (selectedValue >= 0)
            {
                var realImage = GetImageForPropertyType(propertyType, selectedValue, ForceAllCreatures);
                if (realImage != null)
                {
                    e.Graphics.DrawImage(realImage, (selectedValue % itemsPerRow) * cellWidth, (selectedValue / itemsPerRow) * cellHeight, cellWidth - 1, cellHeight - 1);
                    e.Graphics.DrawRectangle(new Pen(Color.Red, 4), (selectedValue % itemsPerRow) * cellWidth, (selectedValue / itemsPerRow) * cellHeight, cellWidth - 1, cellHeight - 1);
                }
            }

        }



        private static void GetDimensionsForPropertyType(ProfilePropertyType propertyType, out int cellWidth, out int cellHeight, out int itemsPerRow)
        {
            if (propertyType == ProfilePropertyType.Creature || propertyType == ProfilePropertyType.SpecCreature || propertyType == ProfilePropertyType.SpecCreatureStatic || propertyType == ProfilePropertyType.SpecCreatureUpgrade)
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
            else if (propertyType == ProfilePropertyType.SpecSecondarySkill)
            {
                cellWidth = 45;
                cellHeight = 45;
                itemsPerRow = 6;
            }
            else if (propertyType == ProfilePropertyType.Spell)
            {
                cellWidth = 59;
                cellHeight = 65;
                itemsPerRow = 10;
            }
            else if (propertyType == ProfilePropertyType.SpecSpell)
            {
                cellWidth = 59;
                cellHeight = 65;
                itemsPerRow = 6;
            }
            else if (propertyType == ProfilePropertyType.SpecResource)
            {
                cellWidth = 83;
                cellHeight = 93;
                itemsPerRow = 7;
            }
            else
            {
                cellWidth = 1;
                cellHeight = 1;
                itemsPerRow = 1;
            }
        }


        //                     
        private static Bitmap GetImageForPropertyType(ProfilePropertyType propertyType, int index, bool forceAllCreatures)
        {
            if (index >= 0)
            {
                if (propertyType == ProfilePropertyType.Creature)
                {
                    int realIndex = CreatureManager.OnlyActiveCreatures[index].CreatureIndex;
                    return CreatureManager.GetImage(Heroes3Master.Master.H3Sprite, realIndex);
                }
                if (propertyType == ProfilePropertyType.SecondarySkill)
                    return SecondarySkill.AllSkills[index / 3].GetImage(Heroes3Master.Master.H3Sprite, 1 + index % 3);

                if (propertyType == ProfilePropertyType.Spell)
                    return Spell.AllSpells[index].GetImage(Heroes3Master.Master.H3Sprite);

                if (propertyType == ProfilePropertyType.SpecCreature || propertyType == ProfilePropertyType.SpecCreatureStatic)
                    return CreatureManager.GetImage(Heroes3Master.Master.H3Sprite, CreatureManager.IndexesOfFirstLevelCreatures[index]);

                if (propertyType == ProfilePropertyType.SpecCreatureUpgrade)
                {
                    if (!forceAllCreatures)
                        return CreatureManager.GetImage(Heroes3Master.Master.H3Sprite, CreatureManager.IndexesOfFirstLevelCreatures[index]);

                    int realIndex = CreatureManager.OnlyActiveCreatures[index].CreatureIndex;
                    return CreatureManager.GetImage(Heroes3Master.Master.H3Sprite, realIndex);
                }

                if (propertyType == ProfilePropertyType.SpecSpell)
                    return Spell.GetSpellByIndex(Spell.specSpellIndexes[index]).GetImage(Heroes3Master.Master.H3Sprite);

                if (propertyType == ProfilePropertyType.SpecSecondarySkill)
                    return SecondarySkill.GetImage(Heroes3Master.Master.H3Sprite, SecondarySkill.IndexesOfAllSpecSkills[index], 1);

                if (propertyType == ProfilePropertyType.SpecResource)
                    return Resource.GetImage(Heroes3Master.Master.H3Sprite, index);
            }
            return null;
        }


        private static Bitmap GetBackgroundImageForPropertyType(ProfilePropertyType propertyType, bool forceAllCreatures)
        {
            if (Heroes3Master.Master != null)
            {
                if (propertyType == ProfilePropertyType.Creature)
                    return new Bitmap(CreatureManager.GetAllCreaturesBitmapParallel(Heroes3Master.Master.H3Sprite));

                if (propertyType == ProfilePropertyType.SecondarySkill)
                    return new Bitmap(SecondarySkill.GetSkillTree(Heroes3Master.Master.H3Sprite));

                if (propertyType == ProfilePropertyType.Spell)
                    return new Bitmap(Spell.GetAllSpellsParallel(Heroes3Master.Master.H3Sprite));

                if (propertyType == ProfilePropertyType.SpecCreature || propertyType == ProfilePropertyType.SpecCreatureStatic)
                    return CreatureManager.GetAllBasicCreatures(Heroes3Master.Master.H3Sprite);
                if (propertyType == ProfilePropertyType.SpecCreatureUpgrade)
                {
                    if (forceAllCreatures)
                        return new Bitmap(CreatureManager.GetAllCreaturesBitmapParallel(Heroes3Master.Master.H3Sprite));
                    return CreatureManager.GetAllBasicCreatures(Heroes3Master.Master.H3Sprite);
                }
                if (propertyType == ProfilePropertyType.SpecSecondarySkill)
                    return SecondarySkill.GetSkillsForSpeciality(Heroes3Master.Master.H3Sprite);

                if (propertyType == ProfilePropertyType.SpecSpell)
                    return Spell.GetAvailableSpellsForSpeciality(Heroes3Master.Master.H3Sprite);

                if (propertyType == ProfilePropertyType.SpecResource)
                    return Resource.GetAllResources(Heroes3Master.Master.H3Sprite);
            }
            return null;
        }
    }
}
