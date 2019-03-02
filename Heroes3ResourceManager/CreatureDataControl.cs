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
    public partial class CreatureDataControl : UserControl
    {
        private int selectedCastle = -1;
        private CreatureAnimationLoop creatureAnimation;
        private bool cacheOther = false;
        private Creature[] otherCreatures = null;

        public CreatureDataControl()
        {
            InitializeComponent();
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
        public void Reset()
        {
            if (cbCastles.Items != null)
                cbCastles.Items.Clear();
            if (cbCreatures.Items != null)
                cbCreatures.Items.Clear();
        }

        public void LoadCastles()
        {
            Reset();
            cbCastles.Items.AddRange(Town.TownNamesWithNeutral);
        }

        private void cbCastles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CreatureManager.Loaded)
            {
                cbCreatures.Items.Clear();
                selectedCastle = cbCastles.SelectedIndex;
                var creatures = CreatureManager.OnlyActiveCreatures.Where(c => c.TownIndex == cbCastles.SelectedIndex && c.CreatureIndex != 149).Select(cs => cs.Name).ToArray();
                if (creatures.Length > 0)
                {
                    cbCreatures.Items.AddRange(creatures);
                    cbCreatures.SelectedIndex = 0;
                }                
            }
        }

        private void cbCreatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Heroes3Master.Master != null && CreatureManager.Loaded)
            {
                var creature = CreatureManager.Get(cbCastles.SelectedIndex, cbCreatures.SelectedIndex);
                LoadCreatureInfo(creature);

                string allCreatures = Properties.Resources.creatures;
                string defName = allCreatures.Split(new[] { "\r\n" }, StringSplitOptions.None)[creature.CreatureIndex].Split(';')[2] + ".def";
                var lodFile = Heroes3Master.Master.Resolve(defName);


                if (creatureAnimation != null)
                {
                    pbCreature.Image = null;
                    creatureAnimation.Dispose();
                }


                var def = lodFile[defName].GetDefFile();
                if (def != null)
                {
                    creatureAnimation = new CreatureAnimationLoop(creature.CreatureIndex, def);
                    creatureAnimation.TimerTick += creatureAnimation_TimerTick;
                    creatureAnimation.Enabled = true;
                }
            }
        }



        private void creatureAnimation_TimerTick(object sender, EventArgs e)
        {
            if (creatureAnimation != null)
            {
                pbCreature.Image = creatureAnimation.GetFrame2(Heroes3Master.Master);
            }
        }

        private void LoadCreatureInfo(Creature creatureStats)
        {
            textBox1.Text = creatureStats.Name;
            textBox3.Text = creatureStats.Attack.ToString();
            textBox4.Text = creatureStats.Defence.ToString();
            textBox5.Text = creatureStats.Arrows.ToString();
            textBox6.Text = creatureStats.HP.ToString();
            textBox7.Text = creatureStats.Speed.ToString();
            textBox8.Text = creatureStats.LoDamage.ToString();
            textBox9.Text = creatureStats.HiDamage.ToString();
            textBox10.Text = creatureStats.PriceLumber.ToString();
            textBox11.Text = creatureStats.PriceMercury.ToString();
            textBox12.Text = creatureStats.PriceOre.ToString();
            textBox13.Text = creatureStats.PriceCrystals.ToString();
            textBox14.Text = creatureStats.PriceGems.ToString();
            textBox15.Text = creatureStats.PriceSulphur.ToString();
            textBox16.Text = creatureStats.PriceGold.ToString();
            textBox17.Text = creatureStats.Plural1;
            textBox18.Text = creatureStats.Plural2;
            textBox19.Text = creatureStats.Growth.ToString();
            textBox20.Text = creatureStats.FightValue.ToString();
            textBox21.Text = creatureStats.AIValue.ToString();
            textBox22.Text = creatureStats.Spells.ToString();
            textBox23.Text = creatureStats.Description;
        }

        private void cbCreatures_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (Heroes3Master.Master != null && e.Index >= 0)
            {
                int castleIndex = selectedCastle;
                var clr = Town.AllColors[castleIndex];

                if (!cacheOther)
                {
                    e.Graphics.FillRectangle(new SolidBrush(clr), e.Bounds);
                    var creature = CreatureManager.Get(castleIndex, e.Index);
                    e.Graphics.DrawImage(CreatureManager.GetSmallImage(Heroes3Master.Master, creature.CreatureIndex), e.Bounds.X, e.Bounds.Y);
                    e.Graphics.DrawString(cbCreatures.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds.X + 46, e.Bounds.Y + 9);
                }
                else
                {
                    if (BitmapCache.DrawItemCreaturesOtherComboBox == null)
                    {
                        otherCreatures = CreatureManager.OnlyActiveCreatures.Where(c => c.TownIndex == 9 && c.CreatureIndex != 149).ToArray();
                        BitmapCache.DrawItemCreaturesOtherComboBox = new Bitmap[otherCreatures.Length];
                    }

                    Bitmap cached;
                    if (BitmapCache.DrawItemCreaturesOtherComboBox[e.Index] == null)
                    {
                        cached = new Bitmap(e.Bounds.Width, e.Bounds.Height);
                        using (var g = Graphics.FromImage(cached))
                        {
                            g.FillRectangle(new SolidBrush(clr), new Rectangle(Point.Empty, e.Bounds.Size));
                            var creature = CreatureManager.Get(castleIndex, e.Index);
                            g.DrawImage(CreatureManager.GetSmallImage(Heroes3Master.Master, creature.CreatureIndex), Point.Empty);
                            g.DrawString(creature.Name.ToString(), e.Font, Brushes.Black, 46, 9);
                        }
                        BitmapCache.DrawItemCreaturesOtherComboBox[e.Index] = cached;
                    }
                    else
                    {
                        cached = BitmapCache.DrawItemCreaturesOtherComboBox[e.Index];
                    }
                    e.Graphics.DrawImage(cached, e.Bounds.Location);

                }
            }
        }


        private void cbCastles_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (Heroes3Master.Master != null && e.Index >= 0)
            {
                int castleIndex = e.Index;
                var town = Town.AllTownsWithNeutral[castleIndex];
                var clr = Town.AllColors[castleIndex];

                e.Graphics.FillRectangle(new SolidBrush(clr), e.Bounds);
                e.Graphics.DrawImage(town.LargeImage, e.Bounds.X, e.Bounds.Y);
                e.Graphics.DrawString(cbCastles.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds.X + 56, e.Bounds.Y + 9);
            }
        }

        public void SaveData()
        {
            var cs = CreatureManager.OnlyActiveCreatures.Where(c => c.TownIndex == cbCastles.SelectedIndex && c.CreatureCastleRelativeIndex == cbCreatures.SelectedIndex).FirstOrDefault();
            cs.Name = textBox1.Text;
            cs.Attack = int.Parse(textBox3.Text);
            cs.Defence = int.Parse(textBox4.Text);
            cs.Arrows = int.Parse(textBox5.Text);
            cs.HP = int.Parse(textBox6.Text);
            cs.Speed = int.Parse(textBox7.Text);
            cs.LoDamage = int.Parse(textBox8.Text);
            cs.HiDamage = int.Parse(textBox9.Text);
            cs.PriceLumber = int.Parse(textBox10.Text);
            cs.PriceMercury = int.Parse(textBox11.Text);
            cs.PriceOre = int.Parse(textBox12.Text);
            cs.PriceCrystals = int.Parse(textBox13.Text);
            cs.PriceGems = int.Parse(textBox14.Text);
            cs.PriceSulphur = int.Parse(textBox15.Text);
            cs.PriceGold = int.Parse(textBox16.Text);
            cs.Plural1 = textBox17.Text;
            cs.Plural2 = textBox18.Text;
            cs.Growth = int.Parse(textBox19.Text);
            cs.FightValue = int.Parse(textBox20.Text);
            cs.AIValue = int.Parse(textBox21.Text);
            cs.Spells = int.Parse(textBox22.Text);
            cs.Description = textBox23.Text;
            CreatureManager.AnyChanges = true;
        }



    }
}
