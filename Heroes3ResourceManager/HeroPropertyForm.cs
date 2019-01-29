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
            if (propertyType == "Creature")
            {
                var img = new Bitmap(CreatureManager.GetAllCreaturesBitmap(Heroes3Master.Master.H3Sprite));
                using (var g = Graphics.FromImage(img))
                    g.FillRectangle(new SolidBrush(Color.FromArgb(190, Color.Gray)), pbMain.ClientRectangle);

                Width = img.Width;
                Height = img.Height + 25;
                pbMain.Image = img;
            }
        }

        private void pbMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (propertyType == "Creature")
            {
                if (currentHover != -1)
                {
                    Hide();
                    if (ItemSelected != null)
                        ItemSelected(currentHover);
                }
            }
        }

        private int currentHover = -1;
        private void pbMain_MouseMove(object sender, MouseEventArgs e)
        {
            int row = e.Y / 65;
            int col = e.X / 59;

            int total = col + row * 14;
            if (total != currentHover)
            {
                if (propertyType == "Creature")
                {
                    if (total < CreatureManager.AllCreatures.Count - 1)
                    {
                        pbMain.Invalidate(new Rectangle((currentHover % 14) * 59, (currentHover / 14) * 65, 58, 64));
                        currentHover = total;
                        pbMain.Invalidate(new Rectangle((currentHover % 14) * 59, (currentHover / 14) * 65, 58, 64));
                    }
                }
            }

        }

        private void pbMain_Paint(object sender, PaintEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            if (currentHover >= 0)
            {
                e.Graphics.DrawImage(CreatureManager.GetImage2(Heroes3Master.Master.H3Sprite, currentHover), (currentHover % 14) * 59, (currentHover / 14) * 65, 58, 64);
            }

            if (selectedValue >= 0)
            {
                e.Graphics.DrawImage(CreatureManager.GetImage2(Heroes3Master.Master.H3Sprite, selectedValue), (selectedValue % 14) * 59, (selectedValue / 14) * 65, 58, 64);
                e.Graphics.DrawRectangle(new Pen(Color.Red, 4), (selectedValue % 14) * 59, (selectedValue / 14) * 65, 58, 64);
            }
            Debug.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
