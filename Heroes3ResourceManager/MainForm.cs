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


        private LodFile selectedLodFile, h3spriteLod, h3bitmapLod;

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
            Text = "Heroes 3 Resource Manager: v" + Application.ProductVersion.ToString();

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
            bmp = Spell.GetAllSpells(spr);


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
                bmp = Spell.GetAllSpells(spr);
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


        private void PositionSkillProbabilityBoxes()
        {
            int stIndex = 47;
            int itemWidth = pbSkillTree.Width / 7;
            int itemHeight = pbSkillTree.Height / 4;


            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 7; j++)
                {
                    int index = i * 7 + j + stIndex;
                    var tb = tabHeroClass.Controls.Find("textBox" + index, true).FirstOrDefault() as TextBox;
                    if (tb != null)
                    {
                        tb.Width = itemWidth;
                        tb.Location = new Point(pbSkillTree.Left + itemWidth * j, pbSkillTree.Top + (itemHeight) * (i + 1) - 19);
                    }
                }
        }
        public void LoadMaster(string executablPath)
        {
            var master = Heroes3Master.LoadInfo(executablPath);
            selectedLodFile = master.H3Bitmap;
            h3bitmapLod = selectedLodFile;
            h3spriteLod = master.H3Sprite;

            lbHeroes.Items.AddRange(HeroesManager.AllHeroes.Select(st => st.Name).ToArray());

            lbHeroClasses.Items.AddRange(HeroClass.AllHeroClasses.Select(st => st.Stats[0]).Where(s => s != "").ToArray());
            var bmp = SecondarySkill.GetSkillTreeForHeroClass(Heroes3Master.Master.H3Sprite);
            pbSkillTree.Width = bmp.Width * 4 / 5;
            pbSkillTree.Height = bmp.Height * 4 / 5;
            pbSkillTree.Image = bmp;

            cbCastles.Items.AddRange(Town.TownNamesWithNeutral);

            spellDataControl.LoadSpells();

            var lodFileNames = master.ResourceFiles.Select(s => s.Name).ToArray();
            cbLodFiles.Items.AddRange(lodFileNames);
            cbLodFiles.SelectedIndex = Array.IndexOf<string>(lodFileNames, selectedLodFile.Name);

            tabsMain.TabPages.Add(tabHeroes);
            tabsMain.TabPages.Add(tabHeroClass);
            tabsMain.TabPages.Add(tabCreatures);
            tabsMain.TabPages.Add(tabSpells);
            tabsMain.TabPages.Add(tabResources);

            PositionSkillProbabilityBoxes();
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
                FatRecord rec = selectedLodFile[lbFiles.SelectedItem.ToString()];
                if (rec.Extension == "TXT")
                    rtbMain.Text = Encoding.Default.GetString(rec.GetRawData(selectedLodFile.stream));
                else if (rec.Extension == "PCX")
                {
                    bmp = rec.GetBitmap(selectedLodFile.stream);
                    Invalidate();
                }
                else if (rec.Extension == "DEF")
                {
                    def = rec.GetDefFile(selectedLodFile);
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
                selectedLodFile[lbFiles.SelectedItems[i].ToString()].SaveToDisk(selectedLodFile.stream);
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
            //  PreviewShow();
        }

        private void PreviewShow()
        {
            var sw = Stopwatch.StartNew();
            int index = -1;
            if (lbFiles.SelectedIndex >= 0)
                index = lbFiles.SelectedIndex;
            else if (chbHover.Checked)
                index = lbFiles.IndexFromPoint(last);



            if (index >= 0)
            {

                if (index >= 0 && index < lbFiles.Items.Count && lastindex != index)
                {
                    lbDecomposed.Visible = false;
                    string temp = lbFiles.Items[index].ToString();
                    if (selectedFile != temp)
                        currentFrame = 0;

                    selectedFile = temp;


                    var rec = selectedLodFile[lbFiles.Items[index].ToString()];
                    if (rec.Extension == "PCX")
                    {
                        trbDefSprites.Maximum = 1;
                        bmp = rec.GetBitmap(selectedLodFile.stream);
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
                        trbDefSprites.Maximum = 1;
                        byte[] bts = rec.GetRawData(selectedLodFile.stream);
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

                        var def = rec.GetDefFile(selectedLodFile.stream);

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
                    var rec = selectedLodFile[lbFiles.Items[index].ToString()];
                    if (rec.Extension == "DEF")
                    {
                        var def = rec.GetDefFile(selectedLodFile.stream);

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


            trbDefSprites.Visible = trbDefSprites.Maximum > 1;
            sw.Stop();
            //Debug.WriteLine("previewshow: " + (sw.ElapsedTicks * 1000.0f / Stopwatch.Frequency));
        }

        private string selectedFile = "";
        private int currentFrame = 0;

        int lastindex = -100;

        private void lbFiles_MouseMove(object sender, MouseEventArgs e)
        {
            if (chbHover.Checked && lbFiles.SelectedIndex == -1)
            {
                last = e.Location;
                PreviewShow();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] buf1 = new byte[4096];
                byte[] buf2 = new byte[4096];

                FileStream test = new FileStream(ofd.FileName, FileMode.Open);
                selectedLodFile.stream.Position = 0;
                int pos = 0;
                while (true)
                {
                    int r1 = test.Read(buf1, 0, 4096);
                    int r2 = selectedLodFile.stream.Read(buf2, 0, 4096);
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

        private void m_exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ResetData()
        {
            if (Heroes3Master.Master != null)
            {
                h3bitmapLod = null;
                h3spriteLod = null;

                Heroes3Master.Master.Dispose();
                Heroes3Master.Master = null;
            }

            if (selectedLodFile != null)
            {
                selectedLodFile.stream.Close();
                selectedLodFile = null;
            }

            lbFiles.Items.Clear();
            tabsMain.TabPages.Clear();
            lbHeroes.Items.Clear();
            lbHeroClasses.Items.Clear();
            cbCastles.Items.Clear();
            spellDataControl.Reset();
            cbLodFiles.Items.Clear();

        }

        private void m_openFile_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ResetData();

                string extension = Path.GetExtension(ofd.FileName).ToLower();
                if (extension == ".exe")
                {
                    LoadMaster(ofd.FileName);
                    m_saveFile.Text = "Save All Data";
                    m_saveFile.Visible = true;
                    m_saveFileAs.Visible = true;
                    btnSaveLocalChanges.Visible = true;
                }
                else
                {
                    m_saveFile.Text = "Save Resource File";
                    m_saveFile.Visible = true;
                    m_saveFileAs.Visible = true;

                    var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    selectedLodFile = new LodFile(null, fs);
                    selectedLodFile.LoadFAT();

                    lbFiles.Items.AddRange(selectedLodFile.GetNames());

                    if (fs.Name.ToLower().Contains("h3bitmap"))
                        h3bitmapLod = selectedLodFile;
                    else if (fs.Name.ToLower().Contains("h3sprite"))
                        h3spriteLod = selectedLodFile;

                    tabsMain.TabPages.Add(tabResources);

                    cbLodFiles.Items.Add(Path.GetFileName(ofd.FileName));
                    cbLodFiles.SelectedIndex = 0;
                    ReloadAndFilterData();
                }
                tabsMain.Visible = true;
            }
        }

        private void cb_Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedLodFile != null)
            {
                FilterData();
            }
        }


        private void m_saveFileAs_Click(object sender, EventArgs e)
        {
            if (selectedLodFile != null)
            {
                if (!selectedLodFile.HasChanges)
                {
                    MessageBox.Show("Nothing to save - no changes ocurred");
                }
                else
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        if (string.Compare(Path.GetFileName(sfd.FileName), selectedLodFile.Name, true) == 0)
                        {
                            MessageBox.Show("Cannot save to original file!");
                            return;
                        }
                        MessageBox.Show(selectedLodFile.SaveToDisk(sfd.FileName) ? (selectedLodFile.Name + " was successfully saved to " + sfd.FileName) : "Nothing was saved to disk");
                    }
                }
            }
        }

        private void m_saveFile_Click(object sender, EventArgs e)
        {
            if (Heroes3Master.Master != null)
            {
                Heroes3Master.Master.Save();
                MessageBox.Show("Saved successfully");
            }
            else if (selectedLodFile != null)
            {
                string lodNewPath = selectedLodFile.Path + "new" + DateTime.Now.ToString("yyyyMMdd_hhmmss");
                if (selectedLodFile.SaveToDisk(lodNewPath))
                {
                    selectedLodFile.stream.Close();
                    File.Move(selectedLodFile.Path, selectedLodFile.Path + ".bak." + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
                    File.Move(lodNewPath, selectedLodFile.Path);
                    selectedLodFile.stream = new FileStream(selectedLodFile.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    MessageBox.Show("Saved successfully to " + selectedLodFile.Name);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (selectedLodFile != null)
                selectedLodFile.stream.Close();

            if (Heroes3Master.Master != null)
            {
                Heroes3Master.Master.Dispose();
                Heroes3Master.Master = null;
            }

            base.OnFormClosing(e);
        }

        private void ldBmp_Click(object sender, EventArgs e)
        {
            var rec = selectedLodFile.GetRecord(lbFiles.SelectedItem.ToString());
            if (rec.Extension == "TXT")
            {
                string filt = ofd.Filter;
                ofd.Filter = "Text |*.txt";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var bytes = File.ReadAllBytes(ofd.FileName);
                    rec.ApplyChanges(bytes);
                }
                ofd.Filter = filt;
            }
            else if (rec.Extension == "PCX")
            {

                string filt = ofd.Filter;
                ofd.Filter = "Images |*.bmp;*.jpeg;*.jpg;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var pcx = PcxFile.FromBitmap((Bitmap)Image.FromFile(ofd.FileName));
                    rec.ApplyChanges(pcx.GetBytes);
                }
                ofd.Filter = filt;
            }
        }

        int selectedCastle = 0;
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

                //Debug.WriteLine("cbCastles_SelectedIndexChanged");
            }
        }

        private CreatureAnimationLoop creatureAnimation;
        private void cbCreatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CreatureManager.Loaded)
            {
                var creature = CreatureManager.Get(cbCastles.SelectedIndex, cbCreatures.SelectedIndex);
                LoadCreatureInfo(creature);

                if (h3spriteLod != null)
                {
                    if (creatureAnimation != null)
                    {
                        pbCreature.Image = null;
                        creatureAnimation.Dispose();
                    }


                    string allCreatures = Properties.Resources.creatures;
                    string defName = allCreatures.Split(new[] { "\r\n" }, StringSplitOptions.None)[creature.CreatureIndex].Split(';')[2] + ".def";
                    var def = h3spriteLod[defName].GetDefFile(h3spriteLod);
                    if (def != null)
                    {
                        creatureAnimation = new CreatureAnimationLoop(creature.CreatureIndex, def);
                        creatureAnimation.TimerTick += creatureAnimation_TimerTick;
                        creatureAnimation.Enabled = true;
                    }
                }
            }
        }

        private void creatureAnimation_TimerTick(object sender, EventArgs e)
        {
            if (creatureAnimation != null)
            {
                pbCreature.Image = creatureAnimation.GetFrame2(Heroes3Master.Master);
                //pbCreature.Image = creatureAnimation.GetFrame(Heroes3Master.Master);
                //  pbCreature.Invalidate();
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
            if (Heroes3Master.Master != null)
            {
                btnSaveLocalChanges.Visible = true;

                if (tabsMain.SelectedTab == tabCreatures)
                {
                    if (cbCastles.SelectedIndex == -1 && cbCastles.Items.Count > 0)
                        cbCastles.SelectedIndex = 0;
                    Width = 600;
                }
                else if (tabsMain.SelectedTab == tabHeroClass)
                {
                    if (lbHeroClasses.SelectedIndex == -1 && lbHeroClasses.Items.Count > 0)
                        lbHeroClasses.SelectedIndex = 0;
                    Width = 600;
                }
                else if (tabsMain.SelectedTab == tabSpells)
                {
                    if (spellDataControl.ItemCount > 0 && spellDataControl.Spell == null)
                        spellDataControl.Spell = Spell.AllSpells.First();

                    Width = 600;
                }
                else if (tabsMain.SelectedTab == tabResources)
                {
                    btnSaveLocalChanges.Visible = false;
                    Width = 955;
                }
                else if (tabsMain.SelectedTab == tabHeroes)
                {
                    Width = 955;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
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
            CreatureManager.HasChanges = true;

        }


        private void lbHeroes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var hs = HeroesManager.AllHeroes[lbHeroes.SelectedIndex];



            if (lbHeroes.SelectedIndex > -1 && Heroes3Master.Master != null)
            {
                //pbPortraitSmall.Image = Heroes3Master.Master.H3Bitmap[HeroesManager.HeroesOrder[hs.ImageIndex].Replace("HPL", "HPS")].GetBitmap(selectedLodFile.stream);
                hpcHeroProfile.LoadHero(lbHeroes.SelectedIndex, Heroes3Master.Master);
                //Text = lbHeroes.SelectedIndex.ToString();
                var hd = HeroExeData.Data[lbHeroes.SelectedIndex];
                lbHeroClasses.SelectedIndex = hd.ClassIndex;
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

        private void btnHeroSave_Click(object sender, EventArgs e)
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

                HeroClass.HasChanges = true;
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


        private void lbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFiles.SelectedIndex >= 0)
            {
                PreviewShow();
                btnMergeChanges.Visible = lbFiles.SelectedIndices.Count <= 1;
            }
            else
            {
                btnMergeChanges.Visible = false;
            }
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

        //TODO
        private void lbFiles_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int index = lbFiles.IndexFromPoint(e.Location);

                if (index == lbFiles.SelectedIndex)
                {
                    last = e.Location;
                    lbFiles.SelectedIndex = -1;
                }
            }
        }

        private void FilterData()
        {
            string selValue = (lbFiles.SelectedItem ?? "").ToString();
            string selFilter = (cbFilter.SelectedItem ?? "*").ToString();


            lbFiles.SelectedIndex = -1;
            lbFiles.Items.Clear();

            var items = selectedLodFile.Filter(selFilter).Select(fat => fat.FileName).ToArray();
            int newFileIndex = Array.IndexOf<string>(items, selValue);

            lbFiles.Items.AddRange(items);
            lbFiles.SelectedIndex = newFileIndex;
        }


        private void ReloadAndFilterData()
        {
            string selFilter = (cbFilter.SelectedItem ?? "*").ToString();

            cbFilter.SelectedIndex = -1;
            cbFilter.Items.Clear();
            cbFilter.Items.Add("*");

            var newExtensions = selectedLodFile.FilesTable.Select(s => s.Extension.ToUpper()).Distinct().OrderBy(z => z).ToArray();
            int newExtIndex = Array.IndexOf<string>(newExtensions, selFilter);
            cbFilter.Items.AddRange(newExtensions);
            cbFilter.SelectedIndex = newExtIndex < 0 ? 0 : 1 + newExtIndex;
            //FilterData();

            if (Heroes3Master.Master != null)
            {
                Text = "Master: " + Heroes3Master.Master.Executable.Path + " | Selected LOD: " + selectedLodFile.Name;
            }
            else
            {
                Text = "Selected LOD: " + selectedLodFile.Path;
            }
        }


        private void cbLodFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Heroes3Master.Master != null)
            {
                selectedLodFile = Heroes3Master.Master.ResourceFiles.FirstOrDefault(f => f.Name == cbLodFiles.SelectedItem.ToString());
                ReloadAndFilterData();
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
                e.Graphics.DrawString(lbHeroClasses.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds.X + 42, e.Bounds.Y + 4);

                if ((e.State & DrawItemState.Selected) != 0)
                {
                    //    e.DrawFocusRectangle();                   
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot }, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 1);
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

        private bool cacheOther = false;
        private Creature[] otherCreatures = null;

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
                    e.Graphics.DrawImage(CreatureManager.GetSmallImage(Heroes3Master.Master.H3Sprite, creature.CreatureIndex), e.Bounds.X, e.Bounds.Y);
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
                            g.DrawImage(CreatureManager.GetSmallImage(Heroes3Master.Master.H3Sprite, creature.CreatureIndex), Point.Empty);
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


        private bool cacheHeroes = true;

        private void lbHeroes_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (Heroes3Master.Master != null && e.Index >= 0)
            {
                // dl("Draw " + e.State + " " + e.Index + " " + lbHeroes.SelectedIndex);

                if (e.State == (DrawItemState.Selected | DrawItemState.Focus | DrawItemState.NoAccelerator | DrawItemState.NoFocusRect))
                    return;


                int castleIndex = e.Index / 16;
                var clr = Town.AllColors[castleIndex];

                if (!cacheHeroes)
                {
                    e.Graphics.FillRectangle(new SolidBrush(clr), e.Bounds);
                    e.Graphics.DrawString(HeroesManager.AllHeroes[e.Index].Name, e.Font, Brushes.Black, e.Bounds.X + 42, e.Bounds.Y + 4);

                    var img = new Bitmap(Heroes3Master.Master.H3Bitmap[HeroesManager.HeroesOrder[e.Index].Replace("HPL", "HPS")].GetBitmap(Heroes3Master.Master.H3Bitmap.stream), 36, 24);
                    e.Graphics.DrawImage(img, e.Bounds.X, e.Bounds.Y);

                }
                else
                {
                    if (BitmapCache.DrawItemHeroesListBox == null)
                    {
                        BitmapCache.DrawItemHeroesListBox = new Bitmap[HeroesManager.HeroesOrder.Length];
                    }

                    Bitmap cached;
                    if (BitmapCache.DrawItemHeroesListBox[e.Index] == null)
                    {
                        cached = new Bitmap(e.Bounds.Width, e.Bounds.Height);
                        using (var g = Graphics.FromImage(cached))
                        {
                            g.FillRectangle(new SolidBrush(clr), new Rectangle(Point.Empty, e.Bounds.Size));
                            g.DrawString(HeroesManager.AllHeroes[e.Index].Name, e.Font, Brushes.Black, 42, 4);

                            var img = new Bitmap(Heroes3Master.Master.H3Bitmap[HeroesManager.HeroesOrder[e.Index].Replace("HPL", "HPS")].GetBitmap(Heroes3Master.Master.H3Bitmap.stream), 36, 24);
                            g.DrawImage(img, Point.Empty);
                        }
                        BitmapCache.DrawItemHeroesListBox[e.Index] = cached;
                    }
                    else
                    {
                        cached = BitmapCache.DrawItemHeroesListBox[e.Index];
                    }

                    e.Graphics.DrawImage(cached, e.Bounds.Location);
                }

                if ((e.State & DrawItemState.Selected) != 0)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot }, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 1);
                }

            }
        }

        private void dl(string s)
        {
            Debug.WriteLine(Name + " " + DateTime.Now.ToString("HH:mm:ss") + " " + s);
        }

        private void btnSaveLocalChanges_Click(object sender, EventArgs e)
        {
            if (tabsMain.SelectedTab == tabHeroes)
            {
                if (lbHeroes.SelectedIndex >= 0)
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
            }
            else if (tabsMain.SelectedTab == tabHeroClass)
            {
                if (lbHeroClasses.SelectedIndex >= 0)
                {
                    var heroClass = HeroClass.AllHeroClasses[lbHeroClasses.SelectedIndex];
                    int index = int.Parse(textBox33.Name.Substring(textBox33.Name.Length - 2));

                    heroClass.Stats[0] = tabHeroClass.Controls["textBox" + index].Text;
                    for (int i = 2; i < heroClass.Stats.Length - 9; i++)
                        heroClass.Stats[i] = tabHeroClass.Controls["textBox" + (index + i)].Text;

                    HeroClass.HasChanges = true;
                }
            }
            else if (tabsMain.SelectedTab == tabCreatures)
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
                CreatureManager.HasChanges = true;
            }
            else if (tabsMain.SelectedTab == tabSpells)
            {
                spellDataControl.SaveData();
            }
            else if (tabsMain.SelectedTab == tabResources)
            {
                //do nothing
            }
        }



    }
}
