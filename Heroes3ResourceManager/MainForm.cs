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


        private LodFile selectedLodFile;

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

            heroMainDataControl.HeroProfileControl.PropertyClicked += HpcHeroProfile_PropertyClicked;
            heroMainDataControl.HeroesList.SelectedIndexChanged += HeroesList_SelectedIndexChanged;
            heroPropertyForm.ItemSelected += HeroPropertyForm_ItemSelected;
            heroPropertyForm.Owner = this;

            LoadOrginalSpecs();
        }

        private void HeroesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (heroMainDataControl.SelectedHeroIndex >= 0)
            {
                heroClassDataControl.HeroClass = HeroClass.AllHeroClasses[heroMainDataControl.SelectedHeroIndex / 8];
            }
        }


        private void LoadOrginalSpecs()
        {
          //  SpecialityDefBuilder.LoadOriginalSpecs(Properties.Resources.allspecs);
           // SpecialityDefBuilder.LoadDefs(Properties.Resources.UN32, Properties.Resources.UN44);
        }

        private void Measure()
        {

            /*var spr = Heroes3Master.Master.H3Sprite;
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



            float ms3 = sw.ElapsedMs();

            Text = ms1 + " " + ms2 + " " + ms3;*/

        }


        private void HeroPropertyForm_ItemSelected(int selIndex, int arg1, int arg2, int arg3)
        {
            ProfilePropertyType type = heroPropertyForm.PropertyType;
            var hpcHeroProfile = heroMainDataControl.HeroProfileControl;
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

                    var un32 = Heroes3Master.Master.Resolve(Speciality.IMG_FNAME_SMALL);
                    var un44 = Heroes3Master.Master.Resolve(Speciality.IMG_FNAME);

                    var un32Def = un32.GetRecord(Speciality.IMG_FNAME_SMALL).GetDefFile();
                    var un44Def = un44.GetRecord(Speciality.IMG_FNAME).GetDefFile();

                    int originalSpecIndex = SpecialityBuilder.TryUpdateSpecImage(hero, un32Def, un44Def);
                    string originalSpec = SpecialityBuilder.OriginalSpecText(originalSpecIndex);

                    var hs = HeroesManager.AllHeroes[hero.Index];
                    hs.Speciality = originalSpec;
                    heroMainDataControl.Speciality = originalSpec;

                    HeroesManager.AnyChanges = true;
                    hpcHeroProfile.LoadHero(hpcHeroProfile.HeroIndex, Heroes3Master.Master);
                    
                }
                //hpcHeroProfile
            }
        }

        private void HpcHeroProfile_PropertyClicked(int heroIndex, ProfilePropertyType type, int relativeIndex, int currentValue)
        {
            if (type == ProfilePropertyType.HeroClass)
            {
                tabsMain.SelectedTab = tabHeroClass;
                heroClassDataControl.GoToPrimarySkills();
            }
            else
            {
                heroPropertyForm.HeroIndex = heroIndex;
                heroPropertyForm.PropertyType = type;
                heroPropertyForm.CurrentIndex = relativeIndex;
                heroPropertyForm.SelectedValue = currentValue;

                heroPropertyForm.ShowDialog(this);
            }

        }


        public void LoadMaster(string executablPath)
        {
            var master = Heroes3Master.LoadInfo(executablPath);
            selectedLodFile = master.GetByName("h3bitmap.lod");

            SpecialityBuilder.LoadFromMaster(master);
            heroMainDataControl.LoadCastles();            
            heroMainDataControl.LoadHeroes();
            heroClassDataControl.LoadHeroClasses();
            creatureDataControl.LoadCastles();
            spellDataControl.LoadSpells();

            var lodFileNames = master.ResourceFiles.Select(s => s.Name).ToArray();
            cbLodFiles.Items.AddRange(lodFileNames);
            cbLodFiles.SelectedIndex = Array.IndexOf<string>(lodFileNames, selectedLodFile.Name);

            tabsMain.TabPages.Add(tabHeroes);
            tabsMain.TabPages.Add(tabHeroClass);
            tabsMain.TabPages.Add(tabCreatures);
            tabsMain.TabPages.Add(tabSpells);
            tabsMain.TabPages.Add(tabResources);


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
                if (rec.Extension == "TXT" || rec.Extension=="ZBK")
                    rtbMain.Text = Encoding.Default.GetString(rec.GetRawData());
                else if (rec.Extension == "PCX")
                {
                    bmp = rec.GetBitmap();
                    Invalidate();
                }
                else if (rec.Extension == "DEF")
                {
                    def = rec.GetDefFile();
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
                        bmp = rec.GetBitmap();
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
                    else if (rec.Extension == "TXT" || rec.Extension == "ZBK")
                    {
                        trbDefSprites.Maximum = 1;
                        byte[] bts = rec.GetRawData();
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

                        var def = rec.GetDefFile();

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
                        var def = rec.GetDefFile();

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

        private void m_exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ResetData()
        {
            if (Heroes3Master.Master != null)
            {
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
            
            heroMainDataControl.ResetCastles();
            heroClassDataControl.Reset();
            creatureDataControl.Reset();
            spellDataControl.Reset();
            spellDataControl.ResetSchools();
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
                var sw = Stopwatch.StartNew();
                Heroes3Master.Master.SaveToDisk();
                MessageBox.Show("Saved successfully in " + sw.ElapsedMs().ToString("F2") + " ms");
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

        private void tabsMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Heroes3Master.Master != null)
            {
                btnSaveLocalChanges.Visible = true;

                if (tabsMain.SelectedTab == tabCreatures)
                {
                    if (creatureDataControl.SelectedCastle == -1)
                        creatureDataControl.SelectedCastle = 0;
                    Width = 580;
                }
                else if (tabsMain.SelectedTab == tabHeroClass)
                {
                    if (heroClassDataControl.HeroClass == null)
                        heroClassDataControl.HeroClass = HeroClass.AllHeroClasses[0];
                    Width = 580;
                }
                else if (tabsMain.SelectedTab == tabSpells)
                {
                    if (spellDataControl.ItemCount > 0 && spellDataControl.Spell == null)
                        spellDataControl.Spell = Spell.AllSpells.First();

                    Width = 580;
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
                else
                {
                    Width = 955;
                }
            }
            else
            {
                Width = 955;
            }
        }

        private void завантажитиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*if (lbHeroes.SelectedIndex >= 0)
            {
                string filter = ofd.Filter;
                ofd.Filter = "Images (*.bmp,*.jpg,*jpeg,*gif)|*.bmp;*.jpeg;*.jpg;*gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    HeroStats hs = HeroesManager.AllHeroes[lbHeroes.SelectedIndex];
                }
                ofd.Filter = filter;
            }*/
        }

        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
           /* if (lbHeroes.SelectedIndex >= 0)
            {
                string filter = sfd.Filter;
                sfd.Filter = "Images (*.bmp,*.jpg,*jpeg,*gif)|*.bmp;*.jpeg;*.jpg;*gif";
                if (sfd.ShowDialog() == DialogResult.OK)
                {

                }
                sfd.Filter = filter;
            }*/
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

        private void lbHeroes_DrawItem(object sender, DrawItemEventArgs e)
        {

        }

        private void dl(string s)
        {
            Debug.WriteLine(Name + " " + DateTime.Now.ToString("HH:mm:ss") + " " + s);
        }

        private void btnSaveLocalChanges_Click(object sender, EventArgs e)
        {
            if (tabsMain.SelectedTab == tabHeroes)
            {
                heroMainDataControl.SaveLocalChanges();               
            }
            else if (tabsMain.SelectedTab == tabHeroClass)
            {
                heroClassDataControl.SaveLocalChanges();
            }
            else if (tabsMain.SelectedTab == tabCreatures)
            {
                creatureDataControl.SaveLocalChanges();
            }
            else if (tabsMain.SelectedTab == tabSpells)
            {
                spellDataControl.SaveLocalChanges();
            }
            else if (tabsMain.SelectedTab == tabResources)
            {
                //do nothing
            }
        }

        private void cbCastles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
