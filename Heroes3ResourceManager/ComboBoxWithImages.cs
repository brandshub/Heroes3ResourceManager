using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace h3magic
{
    public class ComboBoxWithImages : ComboBox
    {

        private int[] drawnItemsIndices;
        private int internalC = 0;
        private int curSel;

        public ComboBoxWithImages()
        {
            DoubleBuffered = true;
        }

        protected override void OnDropDown(EventArgs e)
        {
            drawnItemsIndices = new int[Items.Count];
            curSel = SelectedIndex;
            base.OnDropDown(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {

            if (Items.Count > 25)
            {
                base.OnDrawItem(e);
                return;
            }

            if ((e.State & DrawItemState.ComboBoxEdit) != 0)
            {
                base.OnDrawItem(e);
                return;
            }

            if (drawnItemsIndices != null && DroppedDown)
            {
                if (e.State == DrawItemState.Selected)
                {
                    if (drawnItemsIndices[e.Index] == 0)
                    {
                        //first draw??
                        drawnItemsIndices[e.Index] = 1;
                        return;
                    }
                    if (drawnItemsIndices[e.Index] < 2 && e.Index == curSel)
                    {
                        base.OnDrawItem(e);
                        dl("SOnDraw +" + e.State + " " + e.Index + " " + drawnItemsIndices[e.Index] + " " + internalC++);
                        drawnItemsIndices[e.Index]++;
                        return;
                    }
                }
                else if (e.State == DrawItemState.None)
                {
                    if (drawnItemsIndices[e.Index] == 0)
                    {
                        base.OnDrawItem(e);
                        dl("NOnDraw +" + e.State + " " + e.Index + " " + e.Bounds + " " + internalC++);
                        drawnItemsIndices[e.Index]++;
                        return;
                    }
                }
            }
            dl("OnDraw -" + e.State + " " + e.Index + " " + e.Bounds + " " + internalC++);
        }

        /* private listn nn = null;

         protected override void WndProc(ref Message m)
         {
             if (m.Msg == 0x134 && nn == null)
             {
                 nn = new listn();
                 nn.AssignHandle(m.LParam);
             }
             else
             {
                 base.WndProc(ref m);
             }
         }*/
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, out SCROLLINFO lpsi);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public enum ScrollInfoMask : uint
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS),
        }
        public enum SBOrientation : int
        {
            SB_HORZ = 0x0,
            SB_VERT = 0x1,
            SB_CTL = 0x2,
            SB_BOTH = 0x3
        }
        [Serializable, StructLayout(LayoutKind.Sequential)]
        struct SCROLLINFO
        {
            public int cbSize; // (uint) int is because of Marshal.SizeOf
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }

        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_FRAMECHANGED = 0x0020;
        public const int SWP_NOOWNERZORDER = 0x0200;

        public const int WM_CTLCOLORLISTBOX = 0x0134;

        private int _hwndDropDown = 0;

        /*   protected override void WndProc(ref Message m)
           {
               if (m.Msg == WM_CTLCOLORLISTBOX)
               {
                   if (_hwndDropDown == 0)
                   {
                       _hwndDropDown = m.LParam.ToInt32();

                       RECT r;
                       GetWindowRect((IntPtr)_hwndDropDown, out r);

                       //height of four items plus 2 pixels for the border in my test
                       int newHeight;

           

                       if(Items.Count * ItemHeight +2 > r.Bottom-r.Top)
                           drawnItemsIndices = new int[Items.Count];
                 

                       SetWindowPos((IntPtr)_hwndDropDown, IntPtr.Zero,
                           r.Left,
                                    r.Top,
                                    DropDownWidth,
                                    newHeight,
                                    SWP_FRAMECHANGED |
                                        SWP_NOACTIVATE |
                                        SWP_NOZORDER |
                                        SWP_NOOWNERZORDER);
                   }

               }
               else
                   base.WndProc(ref m);
           }
           */

        private void dl(string s)
        {
            //Debug.WriteLine(Name + " " + DateTime.Now.ToString("HH:mm:ss") + " " + s);
        }
    }

    /*    public class listn : NativeWindow
        {

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x115)
                {




                }
                base.WndProc(ref m);

            }
        }*/
}
