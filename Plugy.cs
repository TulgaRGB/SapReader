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
using System.IO;

namespace SapReader
{
    public partial class Plugy : Form
    {
        LSFB main;
        string xml;
        string name = null;
        Dictionary<string, string> info = new Dictionary<string, string>();
        public Plugy(string xml)
        {
            InitializeComponent();
            this.xml = xml;
            menuStrip1.Renderer = new LSFB.MyRenderer() { Transparent = true };
            main = new LSFB(this, 3, 24);
            main.btns.First().Enabled = false;
            string auth = null;
            bool scrpt;
            if (Main.parames.ContainsKey("Bool.DontAskForScript") ? !Convert.ToBoolean(Main.parames["Bool.DontAskForScript"]) : false)
                scrpt = MessageBox.Show("Запустить плагин со скриптами?\n(Скрипт будет загружен в поле)", "Добавить плагин", MessageBoxButtons.YesNo) != DialogResult.Yes;
            else
                scrpt = Main.parames.ContainsKey("Bool.AllowScript") ? !Convert.ToBoolean(Main.parames["Bool.AllowScript"]) : true;
            if (!scrpt)
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
                    info = LSFB.GetInfo(ExtractFromString(xml, "Info([[", "]])").First());
                    info.TryGetValue("name", out name);
                }
                catch { }
                foreach (string s in ExtractFromString(xml.Replace("Info([[", "Draw([["), "Draw([[", "]])"))
                {
                    Dictionary<string, Control> cc = LSFB.DrawForm(splitContainer1.Panel2, s);
                    foreach (KeyValuePair<string, Control> v in cc)
                    {
                        LSFB.AddHelp(v.Value, v.Key);
                    }
                }
                if (splitContainer1.Panel2.Controls.Count == 0)
                {
                    LSFB.AddHelp(splitContainer1.Panel2, "Нет предварительного просмотра");
                }
            }
            if (String.IsNullOrEmpty(name))
                name = "Безымянный плагин";
            Text = name + (String.IsNullOrEmpty(auth) ? "" : " от " + auth);
            richTextBox1.BackColor = main.work.BackColor;
            richTextBox1.ForeColor = ForeColor;
            richTextBox1.Text = xml;
            if (info.ContainsKey("sign"))
                richTextBox1.Text = richTextBox1.Text.Replace(info["sign"], "...");
            LSFB.AddCms(richTextBox1);
            Size = LSFB.MainForm.Size;
            Show();
        }
        public static List<string> ExtractFromString(
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
        private void addPlugin(bool allowed)
        {
            if (!allowed)
                if (MessageBox.Show("Всё равно добавить?", name, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    allowed = true;
            if (allowed)
            {
                if (!Directory.Exists("Plugins"))
                    Directory.CreateDirectory("Plugins");
                File.WriteAllText(@"Plugins\" + name + ".lua", xml);
                Main.pluginsSha.Add(Sapphire.GetSha512(File.ReadAllBytes(@"Plugins\" + name + ".lua")));
                Main.main.UpdatePlugs();
                Close();
            }
        }

        private void checkSignature(object sender, EventArgs e)
        {
            try
            {
                if (Main.fc.key == null)
                    throw (new Exception());
                string hash = Sapphire.GetShaForPass(xml.Replace(info["sign"], ""));
                Main.fc.Request("<REQUEST type='checkSer' author='" + info["author"] + "' hash='" + hash + "' sign='" + info["sign"] + "'/>", new Action<string>((ses) =>
                {
                        try
                        {
                            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                            doc.LoadXml(ses);
                        if (((ToolStripMenuItem)sender).Text == "Добавить")
                            if (doc.FirstChild.Attributes.GetNamedItem("result").InnerText == "200")
                                addPlugin(true);
                            else
                                addPlugin(false);
                            Text = name + " - " + doc.FirstChild.InnerText; 

                        }
                        catch
                        {
                            Main.main.DebugMessage("Не удалось проверить плагин");
                            addPlugin(false);
                        }
                }));
            }
            catch
            {
                Main.main.DebugMessage("Не удалось проверить плагин");
                if (((ToolStripMenuItem)sender).Text == "Добавить")
                    addPlugin(false);
            }
        }

        private void показатьПодписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (info.ContainsKey("sign"))
                LSFB.Show(info["sign"], "Подпись плагина");
            else
                Main.main.DebugMessage("Плагин не подписан");
        }

        private void отменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
