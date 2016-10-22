using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LS;
using System.Xml;

namespace SapReader
{
    public partial class Plugy : Form
    {
        LSFB main;
        string xml;
        string name;
        public Plugy(string xml)
        {
            InitializeComponent();
            this.xml = xml;
            menuStrip1.Renderer = new MyRenderer();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            main = new LSFB(this, 0, 24);
            name = doc.SelectSingleNode("/FORM").Attributes.GetNamedItem("name").InnerText;
            Text = name + " от " + doc.SelectSingleNode("/FORM").Attributes.GetNamedItem("author").InnerText;
            LSFB.drawForm(main.work, xml, MessageBox.Show("Запустить плагин со скриптами?", "Добавить плагин", MessageBoxButtons.YesNo) != DialogResult.Yes);
            ShowDialog();
        }

        private void отменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Form1)LSFB.MainForm).плагиныToolStripMenuItem.DropDownItems.Remove(((Form1)LSFB.MainForm).пустоToolStripMenuItem);
            ((Form1)LSFB.MainForm).плагиныToolStripMenuItem.DropDownItems.Add(((Form1)LSFB.MainForm).PlugMaker(name));
            ((Form1)LSFB.MainForm).plugs[name] = xml;
            Close();
        }
    }
}
