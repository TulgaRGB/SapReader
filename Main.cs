using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LS;
using System.Runtime.InteropServices;
using System.IO;
using System.Media;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.Drawing.Drawing2D;

namespace SapReader
{
    public partial class Main : LSFB.Window
    {
        public static Main main;
        public static List<string> pluginsSha = new List<string>();
        public static List<List<string>> history = new List<List<string>>();
        public static string owner = Environment.UserName;
        public static Dictionary<string, string> parames = new Dictionary<string, string>
        {
            {"Bool.AutoConnect","False" },
            {"Bool.AutoStart","False" },
            {"Bool.AllowScript","False"},
            {"Bool.AllowPluginConnection","False" },
            {"Bool.DontAskForScript","False"},
            {"Bool.MaximizeOnStart","False"},
            {"Bool.UseNotValidPlugins","False"},
            {"Bool.UsePluginsNoSha","False" },
            {"Color.BackColor","2E2E30"},
            {"Color.ForeColor","FFFFFF"},
            {"Color.Image","" },
            {"Pro.Login",""},
            {"Pro.Pass",""},
            {"Pro.Custom",""},
            {"Pro.Ip",""},
            {"Browser.Formats","" }
        };
        public static Cyan fc;
        public Dictionary<TabPage, FastLua> Pages = new Dictionary<TabPage, FastLua>();
        public Dictionary<TabPage, DirectoryInfo> Directories = new Dictionary<TabPage, DirectoryInfo>();
        public TabPage page { get { return pages.SelectedTab; } }
        public readonly string nazvanie = "SapReader" + FirstTwo(Application.ProductVersion);
        LSFB lsfb;
        private FastLua pomoika = new FastLua(null);
        public FastLua flua { get { if (page != null) return Pages[page]; else return pomoika; } set { Pages.Add(page, value); } }
        public Main(bool first = true)
        {
            InitializeComponent();
            fc = new Cyan(ParseResponse);
            fc.OnDisconnect += () =>
            {
                conLabel.Text = "Нет соединения";
            };
            fc.OnConnect += () =>
            {
                conLabel.Text = "Вход не выполнен";
            };
            ReloadAllParams();
            if (first)
            {
                FastLua.InitThis = new Type[] { typeof(LSFB), typeof(Sapphire), typeof(Main) };
                try
                {
                    LSFB.Wall = Image.FromFile(parames["Color.Image"]);
                }
                catch(Exception ex) { Log.Write(ex); }
                LSFB.MainForm = this; main = this;
            }
            Size = new Size(Screen.PrimaryScreen.WorkingArea.Width / 2, Screen.PrimaryScreen.WorkingArea.Height / 2);
            lsfb = LSFB.AddLSFB(this, first ? 4 : 3, 24, autoscroll: true, help: (object sender, EventArgs e) => { new About("SapphireReader"); });
            Text = nazvanie;
            mm.Renderer = new LSFB.MyRenderer() { Transparent = true };
            cms.Renderer = new LSFB.MyRenderer();
            conLabel.SendToBack();
            NewTab();
            pages.Dock = DockStyle.Fill;
            flua.DoString(Encoding.UTF8.GetString(Properties.Resources.HOME));
            if (parames["Bool.MaximizeOnStart"] == "True")
                WindowState = FormWindowState.Maximized;
            if (parames.ContainsKey("Auto.Size"))
            {
                string[] sizeParts = parames["Auto.Size"].Split(':');
                int height = int.Parse(sizeParts[0]);
                int width = int.Parse(sizeParts[1]);
                Size = new Size(height, width);
            }
            if (parames.ContainsKey("Auto.Plugs"))
                pluginsSha = parames["Auto.Plugs"].Split('|').ToList();

            if (!parames.ContainsKey("Auto.NewSapphire"))
                parames["Auto.NewSapphire"] = "False";
            if (parames.ContainsKey("Auto.NewSapphire"))
                if (parames["Auto.NewSapphire"] == "True")
                {
                    sapphire2ToolStripMenuItem.Checked = false;
                    sapphire16ToolStripMenuItem.Checked = true;
                }

            if (!parames.ContainsKey("Auto.NewHash"))
                parames["Auto.NewHash"] = "False";
            if (parames.ContainsKey("Auto.NewHash"))
                if (parames["Auto.NewHash"] == "True")
                {
                    mD5ToolStripMenuItem.Checked = false;
                    sHA512ToolStripMenuItem.Checked = true;
                }
            tray.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            UpdatePlugs();
            if (!first)
                mm.Items.Remove(proToolStripMenuItem);
            TabStyleProvider tmp = new LSTabStyleProvider(pages);
            pages.DisplayStyleProvider = tmp;

            if (drag == null)
            {
                drag = new Drag(this);
                DragOver += drag.Main_DragOver;
            }

            pages.SelectedIndexChanged += Pages_SelectedIndexChanged;
        }

        private void Pages_SelectedIndexChanged(object sender, EventArgs e)
        {
            At.Single.ClearAttrs();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Application.DoEvents();
            if (parames["Bool.AutoConnect"] == "True")
                if (Main.fc.Key == null)
                    proToolStripMenuItem_Click(null, null);
        }
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
        }
        public static void Pro(FastLua th, string message, string responseFunc = null)
        {
            bool granted = parames["Bool.AllowPluginConnection"] == "True";
            if (!granted)
                granted = MessageBox.Show("Выполнить запрос на сервер:\n" + message + "?", th.Name, MessageBoxButtons.YesNo) == DialogResult.Yes;
            if (granted)
                if (responseFunc == null)
                    fc.ClientSend(message);
                else
                    fc.Request(message, new Action<string>((response) =>
                    {
                        NLua.LuaFunction func = th.GetFunction(responseFunc);
                        if (func != null)
                            func.Call(response);
                    }));
        }
        #region pages
        public class LSTabStyleProvider : TabStyleAngledProvider
        {
            //#007ACC
            private Color cc { get { return tabControl.Parent.BackColor; } }
            private Color fc { get { return tabControl.FindForm().BackColor; } }
            public CustomTabControl tabControl;
            public LSTabStyleProvider(CustomTabControl tabControl) : base(tabControl)
            {
                this.tabControl = tabControl;
                ShowTabCloser = true;
                Invalidate();
                tabControl.FindForm().BackColorChanged += (object sender, EventArgs e) =>
                {
                    Invalidate();
                };
                tabControl.FindForm().ForeColorChanged += (object sender, EventArgs e) =>
                {
                    Invalidate();
                };
            }
            public void Invalidate()
            {
                CloserColor = tabControl.Parent.ForeColor;
                TextColorSelected = tabControl.Parent.ForeColor;
                CloserColorActive = LSFB.FromHex("E04343");
                TextColor = LSFB.Colorize(TextColorSelected);
                BorderColorHot = LSFB.Colorize(tabControl.Parent.ForeColor);
                BorderColorSelected = tabControl.Parent.ForeColor;
                BorderColor = fc;
                foreach (TabPage tp in tabControl.TabPages)
                {
                    tp.ForeColor = tabControl.FindForm().ForeColor;
                    tp.BackColor = cc;
                }
            }
            protected override Brush GetTabBackgroundBrush(int index)
            {
                LinearGradientBrush fillBrush = null;
                Color dark = cc;
                Color light = fc;
                Rectangle tabBounds = this.GetTabRect(index);
                fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical);
                return fillBrush;
            }
            public override void AddTabBorder(GraphicsPath path, Rectangle tabBounds)
            {
                path.AddLine(tabBounds.X, tabBounds.Y + tabBounds.Height, tabBounds.X, tabBounds.Y);
                path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.X + tabBounds.Width - 15, tabBounds.Y);
                path.AddLine(tabBounds.X + tabBounds.Width - 15, tabBounds.Y, tabBounds.X + tabBounds.Width, tabBounds.Y + tabBounds.Height);
                path.AddLine(tabBounds.X, tabBounds.Y + tabBounds.Height, tabBounds.X + tabBounds.Width, tabBounds.Y + tabBounds.Height);
            }
        }
        public void NewTab()
        {
            if (pages.TabCount == 0 || page.Controls.Count != 0)
            {
                TabPage n = new TabPage { Text = "Новая вкладка" };
                pages.TabPages.Add(n);
                n.BackColor = lsfb.work.BackColor;
                n.ForeColor = ForeColor;
                pages.SelectedTab = n;
                flua = new FastLua(page);
            }
        }
        #endregion
        #region ParseResponse
        public void ParseResponse(string message)
        {
            XmlDocument _response = new XmlDocument();
            _response.LoadXml(message);
            XmlNode response = _response.FirstChild;
            if (response.Attributes.GetNamedItem("type") != null)
                switch (response.Attributes.GetNamedItem("type").InnerText)
                {
                    case "comments":
                        MessageBox.Show(response.InnerXml);
                        break;
                    case "news":
                        page.Text = "Новости";
                        LSDB newsDB = new LSDB();
                        newsDB.LoadXml(response.InnerXml);
                        LSDB.Table newsTable = newsDB.SelectTable("news");
                        if (newsTable != null)
                        {
                            foreach (LSDB.Table.Row r in newsTable.Values)
                                page.Controls.Add(new News
                                   (
                                       r.SelectField("id"),
                                       r.SelectField("title"),
                                       r.SelectField("time"),
                                       r.SelectField("comments"),
                                       r.SelectField("author"),
                                       r.SelectField("text"),
                                       page.ForeColor
                                   )
                                {
                                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                                    Size = new Size(page.Width - 24, 150),
                                    Location = new Point(12, page.Controls.Count == 0 ? 12 : (page.Controls[page.Controls.Count - 1].Top + page.Controls[page.Controls.Count - 1].Height + 12))
                                }
                                       );
                        }
                        break;
                    case "chat":
                        if (page.Controls.Find("box", false).Last() != null)
                        {
                            page.Controls.Find("box", false).Last().Text += response.InnerText + "\n";
                        }
                        else
                        {
                            page.Controls.Clear();
                            flua.DoString(File.ReadAllText("CHAT.lua"));
                            page.Controls.Find("send", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                fc.ClientSend("<REQUEST type=\"chat\">" + page.Controls.Find("ent", false).Last().Text + "</REQUEST>");
                                page.Controls.Find("ent", false).Last().Text = "";
                            };
                        }
                        break;
                    case "persData":
                        page.Text = "Смена данных";
                        flua.DoString(Encoding.UTF8.GetString(Properties.Resources.DATA));
                        page.Controls.Find("error", false).Last().Text = response.InnerText;
                        string oldpass = null;
                        string newpass = null;
                        page.Controls.Find("save", false).Last().Click += (object sender, EventArgs e) =>
                        {
                            page.Controls.Find("error", false).Last().Text = "";
                            if (page.Controls.Find("newPass", false).Last().Text != "")
                            {
                                if (page.Controls.Find("oldPass", false).Last().Text != page.Controls.Find("newPass", false).Last().Text)
                                {
                                    if (page.Controls.Find("checkPass", false).Last().Text == page.Controls.Find("newPass", false).Last().Text)
                                    {
                                        oldpass = " oldPass =\"" + Sapphire.GetMd5Hash(page.Controls.Find("oldPass", false).Last().Text) + "\" ";
                                        newpass = "newPass=\"" + Sapphire.GetMd5Hash(page.Controls.Find("newPass", false).Last().Text) + "\"";
                                    }
                                    else
                                        page.Controls.Find("error", false).Last().Text = "Новые пароли должны совпадать!";
                                }
                                else
                                    page.Controls.Find("error", false).Last().Text = "Новый пароль должен отличаться от старого!";
                            }
                            if (page.Controls.Find("error", false).Last().Text == "")
                            {
                                fc.ClientSend("<REQUEST type=\"persData\" newName=\"" + page.Controls.Find("newName", false).Last().Text + "\"" + oldpass + newpass + "/>");
                                page.Controls.Clear();
                            }
                        };
                        if (response.Attributes.GetNamedItem("newName") != null)
                            conLabel.Text = response.Attributes.GetNamedItem("newName").InnerText;
                        break;
                    case "box":
                        LSFB.Show(response.InnerText, "Pro");
                        break;
                    case "error":
                        DebugMessage(response.InnerText, "Pro - Ошибка!");
                        break;
                    case "message":
                        DebugMessage(response.InnerText, "Pro");
                        break;
                    case "exit":
                        if (response.Attributes.GetNamedItem("result").InnerText == "succ")
                        {
                            conLabel.Text = "Вход не выполнен";
                            page.Controls.Clear();
                            page.Text = "Новая вкладка";
                        }
                        break;
                    case "form":
                        if (response.Attributes.GetNamedItem("pro") != null)
                        {
                            page.Controls.Clear();
                            flua.DoString(response.InnerText.Replace("lt;", "<").Replace("gt;", ">"));
                        }
                        else
                            try
                            {
                                new Plugy(response.InnerText.Replace("lt;", "<").Replace("gt;", ">"));
                            }
                            catch (Exception ex) { DebugMessage(ex.Message); Log.Write(ex); }
                        break;
                    case "forms":
                        LSDB formsDB = new LSDB();
                        formsDB.LoadXml(response.InnerXml);
                        LSDB.Table forms = formsDB.SelectTable("forms");
                        if (forms != null)
                        {
                            ListView temp = new ListView();
                            temp.Resize += (object sender, EventArgs e) =>
                            {
                                foreach (ColumnHeader c in temp.Columns)
                                {
                                    c.Width = temp.Width / 4;
                                }
                            };
                            temp.MouseDoubleClick += (object sender, MouseEventArgs e) =>
                            {
                                if (e.Button == MouseButtons.Left)
                                {
                                    fc.ClientSend("<REQUEST type=\"form\" id=\"" + temp.SelectedItems[0].Text + "\"/>");
                                }
                            };
                            temp.View = View.Details;
                            temp.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom);
                            temp.Size = page.Size;
                            page.Controls.Add(temp);
                            temp.Columns.Add("ID", temp.Width / 4);
                            temp.Columns.Add("Название", temp.Width / 4);
                            temp.Columns.Add("Автор", temp.Width / 4);
                            temp.Columns.Add("Проверенно", temp.Width / 4 - 10);
                            foreach (LSDB.Table.Row f in forms.Values)
                            {
                                ListViewItem item = new ListViewItem(new[]
                                {
                                    f.GetId(),
                                    f.SelectField("name"),
                                    f.SelectField("author"),
                                    f.SelectField("validated")!= "False"? "ДА" : "НЕТ"
                            });
                                temp.Items.Add(item);
                            }
                        }
                        break;
                    case "login":
                        if (response.Attributes.GetNamedItem("result").InnerText == "succ")
                        {
                            page.Controls.Clear();
                            page.Text = "Pro";
                            conLabel.Text = response.Attributes.GetNamedItem("user").InnerText;
                            flua.DoString(Encoding.UTF8.GetString(Properties.Resources.PRO));
                            page.Controls.Find("hi", false).Last().Text += ", " + conLabel.Text + "!";
                            page.Controls.Find("libPlugin", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                page.Controls.Clear();
                                fc.ClientSend(@"
<QUERY return='forms'>
        <SELECT FROM='forms' ORDERBY='+name'>
            <WHERE Field='validated' IS='True" + (parames["Bool.UseNotValidPlugins"] == "True" ? "|False" : "") + @"'/>
        </SELECT>
    </QUERY>");
                            };
                            page.Controls.Find("checkApp", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                fc.ClientSend("<REQUEST type=\"sum\" hash=\"" + Sapphire.GetMd5HashBytes(File.ReadAllBytes(System.Reflection.Assembly.GetEntryAssembly().Location)) + "\"/>");
                            };
                            page.Controls.Find("exitButton", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                fc.ClientSend("<REQUEST type=\"exit\"/>");
                            };
                            page.Controls.Find("persData", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                page.Controls.Clear();
                                fc.ClientSend("<REQUEST type=\"persData\"/>");
                            };
                            page.Controls.Find("joinChat", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                page.Controls.Clear();
                                fc.ClientSend("<REQUEST type=\"chat\"/>");
                            }; page.Controls.Find("news", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                page.Controls.Clear();
                                fc.ClientSend(@"
    <QUERY return='news'>
        <SELECT FROM='news' ORDERBY='-time'/>
    </QUERY>");
                            };
                        }
                        else
                            if (response.Attributes.GetNamedItem("result").InnerText == "request")
                        {
                            if (parames["Pro.Pass"] != "")
                                fc.ClientSend("<REQUEST type=\"login\" login=\"" + parames["Pro.Login"] + "\" pass=\"" + Sapphire.GetMd5Hash(parames["Pro.Pass"]) + "\" />");
                        }
                        else
                        {
                            Login tmp = new Login(parames["Pro.Login"], parames["Pro.Pass"], response.InnerText);
                            if (!tmp.cancel)
                            {
                                parames["Pro.Login"] = tmp.textBox1.Text;
                                parames["Pro.Pass"] = tmp.pass.Text;
                                fc.ClientSend("<REQUEST type=\"login\" login=\"" + tmp.textBox1.Text + "\" pass=\"" + Sapphire.GetMd5Hash(tmp.pass.Text) + "\" />");
                            }
                        }
                        break;
                }

        }
        #endregion
        public void ReloadAllParams()
        {
            Dictionary<string, string> tmp = LSFB.LoadParams("SapReader");
            foreach (KeyValuePair<string, string> t in tmp)
                parames[t.Key] = t.Value;
            BackColor = LSFB.FromHex(parames["Color.BackColor"] + "ffffff");
            ForeColor = LSFB.FromHex(parames["Color.ForeColor"] + "000000");
        }
        #region menu options
        public void MainScr()
        {
            nowDir = null;
            page.Text = "Устройства и диски";
            ListView browser = (ListView)page.Controls.Find("browser", false).First();
            browser.Items.Clear();
            browser.LargeImageList = new ImageList();
            browser.LargeImageList.ImageSize = new Size(32, 32);
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                browser.LargeImageList.Images.Add(Properties.Resources.Folder);
                browser.Items.Add(d.Name.Replace(@"\", ""), browser.LargeImageList.Images.Count - 1);
            }
            browser.Show();
        }
        public void UpdatePlugs(string way = "Plugins", ToolStripItemCollection c = null)
        {
            if (Directory.Exists(way))
            {
                bool empty = true;
                if (c == null)
                {
                    c = инструментыToolStripMenuItem.DropDownItems;
                    c.Clear();
                    c.AddRange(new ToolStripItem[]
                    {
                        главнаяСтраницаToolStripMenuItem,
                        проводникToolStripMenuItem1,
                        toolStripSeparator3,
                        добавитьToolStripMenuItem,
                        настроитьToolStripMenuItem,
                        обновитьToolStripMenuItem2,
                        toolStripSeparator9 });
                }
                else
                    c.Clear();
                foreach (string d in Directory.GetDirectories(way))
                {
                    ToolStripMenuItem temp = new ToolStripMenuItem { Text = new DirectoryInfo(d).Name, Tag = "Plugin" };
                    UpdatePlugs(d, temp.DropDownItems);
                    c.Add(temp);
                    empty = false;
                }
                foreach (string d in Directory.GetFiles(way))
                {
                    ToolStripMenuItem temp = new ToolStripMenuItem { Text = new DirectoryInfo(d).Name, Tag = "Plugin" };
                    string xml = File.ReadAllText(d);
                    temp.Click += (object sender, EventArgs e) =>
                    {
                        if (!pluginsSha.Contains(Sapphire.GetSha512(File.ReadAllBytes(d))) && parames["Bool.UsePluginsNoSha"] != "True")
                        {
                            DebugMessage("Плагин " + d + " был изменён, проверьте код и добавьте его заного");
                            new Plugy(File.ReadAllText(d));
                        }
                        else
                            try
                            {
                                Dictionary<string, string> tmp = LSFB.GetInfo(Plugy.ExtractFromString(xml, "([[", "]])").First());
                                bool doo = tmp.ContainsKey("window") ? tmp["window"] == "True" ? false : true : true;
                                if (doo)
                                {
                                    page.Controls.Clear();
                                }
                                flua.DoString(xml);
                                if (doo)
                                    page.Text = flua.Name;
                            }
                            catch (Exception ex) { DebugMessage(ex.Message); }
                    };
                    c.Add(temp);
                    empty = false;
                }
                if (empty)
                    c.Add(new ToolStripMenuItem { Text = "(Пусто)", Enabled = false });
            }
        }
        public DirectoryInfo nowDir { get { if (Directories.ContainsKey(page)) return Directories[page]; else return null; } set { Directories[page] = value; } }
        public void Go(string way)
        {
            DirectoryInfo bu = nowDir;
            ListView browser = (ListView)page.Controls.Find("browser", false).First();
            try
            {
                nowDir = new DirectoryInfo(way);
                page.Text = way;
                browser.Items.Clear();
                browser.LargeImageList = new ImageList();
                browser.LargeImageList.ImageSize = new Size(32, 32);
                foreach (string d in Directory.GetDirectories(way))
                {
                    browser.LargeImageList.Images.Add(Properties.Resources.Folder);
                    browser.Items.Add(new DirectoryInfo(d).Name, browser.LargeImageList.Images.Count - 1);
                }
                foreach (string d in Directory.GetFiles(way))
                {
                    browser.LargeImageList.Images.Add(Icon.ExtractAssociatedIcon(d));
                    browser.Items.Add(new DirectoryInfo(d).Name, browser.LargeImageList.Images.Count - 1);
                }
                browser.Show();
            }
            catch (Exception e)
            {
                nowDir = bu;
                browser.Items.Clear();
                browser.LargeImageList = new ImageList();
                browser.LargeImageList.Images.Add(SystemIcons.Error);
                browser.Items.Add(e.Message, 0);
            }
        }
        public RichTextBox BoxToWrite(string startText = null, string posibleWay = null)
        {
            page.Controls.Clear();
            Panel basa = new Panel
            {
                Dock = DockStyle.Fill,
            };
            page.Controls.Add(basa);
            RichTextBox file = new RichTextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                Name = "BoxToWrite",
                Width = basa.Width - 36,
                Height = basa.Height,
                Left = 36,
                AcceptsTab = true,
                BorderStyle = BorderStyle.None,
                BackColor = basa.BackColor,
                ForeColor = basa.ForeColor,
                WordWrap = false,
                Text = startText,
                Tag = posibleWay
            };
            LineNumbers.LineNumbers_For_RichTextBox numbers = new LineNumbers.LineNumbers_For_RichTextBox()
            {
                Height = file.Height,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom,
                AutoSize = false,
                Width = 24,
                ParentRichTextBox = file,
                Show_GridLines = false,
                Show_BorderLines = false,
                Show_MarginLines = false,
                Show_BackgroundGradient = false,
                LineNrs_Alignment = ContentAlignment.TopLeft

            };
            basa.Controls.Add(file);
            basa.Controls.Add(numbers);
            LSFB.AddCms(file);
            lsfb.MakeControlLikeWork(file);
            return file;
        }
        public bool CreateBrow()
        {
            if (page.Controls.Find("browser", false).Length != 1)
            {
                page.Controls.Clear();
                ListView browser = new ListView();
                browser.AllowDrop = true;
                browser.Name = "browser";
                browser.BackColor = page.BackColor;
                browser.ForeColor = ForeColor;
                browser.MouseDoubleClick += browser_MouseDoubleClick;
                browser.SelectedIndexChanged += Browser_SelectedIndexChanged;
                browser.ContextMenuStrip = cms;
                browser.ForeColor = ForeColor;
                browser.Dock = DockStyle.Fill;
                page.Controls.Add(browser);
                page.Text = "Проводник";
                lsfb.MakeControlLikeWork(browser);
                return true;
            }
            return false;
        }

        private void Browser_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView browser = (ListView)page.Controls.Find("browser", false).First();
            if (browser.SelectedItems.Count != 1)
                At.Single.ClearAttrs();
            else
                try
                {
                    At.Single.SetAttrs(nowDir + browser.SelectedItems[0].Text);
                }
                catch (Exception ex) { Log.Write(ex); };
        }
        #endregion
        public void MenuClick(string option)
        {
            MenuHandler(new ToolStripMenuItem { Tag = option }, null);
        }
        public void MenuHandler(object sender, EventArgs e)
        {
            try
            {
                ListView browser = null;
                try
                {
                    browser = (ListView)page.Controls.Find("browser", false).First();
                }
                catch { }
                ToolStripMenuItem s = null;
                ContextMenuStrip s2 = null;
                try
                {
                    s = (ToolStripMenuItem)sender;
                }
                catch { s2 = (ContextMenuStrip)sender; }
                switch (s != null ? s.Tag + "" : s2.Tag + "")
                {
                    #region Файл
                    #region New
                    case "NewText":
                        NewTab();
                        page.Text = "Безымянный.srtf";
                        RichTextBox file = BoxToWrite();
                        break;
                    case "NewLua":
                        NewTab();
                        page.Text = "Безымянный.lua";
                        file = BoxToWrite();
                        break;
                    #endregion
                    case "RunLua":
                        if (page.Text.Contains(".lua"))
                            DebugMessage(new NLua.Lua().DoString(page.Controls.Find("BoxToWrite", true).First().Text).First(), "NLua");
                        break;
                    case "RunPlugin":
                        if (page.Text.Contains(".lua"))
                        {
                            Form pl = new Form { StartPosition = FormStartPosition.CenterScreen };
                            LSFB ls = new LSFB(pl, 1);
                            FastLua fl = new FastLua(ls.work);
                            fl.DoString(page.Controls.Find("BoxToWrite", true).First().Text);
                            if (fl.OnWindow == false)
                                pl.Show();
                        }
                        break;
                    case "Sign":
                        if (page.Text.Contains(".lua"))
                        {
                            string hash = Sapphire.GetShaForPass(page.Controls.Find("BoxToWrite", true).First().Text);
                            string key = null;
                            if (LSFB.InputBox("Подписание плагина", "Введите ваш ключ:", ref key) == DialogResult.OK)
                            {
                                string sign = Sapphire.GetCode(hash, Sapphire.GetShaForPass(key));
                                string newSign = "";
                                for (int i = 0; i < 6; i++)
                                    newSign += sign.Substring(i * 50, 50) + Environment.NewLine;
                                newSign += sign.Substring(300);
                                LSFB.Show(newSign, "Подпись для плагина");
                            }
                        }
                        break;
                    case "FileAdd":
                        if (page.Text.Contains(".lua"))
                        {
                            new Plugy(page.Controls.Find("BoxToWrite", true).First().Text);
                        }
                        break;
                    case "FileOpen":
                        OpenFileDialog ofd = new OpenFileDialog { Filter = "Все файлы|*.*|Шифрованные текстовые файлы|*.srtf|Файлы плагинов|*.lua", Multiselect = true };
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            foreach (string fil in ofd.FileNames)
                            {
                                OpenFile(fil);
                            }
                        }
                        break;
                    case "FileSave":
                        if (page.Text.Contains("."))
                        {
                            string fil = page.Controls.Find("BoxToWrite", true).First().Tag + "";
                            string save = page.Controls.Find("BoxToWrite", true).First().Text;
                            if (fil.Split('.').Last() == "srtf")
                            {
                                string key = null;
                                if (LSFB.InputBox("Сохранить как " + new DirectoryInfo(fil).Name, "Введите ключ шифрования:", ref key) == DialogResult.OK)
                                {
                                    File.WriteAllBytes(fil, Sapphire.GetCodeBytes(Encoding.UTF8.GetBytes(save), key));
                                    page.Text = new DirectoryInfo(fil).Name;
                                }
                            }
                            else
                                File.WriteAllText(fil, save);
                        }
                        break;
                    case "FileSaveAss":
                        if (page.Text.Contains("."))
                        {
                            SaveFileDialog sfd = new SaveFileDialog { Filter = "Все файлы|*.*|Шифрованные текстовые файлы|*.srtf|Файлы плагинов|*.lua", FileName = page.Text };
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                string fil = sfd.FileName;
                                string save = page.Controls.Find("BoxToWrite", true).First().Text;
                                if (fil.Split('.').Last() == "srtf")
                                {
                                    string key = null;
                                    if (LSFB.InputBox("Сохранить как " + new DirectoryInfo(fil).Name, "Введите ключ шифрования:", ref key) == DialogResult.OK)
                                    {
                                        File.WriteAllBytes(fil, Sapphire.GetCodeBytes(Encoding.UTF8.GetBytes(save), key));
                                        page.Text = new DirectoryInfo(fil).Name;
                                    }
                                }
                                else
                                    File.WriteAllText(fil, save);
                                page.Controls.Find("BoxToWrite", true).First().Tag = fil;
                            }
                        }
                        break;
                    case "NewPage":
                        NewTab();
                        break;
                    case "NewWindow":
                        new Main(false).Show();
                        break;
                    #endregion
                    #region Проводник
                    case "MainScr":
                        CreateBrow();
                        MainScr();
                        break;
                    case "PluginsFolder":
                        CreateBrow();
                        Go(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Plugins\\");
                        break;
                    //separator
                    case "Root":
                        if (!CreateBrow())
                            Go(nowDir.Root + "");
                        break;
                    case "Up":
                        if (!CreateBrow())
                            if (nowDir + "" == nowDir.Root + "")
                                MainScr();
                            else
                                Go(nowDir.Parent.FullName + (nowDir.Parent.FullName.Last() != '\\' ? @"\" : ""));
                        break;
                    case "Refr":
                        if (!CreateBrow())
                            Go(nowDir.FullName);
                        break;
                    //separator
                    case "Open":
                        if (browser.SelectedItems.Count > 0)
                        {
                            foreach (ListViewItem i in browser.SelectedItems)
                            {
                                string fil = nowDir + i.Text;
                                System.Diagnostics.Process.Start(fil);
                            }
                        }
                        else
                            System.Diagnostics.Process.Start(nowDir + "");
                        break;
                    case "OpenHere":
                        if (browser.SelectedItems.Count > 0)
                        {
                            foreach (ListViewItem i in browser.SelectedItems)
                                if (!File.GetAttributes(nowDir + i.Text).HasFlag(FileAttributes.Directory))
                                {
                                    string fil = nowDir + i.Text;
                                    OpenFile(fil);
                                }
                        }
                        break;
                    //separator
                    case "CreateFile":
                        break;
                    case "CreateFold":
                        break;
                    case "CreateArch":
                        break;
                    case "Rename":
                        break;
                    case "Delete":
                        break;
                    //separator
                    case "Encrypt":
                        Encrypt(browser.SelectedItems, true);
                        break;
                    case "Decrypt":
                        Encrypt(browser.SelectedItems, false);
                        break;
                    case "Hash":
                        string type = parames["Auto.NewHash"] == "True" ? "SHA-512" : "MD5";
                        string outp = type + " выбранных файлов:";
                        foreach (ListViewItem item in browser.SelectedItems)
                        {
                            outp += Environment.NewLine + item.Text + " - ";
                            try
                            {
                                outp += parames["Auto.NewHash"] == "True" ? Sapphire.GetSha512(File.ReadAllBytes(nowDir + item.Text)) : Sapphire.GetMd5HashBytes(File.ReadAllBytes(nowDir + item.Text));
                            }
                            catch (Exception ex) { outp += ex.Message; }
                        }
                        LSFB.Show(outp, type);
                        break;
                    case "MD5":
                        parames["Auto.NewHash"] = "False";
                        mD5ToolStripMenuItem.Checked = true;
                        sHA512ToolStripMenuItem.Checked = false;
                        break;
                    case "SHA-512":
                        parames["Auto.NewHash"] = "True";
                        mD5ToolStripMenuItem.Checked = false;
                        sHA512ToolStripMenuItem.Checked = true;
                        break;
                    case "History":
                        flua.DoString(Encoding.UTF8.GetString(Properties.Resources.HISTORY));
                        break;
                    #endregion
                    #region Инструменты
                    case "Main":
                        page.Controls.Clear();
                        flua.DoString(Encoding.UTF8.GetString(Properties.Resources.HOME));
                        break;
                    case "Brow":
                        CreateBrow();
                        break;
                    case "Add":
                        System.Windows.Forms.OpenFileDialog od = new System.Windows.Forms.OpenFileDialog { Filter = "*.lua|*.lua" };
                        if (od.ShowDialog() == DialogResult.OK)
                        {
                            string xml = File.ReadAllText(od.FileName);
                            new Plugy(xml);
                        }
                        break;
                    case "Update":
                        UpdatePlugs();
                        break;
                    case "Fix":
                        new Props(3);
                        break;
                    #endregion
                    case "Props":
                        new Props();
                        break;
                    #region Pro
                    #endregion
                    case "InfoSum":
                        Clipboard.SetText(Sapphire.GetMd5HashBytes(File.ReadAllBytes(System.Reflection.Assembly.GetEntryAssembly().Location)));
                        DebugMessage("Хэш Сумма " + Clipboard.GetText() + " в буфере обмена", "Успешно!");
                        break;
                }
            }
            // try { }
            catch (Exception ex) { DebugMessage(ex.Message + ""); Log.Write(ex); }
        }
        public void DebugMessage(object Text, string header = "Ошибка!")
        {
            tray.ShowBalloonTip(1000, header, Text + "", ToolTipIcon.None);
            Log.Write(Text);
        }
        public void OpenFile(string fil)
        {
            NewTab();
            page.Text = Path.GetFileName(fil);
            if (fil.Split('.').Last() == "srtf")
            {
                string key = null;
                if (LSFB.InputBox("Открыть " + new DirectoryInfo(fil).Name, "Введите ключ шифрования:", ref key) == DialogResult.OK)
                {
                    BoxToWrite(Encoding.UTF8.GetString(Sapphire.GetTextBytes(File.ReadAllBytes(fil), key)), fil);
                }
            }
            else if (new List<string>() { "jpg","png","bmp" }.Contains(fil.Split('.').Last()))
            {
                page.BackgroundImage = Image.FromFile(fil);
                page.BackgroundImageLayout = ImageLayout.Center;
            }
            else
                BoxToWrite(File.ReadAllText(fil), fil);
        }
        public void Encrypt(ListView.SelectedListViewItemCollection files, bool encrypt)
        {
            if (files.Count > 0)
            {
                List<string> tmp = new List<string>();
                foreach (ListViewItem item in files)
                    tmp.Add(item.Text);
                Crypy prog = new Crypy(tmp, encrypt);
                prog.ShowDialog();
            }
        }
        public static string FirstTwo(string Text)
        {
            return Text.Split('.')[0] + "." + Text.Split('.')[1];
        }

        private void browser_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView browser = (ListView)page.Controls.Find("browser", false).First();
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    if (File.GetAttributes(nowDir + browser.SelectedItems[0].Text).HasFlag(FileAttributes.Directory))
                        Go(nowDir + browser.SelectedItems[0].Text + @"\");
                    else
                    {
                        string fil = nowDir + browser.SelectedItems[0].Text;

                        if (!parames["Browser.Formats"].Split('|').Contains(fil.Split('.').Last()))
                            System.Diagnostics.Process.Start(fil);
                        else
                        {
                            NewTab();
                            page.Text = Path.GetFileName(fil);
                            if (fil.Split('.').Last() == "srtf")
                            {
                                string key = null;
                                if (LSFB.InputBox("Открыть " + new DirectoryInfo(fil).Name, "Введите ключ шифрования:", ref key) == DialogResult.OK)
                                {
                                    BoxToWrite(Encoding.UTF8.GetString(Sapphire.GetTextBytes(File.ReadAllBytes(fil), key)), fil);
                                }
                            }
                            else
                                BoxToWrite(File.ReadAllText(fil), fil);
                        }
                    }
                }
                catch(Exception ex) { SystemSounds.Exclamation.Play(); Log.Write(ex); }
            }
        }
        public void AccessToWebService(string host)
        {
            new Task(() =>
            {
                TcpClient tcpClient = new TcpClient();
                try
                {
                    Invoke((MethodInvoker)delegate { proToolStripMenuItem.Enabled = false; });
                    tcpClient.Connect(host, 228);
                    Invoke((MethodInvoker)delegate { conLabel.Text = "Подключение"; });
                    Invoke((MethodInvoker)delegate
                    {
                        fc = new Cyan(ParseResponse);
                        fc.OnDisconnect += () =>
                        {
                            conLabel.Text = "Нет соединения";
                        };
                        fc.OnConnect += () =>
                        {
                            conLabel.Text = "Вход не выполнен";
                        };
                        fc.Connect(parames["Pro.Ip"], 228);
                    });
                }
                catch (Exception ex)
                {
                    DebugMessage("Не удалось подключится к серверу Pro"); Log.Write(ex);
                }
                Invoke((MethodInvoker)delegate { proToolStripMenuItem.Enabled = true; });
            }).Start();
        }
        private void proToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (page == null)
                NewTab();
            if (fc.Key == null)
            {
                if (parames["Pro.Custom"] != "True")
                    parames["Pro.Ip"] = Properties.Resources.Host;
                AccessToWebService(parames["Pro.Ip"]);
            }
            else
                fc.ClientSend("<REQUEST type=\"login\" login=\"" + parames["Pro.Login"] + "\" pass=\"\" />");
        }
        //work done
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parames["Auto.Size"] = string.Format("{0}:{1}", Width, Height);
            }
            parames["Auto.Plugs"] = String.Join("|", pluginsSha.Where(s => s != "").ToArray());
            LSFB.SaveParams("SapReader", parames.Where(s => s.Key.Split('.').First() == "Auto")
                        .ToDictionary(dict => dict.Key, dict => dict.Value));
            File.WriteAllText( DateTime.Now.Date +  ".log", Log.Read);
        }
        #region Ya Zaebalsya
        private void sapphire2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parames["Auto.NewSapphire"] = "False";
            sapphire16ToolStripMenuItem.Checked = false;
            sapphire2ToolStripMenuItem.Checked = true;
        }

        private void sapphire16ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parames["Auto.NewSapphire"] = "True";
            sapphire2ToolStripMenuItem.Checked = false;
            sapphire16ToolStripMenuItem.Checked = true;

        }
        private void тестыSapphireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About("Тесты Sapphire");
        }
        #endregion
        public Drag drag = null;
        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            NewTab();
            e.Effect = DragDropEffects.All;
            drag.Show();
            BringToFront();
            Activate();
        }

        private void Main_DragLeave(object sender, EventArgs e)
        {
            drag.Hide();
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            drag.Hide();

            if (drag.State != Drag.state.none)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                switch (drag.State)
                {

                    case Drag.state.open:
                        foreach (string fil in files)
                            try
                            {
                                if (!File.GetAttributes(fil).HasFlag(FileAttributes.Directory))
                                {
                                    OpenFile(fil);
                                }
                                else
                                {
                                    NewTab();
                                    page.Text = Path.GetDirectoryName(fil);
                                    CreateBrow();
                                    Go(fil + "\\");
                                }
                            }
                            catch (Exception ex) { DebugMessage(ex.Message); Log.Write(ex); }
                        break;
                    case Drag.state.encrypt:
                        List<string> tmp = new List<string>();

                        foreach (string s in files)
                            tmp.Add(Path.GetFileName(s));
                        new Crypy(tmp, true) { altDir = Path.GetDirectoryName(files.First()) + "\\" }.ShowDialog();
                        break;
                    case Drag.state.decrypt:
                        tmp = new List<string>();

                        foreach (string s in files)
                            tmp.Add(Path.GetFileName(s));
                        new Crypy(tmp, false) { altDir = Path.GetDirectoryName(files.First()) + "\\" }.ShowDialog();
                        break;
                    case Drag.state.decopen:
                        string key = null;
                        if (LSFB.InputBox("Расшифровать сюда","Введите ключ:", ref key) == DialogResult.OK)
                        {
                            try
                            {
                                string fil = files.First();
                                NewTab();
                                page.Text = Path.GetFileName(fil);
                                if (new List<string>() { "jpg", "png", "bmp" }.Contains(fil.Split('.').Last()))
                                {
                                    byte[] img = parames["Auto.NewSapphire"] == "True" ? Sapphire.NewGetText(File.ReadAllBytes(fil), key) : Sapphire.GetTextBytes(File.ReadAllBytes(fil), key);
                                    using (var ms = new MemoryStream(img))
                                        try
                                        {
                                            page.BackgroundImage = Image.FromStream(ms);
                                        }
                                        catch (Exception ex) { throw new Exception("Не удалось открыть изображение:\n" + ex.Message);  }
                                    page.BackgroundImageLayout = ImageLayout.Center;
                                }
                                else
                                    BoxToWrite(Encoding.UTF8.GetString(parames["Auto.NewSapphire"] == "True" ? Sapphire.NewGetText(File.ReadAllBytes(fil), key) : Sapphire.GetTextBytes(File.ReadAllBytes(fil), key)));
                            }
                            catch (Exception ex) { DebugMessage(ex.Message); Log.Write(ex); }
                            }
                        break;
                    case Drag.state.sum:
                        string type = parames["Auto.NewHash"] == "True" ? "SHA-512" : "MD5";
                        string outp = type + " выбранных файлов:";
                        foreach (string item in files)
                        {
                            outp += Environment.NewLine + Path.GetFileName(item) + " - ";
                            try
                            {
                                outp += parames["Auto.NewHash"] == "True" ? Sapphire.GetSha512(File.ReadAllBytes(item)) : Sapphire.GetMd5HashBytes(File.ReadAllBytes(item));
                            }
                            catch (Exception ex) { outp += ex.Message; Log.Write(ex); }
                        }
                        LSFB.Show(outp, type);
                        break;
                }
                drag.State = Drag.state.none;
            }
            else
                SystemSounds.Exclamation.Play();
        }

        private void показатьЛогToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Log.Read, "Системный лог");
        }

        private void атрибутыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            At.Single.Show();
            At.Single.BringToFront();
        }
    }
    public static class Lel
    {
        public static void ClientSend(this Cyan c, string text)
        {
            c.Request(text);
        }
    }
    public static class Log
    {
        public static string Read
        {
            get
            {
                return log;
            }
        }
        public static void Write(object text)
        {
            Write("[ИФНО] " + text);
        }
        private static string log = DateTime.Now + " [ИНФО] Sapreader успешно работает!";
        private static void Write(string text)
        {
            log += Environment.NewLine + DateTime.Now + " " + text;
        }
        public static void Write(Exception ex)
        {
            Write("[ОШИБ] " + ex.Message);
        }        
    }
}
