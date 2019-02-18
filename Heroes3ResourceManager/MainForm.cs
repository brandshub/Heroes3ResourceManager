using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ComponentAce.Compression.Libs.zlib;
using System.IO.Compression;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace h3magic
{
    public partial class MainForm : Form
    {

        private OpenFileDialog ofd = new OpenFileDialog() { Filter = "Hero Files (*.LOD;*.EXE;*.PAC)|*.LOD;*.EXE;*.PAC" };
        private SaveFileDialog sfd = new SaveFileDialog() { Filter = "Hero Files (*.LOD)|*.LOD" };


        private LodFile lodFile, h3spriteLod, h3bitmapLod;

        private Bitmap bmp;
        private DefFile def;

        private Dictionary<string, Bitmap> icons = new Dictionary<string, Bitmap>();

        [DllImport("shell32.dll")]
        static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern uint ExtractIconEx(string szFileName, int nIconIndex,
           IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        private Timer timer = new Timer();

        private HeroPropertyForm heroPropertyForm = new HeroPropertyForm();

        public MainForm()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            timer.Interval = 143;
            timer.Tick += Timer_Tick;

            icons.Add("H3C", Properties.Resources.H3C);
            icons.Add("TXT", Properties.Resources.TXT);
            icons.Add("PCX", Properties.Resources.PCX);
            icons.Add("ANY", Properties.Resources.ANY);

            hpcHeroProfile.PropertyClicked += HpcHeroProfile_PropertyClicked;
            heroPropertyForm.ItemSelected += HeroPropertyForm_ItemSelected;
            heroPropertyForm.Owner = this;

            LoadOrginalSpecs();
        }


        private void LoadOrginalSpecs()
        {
            SpecialityDefBuilder.LoadOriginalSpecs(Properties.Resources.allspecs);
            SpecialityDefBuilder.LoadDefs(Properties.Resources.UN32, Properties.Resources.UN44);
        }

        private void Measure()
        {

            var spr = Heroes3Master.Master.H3Sprite;
            var bmp = Spell.GetAllSpells(spr);
            bmp = Spell.GetAllSpellsParallel(spr);


            int n = 50;
            long z = 0;


            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                bmp = Spell.GetAllSpells(spr);
                z = bmp.Width;
            }
            sw.Stop();
            float ms1 = sw.ElapsedMs();
            sw.Restart();

            for (int i = 0; i < n; i++)
            {
                bmp = Spell.GetAllSpellsParallel(spr);
                z = bmp.Width;
            }
            sw.Stop();
            float ms2 = sw.ElapsedMs();

            sw.Restart();

            /* for (int i = 0; i < n; i++)
             {
                 bmp = CreatureManager.GetAllCreaturesBitmapParallel2(spr);
                 z = bmp.Width;
             }
             sw.Stop();*/
            float ms3 = sw.ElapsedMs();

            Text = ms1 + " " + ms2 + " " + ms3;

        }


        private void HeroPropertyForm_ItemSelected(int selIndex, int arg1, int arg2, int arg3)
        {
            ProfilePropertyType type = heroPropertyForm.PropertyType;
            var hero = hpcHeroProfile.Hero;

            if (type == ProfilePropertyType.Creature)
            {
                int realIndex = CreatureManager.OnlyActiveCreatures[selIndex].CreatureIndex;
                hero.HasChanged = true;
                switch (heroPropertyForm.CurrentIndex)
                {
                    case 0: hero.Unit1Index = realIndex; break;
                    case 1: hero.Unit2Index = realIndex; break;
                    case 2: hero.Unit3Index = realIndex; break;
                }
                hpcHeroProfile.LoadHero(hpcHeroProfile.HeroIndex, Heroes3Master.Master);
            }
            else if (type == ProfilePropertyType.SecondarySkill)
            {
                int skill = selIndex / 3;
                int level = 1 + selIndex % 3;

                hero.HasChanged = true;
                if (heroPropertyForm.CurrentIndex == 0)
                {
                    hero.FirstSkillIndex = skill;
                    hero.FirstSkillLevel = level;
                }
                else
                {
                    hero.SecondSkillIndex = skill;
                    hero.SecondSkillLevel = level;
                }
                hpcHeroProfile.LoadHero(hpcHeroProfile.HeroIndex, Heroes3Master.Master);

            }
            else if (type == ProfilePropertyType.Spell)
            {
                hero.HasChanged = true;
                hero.SpellBook = 1;
                hero.SpellIndex = selIndex;
                hpcHeroProfile.LoadHero(hpcHeroProfile.HeroIndex, Heroes3Master.Master);
            }
            else
            {
                var specType = Speciality.FromProfileProperty(type);
                if (specType != SpecialityType.Invalid)
                {
                    Speciality.UpdateSpecialityData(specType, hero.Index, selIndex, arg1, arg2, arg3);
                    hero.HasChanged = true;

                    int originalSpecIndex = SpecialityDefBuilder.TryUpdateSpecImage(hero, Heroes3Master.Master.H3Sprite.Un32Def, Heroes3Master.Master.H3Sprite.Un44Def);
                    string originalSpec = HeroesManager.GetSpecDescription(originalSpecIndex);
                    var hs = HeroesManager.AllHeroes[hero.Index];
                    hs.Speciality = originalSpec;
                    tbHeroSpecDesc.Text = originalSpec;

                    HeroesManager.HasChanges = true;

                    hpcHeroProfile.LoadHero(hpcHeroProfile.HeroIndex, Heroes3Master.Master);
                    //TODO
                }

                //hpcHeroProfile
            }

        }

        private void HpcHeroProfile_PropertyClicked(int heroIndex, ProfilePropertyType type, int relativeIndex, int currentValue)
        {
            heroPropertyForm.HeroIndex = heroIndex;
            heroPropertyForm.PropertyType = type;
            heroPropertyForm.CurrentIndex = relativeIndex;
            heroPropertyForm.SelectedValue = currentValue;

            heroPropertyForm.ShowDialog(this);
        }

        public void LoadMaster(string executablPath)
        {
            var master = Heroes3Master.LoadInfo(executablPath);
            lodFile = master.H3Bitmap;
            h3bitmapLod = lodFile;
            h3spriteLod = master.H3Sprite;

            lbFiles.Items.AddRange(lodFile.GetNames());
            lbHeroes.Items.AddRange(HeroesManager.AllHeroes.Select(st => st.Name).ToArray());
            lbHeroClasses.Items.AddRange(HeroClass.AllHeroClasses.Select(st => st.Stats[0]).Where(s => s != "").ToArray());
            cbCastles.Items.AddRange(CreatureManager.Castles);


            if (lodFile["HCTRAITS.TXT"] == null)
            {
                tabsMain.TabPages.Add(tabMain);
            }
            else
            {
                tabsMain.TabPages.Add(tabHeroes);
                tabsMain.TabPages.Add(tabHeroClass);
                tabsMain.TabPages.Add(tabCreatures);
                tabsMain.TabPages.Add(tabSpells);
                tabsMain.TabPages.Add(tabMain);
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            currentFrame++;
            if (currentFrame > trbDefSprites.Maximum)
                currentFrame = 0;

            trbDefSprites.Value = currentFrame;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (lbFiles.SelectedIndex != -1)
            {
                FatRecord rec = lodFile[lbFiles.SelectedItem.ToString()];
                if (rec.Extension == "TXT")
                    rtbMain.Text = Encoding.Default.GetString(rec.GetRawData(lodFile.stream));
                else if (rec.Extension == "PCX")
                {
                    bmp = rec.GetBitmap(lodFile.stream);
                    Invalidate();
                }
                else if (rec.Extension == "DEF")
                {
                    def = rec.GetDefFile(lodFile);
                    bmp = def.GetSprite(0, 0);
                    lbDecomposed.Items.Clear();
                    for (int i = 0; i < def.BlockCount; i++)
                        lbDecomposed.Items.AddRange(def.headers[i].Names);
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {   
            for (int i = 0; i < lbFiles.SelectedItems.Count; i++)
            {
                lodFile[lbFiles.SelectedItems[i].ToString()].SaveToDisk(lodFile.stream);                            
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {


        }

        private void button5_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }


        private Point last = new Point(-1, -1);

        private void listBox1_MouseHover(object sender, EventArgs e)
        {
            PreviewShow();
        }

        private void PreviewShow()
        {
            var sw = Stopwatch.StartNew();
            if (last.X >= 0 && last.Y >= 0)
            {
                int index = lbFiles.IndexFromPoint(last);
                if (index >= 0 && index < lbFiles.Items.Count && lastindex != index)
                {
                    lbDecomposed.Visible = false;
                    string temp = lbFiles.Items[index].ToString();
                    if (selectedFile != temp)
                        currentFrame = 0;

                    selectedFile = temp;


                    FatRecord rec = lodFile[lbFiles.Items[index].ToString()];
                    if (rec.Extension == "PCX")
                    {
                        bmp = rec.GetBitmap(lodFile.stream);
                        if (bmp != null)
                        {
                            //form.Show(bmp);
                            pbResourceView.Visible = true;
                            rtbMain.Visible = false;
                            if (bmp.Width > pbResourceView.Width || bmp.Height > pbResourceView.Height)
                                pbResourceView.SizeMode = PictureBoxSizeMode.Zoom;
                            else
                                pbResourceView.SizeMode = PictureBoxSizeMode.CenterImage;
                            pbResourceView.Image = bmp;
                        }
                    }
                    else if (rec.Extension == "TXT")
                    {
                        byte[] bts = rec.GetRawData(lodFile.stream);
                        if (bts != null)
                        {
                            pbResourceView.Visible = false;
                            rtbMain.Visible = true;
                            rtbMain.Text = Encoding.Default.GetString(bts);
                            //form.Show(Encoding.Default.GetString(bts));

                        }
                    }
                    else if (rec.Extension == "DEF")
                    {
                        lbDecomposed.Visible = true;

                        var def = rec.GetDefFile(lodFile.stream);

                        trbDefSprites.Maximum = def.headers.Sum(s => s.SpritesCount) - 1;
                        trbDefSprites.Value = 0;

                        bmp = def.GetSprite(trbDefSprites.Value);//def.GetSprite(0, trbDefSprites.Value);
                        pbResourceView.Visible = true;
                        rtbMain.Visible = false;
                        if (bmp.Width > pbResourceView.Width || bmp.Height > pbResourceView.Height)
                            pbResourceView.SizeMode = PictureBoxSizeMode.Zoom;
                        else
                            pbResourceView.SizeMode = PictureBoxSizeMode.CenterImage;
                        pbResourceView.Image = bmp;
                    }
                }
                else if (index > -1)
                {
                    var rec = lodFile[lbFiles.Items[index].ToString()];
                    if (rec.Extension == "DEF")
                    {
                        var def = rec.GetDefFile(lodFile.stream);

                        bmp = def.GetSprite(trbDefSprites.Value);//def.GetSprite(0, trbDefSprites.Value);
                        if (bmp.Width > pbResourceView.Width || bmp.Height > pbResourceView.Height)
                            pbResourceView.SizeMode = PictureBoxSizeMode.Zoom;
                        else
                            pbResourceView.SizeMode = PictureBoxSizeMode.CenterImage;
                        pbResourceView.Image = bmp;
                    }
                }
                lastindex = index;
            }
            sw.Stop();
            //Debug.WriteLine("previewshow: " + (sw.ElapsedTicks * 1000.0f / Stopwatch.Frequency));
        }

        private string selectedFile = "";
        private int currentFrame = 0;

        int lastindex = -100;

        private void lbFiles_MouseMove(object sender, MouseEventArgs e)
        {
            last = e.Location;
            PreviewShow();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] buf1 = new byte[4096];
                byte[] buf2 = new byte[4096];

                FileStream test = new FileStream(ofd.FileName, FileMode.Open);
                lodFile.stream.Position = 0;
                int pos = 0;
                while (true)
                {
                    int r1 = test.Read(buf1, 0, 4096);
                    int r2 = lodFile.stream.Read(buf2, 0, 4096);
                    if (r1 == 0 && r2 == 0)
                        break;
                    for (int i = 0; i < buf2.Length; i++)
                    {
                        if (buf1[i] != buf2[i])
                        {
                            Text = pos + i.ToString();
                        }
                    }
                    pos += r1;
                }
                test.Close();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FatRecord fr = lodFile.GetRecord(lbFiles.SelectedItem.ToString());
            if (fr.Extension == "TXT")
            {
                fr.ApplyChanges(Encoding.Default.GetBytes(rtbMain.Text));
            }
        }



        private void button10_Click(object sender, EventArgs e)
        {
            rtbMain.Clear();
        }

        private void m_exit_Click(object sender, EventArgs e)
        {
            if (lodFile != null)
                lodFile.stream.Close();
            Close();

        }

        private void m_openFile_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (lodFile != null)
                    lodFile.stream.Close();

                lbFiles.Items.Clear();
                tabsMain.TabPages.Clear();
                lbHeroes.Items.Clear();
                lbHeroClasses.Items.Clear();
                cbCastles.Items.Clear();
                cbFilter.Items.Clear();

                cbFilter.Items.Add("*");


                string extension = Path.GetExtension(ofd.FileName).ToLower();
                if (extension == ".exe")
                {
                    LoadMaster(ofd.FileName);
                }
                else
                {
                    Heroes3Master.Master = null;

                    var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    lodFile = new LodFile(fs);
                    lodFile.LoadFAT();

                    lbFiles.Items.AddRange(lodFile.GetNames());

                    if (fs.Name.ToLower().Contains("h3bitmap"))
                        h3bitmapLod = lodFile;
                    else if (fs.Name.ToLower().Contains("h3sprite"))
                        h3spriteLod = lodFile;

                    tabsMain.TabPages.Add(tabMain);

                }

                cbFilter.Items.AddRange(lodFile.FilesTable.Select(s => s.Extension.ToUpper()).Distinct().OrderBy(z => z).ToArray<object>());
                tabsMain.Visible = true;
            }
        }

        private void cb_Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lodFile != null)
            {
                string val = cbFilter.SelectedItem.ToString();
                lbFiles.Items.Clear();
                lbFiles.Items.AddRange(lodFile.Filter(val).Select(fat => fat.FileName).ToArray());
            }
        }


        private void m_saveFileAs_Click(object sender, EventArgs e)
        {
            if (lodFile != null)
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (sfd.FileName == lodFile.Name)
                    {
                        MessageBox.Show("Cannot save to original file!");
                        return;
                    }

                    var watch = Stopwatch.StartNew();
                    Heroes3Master.Master.Save();
                    Text = string.Format("Saved in {0:F2} ms", watch.ElapsedMs());
                }
        }




        private void m_saveFile_Click(object sender, EventArgs e)
        {
            if (lodFile != null)
            {
                var watch = Stopwatch.StartNew();
                Heroes3Master.Master.Save();
                Text = string.Format("Saved in {0:F2} ms", watch.ElapsedMs());
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (lodFile != null)
                lodFile.stream.Close();

            base.OnFormClosing(e);
        }

        private void ldBmp_Click(object sender, EventArgs e)
        {
            string filt = ofd.Filter;
            ofd.Filter = "Images |*.bmp;*.jpeg;*.jpg;*.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var rec = lodFile.GetRecord(lbFiles.SelectedItem.ToString());
                var pcx = PcxFile.FromBitmap((Bitmap)Image.FromFile(ofd.FileName));
                rec.ApplyChanges(pcx.GetBytes);
            }
            ofd.Filter = filt;
        }


        private void cb_castles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CreatureManager.Loaded)
            {
                cb_creatures.Items.Clear();
                cb_creatures.Items.AddRange(CreatureManager.OnlyActiveCreatures.Where(c => c.CastleIndex == cbCastles.SelectedIndex).Select(cs => cs.Name).ToArray());
                cb_creatures.Select();
                cb_creatures.SelectedIndex = 0;
            }
        }

        private void cb_creatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CreatureManager.Loaded)
            {
                LoadCreatureInfo(CreatureManager.Get(cbCastles.SelectedIndex, cb_creatures.SelectedIndex));
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

        private void tabsMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lodFile != null)
            {
                if (tabsMain.SelectedIndex == 2)
                {
                    CreatureManager.LoadInfo(lodFile);
                    cbCastles.Items.Clear();
                    cbCastles.Items.AddRange(CreatureManager.Castles);

                }
                else if (tabsMain.SelectedIndex == 1)
                {
                    lbHeroClasses.Items.Clear();
                    lbHeroClasses.Items.AddRange(HeroClass.AllHeroClasses.Select(st => st.Stats[0]).Where(s => s != "").ToArray());
                    pbSkillTree.Image = SecondarySkill.GetSkillTreeForHeroClass(Heroes3Master.Master.H3Sprite);
                }
                else if (tabsMain.SelectedIndex == 0)
                {

                    lbHeroes.Items.Clear();
                    lbHeroes.Items.AddRange(HeroesManager.AllHeroes.Select(st => st.Name).ToArray());

                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var cs = CreatureManager.OnlyActiveCreatures.Where(c => c.CastleIndex == cbCastles.SelectedIndex && c.CreatureCastleRelativeIndex == cb_creatures.SelectedIndex).FirstOrDefault();
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
            CreatureManager.HasChanges = true;

        }


        private void lbHeroes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var hs = HeroesManager.AllHeroes[lbHeroes.SelectedIndex];

            pbPortraitSmall.Image = lodFile[HeroesManager.HeroesOrder[hs.ImageIndex].Replace("HPL", "HPS")].GetBitmap(lodFile.stream);

            if (lbHeroes.SelectedIndex > -1 && Heroes3Master.Master != null)
            {
                hpcHeroProfile.LoadHero(lbHeroes.SelectedIndex, Heroes3Master.Master);
                Text = lbHeroes.SelectedIndex.ToString();
            }

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

        private void button2_Click(object sender, EventArgs e)
        {
            if (lbHeroes.SelectedIndex != -1)
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
                HeroesManager.AllHeroes[lbHeroes.SelectedIndex] = hs;
                HeroesManager.HasChanges = true;
            }

            // Heroes3Master.Master.SaveHeroExeData();

            //  MessageBox.Show("Success!");

        }

        private void завантажитиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbHeroes.SelectedIndex >= 0)
            {
                string filter = ofd.Filter;
                ofd.Filter = "Images (*.bmp,*.jpg,*jpeg,*gif)|*.bmp;*.jpeg;*.jpg;*gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    HeroStats hs = HeroesManager.AllHeroes[lbHeroes.SelectedIndex];
                    /*if (contextMenuStrip1.SourceControl == pictureBox3)
                    {
                        hs.Large = (Bitmap)Bitmap.FromFile(ofd.FileName);
                        pictureBox3.Image = hs.Large;
                    }
                    else
                    {
                        hs.Small = (Bitmap)Bitmap.FromFile(ofd.FileName);
                        pictureBox4.Image = hs.Small;
                    }*/

                }
                ofd.Filter = filter;
            }
        }

        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbHeroes.SelectedIndex >= 0)
            {
                string filter = sfd.Filter;
                sfd.Filter = "Images (*.bmp,*.jpg,*jpeg,*gif)|*.bmp;*.jpeg;*.jpg;*gif";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    /* if (contextMenuStrip1.SourceControl == pictureBox3)
                         pictureBox3.Image.Save(sfd.FileName);
                     else
                         pictureBox4.Image.Save(sfd.FileName);*/
                }
                sfd.Filter = filter;
            }
        }

        private void btnSaveHeroClass_Click(object sender, EventArgs e)
        {
            if (lbHeroClasses.SelectedIndex >= 0)
            {
                var heroClass = HeroClass.AllHeroClasses[lbHeroClasses.SelectedIndex];
                int index = int.Parse(textBox33.Name.Substring(textBox33.Name.Length - 2));

                heroClass.Stats[0] = tabHeroClass.Controls["textBox" + index].Text;
                for (int i = 2; i < heroClass.Stats.Length - 9; i++)
                    heroClass.Stats[i] = tabHeroClass.Controls["textBox" + (index + i)].Text;
            }
        }

        private void lbHeroClasses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbHeroClasses.SelectedIndex >= 0)
            {
                var heroClass = HeroClass.AllHeroClasses[lbHeroClasses.SelectedIndex];
                int index = int.Parse(textBox33.Name.Substring(textBox33.Name.Length - 2));
                tabHeroClass.Controls["textBox" + index].Text = heroClass.Stats[0];
                for (int i = 2; i < heroClass.Stats.Length - 9; i++)
                    tabHeroClass.Controls["textBox" + (index + i)].Text = heroClass.Stats[i];
            }

        }

        private void lbFiles_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                if ((e.State & DrawItemState.Selected) != 0)
                    e.Graphics.FillRectangle(Brushes.SkyBlue, e.Bounds);
                else
                    e.DrawBackground();
                string ext = lbFiles.Items[e.Index].ToString();
                ext = ext.Substring(ext.IndexOf('.') + 1).ToUpper();
                if (icons.ContainsKey(ext))
                    e.Graphics.DrawImage(icons[ext], e.Bounds.X + 1, e.Bounds.Y + 1);
                else
                    e.Graphics.DrawImage(icons["ANY"], e.Bounds.X + 1, e.Bounds.Y + 1);
                e.Graphics.DrawString(lbFiles.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds.X + 18, e.Bounds.Y + 3);

            }

        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lbDecomposed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbDecomposed.SelectedIndex >= 0)
            {
                pbResourceView.Visible = true;
                rtbMain.Visible = false;
                Bitmap bmp = def.GetByName(lbDecomposed.SelectedItem.ToString());
                if (bmp != null)
                {
                    if (bmp.Width > pbResourceView.Width || bmp.Height > pbResourceView.Height)
                        pbResourceView.SizeMode = PictureBoxSizeMode.Zoom;
                    else
                        pbResourceView.SizeMode = PictureBoxSizeMode.CenterImage;
                    pbResourceView.Image = bmp;
                }
            }
        }

        private void trbDefSprites_ValueChanged(object sender, EventArgs e)
        {
            PreviewShow();
            toolTip1.SetToolTip(trbDefSprites, trbDefSprites.Value.ToString());
        }

        private void chbTimerEnabled_CheckedChanged(object sender, EventArgs e)
        {
            timer.Enabled = chbTimerEnabled.Checked;
        }


    }
}
