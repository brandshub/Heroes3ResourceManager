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
        private int selectedBoxIndex = -1;
        private int Box1Creature = -1;
        private int Box2Creature = -1;
        private int Box3Creature = -1;

        public ProfilePropertyType PropertyType
        {
            get { return gridImages.PropertyType; }
            set
            {
                gridImages.PropertyType = value;
                UpdateControls();
                cbSpecialityType.SelectedValue = (int)value;
            }
        }

        public int HeroIndex
        {
            get { return gridImages.HeroIndex; }
            set { gridImages.HeroIndex = value; }
        }

        public int CurrentIndex
        {
            get { return gridImages.CurrentIndex; }
            set { gridImages.CurrentIndex = value; }
        }

        public int SelectedValue
        {
            get { return gridImages.SelectedValue; }
            set
            {
                gridImages.SelectedValue = value;
                if (value >= 0)
                {
                    if (PropertyType == ProfilePropertyType.SpecCreatureStatic)
                    {
                        var spec = Speciality.AllSpecialities[HeroIndex];
                        int a, d, dmg;
                        if (spec.TryGetCreatureStaticBonuses(out a, out d, out dmg))
                        {
                            tbAttack.Value = a;
                            tbDefense.Value = d;
                            tbDamage.Value = dmg;
                        }
                    }
                    else if (PropertyType == ProfilePropertyType.SpecCreatureUpgrade)
                    {
                        var spec = Speciality.AllSpecialities[HeroIndex];
                        int cr1, cr2, cr3;
                        if (spec.TryGetCreatureUpgrade(out cr1, out cr2, out cr3))
                        {
                            Box1Creature = cr1;
                            Box2Creature = cr2;
                            Box3Creature = cr3;
                            UpdateCreatureBoxes();
                        }
                    }
                }
            }
        }

        public HeroPropertyForm()
        {
            InitializeComponent();
            InitSpecialityTypeCombo();


            gridImages.ItemSelected += gridImages_ItemSelected;
        }

        private void gridImages_ItemSelected(int value)
        {
            
            if (PropertyType == ProfilePropertyType.SpecCreatureStatic)
            {
                btnSave.Focus();
            }
            else if (PropertyType == ProfilePropertyType.SpecCreatureUpgrade)
            {
                if (selectedBoxIndex == 0)
                {
                    Box1Creature = CreatureManager.IndexesOfFirstLevelCreatures[value];
                    pbCreature1.Image = CreatureManager.GetImage(Heroes3Master.Master, Box1Creature);
                }
                else if (selectedBoxIndex == 1)
                {
                    Box2Creature = CreatureManager.IndexesOfFirstLevelCreatures[value];
                    pbCreature2.Image = CreatureManager.GetImage(Heroes3Master.Master, Box2Creature);
                }
                else if (selectedBoxIndex == 2)
                {
                    Box3Creature = CreatureManager.OnlyActiveCreatures[value].CreatureIndex;
                    pbCreature3.Image = CreatureManager.GetImage(Heroes3Master.Master, Box3Creature);
                }
            }
            else
            {
                int objId = value;
                if (PropertyType == ProfilePropertyType.SpecCreature)
                {                    
                    objId = CreatureManager.IndexesOfFirstLevelCreatures[value];
                }
                else if (PropertyType == ProfilePropertyType.SpecSpell)
                {
                    objId = Spell.SpecSpellIndexes[value];
                }
                else if (PropertyType == ProfilePropertyType.SpecSecondarySkill)
                {
                    objId = SecondarySkill.IndexesOfAllSpecSkills[value];
                }

                if (this.ItemSelected != null)
                {
                    this.ItemSelected(objId, 0, 0, 0);
                    Close();
                }
            }
        }

        public event Action<int, int, int, int> ItemSelected;


        // anonimous class works weirdly
        private class specitem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void InitSpecialityTypeCombo()
        {
            var list = new[]
            {
                new specitem { Text = "Creature Bonus", Value = (int)ProfilePropertyType.SpecCreature },
                new specitem{ Text = "Secondary Skill", Value = (int)ProfilePropertyType.SpecSecondarySkill },
                new specitem{ Text = "Spell", Value = (int)ProfilePropertyType.SpecSpell },
                new specitem{ Text = "Resource", Value = (int)ProfilePropertyType.SpecResource},
                new specitem{ Text = "Speed +2", Value = (int)ProfilePropertyType.SpecSpeed},
                new specitem{ Text = "Creature Bonus Static", Value = (int)ProfilePropertyType.SpecCreatureStatic},
                new specitem{ Text = "Creature Upgrade", Value = (int)ProfilePropertyType.SpecCreatureUpgrade}
            };

            cbSpecialityType.DisplayMember = "Text";
            cbSpecialityType.ValueMember = "Value";

            cbSpecialityType.DataSource = list;
        }

        private void UpdateControls()
        {
            int BorderWidth = (Width - ClientSize.Width) / 2;
            int TitlebarHeight = Height - ClientSize.Height - 2 * BorderWidth;

            int width = gridImages.Width + 2 * BorderWidth;
            int height = 0;
            var pt = PropertyType;

            Width = width;
            btnSave.Visible = false;
            pnlCreatureStatic.Visible = false;
            pnlCreatureUpgrade.Visible = false;

            gridImages.Visible = true;

            if (pt == ProfilePropertyType.Creature || pt == ProfilePropertyType.SecondarySkill || pt == ProfilePropertyType.Spell)
            {
                cbSpecialityType.Visible = false;
                lblType.Visible = false;
                gridImages.Top = 0;
                height = gridImages.Height + TitlebarHeight + 2 * BorderWidth + 1;
            }
            else if (pt == ProfilePropertyType.SpecCreatureStatic)
            {
                pnlCreatureStatic.Visible = true;
                btnSave.Visible = true;
                gridImages.Top = 120;
                height = gridImages.Height + TitlebarHeight + 2 * BorderWidth + 120 + 1;
            }
            else if (pt == ProfilePropertyType.SpecCreatureUpgrade)
            {
                pnlCreatureUpgrade.Visible = true;
                UpdateCreatureBoxes();
                btnSave.Visible = true;
                gridImages.Top = 135;
                height = gridImages.Height + TitlebarHeight + 2 * BorderWidth + 135 + 1;
            }
            else if (pt == ProfilePropertyType.SpecSpeed)
            {
                gridImages.Visible = false;
                height = TitlebarHeight + 2 * BorderWidth + 120;
                btnSave.Visible = true;
            }
            else
            {
                cbSpecialityType.Visible = true;
                lblType.Visible = true;

                gridImages.Top = 70;
                height = gridImages.Height + TitlebarHeight + 2 * BorderWidth + 70 + 1;
            }

            Height = height;
        }

        private void UpdateCreatureBoxes()
        {
            if (Box1Creature >= 0)
                pbCreature1.Image = CreatureManager.GetImage(Heroes3Master.Master, Box1Creature);

            if (Box2Creature >= 0)
                pbCreature2.Image = CreatureManager.GetImage(Heroes3Master.Master, Box2Creature);

            if (Box3Creature >= 0)
                pbCreature3.Image = CreatureManager.GetImage(Heroes3Master.Master, Box3Creature);
        }


        private void cbSpecialityType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSpecialityType.SelectedValue != null)
            {
                PropertyType = (ProfilePropertyType)(int)cbSpecialityType.SelectedValue;
                gridImages.ForceAllCreatures = false;

                if (PropertyType == ProfilePropertyType.SpecCreatureUpgrade)
                    selectedBoxIndex = 0;


                this.SelectedValue = 0;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ItemSelected != null)
            {
                bool flag = false;
                int arg0 = -1, arg1 = -1, arg2 = -1, arg3 = -1;

                if (PropertyType == ProfilePropertyType.SpecCreatureStatic)
                {
                    flag = true;
                    arg0 = CreatureManager.IndexesOfFirstLevelCreatures[gridImages.SelectedValue];
                    arg1 = (int)tbAttack.Value;
                    arg2 = (int)tbDefense.Value;
                    arg3 = (int)tbDamage.Value;

                }
                else if (PropertyType == ProfilePropertyType.SpecCreatureUpgrade)
                {
                    if (Box3Creature >= 0 && (Box1Creature >= 0 || Box2Creature >= 0))
                    {

                        flag = true;
                        arg0 = HeroIndex;
                        arg1 = Box1Creature;
                        arg2 = Box2Creature;
                        arg3 = Box3Creature;
                    }
                }
                else if (PropertyType == ProfilePropertyType.SpecSpeed)
                {

                    flag = true;
                    arg0 = HeroIndex;
                    arg1 = -1;
                    arg2 = -1;
                    arg3 = -1;
                }

                if (flag)
                {
                    this.ItemSelected(arg0, arg1, arg2, arg3);
                    Close();
                }
            }

        }

        private void pbCreature1_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender == pbCreature1)
            {
                selectedBoxIndex = 0;
                gridImages.ForceAllCreatures = false;
                gridImages.PropertyType = gridImages.PropertyType;
                UpdateControls();

                if (e.Button == MouseButtons.Right)
                {
                    Box1Creature = -1;
                    pbCreature1.Image = null;
                }
                else if (Box1Creature >= 0)
                {
                    gridImages.SelectedValue = Box1Creature;
                }
            }
            else if (sender == pbCreature2)
            {
                selectedBoxIndex = 1;
                gridImages.ForceAllCreatures = false;
                gridImages.PropertyType = gridImages.PropertyType;
                UpdateControls();

                if (e.Button == MouseButtons.Right)
                {
                    Box2Creature = -1;
                    pbCreature2.Image = null;
                }
                else if (Box2Creature >= 0)
                {
                    gridImages.SelectedValue = Box2Creature;
                }
            }
            else if (sender == pbCreature3)
            {
                selectedBoxIndex = 2;
                gridImages.ForceAllCreatures = true;
                gridImages.PropertyType = gridImages.PropertyType;
                UpdateControls();

                if (e.Button == MouseButtons.Right)
                {
                    Box3Creature = -1;
                    pbCreature3.Image = null;
                }
                else if (Box3Creature >= 0)
                {
                    gridImages.SelectedValue = Box3Creature;
                }
            }

            pHighlight.Left = (sender as Control).Left;

        }

    }
}
