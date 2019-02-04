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
            this.gridImages = new h3magic.ImageGridControl();
            this.SuspendLayout();
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblType.Location = new System.Drawing.Point(1, 27);
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
            this.cbSpecialityType.Location = new System.Drawing.Point(697, 24);
            this.cbSpecialityType.Name = "cbSpecialityType";
            this.cbSpecialityType.Size = new System.Drawing.Size(184, 24);
            this.cbSpecialityType.TabIndex = 3;
            this.cbSpecialityType.ValueMember = "Value";
            this.cbSpecialityType.SelectedValueChanged += new System.EventHandler(this.cbSpecialityType_SelectedIndexChanged);
            // 
            // gridImages
            // 
            this.gridImages.CurrentIndex = 0;
            this.gridImages.Location = new System.Drawing.Point(0, 70);
            this.gridImages.Name = "gridImages";
            this.gridImages.PropertyType = h3magic.ProfilePropertyType.Creature;
            this.gridImages.SelectedValue = -1;
            this.gridImages.Size = new System.Drawing.Size(888, 399);
            this.gridImages.TabIndex = 5;
            // 
            // HeroPropertyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 481);
            this.Controls.Add(this.gridImages);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.cbSpecialityType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HeroPropertyForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.HeroPropertyForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cbSpecialityType;
        private h3magic.ImageGridControl gridImages;
    }
}