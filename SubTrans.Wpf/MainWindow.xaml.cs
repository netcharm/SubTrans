using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SubTitles
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ASS ass = new ASS();
        Dictionary<int, ListViewItem> lvItemCache = new Dictionary<int, ListViewItem>();
        string LastFilename = string.Empty;

        private void InitListView(ListView lv, string[] headers)
        {
            if (headers != null)
            {
                lv.Items.Clear();

                GridView gv = new GridView();

                var col = new GridViewColumn();
                col.Header = "ID";
                col.DisplayMemberBinding = new Binding(col.Header.ToString());
                gv.Columns.Add(col);

                foreach (string header in headers)
                {
                    col = new GridViewColumn();
                    col.Header = header;
                    col.DisplayMemberBinding = new Binding(header);
                    gv.Columns.Add(col);
                }
                gv.Columns[gv.Columns.Count - 1].Width = 640;

                col = new GridViewColumn();
                col.Header = "Translated";
                col.DisplayMemberBinding = new Binding(col.Header.ToString());
                col.Width = 640;
                gv.Columns.Add(col);

                lv.View = gv;
            }
            else
            {
                lv.View = null;
            }
        }

        private void LoadSubTitle(string subtitle)
        {
            ass.Load(subtitle);
            InitListView(lvItems, ass.EventFields);
            LastFilename = subtitle;
            for(int i = 0; i < ass.Events.Count; i++)
            {
                lvItems.Items.Add(ass.Events[i]);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            //Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            MainGrid.AllowDrop = true;
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] dragFiles = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                if (dragFiles.Length > 0)
                {
                    e.Effects = DragDropEffects.Copy;
                }
            }

        }

        private void Grid_Drop(object sender, DragEventArgs e)
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
                        string ext = System.IO.Path.GetExtension(dragFileName).ToLower();

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
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (lvItems.Items.Count <= 0) return;
            if (lvItems.SelectedItems.Count <= 0) return;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lvItems.SelectedItems.Count; i++)
            {
                var idx = lvItems.Items.IndexOf(lvItems.SelectedItems[i]);
                var evt = ass.Events[idx];
                sb.AppendLine(evt.Text);
            }
            Clipboard.SetText(sb.ToString());

        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            if (lvItems.Items.Count <= 0) return;
            if (lvItems.SelectedItems.Count <= 0) return;

            StringBuilder sb = new StringBuilder();
            var texts = Clipboard.GetText();
            //var lines = texts.Split(new string[] { "\r", "\n", "\n\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var lines = texts.Split(new string[] { "\n\r", "\r\n", "\r", "\n", }, StringSplitOptions.None);

            for (int i = 0; i < lvItems.SelectedItems.Count; i++)
            {
                if (i >= lines.Length) continue;

                var idx = lvItems.Items.IndexOf(lvItems.SelectedItems[i]);
                var evt = ass.Events[idx];
                evt.Translated = lines[i];

                lvItems.SelectedItems[i] = evt;
            }
            ICollectionView view = CollectionViewSource.GetDefaultView(lvItems.Items);
            view.Refresh();
        }

        private void btnMerge_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlgSave = new SaveFileDialog();
            if (!string.IsNullOrEmpty(LastFilename))
                dlgSave.InitialDirectory = System.IO.Path.GetDirectoryName(LastFilename);
            dlgSave.DefaultExt = ".ass";
            dlgSave.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|WebVTT File|*.vtt";
            dlgSave.FilterIndex = 0;
            if (dlgSave.ShowDialog() == true)
            {
                var flags = ASS.SaveFlags.Merge;
                if (dlgSave.FilterIndex == 3)
                    flags = flags | ASS.SaveFlags.SRT;
                else if (dlgSave.FilterIndex == 4)
                    flags = flags | ASS.SaveFlags.VTT;
                ass.Save(dlgSave.FileName, flags);
            }
        }

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LastFilename) || ass == null) return;
            SaveFileDialog dlgSave = new SaveFileDialog();
            if (!string.IsNullOrEmpty(LastFilename))
                dlgSave.InitialDirectory = System.IO.Path.GetDirectoryName(LastFilename);
            dlgSave.DefaultExt = ".ass";
            dlgSave.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|WebVTT File|*.vtt";
            dlgSave.FilterIndex = 0;
            if (dlgSave.ShowDialog() == true)
            {
                var flags = ASS.SaveFlags.Replace;
                if (dlgSave.FilterIndex == 3)
                    flags = flags | ASS.SaveFlags.SRT;
                else if (dlgSave.FilterIndex == 4)
                    flags = flags | ASS.SaveFlags.VTT;
                ass.Save(dlgSave.FileName, flags);
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.DefaultExt = ".ass";
            //dlgOpen.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|Text File|*.txt|All File|*.*";
            dlgOpen.Filter = "ASS File|*.ass|SSA File|*.ssa";
            dlgOpen.FilterIndex = 0;
            if (dlgOpen.ShowDialog() == true)
            {
                LoadSubTitle(dlgOpen.FileName);
            }
        }

        private void cmiSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LastFilename) || ass == null) return;
            SaveFileDialog dlgSave = new SaveFileDialog();
            if (!string.IsNullOrEmpty(LastFilename))
                dlgSave.InitialDirectory = System.IO.Path.GetDirectoryName(LastFilename);
            dlgSave.DefaultExt = ".ass";
            dlgSave.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|WebVTT File|*.vtt";
            dlgSave.FilterIndex = 0;
            if (dlgSave.ShowDialog() == true)
            {
                var flags = ASS.SaveFlags.None;
                if (dlgSave.FilterIndex == 3)
                    flags = flags | ASS.SaveFlags.SRT;
                else if (dlgSave.FilterIndex == 4)
                    flags = flags | ASS.SaveFlags.VTT;
                ass.Save(dlgSave.FileName, flags);
            }
        }

        private void cmiExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) )//|| Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.C)
                {
                    ButtonAutomationPeer peer = new ButtonAutomationPeer(btnCopy);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
                }
                else if (e.Key == Key.V)
                {
                    ButtonAutomationPeer peer = new ButtonAutomationPeer(btnPaste);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
                }
            }
        }
    }
}
