using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using LS;

namespace SapReader
{
    public partial class Props : Form
    {
        LSFB main;
        public Props(int page = 0)
        {
            InitializeComponent();
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(0, 1);
            tabControl.SizeMode = TabSizeMode.Fixed;
            mm.Renderer = new LSFB.MyRenderer() { Transparent = true };
            main = new LS.LSFB(this, 2, 0, 80, false);
            main.MakeControlLikeWork(yolka);
            main.MakeControlLikeWork(formats);
            foreach (TabPage p in tabControl.TabPages)
            {
                main.MakeControlLikeWork(p);
                p.AutoScroll = true;
                mm.Items.Add(p.Text);
                mm.Items[mm.Items.Count - 1].Click += tabSelect;
            }
            tabControl.Dock = DockStyle.Fill;
            cmsPlugs.Renderer = new LSFB.MyRenderer();
            cmsBrow.Renderer = new LSFB.MyRenderer();
            #region Colors
            Image image = new Bitmap(201, 201);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.FillEllipse(Brushes.White, 0, 0, 201, 201);
                for (int y = -100; y < 101; y++)
                {
                    for (int x = -100; x < 101; x++)
                    {
                        Color c = ((Bitmap)image).GetPixel(x + 100, y + 100);
                        if (c.A > 0)
                        {
                            Point pol = DecToPol(x, y);
                            Color rgb = HSV(pol.Y, pol.X, 100);
                            g.FillRectangle(new SolidBrush(rgb), x + 100, y + 100, 1, 1);
                        }
                    }
                }
            }
            Image r = new Bitmap(201, 21);
            using (Graphics g = Graphics.FromImage(r))
            {
                for (int i = 0; i < 202; i++)
                {
                    g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(255, (int)(i / 201D * 255), 0, 0))), i, 0, i, 20);
                }
            }
            Image gc = new Bitmap(201, 21);
            using (Graphics e = Graphics.FromImage(gc))
            {
                for (int i = 0; i < 202; i++)
                {
                    e.DrawLine(new Pen(new SolidBrush(Color.FromArgb(255, 0, (int)(i / 201D * 255), 0))), i, 0, i, 20);
                }
            }
            Image b = new Bitmap(201, 21);
            using (Graphics g = Graphics.FromImage(b))
            {
                for (int i = 0; i < 202; i++)
                {
                    g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(255, 0, 0, (int)(i / 201D * 255)))), i, 0, i, 20);
                }
            }
            rb.Paint += (object sender, PaintEventArgs e) =>
            {
                int u = (int)(BackColor.R / 255D * 201);
                int d = (int)(ForeColor.R / 255D * 201);
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                string text = BackColor.R + " " + ForeColor.R;
                SizeF textSize = e.Graphics.MeasureString(text, LSFB.sizer(8));
                PointF locationToDraw = new PointF();
                locationToDraw.X = (rb.Width / 2) - (textSize.Width / 2);
                locationToDraw.Y = (rb.Height / 2) - (textSize.Height / 2);
                e.Graphics.DrawString(text, LSFB.sizer(8), Brushes.White, locationToDraw);
            };
            gb.Paint += (object sender, PaintEventArgs e) =>
            {
                int u = (int)(BackColor.G / 255D * 201);
                int d = (int)(ForeColor.G / 255D * 201);
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                string text = BackColor.G + " " + ForeColor.G;
                SizeF textSize = e.Graphics.MeasureString(text, LSFB.sizer(8));
                PointF locationToDraw = new PointF();
                locationToDraw.X = (rb.Width / 2) - (textSize.Width / 2);
                locationToDraw.Y = (rb.Height / 2) - (textSize.Height / 2);
                e.Graphics.DrawString(text, LSFB.sizer(8), Brushes.White, locationToDraw);
            };
            bb.Paint += (object sender, PaintEventArgs e) =>
            {
                int u = (int)(BackColor.B / 255D * 201);
                int d = (int)(ForeColor.B / 255D * 201);
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                string text = BackColor.B + " " + ForeColor.B;
                SizeF textSize = e.Graphics.MeasureString(text, LSFB.sizer(8));
                PointF locationToDraw = new PointF();
                locationToDraw.X = (rb.Width / 2) - (textSize.Width / 2);
                locationToDraw.Y = (rb.Height / 2) - (textSize.Height / 2);
                e.Graphics.DrawString(text, LSFB.sizer(8), Brushes.White, locationToDraw);
            };
            pictureBox1.Image = image;
            rb.Image = r;
            gb.Image = gc;
            bb.Image = b;
            rb.MouseClick += RGB;
            gb.MouseClick += RGB;
            bb.MouseClick += RGB;
            rb.MouseMove += RGB;
            gb.MouseMove += RGB;
            bb.MouseMove += RGB;
            pictureBox1.MouseMove += pick;
            pictureBox1.MouseClick += pick;
            #endregion
            #region Bools
            foreach (string k in Bools.Keys)
            {
                bool ch = false;
                try
                {
                    ch = Convert.ToBoolean(Main.parames["Bool." + Bools[k]]);
                }
                catch { }
                Мойвыбор.Controls.Add(new CheckBox { Text = k, AutoSize = true, Location = new Point(12, 12 + 23 * Мойвыбор.Controls.Count), Checked = ch, FlatStyle = FlatStyle.Flat });
            }
            #endregion
            #region Pros
            checkBox1.Checked = Main.parames["Pro.Custom"] == "True";
            login.Text = Main.parames["Pro.Login"];
            pass.Text = Main.parames["Pro.Pass"];
            ip.Text = Main.parames["Pro.Ip"];
            ip.Enabled = checkBox1.Checked;
            #endregion
            #region Yolka            
            Yolka();
            #endregion
            #region Formats
            foreach (string s in Main.parames["Browser.Formats"].Split('|'))
                formats.Items.Add(s);
            #endregion
            textBox1.Text = Main.parames["Color.Image"];
            tabSelect(mm.Items[page], null);
            Show();
        }
        void Yolka(TreeNode n = null, ToolStripItemCollection cc = null)
        {
            if (n == null)
            {
                переместитьToolStripMenuItem.DropDownItems.Clear();
                yolka.Nodes.Clear();
                ToolStripMenuItem l = new ToolStripMenuItem("...");
                l.Click += (object sender, EventArgs e) =>
                {
                    try
                    {
                        TreeNode rnode = yolka.SelectedNode;
                        if (rnode.Text.Contains('.'))
                        {
                            if (@"Plugins\" + rnode.Text != @"Plugins\" + rnode.FullPath)
                            {
                                File.WriteAllBytes(@"Plugins\" + rnode.Text, File.ReadAllBytes(@"Plugins\" + rnode.FullPath));
                                File.Delete(@"Plugins\" + rnode.FullPath);
                            }
                        }
                        else
                        {
                            Directory.Delete(@"Plugins\" + rnode.FullPath);
                            Directory.CreateDirectory(@"Plugins\" + rnode.Text);
                        }
                        обновитьToolStripMenuItem_Click(null, null);
                    }
                    catch (Exception ex) { Main.main.DebugMessage(ex.Message); }
                };
                переместитьToolStripMenuItem.DropDownItems.Add(l);
                переместитьToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            }
            if (cc == null)
                cc = Main.main.инструментыToolStripMenuItem.DropDownItems;
            foreach (ToolStripItem i in cc)
            {
                if (i.Tag + "" == "Plugin")
                {
                    TreeNode node = new TreeNode(i.Text);
                    if (n == null)
                        yolka.Nodes.Add(node);
                    else
                        n.Nodes.Add(node);
                    Yolka(node, ((ToolStripMenuItem)i).DropDownItems);
                    if (!node.Text.Contains('.'))
                    {
                        ToolStripMenuItem l = new ToolStripMenuItem(node.Text);
                        l.Click += (object sender, EventArgs e) =>
                        {
                            try
                            {
                                TreeNode rnode = yolka.SelectedNode;
                                if (rnode.Text.Contains('.'))
                                {
                                    if (@"Plugins\" + node.FullPath + "\\" + rnode.Text != @"Plugins\" + rnode.FullPath)
                                    {
                                        File.WriteAllBytes(@"Plugins\" + node.FullPath + "\\" + rnode.Text, File.ReadAllBytes(@"Plugins\" + rnode.FullPath));
                                        File.Delete(@"Plugins\" + rnode.FullPath);
                                    }
                                }
                                else
                                {
                                    Directory.Delete(@"Plugins\" + rnode.FullPath);
                                    Directory.CreateDirectory(@"Plugins\" + node.FullPath + @"\" + rnode.Text);
                                }
                                обновитьToolStripMenuItem_Click(null, null);
                            }
                            catch (Exception ex) { Main.main.DebugMessage(ex.Message); }
                        };
                        переместитьToolStripMenuItem.DropDownItems.Add(l);
                    }
                }
            }
            if (n == null)
                yolka.ExpandAll();
        }
        Dictionary<string, string> Bools = new Dictionary<string, string>()
        {
            { "Автозапуск при старте Windows","AutoStart" },
            { "Подключаться к серверу при запуске","AutoConnect" },
            { "Запускать плагины со скриптами", "AllowScript" },
            { "Не спрашивать при запуске скриптов", "DontAskForScript" },
            { "Разворачивать окно при старте", "MaximizeOnStart" },
            { "Работать с непровереннымим плагинами","UseNotValidPlugins" },
            { "Разрешать запуск плагинов без Хеша", "UsePluginsNoSha" },
            { "Давать плагинам доступ к серверу", "AllowPluginConnection" }
        };
        void pick(object sender, MouseEventArgs e)
        {
            Color pxl = ((Bitmap)(pictureBox1.Image)).GetPixel(e.X > 200 ? 200 : e.X < 0 ? 0 : e.X, e.Y > 200 ? 200 : e.Y < 0 ? 0 : e.Y);
            if (pxl.A == 255)
                if (e.Button == MouseButtons.Left)
                {
                    LSFB.MainForm.BackColor = pxl;
                    Color tmp = LSFB.MainForm.ForeColor;
                    LSFB.MainForm.ForeColor = LSFB.MainForm.BackColor;
                    LSFB.MainForm.ForeColor = tmp;
                    Main.parames["Color.BackColor"] = LSFB.ToHex(pxl);
                }
                else
                    if (e.Button == MouseButtons.Right)
                {
                    LSFB.MainForm.ForeColor = pxl;
                    Main.parames["Color.ForeColor"] = LSFB.ToHex(pxl);
                }

        }
        void RGB(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                string name = ((PictureBox)sender).Name;
                int[] p = { -1, -1, -1 };
                switch (name)
                {
                    case "rb":
                        p[0] = (int)(e.X / 201D * 255) > 255 ? 255 : (int)(e.X / 201D * 255) < 0 ? 0 : (int)(e.X / 201D * 255);
                        break;
                    case "gb":
                        p[1] = (int)(e.X / 201D * 255) > 255 ? 255 : (int)(e.X / 201D * 255) < 0 ? 0 : (int)(e.X / 201D * 255);
                        break;
                    case "bb":
                        p[2] = (int)(e.X / 201D * 255) > 255 ? 255 : (int)(e.X / 201D * 255) < 0 ? 0 : (int)(e.X / 201D * 255);
                        break;
                }
                if (e.Button == MouseButtons.Left)
                {
                    for (int i = 0; i < 3; i++)
                        if (p[i] == -1)
                            p[i] = i < 1 ? LSFB.MainForm.BackColor.R : i > 1 ? LSFB.MainForm.BackColor.B : LSFB.MainForm.BackColor.G;
                    LSFB.MainForm.BackColor = Color.FromArgb(255, p[0], p[1], p[2]);
                    Color tmp = LSFB.MainForm.ForeColor;
                    LSFB.MainForm.ForeColor = LSFB.MainForm.BackColor;
                    LSFB.MainForm.ForeColor = tmp;
                    Main.parames["Color.BackColor"] = LSFB.ToHex(LSFB.MainForm.BackColor);
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                        if (p[i] == -1)
                            p[i] = i < 1 ? LSFB.MainForm.ForeColor.R : i > 1 ? LSFB.MainForm.ForeColor.B : LSFB.MainForm.ForeColor.G;
                    LSFB.MainForm.ForeColor = Color.FromArgb(255, p[0], p[1], p[2]);
                    Main.parames["Color.ForeColor"] = LSFB.ToHex(LSFB.MainForm.ForeColor);
                }
            }
        }
        static Color HSV(int deg, int rad, int val)
        {
            if (rad == 101)
                rad = 100;
            int r = 0;
            int g = 0;
            int b = 0;
            if (deg < -59 && deg > -121)
            {
                r = (int)((1 + ((60 + deg) / 60D)) * 255D);
            }
            if (deg < -119)
                g = (int)((1 - ((180D + deg) / 60)) * 255D);
            if (deg > 119)
                b = (int)((1 - ((180D - deg) / 60)) * 255D);
            if (deg > 60 && deg < 121)
                r = (int)(((120D - deg) / 60) * 255D);
            if (deg < 61 && deg > 0)
                g = (int)(deg / 60D * 255D);
            if (deg < 0 && deg > -60)
                b = -(int)(deg / 60D * 255D);
            if (deg < 61 && deg > -61)
                r = 255;
            if (deg < -59)
                b = 255;
            if (deg > 59)
                g = 255;
            return Color.FromArgb((int)(rad / 100D * 255), r, g, b);
        }
        public static Point DecToPol(int x, int y)
        {
            Point point = new Point();
            point.X = (int)Math.Sqrt(x * x + y * y);
            point.Y = (int)(Math.Atan2(y, x) * 180 / Math.PI);
            return point;
        }

        private void tabSelect(object sender, EventArgs e)
        {
            ToolStripMenuItem th = (ToolStripMenuItem)sender;
            if (mm.Items.IndexOf(th) < tabControl.TabCount)
                tabControl.SelectTab(mm.Items.IndexOf(th));
            Text = "Настройки" + (th.Text != "Настройки" ? " - " + th.Text : "");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ((Main)LSFB.MainForm).ReloadAllParams();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Main.parames["Pro.Login"] = login.Text;
            Main.parames["Pro.Ip"] = ip.Text;
            Main.parames["Pro.Custom"] = checkBox1.Checked + "";
            foreach (CheckBox c in Мойвыбор.Controls)
            {
                if (Bools[c.Text] == "AutoStart")
                    LSFB.AutoRun("SapReader", c.Checked);
                Main.parames["Bool." + Bools[c.Text]] = c.Checked + "";
            }
            Main.parames["Pro.Pass"] = pass.Text;
            if (!LSFB.SaveParams("SapReader", Main.parames)) ((Main)LSFB.MainForm).DebugMessage("Настройки не были сохранены!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (label2.Visible)
                LSFB.ResetParams("SapReader");
            label2.Visible = !label2.Visible;
        }
        #region cmsPro
        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Main.main.UpdatePlugs();
            Yolka();
        }
        private void сброситьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LSFB.ResetParams("SapReader", new[] { "Auto.Plugs" }))
                Main.pluginsSha.Clear();
        }
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode now = yolka.SelectedNode;
            string way = @"Plugins\" + now.FullPath;
            try
            {
                if (now.Text.Contains('.'))
                {
                    File.Delete(way);
                }
                else
                {
                    Directory.Delete(way);
                }
                обновитьToolStripMenuItem_Click(null, null);
            }
            catch (Exception ex) { Main.main.DebugMessage(ex.Message); }
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Main.main.MenuHandler(new ToolStripMenuItem { Tag = "Add" }, null);
        }

        private void новаяПапкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = null;
            if (LSFB.InputBox(Text, "Введите название папки:", ref text, false) == DialogResult.OK)
            {
                Directory.CreateDirectory(@"Plugins\" + text);
                обновитьToolStripMenuItem_Click(null, null);
            }
        }
        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ip.Enabled = checkBox1.Checked;
            Main.parames["Pro.Custom"] = checkBox1.Checked + "";
        }

        private void добавитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string add = null;
            if (LSFB.InputBox("Новый формат", "Введите расширение(я) которое(ые) необходимо открывать в редакторе (через пробел)", ref add, false) == DialogResult.OK)
            {
                foreach (string ext in add.Split(' '))
                    if (!Main.parames["Browser.Formats"].Split('|').Contains(ext))
                    {
                        Main.parames["Browser.Formats"] += (Main.parames["Browser.Formats"] != "" ? "|" : "") + ext;
                        formats.Items.Add(ext);
                    }
            }
        }

        private void удалитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string i = formats.SelectedItem + "";
            if (i != "")
            {
                Main.parames["Browser.Formats"] = Main.parames["Browser.Formats"].Replace(i, "");
                Main.parames["Browser.Formats"] = Main.parames["Browser.Formats"].Replace("||", "|");
                formats.Items.Remove(i);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement main = doc.CreateElement("OPTIONS");
            doc.AppendChild(main);
            main.SNI("Info.Name", "SapReader");
            main.SNI("Info.Version", Application.ProductVersion);
            foreach (KeyValuePair<string, string> par in Main.parames.Where(s => s.Key.Split('.').First() != "Auto" && s.Key != "Pro.Pass")
                        .ToDictionary(dict => dict.Key, dict => dict.Value))
                main.SNI(par.Key, par.Value);
            Main.main.NewTab();
            Main.main.page.Text = "Безымянный.CONFIG";
            Main.main.BoxToWrite(LSFB.XmlTostring(doc).Replace(" ", Environment.NewLine));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Main.main.page.Controls.Find("BoxToWrite", true).First().Text);
                if (doc.FirstChild.Attributes.GetNamedItem("Info.Name").InnerText == "SapReader")
                {
                    foreach (XmlAttribute a in doc.FirstChild.Attributes)
                        if (a.Name.Split('.').First() != "Info")
                        {
                            Main.parames[a.Name] = a.InnerText;
                        }
                }
            }
            catch (Exception ex) { Main.main.DebugMessage(ex.Message + ""); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Файлы изображений|*.png;*.jpg;*.jpeg;*.bmp;*.tga" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Main.parames["Color.Image"] = textBox1.Text;
                LSFB.Wall = Image.FromFile(Main.parames["Color.Image"]);
            }
            catch { LSFB.Wall = null; }
        }
    }
}
