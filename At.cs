using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SapReader
{
    public partial class At : Form
    {
        public At()
        {
            InitializeComponent();
            new LS.LSFB(this,2, 0,0, false);
        }
        private static At single = null;
        public static At Single
        {
            get
            {
                if (single == null)
                    single = new At();
                return single;
            }
        }
        public void ClearAttrs()
        {
            listView1.Items.Clear();
        }
        public void SetAttrs(string path)
        {
            ClearAttrs();
            string attr = File.GetAttributes(path).ToString();
            FileInfo info = new FileInfo(path);
            AddAttr("Вес", info.Length + " байт");
            AddAttr("Дата создания", info.CreationTime.ToShortDateString());
            AddAttr("Время создания", info.CreationTime.ToLongTimeString());
            AddAttr("Дата изменения", info.LastWriteTime.ToShortDateString());
            AddAttr("Время изменения", info.LastWriteTime.ToLongTimeString());
            AddAttr("Только для чтения", attr.Contains("ReadOnly") ? "Да" : "Нет");
            AddAttr("Скрытый", attr.Contains("Hidden") ? "Да" : "Нет");
            AddAttr("Системный", attr.Contains("System") ? "Да" : "Нет");
            AddAttr("Временный", attr.Contains("Temporary") ? "Да" : "Нет");
        }
        public void AddAttr(string key, string value)
        {
            listView1.Items.Add(new ListViewItem(new string[] { key, value }));
        }
    }
}
