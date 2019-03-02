namespace h3magic
{
    partial class HeroClassDataControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbName = new System.Windows.Forms.TextBox();
            this.lbl03 = new System.Windows.Forms.Label();
            this.lbl02 = new System.Windows.Forms.Label();
            this.pbPrimarySkills = new System.Windows.Forms.PictureBox();
            this.pbSkillTree = new System.Windows.Forms.PictureBox();
            this.lbl01 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbAggression = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbHeroClasses = new h3magic.ListBoxWithImages();
            ((System.ComponentModel.ISupportInitialize)(this.pbPrimarySkills)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSkillTree)).BeginInit();
            this.SuspendLayout();
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbName.Location = new System.Drawing.Point(210, 0);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(308, 22);
            this.tbName.TabIndex = 62;
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl03
            // 
            this.lbl03.AutoSize = true;
            this.lbl03.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl03.Location = new System.Drawing.Point(207, 152);
            this.lbl03.Name = "lbl03";
            this.lbl03.Size = new System.Drawing.Size(93, 13);
            this.lbl03.TabIndex = 67;
            this.lbl03.Text = "Level 10+ chance";
            // 
            // lbl02
            // 
            this.lbl02.AutoSize = true;
            this.lbl02.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl02.Location = new System.Drawing.Point(207, 128);
            this.lbl02.Name = "lbl02";
            this.lbl02.Size = new System.Drawing.Size(90, 13);
            this.lbl02.TabIndex = 64;
            this.lbl02.Text = "Level 2-9 chance";
            // 
            // pbPrimarySkills
            // 
            this.pbPrimarySkills.Location = new System.Drawing.Point(350, 60);
            this.pbPrimarySkills.Name = "pbPrimarySkills";
            this.pbPrimarySkills.Size = new System.Drawing.Size(168, 42);
            this.pbPrimarySkills.TabIndex = 65;
            this.pbPrimarySkills.TabStop = false;
            // 
            // pbSkillTree
            // 
            this.pbSkillTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbSkillTree.InitialImage = null;
            this.pbSkillTree.Location = new System.Drawing.Point(210, 228);
            this.pbSkillTree.Name = "pbSkillTree";
            this.pbSkillTree.Size = new System.Drawing.Size(308, 256);
            this.pbSkillTree.TabIndex = 63;
            this.pbSkillTree.TabStop = false;
            // 
            // lbl01
            // 
            this.lbl01.AutoSize = true;
            this.lbl01.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl01.Location = new System.Drawing.Point(207, 106);
            this.lbl01.Name = "lbl01";
            this.lbl01.Size = new System.Drawing.Size(98, 13);
            this.lbl01.TabIndex = 69;
            this.lbl01.Text = "Level 1 Parameters";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(207, 202);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 13);
            this.label2.TabIndex = 68;
            this.label2.Text = "Secondary Skill Chance";
            // 
            // cbAggression
            // 
            this.cbAggression.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAggression.FormattingEnabled = true;
            this.cbAggression.Items.AddRange(new object[] {
            "10s",
            "20s"});
            this.cbAggression.Location = new System.Drawing.Point(468, 33);
            this.cbAggression.Name = "cbAggression";
            this.cbAggression.Size = new System.Drawing.Size(50, 21);
            this.cbAggression.TabIndex = 70;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(207, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 71;
            this.label1.Text = "Agression";
            // 
            // lbHeroClasses
            // 
            this.lbHeroClasses.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbHeroClasses.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbHeroClasses.FormattingEnabled = true;
            this.lbHeroClasses.ItemHeight = 24;
            this.lbHeroClasses.Location = new System.Drawing.Point(0, 0);
            this.lbHeroClasses.Name = "lbHeroClasses";
            this.lbHeroClasses.Size = new System.Drawing.Size(182, 484);
            this.lbHeroClasses.TabIndex = 61;
            this.lbHeroClasses.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbHeroClasses_DrawItem);
            this.lbHeroClasses.SelectedIndexChanged += new System.EventHandler(this.lbHeroClasses_SelectedIndexChanged);
            // 
            // HeroClassDataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbAggression);
            this.Controls.Add(this.lbl01);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lbl03);
            this.Controls.Add(this.lbl02);
            this.Controls.Add(this.pbPrimarySkills);
            this.Controls.Add(this.lbHeroClasses);
            this.Controls.Add(this.pbSkillTree);
            this.Name = "HeroClassDataControl";
            this.Size = new System.Drawing.Size(534, 497);
            ((System.ComponentModel.ISupportInitialize)(this.pbPrimarySkills)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSkillTree)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label lbl03;
        private System.Windows.Forms.Label lbl02;
        private System.Windows.Forms.PictureBox pbPrimarySkills;
        private ListBoxWithImages lbHeroClasses;
        private System.Windows.Forms.PictureBox pbSkillTree;
        private System.Windows.Forms.Label lbl01;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbAggression;
        private System.Windows.Forms.Label label1;
    }
}
