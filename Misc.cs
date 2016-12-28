using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using LS;

namespace SapReader
{
    class News : Button
    {
        public News(string id, string title,  string date, string comments, string author, string text, Color ForeColor)
        {
            FlatStyle = FlatStyle.Flat;
            Paint += LSFB.DrawText(title,LSFB.sizer(22), ForeColor, new Point(12, 12));
            Paint += LSFB.DrawText(date, LSFB.sizer(8), ForeColor, new Point(12, 50));
            Paint += LSFB.DrawText("От " + author, LSFB.sizer(8), ForeColor, new Point(120, 50));
            Paint += LSFB.DrawText((comments != "" ? comments.Split('|').Length + "" : "нет") + " коментариев", LSFB.sizer(8), ForeColor, new Point(12,69));
            Paint += LSFB.DrawText(text, LSFB.sizer(12), ForeColor, new Point(12, 100));
            Click += (object sender, EventArgs e) =>
            {
                Control tmp = Parent;
                tmp.Controls.Clear();
                Main.main.flua.DoString(Encoding.UTF8.GetString(Properties.Resources.NEWS));
                LSFB.CbyName(tmp.Controls, "header").Text = title;
                LSFB.CbyName(tmp.Controls, "time").Text = date + " - " + author;
                LSFB.CbyName(tmp.Controls, "article").Text = text;
                Main.fc.ClientSend(@"
<QUERY return='comments'>
        <SELECT FROM='newsCom'>
			<WHERE Field='id' IS='" + comments + @"'/>
        </SELECT>
    </QUERY>");
            };
        }
    }
}
