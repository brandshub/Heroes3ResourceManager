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
    public partial class HeroMainDataControl : UserControl
    {
        private int selectedCastle;
        private int selectedHeroIndex;

        private HeroStats[] filteredHeroes;

        public HeroMainDataControl()
        {
            InitializeComponent();
        }


        public HeroProfileControl HeroProfileControl
        {
            get { return hpcHeroProfile; }
        }

        public ListBoxWithImages HeroesList
        {
            get { return lbHeroes; }
        }

        public HeroExeData Hero
        {
            get { return hpcHeroProfile.Hero; }
            set { hpcHeroProfile.Hero = value; }
        }

        public int SelectedHeroIndex
        {
            get
            {
                return selectedHeroIndex;
            }
            set
            {
                selectedHeroIndex = value;
            }
            }

        public int RealHeroIndex
        {
            get
            {
                if (cbCastles.SelectedIndex == 0)
                    return lbHeroes.SelectedIndex;

                if (cbCastles.SelectedIndex == -1)
                    return -1;

                return (cbCastles.SelectedIndex - 1) * 16 + lbHeroes.SelectedIndex;
            }
        }

        public void Reset()
        {
            lbHeroes.Items.Clear();
            selectedHeroIndex = 0;
        }



        public void ResetCastles()
        {
            cbCastles.Items.Clear();
            Reset();
        }

        public void LoadCastles()
        {
            var array = new string[Town.TownNamesWithNeutral.Length];
            array[0] = "All Castles";
            for (int i = 1; i < array.Length; i++)
                array[i] = Town.TownNamesWithNeutral[i - 1];

            cbCastles.Items.AddRange(array);
            cbCastles.SelectedIndex = 0;
        }



        public int SelectedCastle
        {
            get
            {
                return selectedCastle;
            }
            set
            {
                selectedCastle = value;
                if (cbCastles.Items != null && value < cbCastles.Items.Count)
                    cbCastles.SelectedIndex = selectedCastle;
            }
        }

        public void LoadHeroes()
        {
            LoadHeroes(HeroesManager.AllHeroes);
        }

        public void LoadHeroes(IEnumerable<HeroStats> data)
        {
            Reset();

            filteredHeroes = data.ToArray();
            lbHeroes.Items.AddRange(data.Select(st => st.Name).ToArray());
        }

        private void cbCastles_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCastle = cbCastles.SelectedIndex;

            if (selectedCastle == 0)
            {
                LoadHeroes();
            }
            else
            {
                var list = HeroesManager.AllHeroes.Skip((selectedCastle - 1) * 16).Take(16);
                LoadHeroes(list);
            }
        }

        private void lbHeroes_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (lbHeroes.SelectedIndex > -1 && Heroes3Master.Master != null)
            {
                //pbPortraitSmall.Image = Heroes3Master.Master.H3Bitmap[HeroesManager.HeroesOrder[hs.ImageIndex].Replace("HPL", "HPS")].GetBitmap(selectedLodFile.stream);
                selectedHeroIndex = RealHeroIndex;
                hpcHeroProfile.LoadHero(selectedHeroIndex, Heroes3Master.Master);
                var hs = HeroesManager.AllHeroes[selectedHeroIndex];

                //Text = lbHeroes.SelectedIndex.ToString();
                var hd = HeroExeData.Data[selectedHeroIndex];
                tbHeroName.Text = hs.Name;
                tbHeroBio.Text = hs.Biography;
                tbHeroSpecDesc.Text = hs.Speciality;
                tbHeroLS1.Text = hs.LowStack1.ToString();
                tbHeroHS1.Text = hs.HighStack1.ToString();
                tbHeroLS2.Text = hs.LowStack2.ToString();
                tbHeroHS2.Text = hs.HighStack2.ToString();
                tbHeroLS3.Text = hs.LowStack3.ToString();
                tbHeroHS3.Text = hs.HighStack3.ToString();
            }

        }
        private void cbCastles_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (Heroes3Master.Master != null && e.Index >= 0)
            {
                int index2 = (e.Index == 0 ? Town.AllTownsWithNeutral.Length : e.Index) - 1;

                var town = Town.AllTownsWithNeutral[index2];
                var clr = Town.AllColors[index2];

                e.Graphics.FillRectangle(new SolidBrush(clr), e.Bounds);
                e.Graphics.DrawImage(town.LargeImage, e.Bounds.X, e.Bounds.Y);
                e.Graphics.DrawString(cbCastles.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds.X + 56, e.Bounds.Y + 9);
            }
        }

        private void lbHeroes_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (Heroes3Master.Master != null && e.Index >= 0)
            {

                if (e.State == (DrawItemState.Selected | DrawItemState.Focus | DrawItemState.NoAccelerator | DrawItemState.NoFocusRect))
                    return;


                int castleIndex = (cbCastles.SelectedIndex == 0 ? Town.AllTownsWithNeutral.Length : cbCastles.SelectedIndex) - 1;
                int realIndex = (cbCastles.SelectedIndex == 0 ? 0 : (cbCastles.SelectedIndex - 1) * 16) + e.Index;

                var clr = Town.AllColors[realIndex / 16];



                if (BitmapCache.DrawItemHeroesListBox == null)
                    BitmapCache.DrawItemHeroesListBox = new Bitmap[HeroesManager.HeroesOrder.Length];



                Bitmap cached;
                if (BitmapCache.DrawItemHeroesListBox[realIndex] == null)
                {
                    cached = new Bitmap(lbHeroes.Width, e.Bounds.Height);
                    using (var g = Graphics.FromImage(cached))
                    {
                        g.FillRectangle(new SolidBrush(clr), new Rectangle(Point.Empty, new Size(lbHeroes.Width,e.Bounds.Height )));

                        g.DrawString(HeroesManager.AllHeroes[realIndex].Name, e.Font, Brushes.Black, 42, 4);
                        var img = new Bitmap(Heroes3Master.Master.ResolveWith(HeroesManager.HeroesOrder[realIndex].Replace("HPL", "HPS")).GetBitmap(), 36, 24);
                        g.DrawImage(img, Point.Empty);
                    }
                    BitmapCache.DrawItemHeroesListBox[realIndex] = cached;
                }
                else
                {
                    cached = BitmapCache.DrawItemHeroesListBox[realIndex];
                }

                e.Graphics.DrawImage(cached, e.Bounds.Location);


                if ((e.State & DrawItemState.Selected) != 0)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot }, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 1);
                }
            }
        }

        public void SaveLocalChanges()
        {
            if (selectedHeroIndex >= 0)
            {
                var hs = new HeroStats();

                hs.Name = tbHeroName.Text;
                hs.Biography = tbHeroBio.Text;
                hs.Speciality = tbHeroSpecDesc.Text;
                hs.LowStack1 = int.Parse(tbHeroLS1.Text);
                hs.HighStack1 = int.Parse(tbHeroHS1.Text);
                hs.LowStack2 = int.Parse(tbHeroLS2.Text);
                hs.HighStack2 = int.Parse(tbHeroHS2.Text);
                hs.LowStack3 = int.Parse(tbHeroLS3.Text);
                hs.HighStack3 = int.Parse(tbHeroHS3.Text);

                HeroesManager.AllHeroes[selectedHeroIndex] = hs;

                HeroesManager.AnyChanges = true;
            }
        }
    }
}
