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
            main = new LSFB(this, 0, 24);
            bool scrpt;
            if (Main.parames.ContainsKey("Bool.DontAskForScript") ? !Convert.ToBoolean(Main.parames["Bool.DontAskForScript"]) : false)
                scrpt = MessageBox.Show("Запустить плагин со скриптами?\n(Скрипт будет загружен в поле)", "Добавить плагин", MessageBoxButtons.YesNo) != DialogResult.Yes;
            else
                scrpt = Main.parames.ContainsKey("Bool.AllowScript") ? !Convert.ToBoolean(Main.parames["Bool.AllowScript"]) : true;
            FastLua tmp = new FastLua(splitContainer1.Panel2);
            tmp.DoString(xml);
            name = tmp.Name;
            Text = name + " от " + tmp.Author;
            richTextBox1.BackColor = main.work.BackColor;
            richTextBox1.ForeColor = ForeColor;
            richTextBox1.Text = xml;
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
            ((Main)LSFB.MainForm).плагиныToolStripMenuItem.DropDownItems.Remove(((Main)LSFB.MainForm).пустоToolStripMenuItem);
            ((Main)LSFB.MainForm).плагиныToolStripMenuItem.DropDownItems.Add(((Main)LSFB.MainForm).PlugMaker(name));
            ((Main)LSFB.MainForm).plugs[name] = xml;
            Close();
        }
    }
}
