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
    public partial class HeroClassDataControl : UserControl
    {
        private static string[] Aggression = new[] { "0,80", "0,90", "1,00", "1,10", "1,20" };

        private HeroClass heroClass;
        private bool controlsCreated;
        public HeroClassDataControl()
        {
            InitializeComponent();
        }

        public HeroClass HeroClass
        {
            get
            {
                return heroClass;
            }
            set
            {
                heroClass = value;
                if (value != null)
                    UpdateInformtion();
            }
        }

        private void UpdateInformtion()
        {
            GenerateControls();
            if (heroClass != null)
                lbHeroClasses.SelectedIndex = heroClass.Index;
        }

        public void LoadHeroClasses()
        {
            Reset();
            lbHeroClasses.Items.AddRange(HeroClass.AllHeroClasses.Where(s => !string.IsNullOrEmpty(s.Name)).Select(st => st.Name).ToArray());
        }

        private void GenerateControls()
        {
            if (!controlsCreated)
            {
                cbAggression.Items.AddRange(Aggression);
                SuspendLayout();
                var bmp = SecondarySkill.GetSkillTreeForHeroClass(Heroes3Master.Master);
                pbSkillTree.Width = bmp.Width;
                pbSkillTree.Height = bmp.Height;
                pbSkillTree.Image = bmp;


                int itemWidth = pbSkillTree.Width / 7;
                int itemHeight = pbSkillTree.Height / 4;

                var bmp2 = HeroesManager.GetPrimarySkillsPanel(Heroes3Master.Master);
                pbPrimarySkills.Image = bmp2;


                int totalCount = HeroClass.Stats.Length - 9;
                int offset = 2;
                int maxTab = Controls.Cast<Control>().Max(s => s.TabIndex) - 1;
                for (int i = offset; i < totalCount; i++)
                {
                    var ctrl = new NumericUpDown();

                    ((ISupportInitialize)ctrl).BeginInit();
                    ctrl.Name = "nmStat" + i;
                    ctrl.TextAlign = HorizontalAlignment.Center;
                    ctrl.Maximum = 99;
                    ctrl.TabIndex = i + maxTab;

                    int row, col;
                    if (i < offset + 12)
                    {
                        ctrl.Size = new Size(42, 20);
                        row = (i - 2) / 4;
                        col = (i - 2) % 4;

                        int y = pbPrimarySkills.Top + pbPrimarySkills.Height + 3 + row * (20 + 5);
                        ctrl.Location = new Point(pbPrimarySkills.Left + col * 42, y);

                        if (row == 0)
                        {
                            ctrl.Font = new Font(ctrl.Font, FontStyle.Bold);
                        }
                        if (col == 0)
                        {
                            var lbl = Controls.Find("lbl0" + (row + 1), false).FirstOrDefault() as Label;
                            if (lbl != null)
                            {
                                lbl.Top = y + (ctrl.Height - lbl.Height) / 2;
                            }
                        }

                    }
                    else
                    {
                        ctrl.Size = new Size(itemWidth, 24);
                        row = (i - 14) / 7;
                        col = (i - 14) % 7;

                        ctrl.Width = itemWidth;
                        ctrl.Location = new Point(pbSkillTree.Left + itemWidth * col, pbSkillTree.Top + (itemHeight) * (row + 1) - 19);
                    }

                    Controls.Add(ctrl);
                    ((ISupportInitialize)ctrl).EndInit();
                }

                pbSkillTree.SendToBack();
                ResumeLayout(false);
                PerformLayout();
                controlsCreated = true;
            }

        }

        private void lbHeroClasses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbHeroClasses.SelectedIndex >= 0)
            {
                var heroClass = HeroClass.AllHeroClasses[lbHeroClasses.SelectedIndex];
                tbName.Text = heroClass.Name;
             //   cbAggression.SelectedIndex = Array.IndexOf(Aggression, heroClass.Stats[1]);

                for (int i = 2; i < heroClass.Stats.Length - 9; i++)
                {
                    var nm = Controls.Find("nmStat" + i, false).FirstOrDefault() as NumericUpDown;
                    if (nm != null)
                        nm.Value = heroClass.GetStat(i);
                }
            }
        }

        public void Reset()
        {
            lbHeroClasses.Items.Clear();
            cbAggression.Items.Clear();
        }

        public void GoToPrimarySkills()
        {
            var nm = Controls.Find("nmStat2", false).FirstOrDefault() as NumericUpDown;
            if (nm != null)
            {
                nm.Focus();
            }
        }

        public void SaveLocalChanges()
        {
            if (lbHeroClasses.SelectedIndex >= 0)
            {
                var heroClass = HeroClass.AllHeroClasses[lbHeroClasses.SelectedIndex];


                if (string.IsNullOrEmpty(tbName.Text) || HeroClass.AllHeroClasses.Any(c => c.Name == tbName.Text && c.Index != heroClass.Index))
                {
                    MessageBox.Show("Specify unique non-blank class name");
                    return;
                }

                if (tbName.Text.Length > 20)
                    tbName.Text = tbName.Text.Substring(0, 20);

                heroClass.Stats[0] = tbName.Text;
               // heroClass.Stats[1] = cbAggression.Text;
                for (int i = 2; i < heroClass.Stats.Length - 9; i++)
                {
                    var nm = Controls.Find("nmStat" + i, false).FirstOrDefault() as NumericUpDown;
                    if (nm != null)
                    {
                        heroClass.Stats[i] = nm.Value.ToString();
                    }
                }

                lbHeroClasses.InvalidateSelected();
                HeroClass.AnyChanges = true;
            }
        }

        private void lbHeroClasses_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (Heroes3Master.Master != null && e.Index >= 0)
            {
                int castleIndex = e.Index / 2;
                var town = Town.AllTownsWithNeutral[castleIndex];
                var clr = Town.AllColors[castleIndex];

                e.Graphics.FillRectangle(new SolidBrush(clr), e.Bounds);
                e.Graphics.DrawImage(town.Image, e.Bounds.X, e.Bounds.Y);
                e.Graphics.DrawString(HeroClass.AllHeroClasses[e.Index].Name, e.Font, Brushes.Black, e.Bounds.X + 42, e.Bounds.Y + 4);

                if ((e.State & DrawItemState.Selected) != 0)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot }, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 1);
                }
            }
        }

    }
}
