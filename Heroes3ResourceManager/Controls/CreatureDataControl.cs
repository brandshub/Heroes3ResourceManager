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
            SelectedCastle = -1;
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

        private void LoadCreatureInfo(Creature cr)
        {
            tbName.Text = cr.Name;

            nmAttack.Value = cr.Attack;
            nmDefense.Value = cr.Defense;
            nmDmgLow.Value = cr.LoDamage;
            nmDmgHigh.Value = cr.HiDamage;
            nmHp.Value = cr.HP;
            nmSpeed.Value = cr.Speed;
            nmArrows.Value = cr.Arrows;

            nmLumber.Value = cr.PriceLumber;
            nmOre.Value = cr.PriceOre;
            nmMercury.Value = cr.PriceMercury;
            nmSulphur.Value = cr.PriceSulphur;
            nmCrystals.Value = cr.PriceCrystals;
            nmGems.Value = cr.PriceGems;
            nmGold.Value = cr.PriceGold;

            nmAiValue.Value = cr.AIValue;
            nmHordeGrowth.Value = cr.Growth;
            nmFightValue.Value = cr.FightValue;
            nmSpells.Value = cr.Spells;

            tbPlural1.Text = cr.Plural1;
            tbPlural2.Text = cr.Plural2;
            tbDescription.Text = cr.Description;
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
                    e.Graphics.DrawString(creature.Name, e.Font, Brushes.Black, e.Bounds.X + 46, e.Bounds.Y + 9);
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

        public void SaveLocalChanges()
        {
            var cr = CreatureManager.OnlyActiveCreatures.Where(c => c.TownIndex == cbCastles.SelectedIndex && c.CreatureCastleRelativeIndex == cbCreatures.SelectedIndex).FirstOrDefault();

            cr.Name = tbName.Text;

            cr.Attack = (int)nmAttack.Value;
            cr.Defense = (int)nmDefense.Value;
            cr.LoDamage = (int)nmDmgLow.Value;
            cr.HiDamage = (int)nmDmgHigh.Value;
            cr.HP = (int)nmHp.Value;
            cr.Speed = (int)nmSpeed.Value;
            cr.Arrows = (int)nmArrows.Value;

            cr.PriceLumber = (int)nmLumber.Value;
            cr.PriceOre = (int)nmOre.Value;
            cr.PriceMercury = (int)nmMercury.Value;
            cr.PriceSulphur = (int)nmSulphur.Value;
            cr.PriceCrystals = (int)nmCrystals.Value;
            cr.PriceGems = (int)nmGems.Value;
            cr.PriceGold = (int)nmGold.Value;

            cr.Growth = (int)nmHordeGrowth.Value;
            cr.FightValue = (int)nmFightValue.Value;
            cr.Spells = (int)nmSpells.Value;
            cr.AIValue = (int)nmAiValue.Value;

            cr.Plural1 = tbPlural1.Text;
            cr.Plural2 = tbPlural2.Text;
            cr.Description = tbDescription.Text;

            cbCreatures.Invalidate();
            CreatureManager.AnyChanges = true;
        }



    }
}
