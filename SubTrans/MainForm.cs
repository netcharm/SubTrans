using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubTitles
{
    public partial class MainForm : Form
    {
        ASS ass = new ASS();

        private void InitListView(ListView lv, string[] headers)
        {
            //var headers = new string[] { "Format", "Layer", "Start", "End", "Style", "Name", "MarginL", "MarginR", "MarginV", "Effect", "Text" };
            lv.Columns.Clear();
            if(headers != null)
            {
                var col = new ColumnHeader();
                col.Text = "ID";
                col.TextAlign = HorizontalAlignment.Right;
                col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                lv.Columns.Add(col);

                foreach (string header in headers)
                {
                    col = new ColumnHeader();
                    col.Text = header;
                    col.TextAlign = HorizontalAlignment.Left;
                    col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                    lv.Columns.Add(col);
                }

                col = new ColumnHeader();
                col.Text = "Translated";
                col.TextAlign = HorizontalAlignment.Left;
                col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                lv.Columns.Add(col);

                lv.VirtualMode = true;
                lv.VirtualListSize = 0;
            }
            else
            {
                lv.VirtualMode = false;
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitListView(lvItems, null);
        }

        private void lvItems_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var evt = ass.Events[e.ItemIndex];
            ListViewItem lvi = new ListViewItem();
            //lvi.SubItems.Add(evt.Text);
            lvi.Text = $"{e.ItemIndex + 1}";
            for (int i=0; i< ass.EventFields.Length;i++)
            {
                lvi.SubItems.Add(evt.Field(ass.EventFields[i]));
            }
            lvi.SubItems.Add(evt.Translated);

            e.Item = lvi;
        }

        private void lvItems_DrawItem(object sender, DrawListViewItemEventArgs e)
        {

        }

        private void lvItems_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.DefaultExt = ".ass";
            dlgOpen.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|Text File|*.txt|All File|*.*";
            dlgOpen.FilterIndex = 0;
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                ass.Load(dlgOpen.FileName);
                InitListView(lvItems, ass.EventFields);
                lvItems.VirtualListSize = ass.Events.Count();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (lvItems.Items.Count <= 0) return;
            if (lvItems.SelectedIndices.Count <= 0) return;

            StringBuilder sb = new StringBuilder();
            for (int i=0; i< lvItems.SelectedIndices.Count; i++)
            {
                var evt = ass.Events[lvItems.SelectedIndices[i]];
                sb.AppendLine(evt.Text);
            }
            Clipboard.SetText(sb.ToString());
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (lvItems.Items.Count <= 0) return;
            if (lvItems.SelectedIndices.Count <= 0) return;

            StringBuilder sb = new StringBuilder();
            var texts = Clipboard.GetText();
            //var lines = texts.Split(new string[] { "\r", "\n", "\n\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var lines = texts.Split(new string[] { "\n\r", "\r\n", "\r", "\n", }, StringSplitOptions.None);

            for (int i = 0; i < lvItems.SelectedIndices.Count; i++)
            {
                var evt = ass.Events[lvItems.SelectedIndices[i]];
                var line = lines[i];//.Replace("\\n", "\\");
                var match_s = Regex.Matches(evt.Text, @"\{.*?\}", RegexOptions.IgnoreCase);
                var match_t = Regex.Matches(line, @"\{.*?\}", RegexOptions.IgnoreCase);
                for (int m = 0; m < match_s.Count; m++)
                {
                    var match = match_s[m].Value;
                    line = line.Replace(match_t[m].Value, match);
                }
                evt.Translated = line.Replace("\\n", "\\N").Replace(" {", "{").Replace("} ", "}").Replace(" \\", "\\");
            }
            lvItems.Update();
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlgSave = new SaveFileDialog();
            dlgSave.DefaultExt = ".ass";
            dlgSave.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|Text File|*.txt|All File|*.*";
            dlgSave.FilterIndex = 0;
            if (dlgSave.ShowDialog() == DialogResult.OK)
            {
                ass.SaveTo(dlgSave.FileName, ASS.SaveOptions.Merge);
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlgSave = new SaveFileDialog();
            dlgSave.DefaultExt = ".ass";
            dlgSave.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|Text File|*.txt|All File|*.*";
            dlgSave.FilterIndex = 0;
            if (dlgSave.ShowDialog() == DialogResult.OK)
            {
                ass.SaveTo(dlgSave.FileName, ASS.SaveOptions.Replace);
            }
        }
    }
}
