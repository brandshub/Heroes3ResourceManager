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

            hpcHeroProfile.PropertyClicked += HpcHeroProfile_PropertyDoubleClicked;
            heroPropertyForm.ItemSelected += HeroPropertyForm_ItemSelected;
            heroPropertyForm.Owner = this;
            LoadMaster(@"d:\Games\h3\Heroes3.exe");
            //  Measure();
        }


        private void Measure()
        {

            var spr = Heroes3Master.Master.H3Sprite;
            //var bmp = CreatureManager.GetAllCreaturesBitmap(spr);
            // bmp = CreatureManager.GetAllCreaturesBitmap2(spr);


            int n = 100;
            long z = 0;

            /* var bytes = Heroes3Master.Master.Executable.Data;
             if (HeroesSection.FindHeroOffset2(bytes) != HeroesSection.FindOffset3X(bytes))
             {
                 Text = "false";
                 return;
             }*/

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                /* var offset = HeroesSection.FindHeroOffset2(bytes);
                 z = offset;*/
            }
            sw.Stop();
            float ms1 = sw.ElapsedMs();
            sw.Restart();

            for (int i = 0; i < n; i++)
            {
                /*  var offset = HeroesSection.FindOffset3X(bytes);
                  z = offset;*/
            }
            sw.Stop();
            float ms2 = sw.ElapsedMs();
            Text = ms1 + " " + ms2;

        }


        private void HeroPropertyForm_ItemSelected(int selIndex)
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
            else if (type == ProfilePropertyType.Speciality)
            {
                hero.HasChanged = true;
                hero.SpecIndex = selIndex;                
                hpcHeroProfile.LoadHero(hpcHeroProfile.HeroIndex, Heroes3Master.Master);
            }
        }

        private void HpcHeroProfile_PropertyDoubleClicked(ProfilePropertyType type, int relativeIndex, int currentValue)
        {
            heroPropertyForm.PropertyType = type;
            heroPropertyForm.CurrentIndex = relativeIndex;
            heroPropertyForm.SelectedValue = currentValue;

            heroPropertyForm.ShowDialog(this);
        }

        public async void LoadMaster(string executablPath)
        {
            var sw = Stopwatch.StartNew();

            var master = Heroes3Master.LoadInfo(executablPath);

            lodFile = master.H3Bitmap;
            h3bitmapLod = lodFile;
            h3spriteLod = master.H3Sprite;

            lbFiles.Items.AddRange(lodFile.GetNames());
            lbHeroes.Items.Clear();
            lbHeroes.Items.AddRange(HeroesManager.AllHeroes.Select(st => st.Name).ToArray());

            lbHeroClasses.Items.Clear();
            lbHeroClasses.Items.AddRange(HeroClass.AllHeroClasses.Select(st => st.Stats[0]).Where(s => s != "").ToArray());

            cb_castles.Items.Clear();
            cb_castles.Items.AddRange(CreatureManager.Castles);

            tabsMain.TabPages.Clear();
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

            cb_Filter.Items.Clear();
            cb_Filter.Items.Add("*");
            cb_Filter.Items.AddRange(lodFile.FilesTable.Select(s => s.Extension.ToUpper()).Distinct().OrderBy(z => z).ToArray<object>());

            sw.Stop();
            Text = sw.ElapsedMilliseconds + " ms";

            tabsMain.Visible = true;
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
            progressBar1.Value = 0;
            Application.DoEvents();
            for (int i = 0; i < lbFiles.SelectedItems.Count; i++)
            {

                lodFile[lbFiles.SelectedItems[i].ToString()].SaveToDisk(lodFile.stream);
                progressBar1.Value = 100 * i / lbFiles.SelectedIndices.Count;
                Application.DoEvents();
            }
            progressBar1.Value = 100;
            Application.DoEvents();
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
            Debug.WriteLine("previewshow: " + (sw.ElapsedTicks * 1000.0f / Stopwatch.Frequency));
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

                if (Path.GetExtension(ofd.FileName) == ".exe")
                {
                    LoadMaster(ofd.FileName);
                }
                else
                {
                    var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    lodFile = new LodFile(fs);
                    lbFiles.Items.Clear();
                    lodFile.LoadFAT();

                    lbFiles.Items.AddRange(lodFile.GetNames());

                    if (fs.Name.ToLower().Contains("h3bitmap"))
                        h3bitmapLod = lodFile;
                    else if (fs.Name.ToLower().Contains("h3sprite"))
                        h3spriteLod = lodFile;

                    tabsMain.TabPages.Clear();
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

                    cb_Filter.Items.Clear();
                    cb_Filter.Items.Add("*");
                    cb_Filter.Items.AddRange(lodFile.FilesTable.Select(s => s.Extension.ToUpper()).Distinct().OrderBy(z => z).ToArray<object>());
                }


                tabsMain.Visible = true;
            }
        }

        private void cb_Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lodFile != null)
            {
                string val = cb_Filter.SelectedItem.ToString();
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
                        MessageBox.Show("Неможливо зберігати у файл-джерело!");
                        return;
                    }

                    var watch = Stopwatch.StartNew();

                    lodFile.SaveToDisk(sfd.FileName);

                    double result = watch.ElapsedTicks / (double)Stopwatch.Frequency;
                    Text = "Збережено за " + result.ToString("F3");
                }
        }




        private void m_saveFile_Click(object sender, EventArgs e)
        {
            if (lodFile != null)
            {
                Stopwatch watch = Stopwatch.StartNew();

                lodFile.SaveToDisk();
                double result = watch.ElapsedTicks / (double)Stopwatch.Frequency;
                Text = "Збережено за " + result.ToString("F3");
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
                cb_creatures.Items.AddRange(CreatureManager.OnlyActiveCreatures.Where(c => c.CastleIndex == cb_castles.SelectedIndex).Select(cs => cs.Name).ToArray());
                cb_creatures.Select();
                cb_creatures.SelectedIndex = 0;
            }
        }

        private void cb_creatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CreatureManager.Loaded)
            {
                LoadCreatureInfo(CreatureManager.Get(cb_castles.SelectedIndex, cb_creatures.SelectedIndex));
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
                    cb_castles.Items.Clear();
                    cb_castles.Items.AddRange(CreatureManager.Castles);

                }
                else if (tabsMain.SelectedIndex == 1)
                {
                    lbHeroClasses.Items.Clear();
                    lbHeroClasses.Items.AddRange(HeroClass.AllHeroClasses.Select(st => st.Stats[0]).Where(s => s != "").ToArray());
                    pbSkillTree.Image = SecondarySkill.GetSkillTree(Heroes3Master.Master.H3Sprite);
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
            var cs = CreatureManager.OnlyActiveCreatures.Where(c => c.CastleIndex == cb_castles.SelectedIndex && c.CreatureCastleRelativeIndex == cb_creatures.SelectedIndex).FirstOrDefault();
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

            pictureBox4.Image = lodFile[HeroesManager.HeroesOrder[hs.ImageIndex].Replace("HPL", "HPS")].GetBitmap(lodFile.stream);

            if (lbHeroes.SelectedIndex > -1 && Heroes3Master.Master != null)
            {
                hpcHeroProfile.LoadHero(lbHeroes.SelectedIndex, Heroes3Master.Master);
                Text = lbHeroes.SelectedIndex.ToString();
            }

            textBox24.Text = hs.Name;
            textBox31.Text = hs.Biography;
            textBox32.Text = hs.Speciality;
            textBox25.Text = hs.LowStack1.ToString();
            textBox26.Text = hs.HighStack1.ToString();
            textBox27.Text = hs.LowStack2.ToString();
            textBox28.Text = hs.HighStack2.ToString();
            textBox29.Text = hs.LowStack3.ToString();
            textBox30.Text = hs.HighStack3.ToString();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lbHeroes.SelectedIndex != -1)
            {
                HeroStats hs = new HeroStats();
                hs.Name = textBox24.Text;
                hs.Biography = textBox31.Text;
                hs.Speciality = textBox32.Text;
                hs.LowStack1 = int.Parse(textBox25.Text);
                hs.HighStack1 = int.Parse(textBox26.Text);
                hs.LowStack2 = int.Parse(textBox27.Text);
                hs.HighStack2 = int.Parse(textBox28.Text);
                hs.LowStack3 = int.Parse(textBox29.Text);
                hs.HighStack3 = int.Parse(textBox30.Text);
                HeroesManager.AllHeroes[lbHeroes.SelectedIndex] = hs;
            }

            Heroes3Master.Master.SaveHeroExeData();

        }

        private void pictureBox3_DoubleClick(object sender, EventArgs e)
        {

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
        }

        private void chbTimerEnabled_CheckedChanged(object sender, EventArgs e)
        {
            timer.Enabled = chbTimerEnabled.Checked;
        }
    }
}
