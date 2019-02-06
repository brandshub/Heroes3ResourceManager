namespace h3magic
{
    partial class HeroPropertyForm
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
            this.lblType = new System.Windows.Forms.Label();
            this.cbSpecialityType = new System.Windows.Forms.ComboBox();
            this.lblAttack = new System.Windows.Forms.Label();
            this.lblDefense = new System.Windows.Forms.Label();
            this.lblDamage = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlCreatureStatic = new System.Windows.Forms.Panel();
            this.tbAttack = new System.Windows.Forms.NumericUpDown();
            this.tbDefense = new System.Windows.Forms.NumericUpDown();
            this.tbDamage = new System.Windows.Forms.NumericUpDown();
            this.pnlCreatureUpgrade = new System.Windows.Forms.Panel();
            this.pbCreature1 = new System.Windows.Forms.PictureBox();
            this.pbCreature2 = new System.Windows.Forms.PictureBox();
            this.pbCreature3 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pHighlight = new System.Windows.Forms.Panel();
            this.gridImages = new h3magic.ImageGridControl();
            this.pnlCreatureStatic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbAttack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDefense)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDamage)).BeginInit();
            this.pnlCreatureUpgrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCreature1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCreature2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCreature3)).BeginInit();
            this.SuspendLayout();
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblType.Location = new System.Drawing.Point(9, 27);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(40, 16);
            this.lblType.TabIndex = 4;
            this.lblType.Text = "Type";
            // 
            // cbSpecialityType
            // 
            this.cbSpecialityType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSpecialityType.DisplayMember = "Text";
            this.cbSpecialityType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSpecialityType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbSpecialityType.FormattingEnabled = true;
            this.cbSpecialityType.Location = new System.Drawing.Point(708, 24);
            this.cbSpecialityType.Name = "cbSpecialityType";
            this.cbSpecialityType.Size = new System.Drawing.Size(173, 24);
            this.cbSpecialityType.TabIndex = 3;
            this.cbSpecialityType.ValueMember = "Value";
            this.cbSpecialityType.SelectedValueChanged += new System.EventHandler(this.cbSpecialityType_SelectedIndexChanged);
            // 
            // lblAttack
            // 
            this.lblAttack.AutoSize = true;
            this.lblAttack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblAttack.Location = new System.Drawing.Point(7, 6);
            this.lblAttack.Name = "lblAttack";
            this.lblAttack.Size = new System.Drawing.Size(55, 16);
            this.lblAttack.TabIndex = 7;
            this.lblAttack.Text = "+ Attack";
            // 
            // lblDefense
            // 
            this.lblDefense.AutoSize = true;
            this.lblDefense.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDefense.Location = new System.Drawing.Point(132, 6);
            this.lblDefense.Name = "lblDefense";
            this.lblDefense.Size = new System.Drawing.Size(69, 16);
            this.lblDefense.TabIndex = 9;
            this.lblDefense.Text = "+ Defense";
            // 
            // lblDamage
            // 
            this.lblDamage.AutoSize = true;
            this.lblDamage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDamage.Location = new System.Drawing.Point(277, 6);
            this.lblDamage.Name = "lblDamage";
            this.lblDamage.Size = new System.Drawing.Size(71, 16);
            this.lblDamage.TabIndex = 11;
            this.lblDamage.Text = "+ Damage";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSave.Location = new System.Drawing.Point(708, 74);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(173, 33);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Accept";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pnlCreatureStatic
            // 
            this.pnlCreatureStatic.Controls.Add(this.tbDamage);
            this.pnlCreatureStatic.Controls.Add(this.tbDefense);
            this.pnlCreatureStatic.Controls.Add(this.tbAttack);
            this.pnlCreatureStatic.Controls.Add(this.lblDamage);
            this.pnlCreatureStatic.Controls.Add(this.lblAttack);
            this.pnlCreatureStatic.Controls.Add(this.lblDefense);
            this.pnlCreatureStatic.Location = new System.Drawing.Point(12, 74);
            this.pnlCreatureStatic.Name = "pnlCreatureStatic";
            this.pnlCreatureStatic.Size = new System.Drawing.Size(417, 33);
            this.pnlCreatureStatic.TabIndex = 13;
            // 
            // tbAttack
            // 
            this.tbAttack.Location = new System.Drawing.Point(68, 6);
            this.tbAttack.Name = "tbAttack";
            this.tbAttack.Size = new System.Drawing.Size(49, 20);
            this.tbAttack.TabIndex = 12;
            this.tbAttack.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbDefense
            // 
            this.tbDefense.Location = new System.Drawing.Point(207, 6);
            this.tbDefense.Name = "tbDefense";
            this.tbDefense.Size = new System.Drawing.Size(49, 20);
            this.tbDefense.TabIndex = 13;
            this.tbDefense.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbDamage
            // 
            this.tbDamage.Location = new System.Drawing.Point(354, 6);
            this.tbDamage.Name = "tbDamage";
            this.tbDamage.Size = new System.Drawing.Size(49, 20);
            this.tbDamage.TabIndex = 14;
            this.tbDamage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // pnlCreatureUpgrade
            // 
            this.pnlCreatureUpgrade.Controls.Add(this.pHighlight);
            this.pnlCreatureUpgrade.Controls.Add(this.label1);
            this.pnlCreatureUpgrade.Controls.Add(this.pbCreature3);
            this.pnlCreatureUpgrade.Controls.Add(this.pbCreature2);
            this.pnlCreatureUpgrade.Controls.Add(this.pbCreature1);
            this.pnlCreatureUpgrade.Location = new System.Drawing.Point(12, 59);
            this.pnlCreatureUpgrade.Name = "pnlCreatureUpgrade";
            this.pnlCreatureUpgrade.Size = new System.Drawing.Size(537, 71);
            this.pnlCreatureUpgrade.TabIndex = 14;
            // 
            // pbCreature1
            // 
            this.pbCreature1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbCreature1.Location = new System.Drawing.Point(0, 0);
            this.pbCreature1.Name = "pbCreature1";
            this.pbCreature1.Size = new System.Drawing.Size(58, 64);
            this.pbCreature1.TabIndex = 15;
            this.pbCreature1.TabStop = false;
            this.pbCreature1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbCreature1_MouseClick);
            // 
            // pbCreature2
            // 
            this.pbCreature2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbCreature2.Location = new System.Drawing.Point(100, 0);
            this.pbCreature2.Name = "pbCreature2";
            this.pbCreature2.Size = new System.Drawing.Size(58, 64);
            this.pbCreature2.TabIndex = 16;
            this.pbCreature2.TabStop = false;
            this.pbCreature2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbCreature1_MouseClick);
            // 
            // pbCreature3
            // 
            this.pbCreature3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbCreature3.Location = new System.Drawing.Point(359, 0);
            this.pbCreature3.Name = "pbCreature3";
            this.pbCreature3.Size = new System.Drawing.Size(58, 64);
            this.pbCreature3.TabIndex = 17;
            this.pbCreature3.TabStop = false;
            this.pbCreature3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbCreature1_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(221, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 24);
            this.label1.TabIndex = 18;
            this.label1.Text = "====>";
            // 
            // panel1
            // 
            this.pHighlight.BackColor = System.Drawing.Color.Red;
            this.pHighlight.Location = new System.Drawing.Point(0, 62);
            this.pHighlight.Name = "panel1";
            this.pHighlight.Size = new System.Drawing.Size(58, 3);
            this.pHighlight.TabIndex = 15;
            // 
            // gridImages
            // 
            this.gridImages.CurrentIndex = 0;
            this.gridImages.HeroIndex = 0;
            this.gridImages.Location = new System.Drawing.Point(0, 192);
            this.gridImages.Name = "gridImages";
            this.gridImages.PropertyType = h3magic.ProfilePropertyType.Creature;
            this.gridImages.SelectedValue = -1;
            this.gridImages.Size = new System.Drawing.Size(888, 277);
            this.gridImages.TabIndex = 5;
            // 
            // HeroPropertyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 481);
            this.Controls.Add(this.pnlCreatureStatic);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gridImages);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.cbSpecialityType);
            this.Controls.Add(this.pnlCreatureUpgrade);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HeroPropertyForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Value";
            this.pnlCreatureStatic.ResumeLayout(false);
            this.pnlCreatureStatic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbAttack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDefense)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDamage)).EndInit();
            this.pnlCreatureUpgrade.ResumeLayout(false);
            this.pnlCreatureUpgrade.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCreature1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCreature2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCreature3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cbSpecialityType;
        private h3magic.ImageGridControl gridImages;
        private System.Windows.Forms.Label lblAttack;
        private System.Windows.Forms.Label lblDefense;
        private System.Windows.Forms.Label lblDamage;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel pnlCreatureStatic;
        private System.Windows.Forms.NumericUpDown tbDamage;
        private System.Windows.Forms.NumericUpDown tbDefense;
        private System.Windows.Forms.NumericUpDown tbAttack;
        private System.Windows.Forms.Panel pnlCreatureUpgrade;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbCreature3;
        private System.Windows.Forms.PictureBox pbCreature2;
        private System.Windows.Forms.PictureBox pbCreature1;
        private System.Windows.Forms.Panel pHighlight;
    }
}