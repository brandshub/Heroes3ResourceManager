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

        private OpenFileDialog ofd = new OpenFileDialog() { Filter = "Hero Files (*.LOD;*.EXE)|*.LOD;*.EXE" };
        private SaveFileDialog sfd = new SaveFileDialog() { Filter = "Hero Files (*.LOD)|*.LOD" };

        private LodFile lodFile;
        private Bitmap bmp;
        private DefFile def;

        private Dictionary<string, Bitmap> icons = new Dictionary<string, Bitmap>();

        [DllImport("shell32.dll")]
        static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern uint ExtractIconEx(string szFileName, int nIconIndex,
           IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        private Timer timer = new Timer();
        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            cb_Filter.SelectedIndex = 0;
            form = new Preview(this);
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            /*  List<int> ptl = new List<int>();
              ptl.Add(0);
              ptl[0]++;*/
            Point[] ptl = new Point[2];
            ptl[0].X++;
            icons.Add("H3C", Properties.Resources.H3C);
            icons.Add("TXT", Properties.Resources.TXT);
            icons.Add("PCX", Properties.Resources.PCX);
            icons.Add("ANY", Properties.Resources.ANY);
            /*
            string path = @"D:\Новая папка1\AboutHeroes3\data\img\skill\";
            Bitmap canvas = new Bitmap(104 * 4, 44 * 7);
            Graphics cg = Graphics.FromImage(canvas);
            for (int i = 0; i < 4; i++)
            {
                Bitmap bmp = new Bitmap(44, 44 * 7);
                Graphics g = Graphics.FromImage(bmp);
                for (int j = 0; j < 7; j++)
                    g.DrawImage(Bitmap.FromFile(path + "skill" + (i * 7 + j) + "a.BMP"), new Rectangle(0, j * 44, 44, 44));

                cg.DrawImage(bmp, i * 104, 0);
            }

            Stopwatch watch = Stopwatch.StartNew();
            canvas.Save("canvas.bmp"
                ); 
            double result = watch.ElapsedTicks / (double)Stopwatch.Frequency;*/
            /* PCXFile.SaveBitmap24("canvasx.bmp", canvas);
             watch.Reset();
             watch.Start();
             PCXFile.SaveBitmap24("canvas1.bmp", canvas);
             double result1 = watch.ElapsedTicks / (double)Stopwatch.Frequency;
             Text = result + " " + result1;*/

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
                FATRecord rec = lodFile[lbFiles.SelectedItem.ToString()];
                if (rec.Extension == "TXT")
                    rtbMain.Text = Encoding.Default.GetString(rec.GetRawData(lodFile.stream));
                else if (rec.Extension == "PCX")
                {
                    bmp = rec.GetBitmap(lodFile.stream);
                    Invalidate();
                }
                else if (rec.Extension == "DEF")
                {
                    def = rec.GetDEFFile(lodFile.stream);
                    bmp = def.GetSprite(0, 0);
                    listBox4.Items.Clear();
                    for (int i = 0; i < def.BlockCount; i++)
                        listBox4.Items.AddRange(def.headers[i].Names);
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


        Point last = Point.Empty;
        Preview form;
        private void listBox1_MouseHover(object sender, EventArgs e)
        {
            PreviewShow();
        }

        private void PreviewShow()
        {
            int index = lbFiles.IndexFromPoint(last);
            if (index >= 0 && index < lbFiles.Items.Count && lastindex != index)
            {
                listBox4.Visible = false;
                string temp = lbFiles.Items[index].ToString();
                if (selectedFile != temp)
                    currentFrame = 0;

                selectedFile = temp;


                FATRecord rec = lodFile[lbFiles.Items[index].ToString()];
                if (rec.Extension == "PCX")
                {
                    bmp = rec.GetBitmap(lodFile.stream);
                    if (bmp != null)
                    {
                        //form.Show(bmp);
                        pictureBox7.Visible = true;
                        rtbMain.Visible = false;
                        if (bmp.Width > pictureBox7.Width || bmp.Height > pictureBox7.Height)
                            pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                        else
                            pictureBox7.SizeMode = PictureBoxSizeMode.CenterImage;
                        pictureBox7.Image = bmp;
                    }
                }
                else if (rec.Extension == "TXT")
                {
                    byte[] bts = rec.GetRawData(lodFile.stream);
                    if (bts != null)
                    {
                        pictureBox7.Visible = false;
                        rtbMain.Visible = true;
                        rtbMain.Text = Encoding.Default.GetString(bts);
                        //form.Show(Encoding.Default.GetString(bts));

                    }
                }
                else if (rec.Extension == "DEF")
                {
                    listBox4.Visible = true;                   
                    
                    var def = rec.GetDEFFile(lodFile.stream);

                    trbDefSprites.Maximum = def.headers[0].SpritesCount - 1;
                    trbDefSprites.Value = 0;

                    bmp = def.GetSprite(0, trbDefSprites.Value);
                    pictureBox7.Visible = true;
                    rtbMain.Visible = false;
                    if (bmp.Width > pictureBox7.Width || bmp.Height > pictureBox7.Height)
                        pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                    else
                        pictureBox7.SizeMode = PictureBoxSizeMode.CenterImage;
                    pictureBox7.Image = bmp;
                }
            }
            else if (index > -1)
            {
                var rec = lodFile[lbFiles.Items[index].ToString()];
                if (rec.Extension == "DEF")
                {
                    var def = rec.GetDEFFile(lodFile.stream);

                    bmp = def.GetSprite(0, trbDefSprites.Value);
                    if (bmp.Width > pictureBox7.Width || bmp.Height > pictureBox7.Height)
                        pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                    else
                        pictureBox7.SizeMode = PictureBoxSizeMode.CenterImage;
                    pictureBox7.Image = bmp;
                }             
            }
            lastindex = index;
        }

        private string selectedFile = "";
        private int currentFrame = 0;

        int lastindex = -100;

        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            last = e.Location;
            PreviewShow();
            // Text = last.ToString();

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
            FATRecord fr = lodFile.GetRecord(lbFiles.SelectedItem.ToString());
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


                var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (Path.GetExtension(ofd.FileName) == ".exe")
                {
                    var sw = Stopwatch.StartNew();
                    ExeFile.LoadData(ofd.FileName);
                    fs.Close();

                    string h3BitmapLod = Path.Combine(Path.Combine(Path.GetDirectoryName(ofd.FileName), "Data"), "h3bitmap.lod");
                    if (File.Exists(h3BitmapLod))
                    {
                        var lodStream = new FileStream(h3BitmapLod, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        lodFile = new LodFile(lodStream);

                        lodFile.LoadFAT();
                        lbFiles.Items.AddRange(lodFile.GetNames());

                        lbHeroes.Items.Clear();
                        HeroesManager.LoadInfo(lodFile);

                        listBox3.Items.Clear();
                        HeroClassManager.LoadInfo(lodFile);
                        listBox3.Items.AddRange(HeroClassManager.AllClasses.Select(st => st.Stats[0]).Where(s => s != "").ToArray());


                        CreatureManager.LoadInfo(lodFile);
                        cb_castles.Items.Clear();
                        cb_castles.Items.AddRange(CreatureManager.Castles);

                        if (lodFile["HCTRAITS.TXT"] == null)
                        {
                            for (int i = 1; i < tabControl1.TabCount; i++)
                            {
                                tabControl1.TabPages.RemoveAt(i);
                                i--;
                            }
                        }
                        else
                        {
                            if (!tabControl1.TabPages.Contains(h_classTab))
                                tabControl1.TabPages.Add(h_classTab);
                            if (!tabControl1.TabPages.Contains(heroesTab))
                                tabControl1.TabPages.Add(heroesTab);
                            if (!tabControl1.TabPages.Contains(creaturesTab))
                                tabControl1.TabPages.Add(creaturesTab);

                            // if(!tabControl1.TabPages.Contains(h_classTab))
                        }

                        SecondarySkill.LoadInfo(lodFile);

                        HeroExeData.LoadData(ExeFile.Executable.Data);
                    }
                }
                else
                {
                    lodFile = new LodFile(fs);
                    lbFiles.Items.Clear();
                    lodFile.LoadFAT();
                    lbFiles.Items.AddRange(lodFile.GetNames());
                    if (lodFile["HCTRAITS.TXT"] == null)
                    {
                        for (int i = 1; i < tabControl1.TabCount; i++)
                        {
                            tabControl1.TabPages.RemoveAt(i);
                            i--;
                        }
                    }
                    else
                    {
                        if (!tabControl1.TabPages.Contains(h_classTab))
                            tabControl1.TabPages.Add(h_classTab);
                        if (!tabControl1.TabPages.Contains(heroesTab))
                            tabControl1.TabPages.Add(heroesTab);
                        if (!tabControl1.TabPages.Contains(creaturesTab))
                            tabControl1.TabPages.Add(creaturesTab);

                        // if(!tabControl1.TabPages.Contains(h_classTab))
                    }
                }
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
                var pcx = PCXFile.FromBitmap((Bitmap)Image.FromFile(ofd.FileName));
                rec.ApplyChanges(pcx.GetBytes);
            }
            ofd.Filter = filt;
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {

        }

        private void cb_castles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CreatureManager.Loaded)
            {
                cb_creatures.Items.Clear();
                cb_creatures.Items.AddRange(CreatureManager.AllCreatures.Where(c => c.CastleIndex == cb_castles.SelectedIndex).Select(cs => cs.Name).ToArray());
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

        private void LoadCreatureInfo(CreatureStats creatureStats)
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lodFile != null)
            {
                if (tabControl1.SelectedIndex == 3)
                {
                    CreatureManager.LoadInfo(lodFile);
                    cb_castles.Items.Clear();
                    cb_castles.Items.AddRange(CreatureManager.Castles);

                }
                else if (tabControl1.SelectedIndex == 2)
                {
                    if (!HeroesManager.Loaded)
                    {
                        lbHeroes.Items.Clear();
                        HeroesManager.LoadInfo(lodFile);
                        lbHeroes.Items.AddRange(HeroesManager.AllHeroes.Select(st => st.Name).ToArray());
                    }
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    if (!HeroClassManager.Loaded)
                    {
                        listBox3.Items.Clear();
                        HeroClassManager.LoadInfo(lodFile);
                        listBox3.Items.AddRange(HeroClassManager.AllClasses.Select(st => st.Stats[0]).Where(s => s != "").ToArray());

                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var cs = CreatureManager.AllCreatures.Where(c => c.CastleIndex == cb_castles.SelectedIndex && c.CreatureCastleRelativeIndex == cb_creatures.SelectedIndex).FirstOrDefault();
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
            HeroStats hs = HeroesManager.AllHeroes[lbHeroes.SelectedIndex];
            pictureBox3.Image = lodFile[HeroesManager.HeroesOrder[hs.ImageIndex]].GetBitmap(lodFile.stream);
            // label19.Text = pictureBox3.Width + " * " + pictureBox3.Height;
            pictureBox4.Image = lodFile[HeroesManager.HeroesOrder[hs.ImageIndex].Replace("HPL", "HPS")].GetBitmap(lodFile.stream);
            // label20.Text = pictureBox4.Width + " * " + pictureBox4.Height;
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
                    if (contextMenuStrip1.SourceControl == pictureBox3)
                    {
                        hs.Large = (Bitmap)Bitmap.FromFile(ofd.FileName);
                        pictureBox3.Image = hs.Large;
                    }
                    else
                    {
                        hs.Small = (Bitmap)Bitmap.FromFile(ofd.FileName);
                        pictureBox4.Image = hs.Small;
                    }

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
                    if (contextMenuStrip1.SourceControl == pictureBox3)
                        pictureBox3.Image.Save(sfd.FileName);
                    else
                        pictureBox4.Image.Save(sfd.FileName);
                }
                sfd.Filter = filter;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string path = @"D:\Новая папка1\AboutHeroes3\data\img\skill\";
            Bitmap canvas = new Bitmap(104 * 4, 44 * 7);
            Graphics cg = Graphics.FromImage(canvas);
            for (int i = 0; i < 4; i++)
            {
                Bitmap bmp = new Bitmap(44, 44 * 7);
                Graphics g = Graphics.FromImage(bmp);
                for (int j = 0; j < 7; j++)
                    g.DrawImage(Bitmap.FromFile(path + "skill" + (i * 7 + j) + "a.BMP"), new Rectangle(0, j * 44, 44, 44));

                cg.DrawImage(bmp, i * 104, 0);
            }
            canvas.Save("canvas.bmp");

            //  HeroClassManager.stats[listBox3.SelectedIndex].Save(h_classTab, textBox33);
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            HeroClassManager.AllClasses[listBox3.SelectedIndex].Load(h_classTab, textBox33);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
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

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex >= 0)
            {
                pictureBox7.Visible = true;
                rtbMain.Visible = false;
                bmp = def.GetByName(listBox4.SelectedItem.ToString());
                if (bmp != null)
                {
                    if (bmp.Width > pictureBox7.Width || bmp.Height > pictureBox7.Height)
                        pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                    else
                        pictureBox7.SizeMode = PictureBoxSizeMode.CenterImage;
                    pictureBox7.Image = bmp;
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
