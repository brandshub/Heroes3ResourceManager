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
    public partial class SpellDataControl : UserControl
    {
        private Spell spell;
        private PictureBox[] pbSorted;
        private bool[] schoolsSelected = new bool[4];

        private int prbImagesWidth, prbImagesHeight;
        private int prbHorizontalOffset = 17, prbVerticalOffset = 21;

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
                return spell;
            }
            set
            {
                spell = value;
                UpdateInformation();
            }

        }

        private void UpdateInformation()
        {
            if (spell != null)
            {
                pbSpell.Image = spell.GetImage(Heroes3Master.Master.H3Sprite);
                tbSpellName.Text = spell.Name;
                cbSpellLevel.SelectedIndex = (spell.Level - 1);
                nmMpLvl0.Value = spell.ManacostLvl0;
                nmMpLvl1.Value = spell.ManacostLvl1;
                nmMpLvl2.Value = spell.ManacostLvl2;
                nmMpLvl3.Value = spell.ManacostLvl3;

                nmBaseLvl0.Value = spell.BaseEffectLvl0;
                nmBaseLvl1.Value = spell.BaseEffectLvl1;
                nmBaseLvl2.Value = spell.BaseEffectLvl2;
                nmBaseLvl3.Value = spell.BaseEffectLvl3;

                nmEffectPerMp.Value = spell.EffectPerMagicPower;

                nmAiValue0.Value = spell.AiValue0;
                nmAiValue1.Value = spell.AiValue1;
                nmAiValue2.Value = spell.AiValue2;
                nmAiValue3.Value = spell.AiValue3;

                LoadSchoolImages();

                if (pbTownProbability.Image == null)
                {
                    pbTownProbability.Image = Town.GetAllTownsImage(Heroes3Master.Master.H3Sprite, 17, 21);
                    int imgWidth = Town.AllTowns[0].LargeImage.Width;
                    int imgHeight = Town.AllTowns[0].LargeImage.Height;
                    PositionProbabilityBoxes(imgWidth, imgHeight, 17, 21);
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
                        nm.Value = spell.GetChanceToGainForCastle(i * 3 + j);
                    }
                }
        }

        private void UpdateProbabilityBoxes()
        {
            for (int i = 0; i < 9; i++)
            {
                var nm = Controls.Find("nmProb" + i, false).FirstOrDefault() as NumericUpDown;
                if (nm != null)
                    nm.Value = spell.GetChanceToGainForCastle(i);
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
                    BitmapCache.SpellsMagicSchools[i] = SecondarySkill.GetImage(Heroes3Master.Master.H3Sprite, SecondarySkill.SchoolSecondarySkillIndexes[i], 3);

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
            if (spell != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    var flag = spell.HasSchool(i);
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



    }
}
