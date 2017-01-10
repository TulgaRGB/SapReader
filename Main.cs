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
using System.Numerics;
using System.Net.Sockets;
using System.Drawing.Drawing2D;

namespace SapReader
{
    public partial class Main : Form
    {
        public static Main main;
        public static List<string> pluginsSha = new List<string>();
        public static List<List<string>> history = new List<List<string>>();
        public static string owner = Environment.UserName;
        public static Dictionary<string, string> parames = new Dictionary<string, string>
        {
            {"Bool.AllowScript",""},
            {"Bool.AllowPluginConnection","" },
            {"Bool.DontAskForScript",""},
            {"Bool.MaximizeOnStart",""},
            {"Bool.UseNotValidPlugins",""},
            {"Bool.UsePluginsNoSha","" },
            {"Color.BackColor",""},
            {"Color.ForeColor",""},
            {"Color.Image","" },
            {"Pro.Login",""},
            {"Pro.Pass",""},
            {"Pro.Custom",""},
            {"Pro.Ip",""},
            {"Browser.Formats","" }
        };
        public static FastConnection fc;
        public Dictionary<TabPage, FastLua> Pages = new Dictionary<TabPage, FastLua>();
        public TabPage page { get { return pages.SelectedTab; } }
        public readonly string nazvanie = "SapReader бета" + FirstTwo(Application.ProductVersion);
        LSFB lsfb;
        private FastLua pomoika = new FastLua(null);
        public FastLua flua { get { if (page != null) return Pages[page]; else return pomoika; } set { Pages.Add(page, value); } }
        public Main(bool first = true)
        {
            InitializeComponent();
            fc = new FastConnection(ParseResponse);
            fc.OnDisconnect += (object sender, NetConnection c) =>
            {
                conLabel.Text = "Нет соединения";
            };
            ReloadAllParams();
            if (first)
            {
                FastLua.InitThis = new Type[] { typeof(LSFB), typeof(Sapphire), typeof(Main) };
                try
                {
                    LSFB.Wall = Image.FromFile(parames["Color.Image"]);
                }
                catch { }
                LSFB.MainForm = this; main = this;
            }
            Size = new Size(Screen.PrimaryScreen.WorkingArea.Width / 2, Screen.PrimaryScreen.WorkingArea.Height / 2);
            lsfb = LSFB.AddLSFB(this, first ? 4 : 3, 24, autoscroll: true, help: (object sender, EventArgs e) => { new About("SapphireReader"); });
            Text = nazvanie;
            mm.Renderer = new LSFB.MyRenderer() { Transparent = true};
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
            tray.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            UpdatePlugs();
            if (!first)
                mm.Items.Remove(proToolStripMenuItem);
            TabStyleProvider tmp = new LSTabStyleProvider(pages);
            pages.DisplayStyleProvider = tmp;
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
                granted = MessageBox.Show("Выполнить запрос на сервер:\n" + message+"?", th.Name, MessageBoxButtons.YesNo) == DialogResult.Yes;
            if (granted)
                if (responseFunc == null)
                    fc.ClientSend(message);
                else
                    fc.Request(message, new Action<string>((response)=> 
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
            private Color cc = Color.White;
            private Color fc = Color.White;
            public LSTabStyleProvider(CustomTabControl tabControl) : base(tabControl)
            {
                ShowTabCloser = true;
                Invalidate(tabControl);
                tabControl.FindForm().BackColorChanged += (object sender, EventArgs e) =>
                {
                    Invalidate(tabControl);
                };
                tabControl.FindForm().ForeColorChanged += (object sender, EventArgs e) =>
                {
                    Invalidate(tabControl);
                };
            }
            public void Invalidate(CustomTabControl tabControl)
            {
                CloserColor = tabControl.Parent.ForeColor;
                TextColorSelected = tabControl.Parent.ForeColor;
                fc = tabControl.FindForm().BackColor;
                cc = tabControl.Parent.BackColor;
                CloserColorActive = LSFB.Colorize(CloserColor);                
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
                switch (this._TabControl.Alignment)
                {
                    case TabAlignment.Top:
                        fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical);
                        break;
                    case TabAlignment.Bottom:
                        fillBrush = new LinearGradientBrush(tabBounds, dark, light, LinearGradientMode.Vertical);
                        break;
                    case TabAlignment.Left:
                        fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Horizontal);
                        break;
                    case TabAlignment.Right:
                        fillBrush = new LinearGradientBrush(tabBounds, dark, light, LinearGradientMode.Horizontal);
                        break;
                }
                return fillBrush;
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
                    case "error":
                        DebugMessage(response.InnerText);
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
                            catch (Exception ex) { DebugMessage(ex.Message); }
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
        public DirectoryInfo nowDir = null;
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
                browser.Name = "browser";
                browser.BackColor = page.BackColor;
                browser.ForeColor = ForeColor;
                browser.MouseDoubleClick += browser_MouseDoubleClick;
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
        #endregion        
        public void MenuClick(string option)
        {
            MenuHandler(new ToolStripMenuItem {Tag = option },null);
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
                            pl.Show();
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
                                NewTab();
                                page.Text = Path.GetFileName(fil);
                                if (fil.Split('.').Last() == "srtf")
                                {
                                    string key = null;
                                    if (LSFB.InputBox("Открыть " + new DirectoryInfo(fil).Name, "Введите ключ шифрования:", ref key) == DialogResult.OK)
                                    {
                                        BoxToWrite(Encoding.UTF8.GetString(Sapphire.GetTextBytes(File.ReadAllBytes(fil), key)),fil);
                                    }
                                }
                                else
                                    BoxToWrite(File.ReadAllText(fil),fil);
                            }
                        }
                        break;
                    case "FileSave":
                        if (page.Text.Contains("."))
                        {
                            string fil = page.Controls.Find("BoxToWrite", true).First().Tag+"";
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
                        Go(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"\\Plugins\\");
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
                        if(browser.SelectedItems.Count > 0)
                        {
                            foreach(ListViewItem i in browser.SelectedItems)
                            {
                                string fil = nowDir + i.Text;
                                System.Diagnostics.Process.Start(fil);
                            }
                        }
                        break;
                    case "OpenHere":
                        if (browser.SelectedItems.Count > 0)
                        {
                            foreach (ListViewItem i in browser.SelectedItems)
                                if(!File.GetAttributes(nowDir + i.Text).HasFlag(FileAttributes.Directory))
                            {
                                string fil = nowDir + i.Text;
                                NewTab();
                                page.Text = Path.GetFileName(fil);
                                if (fil.Split('.').Last() == "srtf")
                                {
                                    string key = null;
                                    if (LSFB.InputBox("Открыть " + new DirectoryInfo(fil).Name, "Введите ключ шифрования:", ref key) == DialogResult.OK)
                                    {
                                        BoxToWrite(Encoding.UTF8.GetString(Sapphire.GetTextBytes(File.ReadAllBytes(fil), key)),fil);
                                    }
                                }
                                else
                                    BoxToWrite(File.ReadAllText(fil),fil);
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
                    case "MD5":
                        string outp = "MD5 выбранных файлов:";
                        foreach (ListViewItem item in browser.SelectedItems)
                        {
                            outp += Environment.NewLine + item.Text + " - ";
                            try
                            {
                                outp += Sapphire.GetMd5HashBytes(File.ReadAllBytes(nowDir + item.Text));
                            }
                            catch (Exception ex) { outp += ex.Message; }
                        }
                        MessageBox.Show(outp, "MD5");
                        break;
                    case "SHA-512":
                        outp = "SHA-512 выбранных файлов:";
                        foreach (ListViewItem item in browser.SelectedItems)
                        {
                            outp += Environment.NewLine + item.Text + " - ";
                            try
                            {
                                outp += Sapphire.GetSha512(File.ReadAllBytes(nowDir + item.Text));
                            }
                            catch (Exception ex) { outp += ex.Message; }
                        }
                        MessageBox.Show(outp, "SHA-512");
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
            catch (Exception ex) { DebugMessage(ex.Message + ""); }
        }
        public void DebugMessage(object Text, string header = "Ошибка!")
        {
            tray.ShowBalloonTip(1000, header, Text + "", ToolTipIcon.None);
        }
        public void Encrypt(ListView.SelectedListViewItemCollection files, bool encrypt)
        {
            if (files.Count > 0)
            {
                Crypy prog = new Crypy(files, encrypt);
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
                                    BoxToWrite(Encoding.UTF8.GetString(Sapphire.GetTextBytes(File.ReadAllBytes(fil), key)),fil);
                                }
                            }
                            else
                                BoxToWrite(File.ReadAllText(fil),fil);
                        }
                    }
                }
                catch { SystemSounds.Exclamation.Play(); }
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
                    DebugMessage("Попытка подключения к серверу Pro", "Pro");
                    tcpClient.Connect(host, 228);
                    Invoke((MethodInvoker)delegate { conLabel.Text = "Вход не выполнен"; });
                    Invoke((MethodInvoker)delegate { fc.Con(parames["Pro.Ip"], 228); });
                }
                catch
                {
                    DebugMessage("Не удалось подключится к серверу Pro");
                }
                Invoke((MethodInvoker)delegate { proToolStripMenuItem.Enabled = true; });
            }).Start();
        }
        private void proToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fc.key == null)
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
                parames["Auto.Size"] = string.Format("{0}:{1}", Width, Height);
            parames["Auto.Plugs"] = String.Join("|", pluginsSha.Where(s => s != "").ToArray());
            LSFB.SaveParams("SapReader", parames.Where(s => s.Key.Split('.').First() == "Auto")
                        .ToDictionary(dict => dict.Key, dict => dict.Value));
        }
    }
}
