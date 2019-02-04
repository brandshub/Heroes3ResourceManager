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

        public int CurrentIndex
        {
            get { return gridImages.CurrentIndex; }
            set { gridImages.CurrentIndex = value; }
        }

        public int SelectedValue
        {
            get { return gridImages.SelectedValue; }
            set { gridImages.SelectedValue = value; }
        }

        public HeroPropertyForm()
        {
            InitializeComponent();
            InitSpecialityTypeCombo();


            gridImages.ItemSelected += gridImages_ItemSelected;
        }

        private void gridImages_ItemSelected(int value)
        {
            if (this.ItemSelected != null)
            {
                this.ItemSelected(value);
                Close();
            }
        }

        public event Action<int> ItemSelected;


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
            if (pt == ProfilePropertyType.Creature || pt == ProfilePropertyType.SecondarySkill || pt == ProfilePropertyType.Spell)
            {
                cbSpecialityType.Visible = false;
                lblType.Visible = false;

                gridImages.Top = 0;
                height = gridImages.Height + TitlebarHeight + 2 * BorderWidth + 1;
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

        private void cbSpecialityType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSpecialityType.SelectedValue != null)
            {
                PropertyType = (ProfilePropertyType)(int)cbSpecialityType.SelectedValue;
                this.SelectedValue = -1;
            }
        }

        private void HeroPropertyForm_Load(object sender, EventArgs e)
        {

        }

    }
}
