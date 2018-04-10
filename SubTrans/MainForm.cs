using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubTitles
{
    public partial class MainForm : Form
    {
        ASS ass = new ASS();
        Dictionary<int, ListViewItem> lvItemCache = new Dictionary<int, ListViewItem>();
        string LastFilename = string.Empty;

        #region ListView Helper routines
        private const int LVM_FIRST = 4096;

        private const int LVM_SETITEMSTATE = LVM_FIRST + 43;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVITEM
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns;
            public IntPtr puColumns;
        };

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageLVItem(HandleRef hWnd, int msg, int wParam, ref LVITEM lvi);

        /// <summary>
        /// Select all rows on the given listview
        /// </summary>
        /// <param name="listView">The listview whose items are to be selected</param>
        public static void SelectAllItems(ListView listView)
        {
            //NativeMethods.SetItemState(listView, -1, 2, 2);
            SetItemState(listView, -1, 2, 2);
        }

        /// <summary>
        /// Set the item state on the given item
        /// </summary>
        /// <param name="list">The listview whose item's state is to be changed</param>
        /// <param name="itemIndex">The index of the item to be changed</param>
        /// <param name="mask">Which bits of the value are to be set?</param>
        /// <param name="value">The value to be set</param>
        public static void SetItemState(ListView listView, int itemIndex, int mask, int value)
        {
            LVITEM lvItem = new LVITEM();
            lvItem.stateMask = mask;
            lvItem.state = value;
            SendMessageLVItem(new HandleRef(listView, listView.Handle), LVM_SETITEMSTATE, itemIndex, ref lvItem);
        }
        #endregion

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
                lv.Columns[lv.Columns.Count - 1].Width = 640;

                col = new ColumnHeader();
                col.Text = "Translated";
                col.Width = 640;
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

        private void LoadSubTitle(string subtitle)
        {
            ass.Load(subtitle);
            InitListView(lvItems, ass.EventFields);
            lvItems.VirtualListSize = ass.Events.Count();
            LastFilename = subtitle;
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitListView(lvItems, null);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            lvItems.DoubleBuffered(true);
            //lvItems.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        #region Drag/Drop Routines
        private void MainForm_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] dragFiles = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                if (dragFiles.Length > 0)
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            // Determine whether string data exists in the drop data. If not, then 
            // the drop effect reflects that the drop cannot occur. 
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //e.Effect = DragDropEffects.Copy;
                try
                {
                    string[] dragFiles = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                    if (dragFiles.Length > 0)
                    {
                        string dragFileName = dragFiles[0].ToString();
                        string ext = Path.GetExtension(dragFileName).ToLower();

                        string[] exts = { ".ass", ".ssa" };

                        if (exts.Contains(ext))
                        {
                            LoadSubTitle(dragFileName);
                        }
                    }
                }
                catch
                {

                }
            }
            return;
        }
        #endregion

        private void lvItems_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (ass == null || e.ItemIndex < 0 || e.ItemIndex >= ass.Events.Count) return;

            if (lvItemCache.ContainsKey(e.ItemIndex))
            {
                e.Item = lvItemCache[e.ItemIndex];
            }
            else
            {
                var evt = ass.Events[e.ItemIndex];
                ListViewItem lvi = new ListViewItem();
                lvi.Text = evt.ID;
                for (int i = 0; i < ass.EventFields.Length; i++)
                {
                    lvi.SubItems.Add(evt.Field(ass.EventFields[i]));
                }
                lvi.SubItems.Add(evt.Translated);

                lvItemCache[e.ItemIndex] = lvi;
                e.Item = lvi;
            }
        }

        private void lvItems_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                SelectAllItems(lvItems);
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                btnCopy.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                btnPaste.PerformClick();
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.DefaultExt = ".ass";
            //dlgOpen.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|Text File|*.txt|All File|*.*";
            dlgOpen.Filter = "ASS File|*.ass|SSA File|*.ssa";
            dlgOpen.FilterIndex = 0;
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                LoadSubTitle(dlgOpen.FileName);
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
                if (i >= lines.Length) continue;

                var evt = ass.Events[lvItems.SelectedIndices[i]];
                evt.Translated = lines[i];

                var lvi = lvItemCache[lvItems.SelectedIndices[i]];
                lvi.SubItems[lvi.SubItems.Count - 1].Text = evt.Translated;
            }
            ///lvItems.Update();
            lvItems.Invalidate();
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlgSave = new SaveFileDialog();
            if(!string.IsNullOrEmpty(LastFilename))
                dlgSave.InitialDirectory = Path.GetDirectoryName(LastFilename);
            dlgSave.DefaultExt = ".ass";
            dlgSave.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|WebVTT File|*.vtt";
            dlgSave.FilterIndex = 0;
            if (dlgSave.ShowDialog() == DialogResult.OK)
            {
                var flags = ASS.SaveFlags.Merge;
                if (dlgSave.FilterIndex == 3)
                    flags = flags | ASS.SaveFlags.SRT;
                else if (dlgSave.FilterIndex == 4)
                    flags = flags | ASS.SaveFlags.VTT;
                ass.Save(dlgSave.FileName, flags);
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LastFilename) || ass == null) return;
            SaveFileDialog dlgSave = new SaveFileDialog();
            if (!string.IsNullOrEmpty(LastFilename))
                dlgSave.InitialDirectory = Path.GetDirectoryName(LastFilename);
            dlgSave.DefaultExt = ".ass";
            dlgSave.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|WebVTT File|*.vtt";
            dlgSave.FilterIndex = 0;
            if (dlgSave.ShowDialog() == DialogResult.OK)
            {
                var flags = ASS.SaveFlags.Replace;
                if (dlgSave.FilterIndex == 3)
                    flags = flags | ASS.SaveFlags.SRT;
                else if (dlgSave.FilterIndex == 4)
                    flags = flags | ASS.SaveFlags.VTT;
                ass.Save(dlgSave.FileName, flags);
            }
        }

        private void cmiSaveAs_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LastFilename) || ass == null) return;
            SaveFileDialog dlgSave = new SaveFileDialog();
            if (!string.IsNullOrEmpty(LastFilename))
                dlgSave.InitialDirectory = Path.GetDirectoryName(LastFilename);
            dlgSave.DefaultExt = ".ass";
            dlgSave.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|WebVTT File|*.vtt";
            dlgSave.FilterIndex = 0;
            if (dlgSave.ShowDialog() == DialogResult.OK)
            {
                var flags = ASS.SaveFlags.None;
                if (dlgSave.FilterIndex == 3)
                    flags = flags | ASS.SaveFlags.SRT;
                else if (dlgSave.FilterIndex == 4)
                    flags = flags | ASS.SaveFlags.VTT;
                ass.Save(dlgSave.FileName, flags);
            }
        }

        private void cmiExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    public static class DoubleBufferListView
    {
        /// <summary>  
        /// 双缓冲，解决闪烁问题  
        /// </summary>  
        /// <param name="lv"></param>  
        /// <param name="flag"></param>  
        public static void DoubleBuffered(this ListView lv, bool flag)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession) return;

            Type lvType = lv.GetType();
            PropertyInfo pi = lvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(lv, flag, null);
        }
    }


}
