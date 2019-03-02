using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace h3magic
{
    public class ListBoxWithImages : ListBox
    {
        public ListBoxWithImages()
        {
            DoubleBuffered = true;
        }

        public void InvalidateSelected()
        {
            if (SelectedIndex >= TopIndex && SelectedIndex < TopIndex + (Height / ItemHeight))
                Invalidate(new Rectangle(0, (SelectedIndex - TopIndex) * ItemHeight, Width, ItemHeight));      
        }
    }
}
