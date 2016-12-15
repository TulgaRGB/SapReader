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
using Microsoft.Win32;
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
        public static Random RNG = new Random();
        static public BigInteger exp;
        static public BigInteger osn = Convert.ToInt32("3");
        static public BigInteger mosn = Convert.ToInt32("93563");
        static public BigInteger mosn2 = Convert.ToInt32("2147483647");
        public static int mysec = RNG.Next(10000, 100000);
        static string key = null;
        public static string owner = Environment.UserName;
        public static Dictionary<string, string> parames = new Dictionary<string, string>
        {
            {"Bool.AllowScript",""},
            {"Bool.DontAskForScript",""},
            {"Bool.MaximizeOnStart",""},
            {"Bool.UseNotValidPlugins",""},
            {"Bool.UsePluginsNoSha","" },
            {"Color.BackColor",""},
            {"Color.ForeColor",""},
            {"Pro.Login",""},
            {"Pro.Pass",""},
            {"Pro.Custom",""},
            {"Pro.Ip",""},
        };
        public readonly string nazvanie = "SapReader бета" + FirstTwo(Application.ProductVersion);
        LSFB lsfb;
        public static NetConnection client = new NetConnection { BufferSize = 8192 };
        public static FastLua flua;
        public Main(bool first = true)
        {
            InitializeComponent(); exp = Diff(osn, mysec, false); main = this;
            ReloadAllParams();
            if (first)
            {
                LSFB.MainForm = this;
            }
            Size = new Size(Screen.PrimaryScreen.WorkingArea.Width / 2, Screen.PrimaryScreen.WorkingArea.Height / 2);
            lsfb = LSFB.AddLSFB(this, first ? 4 : 3, 24, autoscroll: true, help: (object sender, EventArgs e) => { new About("SapphireReader"); });
            Text = nazvanie;
            mm.Renderer = new LSFB.MyRenderer();
            cms.Renderer = new LSFB.MyRenderer();
            conLabel.SendToBack();
            NewTab();
            flua = new FastLua(pages.SelectedTab);
            pages.Dock = DockStyle.Fill;
            flua.RegisterFunction("Send", this, GetType().GetMethod("ClientSend"));
            flua.DoString(Encoding.UTF8.GetString(Properties.Resources.HOME));
            if (flua.Name != null)
                pages.SelectedTab.Text = flua.Name;
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
        #region pages
        public class LSTabStyleProvider : TabStyleRoundedProvider
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
                BorderColorHot = tabControl.Parent.ForeColor;
                BorderColorSelected = fc;
                BorderColor = fc;
                foreach(TabPage tp in tabControl.TabPages)
               tp.BackColor =cc;                
            }
            protected override Brush GetTabBackgroundBrush(int index)
            {
                LinearGradientBrush fillBrush = null;
                Color dark = cc;
                Color light =fc;
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
            TabPage n = new TabPage {Text = "Новая вкладка" };
            pages.TabPages.Add(n);
            n.BackColor = lsfb.work.BackColor;
            n.ForeColor = ForeColor;
            pages.SelectedTab = n;
        }
        #endregion
        public static BigInteger Diff(BigInteger inp, int step, bool second)
        {
            return !second ? BigInteger.Pow(inp, step) % mosn : BigInteger.Pow((BigInteger.Pow(inp, step) % mosn), (int)osn) % mosn2;
        }
        #region ParseResponse
        public void ParseResponse(XmlDocument _response)
        {
            XmlNode response = _response.FirstChild;
            if (response.Attributes.GetNamedItem("type") != null)
                switch (response.Attributes.GetNamedItem("type").InnerText)
                {
                    case "comments":
                        MessageBox.Show(response.InnerXml);
                        break;
                    case "news":
                        pages.SelectedTab.Text = "Новости";
                        LSDB newsDB = new LSDB();
                        newsDB.LoadXml(response.InnerXml);
                        LSDB.Table newsTable = newsDB.SelectTable("news");
                        if (newsTable != null)
                        {
                            foreach (LSDB.Table.Row r in newsTable.Values)
                                pages.SelectedTab.Controls.Add(new News
                                   (
                                       r.SelectField("id"),
                                       r.SelectField("title"),
                                       r.SelectField("time"),
                                       r.SelectField("comments"),
                                       r.SelectField("author"),
                                       r.SelectField("text")
                                   )
                                {
                                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                                    Size = new Size(pages.SelectedTab.Width - 24, 150),
                                    Location = new Point(12, pages.SelectedTab.Controls.Count == 0 ? 12 : (pages.SelectedTab.Controls[pages.SelectedTab.Controls.Count - 1].Top + pages.SelectedTab.Controls[pages.SelectedTab.Controls.Count - 1].Height + 12))
                                }
                                       );
                        }
                        break;
                    case "chat":
                        if (pages.SelectedTab.Controls.Find("box", false).Last() != null)
                        {
                            pages.SelectedTab.Controls.Find("box", false).Last().Text += response.InnerText + "\n";
                        }
                        else
                        {
                            pages.SelectedTab.Controls.Clear();
                            flua.DoString(File.ReadAllText("CHAT.lua"));
                            pages.SelectedTab.Controls.Find("send", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                ClientSend("<REQUEST type=\"chat\">" + pages.SelectedTab.Controls.Find("ent", false).Last().Text + "</REQUEST>");
                                pages.SelectedTab.Controls.Find("ent", false).Last().Text = "";
                            };
                        }
                        break;
                    case "persData":
                        pages.SelectedTab.Text = "Смена данных";
                        flua.DoString(Encoding.UTF8.GetString(Properties.Resources.DATA));
                        pages.SelectedTab.Controls.Find("error", false).Last().Text = response.InnerText;
                        string oldpass = null;
                        string newpass = null;
                        pages.SelectedTab.Controls.Find("save", false).Last().Click += (object sender, EventArgs e) =>
                        {
                            pages.SelectedTab.Controls.Find("error", false).Last().Text = "";
                            if (pages.SelectedTab.Controls.Find("newPass", false).Last().Text != "")
                            {
                                if (pages.SelectedTab.Controls.Find("oldPass", false).Last().Text != pages.SelectedTab.Controls.Find("newPass", false).Last().Text)
                                {
                                    if (pages.SelectedTab.Controls.Find("checkPass", false).Last().Text == pages.SelectedTab.Controls.Find("newPass", false).Last().Text)
                                    {
                                        oldpass = " oldPass =\"" + Sapphire.GetMd5Hash(pages.SelectedTab.Controls.Find("oldPass", false).Last().Text) + "\" ";
                                        newpass = "newPass=\"" + Sapphire.GetMd5Hash(pages.SelectedTab.Controls.Find("newPass", false).Last().Text) + "\"";
                                    }
                                    else
                                        pages.SelectedTab.Controls.Find("error", false).Last().Text = "Новые пароли должны совпадать!";
                                }
                                else
                                    pages.SelectedTab.Controls.Find("error", false).Last().Text = "Новый пароль должен отличаться от старого!";
                            }
                            if (pages.SelectedTab.Controls.Find("error", false).Last().Text == "")
                            {
                                ClientSend("<REQUEST type=\"persData\" newName=\"" + pages.SelectedTab.Controls.Find("newName", false).Last().Text + "\"" + oldpass + newpass + "/>");
                                pages.SelectedTab.Controls.Clear();
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
                            pages.SelectedTab.Controls.Clear();
                        }
                        break;
                    case "form":
                        if (response.Attributes.GetNamedItem("pro") != null)
                        {
                            pages.SelectedTab.Controls.Clear();
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
                                    ClientSend("<REQUEST type=\"form\" id=\"" + temp.SelectedItems[0].Text + "\"/>");
                                }
                            };
                            temp.View = View.Details;
                            temp.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom);
                            temp.Size = pages.SelectedTab.Size;
                            pages.SelectedTab.Controls.Add(temp);
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
                            pages.SelectedTab.Controls.Clear();
                            pages.SelectedTab.Text = "Pro";
                            conLabel.Text = response.Attributes.GetNamedItem("user").InnerText;
                            flua.DoString(Encoding.UTF8.GetString(Properties.Resources.PRO));
                            pages.SelectedTab.Controls.Find("hi", false).Last().Text += ", " + conLabel.Text + "!";
                            pages.SelectedTab.Controls.Find("libPlugin", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                pages.SelectedTab.Controls.Clear();
                                ClientSend(@"
<REQUEST type='FQL' return-type='forms'>
    <QUERY>
        <SELECT FROM='forms' ORDERBY='+name'>
            <WHERE Field='validated' IS='True" + (parames["Bool.UseNotValidPlugins"] == "True" ? "|False" : "") + @"'/>
        </SELECT>
    </QUERY>
</REQUEST>");
                            };
                            pages.SelectedTab.Controls.Find("checkApp", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                ClientSend("<REQUEST type=\"sum\" hash=\"" + Sapphire.GetMd5HashBytes(File.ReadAllBytes(System.Reflection.Assembly.GetEntryAssembly().Location)) + "\"/>");
                            };
                            pages.SelectedTab.Controls.Find("exitButton", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                ClientSend("<REQUEST type=\"exit\"/>");
                            };
                            pages.SelectedTab.Controls.Find("persData", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                pages.SelectedTab.Controls.Clear();
                                ClientSend("<REQUEST type=\"persData\"/>");
                            };
                            pages.SelectedTab.Controls.Find("joinChat", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                pages.SelectedTab.Controls.Clear();
                                ClientSend("<REQUEST type=\"chat\"/>");
                            }; pages.SelectedTab.Controls.Find("news", false).Last().Click += (object sender, EventArgs e) =>
                            {
                                pages.SelectedTab.Controls.Clear();
                                ClientSend(@"
<REQUEST type='FQL' return-type='news'>
    <QUERY>
        <SELECT FROM='news' ORDERBY='-time'/>
    </QUERY>
</REQUEST>");
                            };
                        }
                        else
                            if (response.Attributes.GetNamedItem("result").InnerText == "request")
                        {
                            ClientSend("<REQUEST type=\"login\" login=\"" + parames["Pro.Login"] + "\" pass=\"" + Sapphire.GetMd5Hash(parames["Pro.Pass"]) + "\" />");
                        }
                        else
                        {
                            Login tmp = new Login(parames["Pro.Login"], parames["Pro.Pass"], response.InnerText);
                            if (!tmp.cancel)
                            {
                                parames["Pro.Login"] = tmp.textBox1.Text;
                                parames["Pro.Pass"] = tmp.pass.Text;
                                ClientSend("<REQUEST type=\"login\" login=\"" + tmp.textBox1.Text + "\" pass=\"" + Sapphire.GetMd5Hash(tmp.pass.Text) + "\" />");
                            }
                        }
                        break;
                }

        }
        #endregion
        public void ClientSend(string query)
        {
            if (key != null)
                try
                {
                    client.Send(Sapphire.GetCodeBytes(UnicodeEncoding.Unicode.GetBytes(query), key));
                }
                catch (Exception ex)
                {
                    DebugMessage("Запрос не был отправлен:\n" + ex.Message);
                }
        }
        #region connection
        delegate void ConDel();
        void Con()
        {
            if (InvokeRequired)
                Invoke(new ConDel(Con));
            else
            {
                try
                {
                    try
                    {
                        client.Disconnect();
                    }
                    catch { }
                    IPAddress ip;
                    bool _ip = IPAddress.TryParse(parames["Pro.Ip"], out ip);
                    if (_ip)
                        client.Connect(ip, 228);
                    else
                        client.Connect(parames["Pro.Ip"], 228);
                    client.OnDisconnect += (object sender1, NetConnection c) =>
                        {
                            key = null;
                            conLabel.Text = "";
                            DebugMessage("Потеряно соединение с сервером!");
                        };
                    client.OnDataReceived += (object sender1, NetConnection c, byte[] b) =>
                        {
                            if (key != null)
                            {
                                XmlDocument _response = new XmlDocument();
                                _response.LoadXml(UnicodeEncoding.Unicode.GetString(Sapphire.GetTextBytes(b, key)));
                                ParseResponse(_response);
                            }
                            else
                            {
                                client.Send(UnicodeEncoding.Unicode.GetBytes(exp + ""));
                                key = Sapphire.GetMd5Hash(Diff(Convert.ToInt32(UnicodeEncoding.Unicode.GetString(b)), mysec, true) + "");
                            }
                        };
                }
                //try{ }
                catch (Exception ex)
                {
                    DebugMessage(ex.Message + "");
                }
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
            pages.SelectedTab.Text = "Устройства и диски";
            ListView browser = (ListView)pages.SelectedTab.Controls.Find("browser", false).First();
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
                                    pages.SelectedTab.Controls.Clear();
                                }
                                flua.DoString(xml);
                                if (doo)
                                    pages.SelectedTab.Text = flua.Name;
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
            ListView browser = (ListView)pages.SelectedTab.Controls.Find("browser", false).First();
            try
            {
                nowDir = new DirectoryInfo(way);
                pages.SelectedTab.Text = way;
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
        #endregion        
        public void MenuHandler(object sender, EventArgs e)
        {
            try
            {
                ListView browser = null;
                try
                {
                    browser = (ListView)pages.SelectedTab.Controls.Find("browser", false).First();
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
                    case "NewPage":
                        NewTab();
                        break;
                    case "NewWindow":
                        new Main(false).Show();
                        break;
                    #endregion
                    #region Проводник
                    case "MainScr":
                        MainScr();
                        break;
                    //separator
                    case "Root":
                        Go(nowDir.Root + "");
                        break;
                    case "Up":
                        if (nowDir + "" == nowDir.Root + "")
                            MainScr();
                        else
                            Go(nowDir.Parent.FullName + (nowDir.Parent.FullName.Last() != '\\' ? @"\" : ""));
                        break;
                    case "Refr":
                        Go(nowDir.FullName);
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
                    #endregion
                    #region Инструменты
                    case "Main":
                        browser.Hide();
                        pages.SelectedTab.Controls.Clear();
                        flua.DoString(Encoding.UTF8.GetString(Properties.Resources.HOME));
                        break;
                    case "Brow":
                        pages.SelectedTab.Controls.Clear();
                        browser = new ListView();
                        browser.Name = "browser";
                        browser.BackColor = pages.SelectedTab.BackColor;
                        browser.ForeColor = ForeColor;
                        browser.MouseDoubleClick += browser_MouseDoubleClick;
                        browser.ContextMenuStrip = cms;
                        browser.ForeColor = ForeColor;
                        browser.Dock = DockStyle.Fill;
                        pages.SelectedTab.Controls.Add(browser);
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
            //  try { }
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
            ListView browser = (ListView)pages.SelectedTab.Controls.Find("browser", false).First();
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    if (File.GetAttributes(nowDir + browser.SelectedItems[0].Text).HasFlag(FileAttributes.Directory))
                        Go(nowDir + browser.SelectedItems[0].Text + @"\");
                    else
                        System.Diagnostics.Process.Start(nowDir + browser.SelectedItems[0].Text);
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
                    client = new NetConnection { BufferSize = 8192 };
                    Invoke((MethodInvoker)delegate { conLabel.Text = "Вход не выполнен"; });
                    Con();
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
            if (key == null)
            {
                if (parames["Pro.Custom"] != "True")
                    parames["Pro.Ip"] = Properties.Resources.Host;
                AccessToWebService(parames["Pro.Ip"]);
            }
            else
                ClientSend("<REQUEST type=\"login\" login=\"" + parames["Pro.Login"] + "\" pass=\"\" />");
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

        private void pages_SelectedIndexChanged(object sender, EventArgs e)
        {
            flua.Form = pages.SelectedTab;
        }
    }
}
