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
            menuStrip1.Renderer = new LSFB.MyRenderer();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            main = new LSFB(this, 0, 24);
            bool scrpt;
            if (Form1.parames.ContainsKey("Bool.DontAskForScript") ? !Convert.ToBoolean(Form1.parames["Bool.DontAskForScript"]) : false)
                scrpt = MessageBox.Show("Запустить плагин со скриптами?\n(Скрипт будет загружен в поле)", "Добавить плагин", MessageBoxButtons.YesNo) != DialogResult.Yes;
            else
                scrpt = Form1.parames.ContainsKey("Bool.AllowScript") ? !Convert.ToBoolean(Form1.parames["Bool.AllowScript"]) : true;
            Dictionary <string,string> tmp = LSFB.drawForm(splitContainer1.Panel2, xml, scrpt);
            name = tmp["name"];
            Text = name + " от " + tmp["author"];
            richTextBox1.BackColor = main.work.BackColor;
            richTextBox1.ForeColor = ForeColor;
            richTextBox1.Text = tmp["script"];
            LSFB.AddCms(richTextBox1);
            Size = LSFB.MainForm.Size;            
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
