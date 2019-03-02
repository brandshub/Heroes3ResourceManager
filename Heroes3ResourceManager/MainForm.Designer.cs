namespace h3magic
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.button4 = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.rtbMain = new System.Windows.Forms.RichTextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_openFile = new System.Windows.Forms.ToolStripMenuItem();
            this.m_saveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.m_saveFileAs = new System.Windows.Forms.ToolStripMenuItem();
            this.m_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.cbFilter = new System.Windows.Forms.ComboBox();
            this.btnMergeChanges = new System.Windows.Forms.Button();
            this.tabsMain = new System.Windows.Forms.TabControl();
            this.tabHeroes = new System.Windows.Forms.TabPage();
            this.tabHeroClass = new System.Windows.Forms.TabPage();
            this.tabCreatures = new System.Windows.Forms.TabPage();
            this.tabSpells = new System.Windows.Forms.TabPage();
            this.tabResources = new System.Windows.Forms.TabPage();
            this.cbLodFiles = new System.Windows.Forms.ComboBox();
            this.chbHover = new System.Windows.Forms.CheckBox();
            this.chbTimerEnabled = new System.Windows.Forms.CheckBox();
            this.trbDefSprites = new System.Windows.Forms.TrackBar();
            this.lbDecomposed = new System.Windows.Forms.ListBox();
            this.pbResourceView = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.завантажитиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.зберегтиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveLocalChanges = new System.Windows.Forms.Button();
            this.heroMainDataControl = new h3magic.HeroMainDataControl();
            this.heroClassDataControl = new h3magic.HeroClassDataControl();
            this.creatureDataControl = new h3magic.CreatureDataControl();
            this.spellDataControl = new h3magic.SpellDataControl();
            this.specialityBuilder1 = new h3magic.ImageGridControl();
            this.menuStrip1.SuspendLayout();
            this.tabsMain.SuspendLayout();
            this.tabHeroes.SuspendLayout();
            this.tabHeroClass.SuspendLayout();
            this.tabCreatures.SuspendLayout();
            this.tabSpells.SuspendLayout();
            this.tabResources.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbDefSprites)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbResourceView)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbFiles
            // 
            this.lbFiles.ColumnWidth = 118;
            this.lbFiles.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.ItemHeight = 18;
            this.lbFiles.Location = new System.Drawing.Point(2, 0);
            this.lbFiles.MultiColumn = true;
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFiles.Size = new System.Drawing.Size(574, 472);
            this.lbFiles.TabIndex = 4;
            this.lbFiles.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbFiles_DrawItem);
            this.lbFiles.SelectedIndexChanged += new System.EventHandler(this.lbFiles_SelectedIndexChanged);
            this.lbFiles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbFiles_MouseDown);
            this.lbFiles.MouseHover += new System.EventHandler(this.listBox1_MouseHover);
            this.lbFiles.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbFiles_MouseMove);
            this.lbFiles.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbFiles_MouseDown);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(420, 483);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(77, 23);
            this.button4.TabIndex = 5;
            this.button4.Text = "Get Item";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // rtbMain
            // 
            this.rtbMain.Location = new System.Drawing.Point(605, 0);
            this.rtbMain.MaxLength = 327670;
            this.rtbMain.Name = "rtbMain";
            this.rtbMain.ReadOnly = true;
            this.rtbMain.Size = new System.Drawing.Size(303, 282);
            this.rtbMain.TabIndex = 6;
            this.rtbMain.Text = "";
            this.rtbMain.WordWrap = false;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(503, 483);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(77, 22);
            this.button5.TabIndex = 7;
            this.button5.Text = "Save Items";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(939, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_openFile,
            this.m_saveFile,
            this.m_saveFileAs,
            this.m_exit});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.файлToolStripMenuItem.Text = "File";
            // 
            // m_openFile
            // 
            this.m_openFile.Name = "m_openFile";
            this.m_openFile.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.m_openFile.Size = new System.Drawing.Size(205, 22);
            this.m_openFile.Text = "Load";
            this.m_openFile.Click += new System.EventHandler(this.m_openFile_Click);
            // 
            // m_saveFile
            // 
            this.m_saveFile.Name = "m_saveFile";
            this.m_saveFile.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.m_saveFile.Size = new System.Drawing.Size(205, 22);
            this.m_saveFile.Text = "Save";
            this.m_saveFile.Visible = false;
            this.m_saveFile.Click += new System.EventHandler(this.m_saveFile_Click);
            // 
            // m_saveFileAs
            // 
            this.m_saveFileAs.Name = "m_saveFileAs";
            this.m_saveFileAs.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.m_saveFileAs.Size = new System.Drawing.Size(205, 22);
            this.m_saveFileAs.Text = "Save Resource File As";
            this.m_saveFileAs.Visible = false;
            this.m_saveFileAs.Click += new System.EventHandler(this.m_saveFileAs_Click);
            // 
            // m_exit
            // 
            this.m_exit.Name = "m_exit";
            this.m_exit.Size = new System.Drawing.Size(205, 22);
            this.m_exit.Text = "Exit";
            this.m_exit.Click += new System.EventHandler(this.m_exit_Click);
            // 
            // cbFilter
            // 
            this.cbFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilter.FormattingEnabled = true;
            this.cbFilter.Location = new System.Drawing.Point(175, 486);
            this.cbFilter.Name = "cbFilter";
            this.cbFilter.Size = new System.Drawing.Size(103, 21);
            this.cbFilter.TabIndex = 16;
            this.cbFilter.SelectedIndexChanged += new System.EventHandler(this.cb_Filter_SelectedIndexChanged);
            // 
            // btnMergeChanges
            // 
            this.btnMergeChanges.Location = new System.Drawing.Point(284, 484);
            this.btnMergeChanges.Name = "btnMergeChanges";
            this.btnMergeChanges.Size = new System.Drawing.Size(130, 23);
            this.btnMergeChanges.TabIndex = 17;
            this.btnMergeChanges.Text = "Load From File";
            this.btnMergeChanges.UseVisualStyleBackColor = true;
            this.btnMergeChanges.Click += new System.EventHandler(this.ldBmp_Click);
            // 
            // tabsMain
            // 
            this.tabsMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabsMain.Controls.Add(this.tabHeroes);
            this.tabsMain.Controls.Add(this.tabHeroClass);
            this.tabsMain.Controls.Add(this.tabCreatures);
            this.tabsMain.Controls.Add(this.tabSpells);
            this.tabsMain.Controls.Add(this.tabResources);
            this.tabsMain.Location = new System.Drawing.Point(12, 28);
            this.tabsMain.Name = "tabsMain";
            this.tabsMain.SelectedIndex = 0;
            this.tabsMain.Size = new System.Drawing.Size(916, 541);
            this.tabsMain.TabIndex = 18;
            this.tabsMain.Visible = false;
            this.tabsMain.SelectedIndexChanged += new System.EventHandler(this.tabsMain_SelectedIndexChanged);
            // 
            // tabHeroes
            // 
            this.tabHeroes.Controls.Add(this.heroMainDataControl);
            this.tabHeroes.Location = new System.Drawing.Point(4, 25);
            this.tabHeroes.Name = "tabHeroes";
            this.tabHeroes.Padding = new System.Windows.Forms.Padding(3);
            this.tabHeroes.Size = new System.Drawing.Size(908, 512);
            this.tabHeroes.TabIndex = 4;
            this.tabHeroes.Text = "Heroes";
            this.tabHeroes.UseVisualStyleBackColor = true;
            // 
            // tabHeroClass
            // 
            this.tabHeroClass.Controls.Add(this.heroClassDataControl);
            this.tabHeroClass.Location = new System.Drawing.Point(4, 25);
            this.tabHeroClass.Name = "tabHeroClass";
            this.tabHeroClass.Padding = new System.Windows.Forms.Padding(3);
            this.tabHeroClass.Size = new System.Drawing.Size(908, 512);
            this.tabHeroClass.TabIndex = 3;
            this.tabHeroClass.Text = "Hero Classes";
            this.tabHeroClass.UseVisualStyleBackColor = true;
            // 
            // tabCreatures
            // 
            this.tabCreatures.Controls.Add(this.creatureDataControl);
            this.tabCreatures.Location = new System.Drawing.Point(4, 25);
            this.tabCreatures.Name = "tabCreatures";
            this.tabCreatures.Padding = new System.Windows.Forms.Padding(3);
            this.tabCreatures.Size = new System.Drawing.Size(908, 512);
            this.tabCreatures.TabIndex = 1;
            this.tabCreatures.Text = "Creatures";
            this.tabCreatures.UseVisualStyleBackColor = true;
            // 
            // tabSpells
            // 
            this.tabSpells.Controls.Add(this.spellDataControl);
            this.tabSpells.Location = new System.Drawing.Point(4, 25);
            this.tabSpells.Name = "tabSpells";
            this.tabSpells.Size = new System.Drawing.Size(908, 512);
            this.tabSpells.TabIndex = 5;
            this.tabSpells.Text = "Spells";
            this.tabSpells.UseVisualStyleBackColor = true;
            // 
            // tabResources
            // 
            this.tabResources.Controls.Add(this.cbLodFiles);
            this.tabResources.Controls.Add(this.chbHover);
            this.tabResources.Controls.Add(this.chbTimerEnabled);
            this.tabResources.Controls.Add(this.trbDefSprites);
            this.tabResources.Controls.Add(this.lbDecomposed);
            this.tabResources.Controls.Add(this.rtbMain);
            this.tabResources.Controls.Add(this.btnMergeChanges);
            this.tabResources.Controls.Add(this.cbFilter);
            this.tabResources.Controls.Add(this.button5);
            this.tabResources.Controls.Add(this.pbResourceView);
            this.tabResources.Controls.Add(this.lbFiles);
            this.tabResources.Controls.Add(this.button4);
            this.tabResources.Location = new System.Drawing.Point(4, 25);
            this.tabResources.Name = "tabResources";
            this.tabResources.Padding = new System.Windows.Forms.Padding(3);
            this.tabResources.Size = new System.Drawing.Size(908, 512);
            this.tabResources.TabIndex = 0;
            this.tabResources.Text = "Resources";
            this.tabResources.UseVisualStyleBackColor = true;
            // 
            // cbLodFiles
            // 
            this.cbLodFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLodFiles.FormattingEnabled = true;
            this.cbLodFiles.Location = new System.Drawing.Point(2, 486);
            this.cbLodFiles.Name = "cbLodFiles";
            this.cbLodFiles.Size = new System.Drawing.Size(140, 21);
            this.cbLodFiles.TabIndex = 24;
            this.cbLodFiles.SelectedIndexChanged += new System.EventHandler(this.cbLodFiles_SelectedIndexChanged);
            // 
            // chbHover
            // 
            this.chbHover.AutoSize = true;
            this.chbHover.Location = new System.Drawing.Point(678, 328);
            this.chbHover.Name = "chbHover";
            this.chbHover.Size = new System.Drawing.Size(85, 17);
            this.chbHover.TabIndex = 23;
            this.chbHover.Text = "Autopreview";
            this.chbHover.UseVisualStyleBackColor = true;
            // 
            // chbTimerEnabled
            // 
            this.chbTimerEnabled.AutoSize = true;
            this.chbTimerEnabled.Location = new System.Drawing.Point(605, 328);
            this.chbTimerEnabled.Name = "chbTimerEnabled";
            this.chbTimerEnabled.Size = new System.Drawing.Size(67, 17);
            this.chbTimerEnabled.TabIndex = 21;
            this.chbTimerEnabled.Text = "Autoplay";
            this.chbTimerEnabled.UseVisualStyleBackColor = true;
            this.chbTimerEnabled.CheckedChanged += new System.EventHandler(this.chbTimerEnabled_CheckedChanged);
            // 
            // trbDefSprites
            // 
            this.trbDefSprites.Location = new System.Drawing.Point(605, 288);
            this.trbDefSprites.Name = "trbDefSprites";
            this.trbDefSprites.Size = new System.Drawing.Size(303, 45);
            this.trbDefSprites.TabIndex = 20;
            this.trbDefSprites.Visible = false;
            this.trbDefSprites.ValueChanged += new System.EventHandler(this.trbDefSprites_ValueChanged);
            // 
            // lbDecomposed
            // 
            this.lbDecomposed.FormattingEnabled = true;
            this.lbDecomposed.Location = new System.Drawing.Point(605, 360);
            this.lbDecomposed.Name = "lbDecomposed";
            this.lbDecomposed.Size = new System.Drawing.Size(300, 147);
            this.lbDecomposed.TabIndex = 19;
            this.lbDecomposed.SelectedIndexChanged += new System.EventHandler(this.lbDecomposed_SelectedIndexChanged);
            // 
            // pbResourceView
            // 
            this.pbResourceView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbResourceView.Location = new System.Drawing.Point(605, 0);
            this.pbResourceView.Name = "pbResourceView";
            this.pbResourceView.Size = new System.Drawing.Size(303, 282);
            this.pbResourceView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbResourceView.TabIndex = 18;
            this.pbResourceView.TabStop = false;
            this.pbResourceView.Visible = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.завантажитиToolStripMenuItem,
            this.зберегтиToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(146, 48);
            // 
            // завантажитиToolStripMenuItem
            // 
            this.завантажитиToolStripMenuItem.Name = "завантажитиToolStripMenuItem";
            this.завантажитиToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.завантажитиToolStripMenuItem.Text = "Завантажити";
            this.завантажитиToolStripMenuItem.Click += new System.EventHandler(this.завантажитиToolStripMenuItem_Click);
            // 
            // зберегтиToolStripMenuItem
            // 
            this.зберегтиToolStripMenuItem.Name = "зберегтиToolStripMenuItem";
            this.зберегтиToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.зберегтиToolStripMenuItem.Text = "Зберегти";
            this.зберегтиToolStripMenuItem.Click += new System.EventHandler(this.зберегтиToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(536, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(44, 131);
            this.panel1.TabIndex = 19;
            this.panel1.Visible = false;
            // 
            // btnSaveLocalChanges
            // 
            this.btnSaveLocalChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveLocalChanges.Location = new System.Drawing.Point(419, 2);
            this.btnSaveLocalChanges.Name = "btnSaveLocalChanges";
            this.btnSaveLocalChanges.Size = new System.Drawing.Size(117, 23);
            this.btnSaveLocalChanges.TabIndex = 20;
            this.btnSaveLocalChanges.Text = "Save Local Changes";
            this.btnSaveLocalChanges.UseVisualStyleBackColor = true;
            this.btnSaveLocalChanges.Visible = false;
            this.btnSaveLocalChanges.Click += new System.EventHandler(this.btnSaveLocalChanges_Click);
            // 
            // heroMainDataControl1
            // 
            this.heroMainDataControl.Location = new System.Drawing.Point(2, 9);
            this.heroMainDataControl.Name = "heroMainDataControl1";
            this.heroMainDataControl.Size = new System.Drawing.Size(900, 497);
            this.heroMainDataControl.TabIndex = 24;
            // 
            // heroClassDataControl
            // 
            this.heroClassDataControl.HeroClass = null;
            this.heroClassDataControl.Location = new System.Drawing.Point(2, 9);
            this.heroClassDataControl.Name = "heroClassDataControl";
            this.heroClassDataControl.Size = new System.Drawing.Size(562, 496);
            this.heroClassDataControl.TabIndex = 0;
            // 
            // creatureDataControl
            // 
            this.creatureDataControl.Location = new System.Drawing.Point(2, 9);
            this.creatureDataControl.Name = "creatureDataControl";
            this.creatureDataControl.SelectedCastle = -1;
            this.creatureDataControl.Size = new System.Drawing.Size(579, 512);
            this.creatureDataControl.TabIndex = 0;
            // 
            // spellDataControl
            // 
            this.spellDataControl.Location = new System.Drawing.Point(2, 9);
            this.spellDataControl.Name = "spellDataControl";
            this.spellDataControl.Size = new System.Drawing.Size(544, 500);
            this.spellDataControl.Spell = null;
            this.spellDataControl.TabIndex = 1;
            // 
            // specialityBuilder1
            // 
            this.specialityBuilder1.CurrentIndex = 0;
            this.specialityBuilder1.ForceAllCreatures = false;
            this.specialityBuilder1.HeroIndex = 0;
            this.specialityBuilder1.Location = new System.Drawing.Point(0, 0);
            this.specialityBuilder1.Name = "specialityBuilder1";
            this.specialityBuilder1.PropertyType = h3magic.ProfilePropertyType.Creature;
            this.specialityBuilder1.SelectedValue = 0;
            this.specialityBuilder1.Size = new System.Drawing.Size(510, 321);
            this.specialityBuilder1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 577);
            this.Controls.Add(this.btnSaveLocalChanges);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabsMain);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "h3Magic";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabsMain.ResumeLayout(false);
            this.tabHeroes.ResumeLayout(false);
            this.tabHeroClass.ResumeLayout(false);
            this.tabCreatures.ResumeLayout(false);
            this.tabSpells.ResumeLayout(false);
            this.tabResources.ResumeLayout(false);
            this.tabResources.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbDefSprites)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbResourceView)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.Button button4;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.RichTextBox rtbMain;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_openFile;
        private System.Windows.Forms.ToolStripMenuItem m_saveFile;
        private System.Windows.Forms.ToolStripMenuItem m_saveFileAs;
        private System.Windows.Forms.ToolStripMenuItem m_exit;
        private System.Windows.Forms.ComboBox cbFilter;
        private System.Windows.Forms.Button btnMergeChanges;
        private System.Windows.Forms.TabControl tabsMain;
        private System.Windows.Forms.TabPage tabResources;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem завантажитиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem зберегтиToolStripMenuItem;
        private System.Windows.Forms.TabPage tabHeroes;
        private System.Windows.Forms.TabPage tabSpells;
        private System.Windows.Forms.ListBox lbDecomposed;
        private System.Windows.Forms.PictureBox pbResourceView;
        private System.Windows.Forms.TrackBar trbDefSprites;
        private System.Windows.Forms.CheckBox chbTimerEnabled;
        private ImageGridControl specialityBuilder1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chbHover;
        private System.Windows.Forms.ComboBox cbLodFiles;
        private System.Windows.Forms.Panel panel1;
        private SpellDataControl spellDataControl;
        private System.Windows.Forms.Button btnSaveLocalChanges;
        private System.Windows.Forms.TabPage tabCreatures;
        private CreatureDataControl creatureDataControl;
        private System.Windows.Forms.TabPage tabHeroClass;
        private HeroClassDataControl heroClassDataControl;
        private HeroMainDataControl heroMainDataControl;
    }
}

