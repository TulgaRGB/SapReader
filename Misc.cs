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
    class News : Panel
    {
        public News(string id, string title,  string date, string comments)
        {
            BorderStyle = BorderStyle.FixedSingle;
            Label h1 = new Label
            {
                Location = new Point(12,12),
                Text = title,
                Font = LSFB.sizer(14),
                AutoSize = true
            };

            Label data = new Label
            {
                Location = new Point(12, h1.Top + h1.Height + 12),
                Text = date,
                Font = LSFB.sizer(8),
                AutoSize = true
            };

            Label com = new Label
            {
                Location = new Point(data.Width + 12, h1.Top + h1.Height + 12),
                Text = comments + " коментариев",
                Font = LSFB.sizer(8),
                AutoSize = true
            };
            Controls.AddRange(new Control[] {h1,data,com });
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowOnly;

            Button b = new Button
            {
                Location = new Point(data.Width + 12, data.Top + data.Height + 12),
                FlatStyle = FlatStyle.Flat,
                Text = "Подробнее...",
                Font = LSFB.sizer(8),
                AutoSize = true,
                Anchor = AnchorStyles.Right
            };
            Controls.Add(b);
            Height += 12;
            b.Click += (object sender, EventArgs e) =>
            {
                Main.main.ClientSend("<REQUEST type=\"news\" id=\"" + id + "\"/>");
            };
        }
    }
}
