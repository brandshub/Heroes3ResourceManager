using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace h3magic
{
    public class HeroClass
    {
        public string[] Stats;

        public HeroClass(string row)
        {
            Stats = row.Split('\t');
        }

        public void Load(TabPage page, TextBox start)
        {

            int index = int.Parse(start.Name.Substring(start.Name.Length - 2));
            page.Controls["textBox" + index].Text = Stats[0];
            for (int i = 2; i < Stats.Length - 9; i++)
                page.Controls["textBox" + (index + i)].Text = Stats[i];
        }

        public void Save(TabPage page, TextBox start)
        {
            int index = int.Parse(start.Name.Substring(start.Name.Length - 2));
            Stats[0] = page.Controls["textBox" + index].Text;
            for (int i = 2; i < Stats.Length - 9; i++)
                Stats[i] = page.Controls["textBox" + (index + i)].Text;
        }

        public string GetRow()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Stats.Length - 1; i++)
            {
                sb.Append(Stats[i]);
                sb.Append('\t');
            }
            sb.Append(Stats.Last());
            return sb.ToString();
        }

        public override string ToString()
        {
            return Stats == null ? "" : Stats[0];
        }
    }
}
