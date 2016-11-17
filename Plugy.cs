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
        string name = null;
        public Plugy(string xml)
        {
            InitializeComponent();
            this.xml = xml;
            menuStrip1.Renderer = new LSFB.MyRenderer();
            main = new LSFB(this, 0, 24);
            string auth = null;
            bool scrpt;
            if (Main.parames.ContainsKey("Bool.DontAskForScript") ? !Convert.ToBoolean(Main.parames["Bool.DontAskForScript"]) : false)
                scrpt = MessageBox.Show("Запустить плагин со скриптами?\n(Скрипт будет загружен в поле)", "Добавить плагин", MessageBoxButtons.YesNo) != DialogResult.Yes;
            else
                scrpt = Main.parames.ContainsKey("Bool.AllowScript") ? !Convert.ToBoolean(Main.parames["Bool.AllowScript"]) : true;
            if(!scrpt)
            {
                FastLua tmp = new FastLua(splitContainer1.Panel2);
                tmp.DoString(xml);
                name = tmp.Name;
                auth = tmp.Author;
            }
            else
            {
                try
                {
                    name = ExtractFromString(xml, "--[[", "]]").First();
                }
                catch { }
                foreach (string s in ExtractFromString(xml, "Draw([[", "]])"))
                {
                    Dictionary<string, Control> cc = LSFB.DrawForm(splitContainer1.Panel2, s);
                    foreach(KeyValuePair<string, Control> v in cc)
                    {
                        LSFB.AddHelp(v.Value, v.Key);
                    }
                }
            }
            if (String.IsNullOrEmpty(name))
                name = "Безымянный плагин";
            Text = name +( String.IsNullOrEmpty(auth)? "" : " от " + auth);
            richTextBox1.BackColor = main.work.BackColor;
            richTextBox1.ForeColor = ForeColor;
            richTextBox1.Text = xml;
            LSFB.AddCms(richTextBox1);
            Size = LSFB.MainForm.Size;            
            ShowDialog();
        }
        private static List<string> ExtractFromString(
        string text, string startString, string endString)
        {
            List<string> matched = new List<string>();
            int indexStart = 0, indexEnd = 0;
            bool exit = false;
            while (!exit)
            {
                indexStart = text.IndexOf(startString);
                indexEnd = text.IndexOf(endString);
                if (indexStart != -1 && indexEnd != -1)
                {
                    matched.Add(text.Substring(indexStart + startString.Length,
                        indexEnd - indexStart - startString.Length));
                    text = text.Substring(indexEnd + endString.Length);
                }
                else
                    exit = true;
            }
            return matched;
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
