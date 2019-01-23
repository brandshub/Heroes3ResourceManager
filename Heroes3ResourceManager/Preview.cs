using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace h3magic
{
    public partial class Preview : Form
    {
        private Bitmap bmp;
        private string text;
        Form parent;

        public Preview(Form form)
        {
            InitializeComponent();
            parent = form;
        }

        public void Show(Bitmap bmp)
        {
            textBox1.Visible = false;
            pictureBox1.Visible = true;

            this.bmp = bmp;
            pictureBox1.Image = bmp;
            Show();

        }

        public void Show(string text)
        {

            textBox1.Visible = true;
            pictureBox1.Visible = false;

            this.text = text;
            textBox1.Text = text;
            Show();
        }

        protected override void OnShown(EventArgs e)
        {
            parent.Focus();
            base.OnShown(e);
        }
    }
}
