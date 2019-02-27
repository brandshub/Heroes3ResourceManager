using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace h3magic
{
    public partial class SpellDataControl : UserControl
    {

        private int selectedFilterIndex = -1;

        private Spell selectedSpell;
        private PictureBox[] pbSorted;
        private bool[] schoolsSelected = new bool[4];
        private Spell[] filteredSpells;
        
        private int prbHorizontalOffset = 17, prbVerticalOffset = 21;

        public int ItemCount
        {
            get { return lbSpells.Items.Count; }
        }

        public SpellDataControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            pbSorted = new PictureBox[] { pbEarth, pbWater, pbFire, pbAir };
        }


        public Spell Spell
        {
            get
            {
                return selectedSpell;
            }
            set
            {
                selectedSpell = value;
                UpdateInformation();
            }
        }

        public void LoadSpells()
        {
            LoadSpells(Spell.AllSpells);
        }

        public void LoadSpells(IEnumerable<Spell> spells)
        {
            Reset();
            filteredSpells = spells.OrderBy(s => s.Level).ThenBy(s => s.Index).ToArray();
            lbSpells.Items.AddRange(filteredSpells);

            if (cbSpellFilter.Items.Count == 0)
            {
                cbSpellFilter.Items.Add("All Magic");
                for (int i = 0; i < 4; i++)
                    cbSpellFilter.Items.Add(SecondarySkill.AllSkills[SecondarySkill.MagicSchoolSecondarySkillIndexes[i]].Name);
                cbSpellFilter.SelectedIndex = 0;
            }
        }

        public void Reset()
        {
            lbSpells.Items.Clear();
        }

        private void UpdateInformation()
        {
            if (selectedSpell != null)
            {
                int index = selectedSpell.Index;
                int filteredIndex = Array.IndexOf(filteredSpells, index);

                if (lbSpells.SelectedIndex == -1 || lbSpells.SelectedIndex != filteredIndex)
                    lbSpells.SelectedIndex = filteredIndex;

                pbSpell.Image = selectedSpell.GetImage(Heroes3Master.Master.H3Sprite);
                tbSpellName.Text = selectedSpell.Name;
                cbSpellLevel.SelectedIndex = (selectedSpell.Level - 1);


                nmMpLvl0.Value = selectedSpell.ManacostLvl0;
                nmMpLvl1.Value = selectedSpell.ManacostLvl1;
                nmMpLvl2.Value = selectedSpell.ManacostLvl2;
                nmMpLvl3.Value = selectedSpell.ManacostLvl3;

                nmBaseLvl0.Value = selectedSpell.BaseEffectLvl0;
                nmBaseLvl1.Value = selectedSpell.BaseEffectLvl1;
                nmBaseLvl2.Value = selectedSpell.BaseEffectLvl2;
                nmBaseLvl3.Value = selectedSpell.BaseEffectLvl3;

                nmEffectPerMp.Value = selectedSpell.EffectPerMagicPower;

                nmAiValue0.Value = selectedSpell.AiValue0;
                nmAiValue1.Value = selectedSpell.AiValue1;
                nmAiValue2.Value = selectedSpell.AiValue2;
                nmAiValue3.Value = selectedSpell.AiValue3;

                LoadSchoolImages();

                if (pbTownProbability.Image == null)
                {
                    pbTownProbability.Image = Town.GetAllTownsImage(prbHorizontalOffset, prbVerticalOffset);
                    int imgWidth = Town.AllTownsWithNeutral[0].LargeImage.Width;
                    int imgHeight = Town.AllTownsWithNeutral[0].LargeImage.Height;
                    PositionProbabilityBoxes(imgWidth, imgHeight, prbHorizontalOffset, prbVerticalOffset);
                }

                UpdateProbabilityBoxes();
            }
        }

        private void PositionProbabilityBoxes(int imageWidth, int imageHeight, int hOff, int vOff)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    var nm = Controls.Find("nmProb" + (i * 3 + j), false).FirstOrDefault() as NumericUpDown;
                    if (nm != null)
                    {
                        nm.Left = pbTownProbability.Left + (hOff + imageWidth) * j;
                        nm.Top = pbTownProbability.Top + imageHeight * (i + 1) + i * vOff;
                        nm.Value = selectedSpell.GetChanceToGainForCastle(i * 3 + j);
                    }
                }
        }

        private void UpdateProbabilityBoxes()
        {
            for (int i = 0; i < 9; i++)
            {
                var nm = Controls.Find("nmProb" + i, false).FirstOrDefault() as NumericUpDown;
                if (nm != null)
                    nm.Value = selectedSpell.GetChanceToGainForCastle(i);
            }
        }

        private void TryLoadStaticImages()
        {
            if (BitmapCache.SpellsMagicSchools == null || BitmapCache.SpellsMagicSchoolsInactive == null)
            {
                BitmapCache.SpellsMagicSchools = new Bitmap[5];
                BitmapCache.SpellsMagicSchoolsInactive = new Bitmap[4];

                for (int i = 0; i < 4; i++)
                {
                    BitmapCache.SpellsMagicSchools[i] = SecondarySkill.GetImage(Heroes3Master.Master.H3Sprite, SecondarySkill.MagicSchoolSecondarySkillIndexes[i], 3);

                    var inactive = new Bitmap(BitmapCache.SpellsMagicSchools[i]);
                    using (var g = Graphics.FromImage(inactive))
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(190, Color.Gray)), 0, 0, inactive.Width, inactive.Height);
                    }
                    BitmapCache.SpellsMagicSchoolsInactive[i] = inactive;
                }
            }
        }


        public void LoadSchoolImages()
        {
            TryLoadStaticImages();
            if (selectedSpell != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    var flag = selectedSpell.HasSchool(i);
                    schoolsSelected[i] = flag;
                    SetSchoolImage(i, flag);
                }
            }
        }

        public void SwitchSchoolImage(int index)
        {
            schoolsSelected[index] = !schoolsSelected[index];
            SetSchoolImage(index, schoolsSelected[index]);
        }

        public void SetSchoolImage(int index, bool active)
        {
            pbSorted[index].Image = active ? BitmapCache.SpellsMagicSchools[index] : BitmapCache.SpellsMagicSchoolsInactive[index];
        }

        private void schoolMagicBox_Click(object sender, EventArgs e)
        {
            int index = -1;
            var pbSender = (PictureBox)sender;
            if (pbSender != null)
            {
                for (int i = 0; i < pbSorted.Length; i++)
                {
                    if (pbSorted[i] == pbSender)
                    {
                        index = i;
                        break;
                    }
                }
                if (index >= 0)
                {
                    SwitchSchoolImage(index);
                }
            }
        }

        private void lbSpells_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Heroes3Master.Master != null && lbSpells.SelectedIndex >= 0)
            {
                var spell = Spell.AllSpells[filteredSpells[lbSpells.SelectedIndex].Index];
                if (spell != selectedSpell)
                    Spell = spell;
            }
        }



        private bool cacheSpells = true;

        private void dl(string s)
        {
            Debug.WriteLine(Name + " " + DateTime.Now.ToString("HH:mm:ss") + " " + s);
        }

        private void lbSpells_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (Heroes3Master.Master != null && e.Index >= 0)
            {

                dl(e.Index+" "+e.State.ToString());
                if (e.State == (DrawItemState.Selected | DrawItemState.Focus | DrawItemState.NoAccelerator | DrawItemState.NoFocusRect))
                    return;

                if (!cacheSpells)
                {
                    e.Graphics.DrawString(Spell.AllSpells[e.Index].Name, e.Font, Brushes.Black, e.Bounds.X + 42, e.Bounds.Y + 4);
                    var img = new Bitmap(Spell.AllSpells[e.Index].GetImage(Heroes3Master.Master.H3Sprite), 36, 24);
                    e.Graphics.DrawImage(img, e.Bounds.X, e.Bounds.Y);
                }
                else
                {
                    if (BitmapCache.DrawItemSpellsListBox == null)
                    {
                        BitmapCache.DrawItemSpellsListBox = new Bitmap[Spell.AllSpells.Count];
                    }

                    Bitmap cached;
                    if (BitmapCache.DrawItemSpellsListBox[filteredSpells[e.Index].Index] == null)
                    {
                        cached = new Bitmap(e.Bounds.Width, e.Bounds.Height);
                        using (var g = Graphics.FromImage(cached))
                        {
                            var spell = filteredSpells[e.Index];
                            var img = new Bitmap(spell.GetImage(Heroes3Master.Master.H3Sprite), 29, 32);

                            var schools = new List<int>();
                            var schoolsCount = 0;
                            for (int i = 0; i < 4; i++)
                                if (spell.HasSchool(i))
                                {
                                    schools.Add(i);
                                    schoolsCount++;
                                }

                            /*if (schoolsCount == 1)
                            {
                                g.FillRectangle(new SolidBrush(Spell.MagicSchoolColors[schools[0]]), new Rectangle(Point.Empty, e.Bounds.Size));
                            }
                            else if (schoolsCount > 1)
                            {
                                float divWidth = (e.Bounds.Width - img.Width) / (float)schoolsCount;
                                for (int i = 0; i < schools.Count; i++)
                                {
                                    g.FillRectangle(new SolidBrush(Spell.MagicSchoolColors[schools[i]]), img.Width + i * divWidth, 0, divWidth, e.Bounds.Height);
                                }
                            }*/

                            //Color clr = selectedFilterIndex == 0 ? Color.Lavender : Spell.MagicSchoolColors[selectedFilterIndex - 1];
                            Color clr = schoolsCount == 1 ? Spell.MagicSchoolColors[schools[0]] : Color.Lavender;

                            g.FillRectangle(new SolidBrush(clr), new Rectangle(Point.Empty, e.Bounds.Size));

                            g.DrawString(string.Format("[{0}] {1}", spell.Level, spell.Name), e.Font, Brushes.Black, 32, 9);
                            g.DrawImage(img, Point.Empty);
                        }
                        BitmapCache.DrawItemSpellsListBox[filteredSpells[e.Index].Index] = cached;
                    }
                    else
                    {
                        cached = BitmapCache.DrawItemSpellsListBox[filteredSpells[e.Index].Index];
                    }

                    e.Graphics.DrawImage(cached, e.Bounds.Location);
                }

                if (e.State == ( DrawItemState.Focus | DrawItemState.NoAccelerator | DrawItemState.NoFocusRect ))
                {
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot }, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 1);
                }
            }
        }



        public void SaveData()
        {
            if (selectedSpell != null)
            {
                selectedSpell.Name = tbSpellName.Text.Replace('\t', ' ');
                selectedSpell.Level = cbSpellLevel.SelectedIndex + 1;

                selectedSpell.IsEarthMagic = schoolsSelected[0];
                selectedSpell.IsWaterMagic = schoolsSelected[1];
                selectedSpell.IsFireMagic = schoolsSelected[2];
                selectedSpell.IsAirMagic = schoolsSelected[3];

                selectedSpell.ManacostLvl0 = (int)nmMpLvl0.Value;
                selectedSpell.ManacostLvl1 = (int)nmMpLvl1.Value;
                selectedSpell.ManacostLvl2 = (int)nmMpLvl2.Value;
                selectedSpell.ManacostLvl3 = (int)nmMpLvl3.Value;

                selectedSpell.EffectPerMagicPower = (int)nmEffectPerMp.Value;

                selectedSpell.BaseEffectLvl0 = (int)nmBaseLvl0.Value;
                selectedSpell.BaseEffectLvl1 = (int)nmBaseLvl1.Value;
                selectedSpell.BaseEffectLvl2 = (int)nmBaseLvl2.Value;
                selectedSpell.BaseEffectLvl3 = (int)nmBaseLvl3.Value;

                selectedSpell.AiValue0 = (int)nmAiValue0.Value;
                selectedSpell.AiValue1 = (int)nmAiValue1.Value;
                selectedSpell.AiValue2 = (int)nmAiValue2.Value;
                selectedSpell.AiValue3 = (int)nmAiValue3.Value;

                selectedSpell.SetChanceToGainForCastle(0, (int)nmProb0.Value);
                selectedSpell.SetChanceToGainForCastle(1, (int)nmProb1.Value);
                selectedSpell.SetChanceToGainForCastle(2, (int)nmProb2.Value);
                selectedSpell.SetChanceToGainForCastle(3, (int)nmProb3.Value);
                selectedSpell.SetChanceToGainForCastle(4, (int)nmProb4.Value);
                selectedSpell.SetChanceToGainForCastle(5, (int)nmProb5.Value);
                selectedSpell.SetChanceToGainForCastle(6, (int)nmProb6.Value);
                selectedSpell.SetChanceToGainForCastle(7, (int)nmProb7.Value);
                selectedSpell.SetChanceToGainForCastle(8, (int)nmProb8.Value);

                selectedSpell.HasChanges = true;
            }
        }



        private void cbSpellFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedFilterIndex = cbSpellFilter.SelectedIndex;
            if (selectedFilterIndex == 0)
            {
                LoadSpells();
            }
            else
            {
                LoadSpells(Spell.AllSpells.Where(s => s.HasSchool(selectedFilterIndex - 1)));
            }
        }

        private void cbSpellFilter_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (Heroes3Master.Master != null && e.Index >= 0 && selectedFilterIndex >= 0)
            {
                Color clr;
                Bitmap schoolImage;

                if (e.Index == 0)
                {
                    clr = Color.Lavender;
                    schoolImage = new Bitmap(SecondarySkill.GetImage(Heroes3Master.Master.H3Sprite, 25, 3), 32, 32); //25 is sorecery
                    //schoolImage = SecondarySkill.GetImage(Heroes3Master.Master.H3Sprite, 25, 3); //25 is sorecery
                }
                else
                {
                    clr = Spell.MagicSchoolColors[e.Index - 1];
                    schoolImage = new Bitmap(SecondarySkill.GetImage(Heroes3Master.Master.H3Sprite, SecondarySkill.MagicSchoolSecondarySkillIndexes[e.Index - 1], 3), 32, 32);
                    // schoolImage = SecondarySkill.GetImage(Heroes3Master.Master.H3Sprite, SecondarySkill.MagicSchoolSecondarySkillIndexes[e.Index - 1], 3);
                }

                e.Graphics.FillRectangle(new SolidBrush(clr), e.Bounds);
                e.Graphics.DrawImage(schoolImage, e.Bounds.X, e.Bounds.Y);
                e.Graphics.DrawString(cbSpellFilter.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds.X + 56, e.Bounds.Y + 9);
            }
        }
    }
}
