using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace SubTitles
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ASS ass = new ASS();
        private ObservableCollection<ASS.EVENT> events = new ObservableCollection<ASS.EVENT>();
        public ObservableCollection<ASS.EVENT> Events
        {
            get { return (events); }
        }

        string LastFilename = string.Empty;

        #region Extract Icon from application
        public static ImageSource GetIcon(string fileName)
        {
            System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(fileName);
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        new Int32Rect(0, 0, icon.Width, icon.Height),
                        BitmapSizeOptions.FromEmptyOptions());
        }

        public static ImageSource GetIcon(string path, bool smallIcon, bool isDirectory)
        {
            // SHGFI_USEFILEATTRIBUTES takes the file name and attributes into account if it doesn't exist
            uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
            if (smallIcon)
                flags |= SHGFI_SMALLICON;

            uint attributes = FILE_ATTRIBUTE_NORMAL;
            if (isDirectory)
                attributes |= FILE_ATTRIBUTE_DIRECTORY;

            SHFILEINFO shfi;
            if (0 != SHGetFileInfo(
                        path,
                        attributes,
                        out shfi,
                        (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                        flags))
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            shfi.hIcon,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
            }
            return null;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("shell32")]
        private static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint flags);

        #region Shell File attributes const define
        private const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
        private const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
        private const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;
        private const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        private const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;
        private const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;
        private const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
        private const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;
        private const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;
        private const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
        private const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;
        private const uint FILE_ATTRIBUTE_VIRTUAL = 0x00010000;

        private const uint SHGFI_ICON = 0x000000100;     // get icon
        private const uint SHGFI_DISPLAYNAME = 0x000000200;     // get display name
        private const uint SHGFI_TYPENAME = 0x000000400;     // get type name
        private const uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes
        private const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location
        private const uint SHGFI_EXETYPE = 0x000002000;     // return exe type
        private const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index
        private const uint SHGFI_LINKOVERLAY = 0x000008000;     // put a link overlay on icon
        private const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state
        private const uint SHGFI_ATTR_SPECIFIED = 0x000020000;     // get only specified attributes
        private const uint SHGFI_LARGEICON = 0x000000000;     // get large icon
        private const uint SHGFI_SMALLICON = 0x000000001;     // get small icon
        private const uint SHGFI_OPENICON = 0x000000002;     // get open icon
        private const uint SHGFI_SHELLICONSIZE = 0x000000004;     // get shell size icon
        private const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute
        #endregion

        #endregion

        private DataTemplate GetCellTemplate(string colname)
        {
            string xaml = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                                          xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> 
               <Border BorderBrush=""LightGray"" BorderThickness=""1,0,0,0"" Margin=""-6,-2,-8,-2"">
                   <TextBlock Text=""{{Binding {0}, IsAsync=True, Mode=OneWay, NotifyOnSourceUpdated=True, NotifyOnTargetUpdated=True}}""/>
               </Border>
            </DataTemplate>";

            StringReader stringReader = new StringReader(string.Format(xaml, colname));
            XmlReader xmlReader = XmlReader.Create(stringReader);
            return(XamlReader.Load(xmlReader) as DataTemplate);
        }

        private void InitListView(ListView lv, string[] headers)
        {
            lv.AlternationCount = 2;           
            events.Clear();

            GridView gv = (GridView)lv.View;
            gv.Columns.Clear();
            if (headers != null)
            {
                var col = new GridViewColumn();
                col.Header = "ID";
                col.DisplayMemberBinding = new Binding(col.Header.ToString());
                col.CellTemplate = GetCellTemplate(col.Header.ToString());
                gv.Columns.Add(col);

                foreach (string header in headers)
                {
                    col = new GridViewColumn();
                    col.Header = header;
                    col.DisplayMemberBinding = new Binding(header);
                    col.CellTemplate = GetCellTemplate(col.Header.ToString());
                    gv.Columns.Add(col);
                }
                gv.Columns[gv.Columns.Count - 1].Width = 640;

                col = new GridViewColumn();
                col.Header = "Translated";
                col.DisplayMemberBinding = new Binding(col.Header.ToString());
                col.CellTemplate = GetCellTemplate(col.Header.ToString());
                col.Width = 640;
                gv.Columns.Add(col);
            }
        }

        private async void LoadSubTitle(string subtitle)
        {
            await ass.Load(subtitle);
            InitListView(lvItems, ass.EventFields);
            LastFilename = subtitle;
            for (int i = 0; i < ass.Events.Count; i++)
            {
                events.Add(ass.Events[i]);
            }
        }

        private void SaveASS(ASS.SaveFlags flags)
        {
            if (ass == null) return;
            else if (string.IsNullOrEmpty(LastFilename) && !string.IsNullOrEmpty(ass.ScriptInfo.Title))
                LastFilename = ass.ScriptInfo.Title;
            else if (!string.IsNullOrEmpty(LastFilename) && string.IsNullOrEmpty(ass.ScriptInfo.Title))
                ass.ScriptInfo.Title = LastFilename;

            SaveFileDialog dlgSave = new SaveFileDialog();
            if (!string.IsNullOrEmpty(LastFilename))
                dlgSave.InitialDirectory = Path.GetDirectoryName(LastFilename);
            dlgSave.DefaultExt = ".ass";
            dlgSave.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|WebVTT File|*.vtt";
            dlgSave.FilterIndex = 0;
            if (dlgSave.ShowDialog() == true)
            {
                if (dlgSave.FilterIndex == 3)
                    flags = flags | ASS.SaveFlags.SRT;
                else if (dlgSave.FilterIndex == 4)
                    flags = flags | ASS.SaveFlags.VTT;
                ass.Save(dlgSave.FileName, flags);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            var apppath = Assembly.GetExecutingAssembly().Location;
            Icon = GetIcon(apppath.ToString());
            MainGrid.AllowDrop = true;
            lvItems.ItemsSource = events;
        }

        #region Drag/Drop Routines
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
                        string ext = Path.GetExtension(dragFileName).ToLower();

                        string[] exts = { ".ass", ".ssa", ".srt" };

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
        #endregion

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))//|| Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.C)
                {
                    //typeof(System.Windows.Controls.Primitives.ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(btnCopy, new object[0]);
                    ButtonAutomationPeer peer = new ButtonAutomationPeer(btnCopy);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
                }
                else if (e.Key == Key.V)
                {
                    //typeof(System.Windows.Controls.Primitives.ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(btnPaste, new object[0]);
                    ButtonAutomationPeer peer = new ButtonAutomationPeer(btnPaste);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
                }
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.DefaultExt = ".ass";
            //dlgOpen.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|Text File|*.txt|All File|*.*";
            dlgOpen.Filter = "All Supported File|*.ass;*.ssa;*.srt|ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt";
            dlgOpen.FilterIndex = 0;
            if (dlgOpen.ShowDialog() == true)
            {
                LoadSubTitle(dlgOpen.FileName);
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
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
                var text = sb.ToString();
                Clipboard.SetDataObject(text);
            }
            catch (Exception) { }
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            if (lvItems.Items.Count <= 0) return;
            if (lvItems.SelectedItems.Count <= 0) return;

            StringBuilder sb = new StringBuilder();
            var texts = Clipboard.GetText();
            var lines = texts.Split(new string[] { "\n\r", "\r\n", "\r", "\n", }, StringSplitOptions.None);

            var idx_t = -1;
            foreach (var item in lvItems.SelectedItems)
            {
                idx_t++;
                if (item is ASS.EVENT)
                {
                    var selected = item as ASS.EVENT;
                    var idx = Convert.ToInt32(selected.ID) - 1;
                    if (idx_t >= lines.Length) continue;

                    var evt = ass.Events[idx];
                    evt.Translated = lines[idx_t];
                    events[idx].Translated = evt.Translated;
                }
            }
        }

        private void btnPasteYoutube_Click(object sender, RoutedEventArgs e)
        {
            events.Clear();
            var texts = Clipboard.GetText();
            var lines = texts.Split(new string[] { "\n\r", "\r\n", "\r", "\n", }, StringSplitOptions.None);

            string title = string.Empty;
            var dlgInput = new InputDialog("Input", title);
            if (dlgInput.ShowDialog() == true)
            {
                title = dlgInput.Text;
            }
            ass.LoadFromYouTube(lines, title);
            InitListView(lvItems, ass.EventFields);
            for (int i = 0; i < ass.Events.Count; i++)
            {
                events.Add(ass.Events[i]);
            }
        }

        private void btnMerge_Click(object sender, RoutedEventArgs e)
        {
            SaveASS(ASS.SaveFlags.Merge);
        }

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            SaveASS(ASS.SaveFlags.Replace);
        }

        private void cmiSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveASS(ASS.SaveFlags.None);
        }

        private void cmiExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
