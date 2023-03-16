using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Serialization;

using Microsoft.Win32;

namespace SubTrans
{
    public enum SupportedLanguage { Any, CHS, CHT, JPN, KOR, ENG, Unknown };
    public enum SplitMode { ByCount, BySymbol };

    public enum FindReplaceRangeMode { None, Current, Selected, All };
    public enum FindReplaceMode { None, Find, Replace };

    public class FindReplaceOptions
    {
        public FindReplaceRangeMode Range { get; set; } = FindReplaceRangeMode.Current;
        public bool IgnoreCase { get; set; } = false;
        public bool UseRegex { get; set; } = false;
        public FindReplaceMode Mode { get; set; } = FindReplaceMode.Find;
        public string TextToFind { get; set; } = string.Empty;
        public string TextToReplace { get; set; } = string.Empty;
        public int FindResult { get; set; } = -3;
        public List<bool> ReplaceResult { get; set; } = new List<bool>();
    }

    public enum PhraseType { Auto, Manual, None };
    public enum PhraseCategory { All, Anime, Movie, Drama, None };

    [XmlRoot(ElementName = "Phrase", IsNullable = true)]
    public partial class Term
    {
        [XmlAttribute(AttributeName = "Original")]
        public string Original { get; set; }
        [XmlAttribute(AttributeName = "Translated")]
        public string Translated { get; set; }
        [XmlAttribute(AttributeName = "Language")]
        public string Language { get; set; }
        [XmlAttribute(AttributeName = "Comment")]
        public string Comment { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public PhraseType Type { get; set; } = PhraseType.Auto;
        [XmlAttribute(AttributeName = "Catalog")]
        public PhraseCategory Category { get; set; }

        public bool Equals(Term term)
        {
            if (!(term is Term)) return (false);
            return (term.Original.Equals(Original) && term.Translated.Equals(Translated) && term.Language.Equals(Language, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool Exists(Term term)
        {
            if (!(term is Term)) return (false);
            return (term.Original.Equals(Original) && term.Language.Equals(Language, StringComparison.CurrentCultureIgnoreCase));
        }
    }

    [XmlRoot(ElementName = "Vocabulary", IsNullable = false)]
    public partial class Terms
    {
        [XmlElement(ElementName = "Phrases")]
        public List<Term> Items { get; set; } = new List<Term>();
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string AppExec = Assembly.GetEntryAssembly().Location;
        private static string AppPath = Path.GetDirectoryName(AppExec);
        private static string AppName = Path.GetFileNameWithoutExtension(AppExec);

        private bool InInit { get; set; } = true;

        private string[] exts = new string[] { ".ass", ".ssa", ".srt", ".vtt", ".lrc" };

        private const string YoutubeLanguage_Key = "YoutubeLanguage";
        private string YoutubeLanguage = Properties.Settings.Default.YoutubeLanguage;

        private const string SaveWithBOM_Key = "SaveWithBOM";
        private bool SaveWithBOM = Properties.Settings.Default.SaveWithBOM;

        private const string PasteRemoveNullLine_Key = "PasteRemoveNullLine";
        private bool PasteRemoveNullLine = Properties.Settings.Default.PasteRemoveNullLine;

        private const string SplitCharMode_Key = "SplitCharMode";
        private SplitMode SplitCharMode = SplitMode.BySymbol;

        private const string SplitCharCount_Key = "SplitCharCount";
        private int SplitCharCount = Properties.Settings.Default.SplitCharCount;

        private const string MergeString_Key = "MergeString";
        private string MergeString = Properties.Settings.Default.MergeString;

        private const string SplitCharSymbols_Key = "SplitCharSymbols";
        private string SplitCharSymbols = string.Join("", new char[] {
            ',', '.', '!', '?', '~',
            '！', '？', '，', '、', '。', '；', '～',
            '…', '⋯', '⁇', '⁈', '⁉', '⸺', '⸻',
            '❤', '♩', '♪', '♫', '♬', '✨'
        });

        private const string OriginalEdiable_Key = "OriginalEdiable";
        private bool OriginalEdiable = Properties.Settings.Default.OriginalEditable;

        ASS ass = new ASS();
        private ObservableCollection<ASS.EVENT> events = new ObservableCollection<ASS.EVENT>();
        public ObservableCollection<ASS.EVENT> Events
        {
            get { return (events); }
        }

        string OriginalTitle = string.Empty;
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

        #region Phrase Helper
        private Terms Phrase = new Terms();
        private string PhraseFile = Path.Combine(AppPath, $"{AppName}.phrase.xml");

        private Terms LoadPhrase(string file = "")
        {
            var result = new Terms();
            if (string.IsNullOrEmpty(file)) file = PhraseFile;
            if (File.Exists(file))
            {
                try
                {
                    var xs = new XmlSerializer(typeof(Terms));
                    using (StringReader tr = new StringReader(File.ReadAllText(file, Encoding.UTF8)))
                    {
                        result = (Terms)xs.Deserialize(tr);
                    }
                }
                catch (Exception ex) { MessageBox.Show(this, ex.Message); }
            }
            return (result);
        }

        private void SavePhrase(string file = "", Terms terms = null)
        {
            if (string.IsNullOrEmpty(file)) file = PhraseFile;
            if (terms == null) terms = Phrase;
            if (terms is Terms)
            {
                try
                {
                    var xs = new XmlSerializer(typeof(Terms));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                        {
                            using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true, IndentChars = "  " }))
                            {
                                xs.Serialize(xw, terms);
                            }
                        }
                        File.WriteAllBytes(file, ms.ToArray());
                    }
                }
                catch (Exception ex) { MessageBox.Show(this, ex.Message); }
            }
        }

        private string PhraseReplace(string text, PhraseType type = PhraseType.Auto)
        {
            var result = string.Join("\\N", text.Split(new string[] { "\\N", "\\n" }, StringSplitOptions.None).Select(t => t.Trim()));
            try
            {
                foreach (var phrase in Phrase.Items)
                {
                    if (phrase.Type != type) continue;
                    if (string.IsNullOrEmpty(phrase.Original)) continue;
                    if (string.IsNullOrEmpty(phrase.Translated)) continue;
                    if (Regex.IsMatch(phrase.Original, @"/.+?/i", RegexOptions.IgnoreCase))
                        result = Regex.Replace(result, phrase.Original.TrimEnd(new char[] { 'i', 'I' }).Trim('/'), phrase.Translated, RegexOptions.IgnoreCase);
                    else if (Regex.IsMatch(phrase.Original, @"/.+?/", RegexOptions.IgnoreCase))
                        result = Regex.Replace(result, phrase.Original.Trim('/'), phrase.Translated, RegexOptions.None);
                    else
                        result = result.Replace(phrase.Original, phrase.Translated);
                }
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message); }
            return (result);
        }

        private string FixBracketingError(string text, PhraseType type = PhraseType.Auto)
        {
            var result = text;
            try
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result = Regex.Replace(text, $@"｛(\\.*?)｝", m =>
                    {
                        var t = $"{{{m.Groups[1].Value}}}";
                        t = Regex.Replace(t, $@"（(.*?)）", ms =>
                        {
                            return ($"({ms.Groups[1].Value})");
                        }, RegexOptions.IgnoreCase);
                        return (t);
                    }, RegexOptions.IgnoreCase);

                    result = result.Replace("｛fruf2｝", "{\blur2}").Replace("｛fru｝", "{\blur2}").TrimEnd(new char[] { '\\', '\r', '\n' });

                    result = Regex.Replace(result, @"\\fn(次?新)?罗马(时代)?", @"\fnTimes New Roman", RegexOptions.IgnoreCase);

                    result = Regex.Replace(result, @"(\{\\.*?\})", m => { return (m.Value.Replace("\\", "\x1F")); }, RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, @"\\(h|n|N)", m => { return ($"{"\x1F"}{m.Groups[1].Value}"); }, RegexOptions.IgnoreCase);
                    result = result.Replace("\\", "\\N").Replace("\u001F", "\\").Replace("\x1F", "\\");
                    if (Regex.IsMatch(result, @"\{.*?\}"))
                    {
                        var prefix = result.StartsWith("{") ? "" : "}";
                        var suffix = result.EndsWith("}") ? "" : "{";
                        var regex = @"\}(.+?)\{";
                        result = Regex.Replace($"{prefix}{result}{suffix}", regex, m =>
                        {
                            return (m.Value.Replace(m.Groups[1].Value, PhraseReplace(m.Groups[1].Value, type)));
                        }, RegexOptions.IgnoreCase);
                        result = result.Remove(result.Length - 1, suffix.Length).Remove(0, prefix.Length);
                    }
                    else result = PhraseReplace(result, type);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return (result);
        }

        private IEnumerable<string> FixBracketingError(IEnumerable<string> lines)
        {
            var result = lines.ToList();
            try
            {
                for (var i = 0; i < lines.Count(); i++)
                {
                    if (string.IsNullOrEmpty(result[i])) continue;
                    result[i] = FixBracketingError(result[i]);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return (result);
        }
        #endregion

        #region Listview Helper
        private string GetValidFileName(string name)
        {
            string result = string.Empty;

            List<char> cl = new List<char>();
            var ci = Path.GetInvalidFileNameChars();
            foreach (char c in name)
            {
                if (ci.Contains(c))
                    cl.Add('_');
                else
                    cl.Add(c);
            }

            return (string.Join("", cl));
        }

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
            return (XamlReader.Load(xmlReader) as DataTemplate);
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
                col.Width = 48;
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
        #endregion

        #region Load/Save Subtitles Helper
        private static Configuration config = ConfigurationManager.OpenExeConfiguration(AppExec);
        private static AppSettingsSection appSection = config.AppSettings;

        private static string CustomStylePrefix = "CustomStyle_";
        private async void InitCustomStyle(ASS ass)
        {
            if (ass is ASS)
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    UIElement[] menus = new UIElement[cmiSetStyle.Items.Count];
                    cmiSetStyle.Items.CopyTo(menus, 0);
                    foreach (var m in menus)
                    {
                        if (m is MenuItem)
                        {
                            var menu = m as MenuItem;
                            if (menu.Tag is string && (menu.Tag as string).StartsWith(CustomStylePrefix)) cmiSetStyle.Items.Remove(menu);
                        }
                    }
                    var styles = ass.GetCustomStyles();
                    if (styles.Count() > 0)
                    {
                        cmiSetStyleSep.Visibility = Visibility.Visible;
                        foreach (var style in styles)
                        {
                            try
                            {
                                var menu = new MenuItem() { Tag = $"{CustomStylePrefix}{style}", Header = style };
                                menu.Click += cmiSetStyle_Click;
                                cmiSetStyle.Items.Add(menu);
                            }
                            catch { }
                        }
                    }
                    else cmiSetStyleSep.Visibility = Visibility.Collapsed;
                }));
            }
        }

        private async void LoadSubtitle(string subtitle)
        {
            try
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadProgress.IsIndeterminate = true;
                }));
                await ass.Load(subtitle);
                InitListView(lvItems, ass.EventFields);
                LastFilename = subtitle;
                for (int i = 0; i < ass.Events.Count; i++)
                {
                    events.Add(ass.Events[i]);
                }
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}"); }
            finally
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadProgress.IsIndeterminate = false;
                }));
                if (!string.IsNullOrEmpty(LastFilename))
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Title = $"{OriginalTitle} - {LastFilename}";
                    }));
                }
                InitCustomStyle(ass);
            }
        }

        private void LoadSubtitle(IEnumerable<string> subtitles)
        {
            if (subtitles.Count() > 0) LoadSubtitle(subtitles.First());
            if (subtitles.Count() > 1)
            {
                foreach (var subtitle in subtitles.Skip(1))
                {
                    var file = Path.IsPathRooted(subtitle) ? subtitle : Path.GetPathRoot(subtitle);
                    if (File.Exists(subtitle))
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            System.Diagnostics.Process.Start(AppExec, $"\"{file}\"");
                        }));
                    }
                }
            }
        }

        private void SaveSubtitle(ASS.SaveFlags flags = ASS.SaveFlags.BOM)
        {
            if (ass == null) return;
            else if (string.IsNullOrEmpty(LastFilename) && !string.IsNullOrEmpty(ass.ScriptInfo.Title))
                LastFilename = GetValidFileName(ass.ScriptInfo.Title);
            else if (!string.IsNullOrEmpty(LastFilename) && string.IsNullOrEmpty(ass.ScriptInfo.Title))
                ass.ScriptInfo.Title = LastFilename;

            SaveFileDialog dlgSave = new SaveFileDialog();
            if (!string.IsNullOrEmpty(LastFilename))
                dlgSave.InitialDirectory = Path.GetDirectoryName(LastFilename);
            dlgSave.DefaultExt = ".ass";
            dlgSave.FileName = Path.GetFileNameWithoutExtension(LastFilename);
            dlgSave.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|WebVTT File|*.vtt|Lyric File|*.lrc|Text File|*.txt|Pure Text File|*.txt";
            dlgSave.FilterIndex = 0;
            if (dlgSave.ShowDialog() == true)
            {
                if (dlgSave.FilterIndex == 1)
                    flags = flags | ASS.SaveFlags.ASS;
                else if (dlgSave.FilterIndex == 2)
                    flags = flags | ASS.SaveFlags.ASS;
                else if (dlgSave.FilterIndex == 3)
                    flags = flags | ASS.SaveFlags.SRT;
                else if (dlgSave.FilterIndex == 4)
                    flags = flags | ASS.SaveFlags.VTT;
                else if (dlgSave.FilterIndex == 5)
                    flags = flags | ASS.SaveFlags.LRC;
                else if (dlgSave.FilterIndex == 6)
                    flags = flags | ASS.SaveFlags.TXT;
                else if (dlgSave.FilterIndex == 7)
                    flags = flags | ASS.SaveFlags.TXT | ASS.SaveFlags.NOTIME | ASS.SaveFlags.NOLINEBREAK;
                ass.Save(dlgSave.FileName, flags);
                LastFilename = dlgSave.FileName;
                Keyboard.ClearFocus();
            }
        }
        #endregion

        #region Edit/Find/Replace/Speech Helper
        private FindReplaceOptions _last_find_replace_option_ = null;

        private void OpenTranslatedEditor()
        {
            if (lvItems.SelectedItem != null && lvItems.SelectedItem is ASS.EVENT)
            {
                var item = lvItems.SelectedItem as ASS.EVENT;
                var dlg = new EventDetail()
                {
                    Owner = this,
                    Icon = Icon,
                    ShowActivated = true,
                    ShowInTaskbar = false,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    OriginalEditable = OriginalEdiable,
                    Event = item,
                };
                if (dlg.ShowDialog() ?? false)
                {
                    item.Text = dlg.Event.Text;
                    item.Translated = dlg.Event.Translated;
                }
            }
        }

        private void OpenFindReplaceEditor()
        {
            if (lvItems.Items.Count > 0)
            {
                var dlg = new FindReplace()
                {
                    Owner = this,
                    Topmost = true,
                    Icon = Icon,
                    ShowActivated = true,
                    ShowInTaskbar = true,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Options = _last_find_replace_option_ is FindReplaceOptions ? _last_find_replace_option_ : new FindReplaceOptions(),
                    UpdateOptionAction = UpdateFindReplaceOptionAction,
                    FindTextAction = FindTextAction,
                    ReplaceTextAction = ReplaceTextAction,
                    FindReplaceResultFunc = FindReplaceResultFunc,
                    TextToFind = _last_find_replace_option_ is FindReplaceOptions ? _last_find_replace_option_.TextToFind : string.Empty,
                    TextToReplace = _last_find_replace_option_ is FindReplaceOptions ? _last_find_replace_option_.TextToReplace : string.Empty,
                };
                dlg.Show();
            }
        }

        private void FindText(FindReplaceOptions opt)
        {
            try
            {
                if (lvItems.Items.Count <= 0) return;
                if (opt == null) opt = new FindReplaceOptions();

                _last_find_replace_option_ = opt;
                var text = opt.TextToFind;
                if (string.IsNullOrEmpty(text)) return;

                int _last_find_index_ = _last_find_replace_option_ is FindReplaceOptions ? _last_find_replace_option_.FindResult : -3;

                var selected = new object[lvItems.SelectedItems.Count];
                lvItems.SelectedItems.CopyTo(selected, 0);

                var events = opt.Range == FindReplaceRangeMode.All || lvItems.SelectedItems.Count <= 0 ? lvItems.Items : lvItems.SelectedItems;
                var items = new object[events.Count];
                events.CopyTo(items, 0);
                items = items.OrderBy(i => lvItems.Items.IndexOf(i)).ToArray();
                var ids = items.Select(i => (i as ASS.EVENT).ID).ToArray();
                if (_last_find_index_ >= Convert.ToInt32(ids.Last()) - 1) _last_find_index_ = -2;

                foreach (var item in items)
                {
                    if (item is ASS.EVENT)
                    {
                        var e = item as ASS.EVENT;
                        var idx = Convert.ToInt32(e.ID) - 1;
                        if (idx <= _last_find_index_) continue;

                        var evt = ass.Events[idx];
                        if (opt.UseRegex)
                        {
                            var opt_r = opt.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
                            if (!string.IsNullOrEmpty(evt.Text) && Regex.IsMatch(evt.Text, $@"{text}", opt_r)) { _last_find_index_ = idx; break; }
                            else if (!string.IsNullOrEmpty(evt.Translated) && Regex.IsMatch(evt.Translated, $@"{text}", opt_r)) { _last_find_index_ = idx; break; }
                            else _last_find_index_ = -2;
                        }
                        else
                        {
                            var opt_r = opt.IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
                            if (!string.IsNullOrEmpty(evt.Text) && evt.Text.IndexOf(text, opt_r) >= 0) { _last_find_index_ = idx; break; }
                            else if (!string.IsNullOrEmpty(evt.Translated) && evt.Translated.IndexOf(text, opt_r) >= 0) { _last_find_index_ = idx; break; }
                            else _last_find_index_ = -2;
                        }
                    }
                }
                if (_last_find_index_ >= 0 && _last_find_index_ < lvItems.Items.Count)
                {
                    lvItems.ScrollIntoView(lvItems.Items[_last_find_index_]);
                }
                foreach (var item in selected) lvItems.SelectedItems.Add(item);
                _last_find_replace_option_.FindResult = _last_find_index_;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private bool ReplaceText(ASS.EVENT e, FindReplaceOptions opt)
        {
            var result = false;
            if (e is ASS.EVENT && opt is FindReplaceOptions)
            {
                var src = opt.TextToFind;
                var dst = opt.TextToReplace;
                if (string.IsNullOrEmpty(src)) return (result);

                var opt_r = opt.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
                var opt_t = opt.IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
                var text_old = string.IsNullOrEmpty(e.Translated) ? e.Text : e.Translated;
                var text_new = string.IsNullOrEmpty(e.Translated) ? e.Text : e.Translated;
                if (opt.UseRegex)
                {
                    if (!string.IsNullOrEmpty(text_old))
                    {
                        text_new = Regex.Replace(text_old, $@"{src}", $@"{dst}", opt_r);
                        if (!text_old.Equals(text_new, opt_t)) { e.Translated = text_new; result = true; }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(text_new))
                    {
                        text_new = text_new.Replace(src, dst);
                        if (text_old.Length != text_new.Length || !text_old.Equals(text_new, opt_t)) { e.Translated = text_new; result = true; }
                    }
                }
            }
            return (result);
        }

        private IEnumerable<bool> ReplaceText(FindReplaceOptions opt)
        {
            var result = new List<bool>();
            try
            {
                if (ass is ASS)
                {
                    _last_find_replace_option_ = opt;
                    _last_find_replace_option_.ReplaceResult.Clear();

                    var back_ass = ass.Clone();

                    if (opt.Range == FindReplaceRangeMode.Current && _last_find_replace_option_.FindResult >= 0 && _last_find_replace_option_.FindResult < lvItems.Items.Count)
                    {
                        var item = lvItems.Items[_last_find_replace_option_.FindResult];
                        if (item is ASS.EVENT) result.Add(ReplaceText(item as ASS.EVENT, opt));
                    }
                    else if (opt.Range == FindReplaceRangeMode.Selected)
                    {
                        foreach (var item in lvItems.SelectedItems)
                        {
                            if (item is ASS.EVENT) result.Add(ReplaceText(item as ASS.EVENT, opt));
                        }
                    }
                    else if (opt.Range == FindReplaceRangeMode.All)
                    {
                        foreach (var item in lvItems.Items)
                        {
                            if (item is ASS.EVENT) result.Add(ReplaceText(item as ASS.EVENT, opt));
                        }
                    }
                    _last_find_replace_option_.ReplaceResult.AddRange(result);
                    if (result.Count > 0) MakeBackup(back_ass);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return (result);
        }

        private Action<FindReplaceOptions> UpdateFindReplaceOptionAction { get; set; } = null;
        private Action<FindReplaceOptions> FindTextAction { get; set; } = null;
        private Action<FindReplaceOptions> ReplaceTextAction { get; set; } = null;
        private Func<string> FindReplaceResultFunc { get; set; } = null;

        private void Text2Voice(bool auto = true)
        {
            if (lvItems.SelectedIndex >= 0 && lvItems.SelectedIndex < lvItems.Items.Count)
            {
                var item = lvItems.SelectedItem as ASS.EVENT;
                if (auto && !string.IsNullOrEmpty(item.Translated.Trim())) Text2Voice(item.Translated.Trim());
                else if (!string.IsNullOrEmpty(item.Text.Trim())) Text2Voice(item.Text.Trim());
            }
        }

        static internal void Text2Voice(string text, string lang = "auto")
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                Speech.SimpleCultureDetect = false;
            else
                Speech.SimpleCultureDetect = true;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                Speech.AltPlayMixedCulture = true;
            else
                Speech.AltPlayMixedCulture = false;

            Speech.AutoChangeSpeechSpeed = false;

            string culture = string.IsNullOrEmpty(lang) || lang.Equals("auto", StringComparison.CurrentCulture) ? "unk" : lang;

            //var slice_words = new List<string>();
            //slice_words.AddRange(Speech.Slice(text.Split(Speech.LineBreak, StringSplitOptions.RemoveEmptyEntries), culture));

            //var tip = string.Join(", ", slice_words);
            //tip = Regex.Replace(tip, @"((.+?, ){5})", $"$1{Environment.NewLine}", RegexOptions.IgnoreCase);
            //if (slice_words.Count > 0)
            //{
            //    hint.SetToolTip(edResult, null);
            //    hint.Show(tip, edResult, edResult.Left, edResult.Bottom, 5000);
            //    hint.SetToolTip(edResult, tip);
            //}

            Speech.Stop();
            if (Speech.IsReady())
            {
                Speech.Play(text.Split(Speech.LineBreak, StringSplitOptions.RemoveEmptyEntries), culture);
            }
        }

        private void MakeCopyText(bool translate = false)
        {
            try
            {
                if (lvItems.Items.Count <= 0) return;
                if (lvItems.SelectedItems.Count <= 0) return;

                var items = new object[lvItems.SelectedItems.Count];
                lvItems.SelectedItems.CopyTo(items, 0);
                items = items.OrderBy(i => lvItems.Items.IndexOf(i)).ToArray();

                StringBuilder sb = new StringBuilder();
                foreach (var item in items)
                {
                    if (item is ASS.EVENT)
                    {
                        var selected = item as ASS.EVENT;
                        var idx = Convert.ToInt32(selected.ID) - 1;
                        var evt = ass.Events[idx];
                        var t = Regex.Replace(translate ? evt.Translated : evt.Text, @"\\[h|n|N]", " $0 ", RegexOptions.IgnoreCase);
                        sb.AppendLine(t);
                    }
                }
                var text = sb.ToString();
                Clipboard.SetDataObject(text);
            }
            catch (Exception) { }
        }

        private void MakePasteText(bool translate = true)
        {
            if (lvItems.Items.Count <= 0) return;
            if (lvItems.SelectedItems.Count <= 0) return;

            if (Clipboard.ContainsText())
            {
                StringBuilder sb = new StringBuilder();
                var texts = Clipboard.GetText();
                var lines = texts.Split(new string[] { "\n\r", "\r\n", "\r", "\n", }, StringSplitOptions.None);
                if (PasteRemoveNullLine) lines = lines.Where(l => !string.IsNullOrEmpty(l.Trim())).ToArray();

                var items = new object[lvItems.SelectedItems.Count];
                lvItems.SelectedItems.CopyTo(items, 0);
                items = items.OrderBy(i => lvItems.Items.IndexOf(i)).ToArray();

                if (items.Count() > 0) MakeBackup();

                var idx_t = 0;
                foreach (var item in items)
                {
                    if (idx_t >= lines.Length) break;
                    if (item is ASS.EVENT)
                    {
                        var selected = item as ASS.EVENT;
                        var idx = Convert.ToInt32(selected.ID) - 1;
                        if (idx < 0 || idx >= ass.Events.Count) break;
                        var evt = ass.Events[idx];

                        if (PasteRemoveNullLine)
                        {
                            if (string.IsNullOrEmpty(evt.Text.Trim()) && !string.IsNullOrEmpty(lines[idx_t].Trim())) continue;
                            if (!string.IsNullOrEmpty(evt.Text.Trim()) && string.IsNullOrEmpty(lines[idx_t].Trim())) idx_t++;
                        }
                        if (idx_t >= lines.Length) break;

                        if (translate)
                        {
                            evt.Translated = string.IsNullOrEmpty(lines[idx_t]) ? string.Empty : FixBracketingError(lines[idx_t]);
                            events[idx].Translated = evt.Translated;
                        }
                        else
                        {
                            evt.Text = string.IsNullOrEmpty(lines[idx_t]) ? string.Empty : FixBracketingError(lines[idx_t]);
                            events[idx].Text = evt.Text;
                        }

                        idx_t++;
                    }
                }
            }
        }

        private ASS _ass_ = new ASS();
        private List<ASS.EVENT> _events_ = new List<ASS.EVENT>();
        private void MakeBackup(ASS sub = null)
        {
            try
            {
                if (sub is ASS) _ass_ = sub.Clone();
                else if (ass is ASS) _ass_ = ass.Clone();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void MakeUndo()
        {
            try
            {
                if (_ass_ is ASS)
                {
                    ass = _ass_.Clone();

                    events.Clear();
                    for (int i = 0; i < ass.Events.Count; i++)
                    {
                        events.Add(ass.Events[i]);
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }
        #endregion

        #region Config Helper
        private void InitDefaultStyle()
        {
            try
            {
                var props = typeof(AssStyle).GetProperties();
                foreach (var prop in props)
                {
                    if (!prop.PropertyType.Name.Equals("string", StringComparison.CurrentCultureIgnoreCase)) continue;
                    var value = GetConfigValue(prop.Name, prop.GetValue(prop));
                    if (string.IsNullOrEmpty(value.Trim())) continue;
                    prop.SetValue(prop, value);
                }
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}"); }
        }

        private string GetConfigValue(string key, object value = null)
        {
            string result = string.Empty;
            try
            {
                if (appSection is AppSettingsSection)
                {
                    if (appSection.Settings.AllKeys.Contains(key))
                    {
                        var v = appSection.Settings[key].Value;
                        result = string.IsNullOrEmpty(v) ? value.ToString() : v;
                    }
                    else
                    {
                        if (value != null)
                            appSection.Settings.Add(key, value.ToString());
                        else
                            appSection.Settings.Add(key, string.Empty);

                        result = value.ToString();
                        config.Save();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}"); }
            return (result);
        }

        private void SetConfigValue(string key, object value = null)
        {
            string result = string.Empty;
            try
            {
                if (appSection is AppSettingsSection)
                {
                    if (appSection.Settings.AllKeys.Contains(key))
                    {
                        var value_old = appSection.Settings[key].Value;
                        var value_new = value.ToString();
                        if (!value_old.Equals(value_new, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (value != null)
                                appSection.Settings[key].Value = value_new;
                            else
                                appSection.Settings.Remove(key);

                            config.Save();
                        }
                    }
                    else if (value != null)
                    {
                        appSection.Settings.Add(key, value.ToString());
                        config.Save();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}"); }
        }
        
        private void LoadConfig(bool reload=false)
        {
            if (reload)
            {
                config = ConfigurationManager.OpenExeConfiguration(AppExec);
                appSection = config.AppSettings;
            }

            bool.TryParse(GetConfigValue(OriginalEdiable_Key, OriginalEdiable), out OriginalEdiable);
            bool.TryParse(GetConfigValue(SaveWithBOM_Key, SaveWithBOM), out SaveWithBOM);
            bool.TryParse(GetConfigValue(PasteRemoveNullLine_Key, PasteRemoveNullLine), out PasteRemoveNullLine);
            Enum.TryParse(GetConfigValue(SplitCharMode_Key, SplitCharMode.ToString()), out SplitCharMode);
            int.TryParse(GetConfigValue(SplitCharCount_Key, SplitCharCount), out SplitCharCount);
            SplitCharSymbols = GetConfigValue(SplitCharSymbols_Key, SplitCharSymbols);
            MergeString = GetConfigValue(MergeString_Key, MergeString);

            YoutubeLanguage = GetConfigValue(YoutubeLanguage_Key, YoutubeLanguage);

            var lang = SupportedLanguage.ENG;
            if (Enum.TryParse(YoutubeLanguage, out lang))
            {
                cmiLangEng.IsChecked = false;
                cmiLangChs.IsChecked = false;
                cmiLangCht.IsChecked = false;
                cmiLangJpn.IsChecked = false;
                cmiLangKor.IsChecked = false;
                switch (lang)
                {
                    case SupportedLanguage.ENG: cmiLangEng.IsChecked = true; break;
                    case SupportedLanguage.CHS: cmiLangChs.IsChecked = true; break;
                    case SupportedLanguage.CHT: cmiLangCht.IsChecked = true; break;
                    case SupportedLanguage.JPN: cmiLangJpn.IsChecked = true; break;
                    case SupportedLanguage.KOR: cmiLangKor.IsChecked = true; break;
                    default: cmiLangEng.IsChecked = true; break;
                }
            }
            cmiSaveWithBOM.IsChecked = SaveWithBOM;
            cmiPasteRemoveNullLine.IsChecked = PasteRemoveNullLine;

            if (!File.Exists(PhraseFile))
                SavePhrase(PhraseFile, Phrase);
            else
                Phrase = LoadPhrase(PhraseFile);
        }
        #endregion

        private void InvokeControl(object sender)
        {
            try
            {
                FrameworkElementAutomationPeer peer = null;
                //typeof(System.Windows.Controls.Primitives.ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(btnCopy, new object[0]);
                if (sender is Button) peer = new ButtonAutomationPeer(sender as Button);
                else if (sender is MenuItem) peer = new MenuItemAutomationPeer(sender as MenuItem);
                if (peer is FrameworkElementAutomationPeer)
                {
                    IInvokeProvider invokeProv = (peer as FrameworkElementAutomationPeer).GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
                }
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}"); }
        }

        public MainWindow()
        {
            InitializeComponent();

            InInit = false;

            var apppath = Assembly.GetExecutingAssembly().Location;
            Icon = GetIcon(apppath.ToString());
            MainGrid.AllowDrop = true;
            lvItems.ItemsSource = events;
            OriginalTitle = Title;

            InitDefaultStyle();

            LoadConfig();

#if DEBUG
            Phrase.Items.Add(new Term() { Original = "一楼", Translated = "一层" });
            Phrase.Items.Add(new Term() { Original = "二楼", Translated = "二层" });
            Phrase.Items.Add(new Term() { Original = "三楼", Translated = "三层" });
            Phrase.Items.Add(new Term() { Original = "四楼", Translated = "四层" });
#endif
            if (UpdateFindReplaceOptionAction == null) UpdateFindReplaceOptionAction = new Action<FindReplaceOptions>((opt) => { _last_find_replace_option_ = opt; });
            if (FindTextAction == null) FindTextAction = new Action<FindReplaceOptions>((opt) => { FindText(opt); });
            if (ReplaceTextAction == null) ReplaceTextAction = new Action<FindReplaceOptions>((opt) => { ReplaceText(opt); });
            if (FindReplaceResultFunc == null) FindReplaceResultFunc = new Func<string>(() =>
            {
                if (_last_find_replace_option_.Mode == FindReplaceMode.Find)
                {
                    var result = "Not Found!";
                    var _last_find_index_ = _last_find_replace_option_ is FindReplaceOptions ? _last_find_replace_option_.FindResult : -3;
                    if (_last_find_index_ < 0) return (result);
                    else return ($"Found ID : {_last_find_index_ + 1}");
                }
                else if (_last_find_replace_option_.Mode == FindReplaceMode.Replace)
                {
                    return ($"Replaced : {_last_find_replace_option_.ReplaceResult.Where(r => r == true).Count()}");
                }
                else return (string.Empty);
            });

            lvItems.Focus();

            InInit = true;
            try
            {
                var args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    var files = args.Skip(1).Where(f => exts.Contains(Path.GetExtension(f).ToLower()) && File.Exists(f));
                    LoadSubtitle(files);
                }
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}"); }
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
                        LoadSubtitle(dragFiles.Where(f => exts.Contains(Path.GetExtension(f).ToLower()) && File.Exists(f)));
                    }
                }
                catch (Exception ex) { MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}"); }
            }
        }
        #endregion

        private void MainForm_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvItems.SelectedItem != null)
            {
                if (e.ChangedButton == MouseButton.XButton2)
                    lvItems.SelectedIndex = Math.Max(lvItems.SelectedIndex - 1, 0);
                else if (e.ChangedButton == MouseButton.XButton1)
                    lvItems.SelectedIndex = Math.Min(lvItems.SelectedIndex + 1, lvItems.Items.Count);
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e is KeyEventArgs && e.KeyStates == KeyStates.Toggled)
            {
                e.Handled = true;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (e.Key == Key.O) InvokeControl(btnLoad);
                    else if (e.Key == Key.S) InvokeControl(cmiSaveAs);
                    else if (e.Key == Key.M) InvokeControl(btnMerge);
                    else if (e.Key == Key.R) InvokeControl(btnReplace);

                    else if (e.Key == Key.C) InvokeControl(btnCopy);
                    else if (e.Key == Key.V) InvokeControl(btnPaste);

                    else if (e.Key == Key.F) OpenFindReplaceEditor();
                    else if (e.Key == Key.F3) OpenFindReplaceEditor();

                    else if (e.Key == Key.Back) cmiEvents_Click(cmiEventsClear, e);

                    else if (e.Key == Key.Z) MakeUndo();

                    else if (e.Key == Key.X) InvokeControl(cmiExit);
                }

                else if (e.Key == Key.OemOpenBrackets) cmiEvents_Click(cmiEventsSplit, e);
                else if (e.Key == Key.OemCloseBrackets) cmiEvents_Click(cmiEventsMerge, e);
                else if (e.Key == Key.Insert) cmiEvents_Click(cmiEventsAdd, e);
                else if (e.Key == Key.Delete) cmiEvents_Click(cmiEventsDel, e);

                else if (e.Key == Key.Space) Text2Voice(true);
                else if (e.Key == Key.OemComma) Text2Voice(false);
                else if (e.Key == Key.OemPeriod) Text2Voice(true);
                else if (e.Key == Key.Enter) OpenTranslatedEditor();
            }
        }

        private void lvItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e is MouseButtonEventArgs)
            {
                e.Handled = true;
                OpenTranslatedEditor();
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var exist = !string.IsNullOrEmpty(LastFilename) && File.Exists(LastFilename);
            if (sender == cmiLoadASS || (sender == cmiReloadASS && !exist))
            {
                OpenFileDialog dlgOpen = new OpenFileDialog();
                dlgOpen.DefaultExt = ".ass";
                //dlgOpen.Filter = "ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|Text File|*.txt|All File|*.*";
                dlgOpen.Filter = "All Supported File|*.ass;*.ssa;*.srt;*.vtt;*.lrc|ASS File|*.ass|SSA File|*.ssa|SRT File|*.srt|VTT Fils|*.vtt|LRC File|*.lrc";
                dlgOpen.FilterIndex = 0;
                dlgOpen.Multiselect = true;
                if (dlgOpen.ShowDialog() == true)
                {
                    //LoadSubtitle(dlgOpen.FileName);
                    LoadSubtitle(dlgOpen.FileNames);
                }
            }
            else if (sender == cmiReloadASS && exist)
            {
                LoadSubtitle(LastFilename);
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            MakeCopyText(Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            MakePasteText(!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
        }

        private void btnPasteYoutube_Click(object sender, RoutedEventArgs e)
        {
            events.Clear();
            var texts = Clipboard.GetText();
            var lines = texts.Split(new string[] { "\n\r", "\r\n", "\r", "\n", }, StringSplitOptions.None);

            var title = string.Empty;
            var no_title = Regex.IsMatch(lines[0], @"^\d{1,2}:\d{1,2}", RegexOptions.IgnoreCase);
            if (no_title)
            {
                title = "No Title";
                var dlgInput = new InputDialog("Input", title);
                if (dlgInput.ShowDialog() == true)
                {
                    title = dlgInput.Text;
                }
            }
            else
            {
                title = lines[0];
                lines = lines.Skip(1).ToArray();
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
            if (SaveWithBOM)
                SaveSubtitle(ASS.SaveFlags.Merge | ASS.SaveFlags.BOM);
            else
                SaveSubtitle(ASS.SaveFlags.Merge);
        }

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            if (SaveWithBOM)
                SaveSubtitle(ASS.SaveFlags.Replace | ASS.SaveFlags.BOM);
            else
                SaveSubtitle(ASS.SaveFlags.Replace);
        }

        private void cmiPasteRemoveNullLine_Click(object sender, RoutedEventArgs e)
        {
            if (ass == null) return;
            PasteRemoveNullLine = cmiPasteRemoveNullLine.IsChecked;
            SetConfigValue(PasteRemoveNullLine_Key, PasteRemoveNullLine);
        }

        private void cmiSaveWithBOM_Click(object sender, RoutedEventArgs e)
        {
            if (ass == null) return;
            ass.SaveWithUTF8BOM = cmiSaveWithBOM.IsChecked;
            SaveWithBOM = ass.SaveWithUTF8BOM;
            SetConfigValue(SaveWithBOM_Key, SaveWithBOM);
        }

        private void cmiLang_Click(object sender, RoutedEventArgs e)
        {
            if (ass == null) return;
            if (sender == cmiLangEng)
            {
                ass.YoutubeLanguage = SupportedLanguage.ENG;
                cmiLangEng.IsChecked = true;
                cmiLangChs.IsChecked = false;
                cmiLangCht.IsChecked = false;
                cmiLangJpn.IsChecked = false;
                cmiLangKor.IsChecked = false;
            }
            else if (sender == cmiLangChs)
            {
                ass.YoutubeLanguage = SupportedLanguage.CHS;
                cmiLangEng.IsChecked = false;
                cmiLangChs.IsChecked = true;
                cmiLangCht.IsChecked = false;
                cmiLangJpn.IsChecked = false;
                cmiLangKor.IsChecked = false;
            }
            else if (sender == cmiLangCht)
            {
                ass.YoutubeLanguage = SupportedLanguage.CHT;
                cmiLangEng.IsChecked = false;
                cmiLangChs.IsChecked = false;
                cmiLangCht.IsChecked = true;
                cmiLangJpn.IsChecked = false;
                cmiLangKor.IsChecked = false;
            }
            else if (sender == cmiLangJpn)
            {
                ass.YoutubeLanguage = SupportedLanguage.JPN;
                cmiLangEng.IsChecked = false;
                cmiLangChs.IsChecked = false;
                cmiLangCht.IsChecked = false;
                cmiLangJpn.IsChecked = true;
                cmiLangKor.IsChecked = false;
            }
            else if (sender == cmiLangKor)
            {
                ass.YoutubeLanguage = SupportedLanguage.KOR;
                cmiLangEng.IsChecked = false;
                cmiLangChs.IsChecked = false;
                cmiLangCht.IsChecked = false;
                cmiLangJpn.IsChecked = false;
                cmiLangKor.IsChecked = true;
            }
            ass.ChangeStyle(ass.YoutubeLanguage);
            YoutubeLanguage = ass.YoutubeLanguage.ToString();
            SetConfigValue(YoutubeLanguage_Key, YoutubeLanguage);
        }

        private void cmiExit_Click(object sender, RoutedEventArgs e)
        {
            if (sender == cmiReload)
                LoadConfig(reload: true);
            else if (sender == cmiExit)
                Close();
        }

        private void cmiFind_Click(object sender, RoutedEventArgs e)
        {
            OpenFindReplaceEditor();
        }

        private void cmiFixBracket_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                e.Handled = true;
                if (lvItems.Items.Count <= 0) return;
                //if (lvItems.SelectedItems.Count <= 0) return;
                var items = lvItems.SelectedItems.Count <= 0 ? lvItems.Items : lvItems.SelectedItems;
                for (int i = 0; i < items.Count; i++)
                {
                    var idx = lvItems.Items.IndexOf(items[i]);
                    var evt = ass.Events[idx];
                    if (!string.IsNullOrEmpty(evt.Translated))
                        evt.Translated = FixBracketingError(evt.Translated);
                    else if (!string.IsNullOrEmpty(evt.Text))
                        evt.Translated = FixBracketingError(evt.Text);
                }
            }
            catch (Exception) { }
        }

        private void cmiSaveAs_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (SaveWithBOM)
                SaveSubtitle(ASS.SaveFlags.BOM);
            else
                SaveSubtitle(ASS.SaveFlags.None);
        }

        private void cmiPhraseOprate_Click(object sender, RoutedEventArgs e)
        {
            if (sender == cmiEditPhraseTable)
            {
                System.Diagnostics.Process.Start(PhraseFile);
            }
            else if (sender == cmiReloadPhraseTable)
            {
                Phrase = LoadPhrase(PhraseFile);
            }
        }

        private void cmiSetStyle_Click(object sender, RoutedEventArgs e)
        {
            if (lvItems.Items.Count <= 0) return;
            if (lvItems.SelectedItems.Count <= 0) return;

            var style = "Default";
            if (sender == cmiSetStyleDefault) style = "Default";
            else if (sender == cmiSetStyleDefaultM) style = "DefaultM";
            else if (sender == cmiSetStyleDefaultF) style = "DefaultF";
            else if (sender == cmiSetStyleNote) style = "Note";
            else if (sender == cmiSetStyleTitle) style = "Title";
            else if (sender == cmiSetStyleComment) style = "Comment";
            else if (sender is MenuItem)
            {
                var menu = sender as MenuItem;
                if (menu.Tag is string && (menu.Tag as string).StartsWith(CustomStylePrefix))
                {
                    style = (menu.Tag as string).Replace(CustomStylePrefix, "");
                }
            }

            var items = new object[lvItems.SelectedItems.Count];
            lvItems.SelectedItems.CopyTo(items, 0);
            items = items.OrderBy(i => lvItems.Items.IndexOf(i)).ToArray();

            foreach (var item in items)
            {
                if (item is ASS.EVENT)
                {
                    var selected = item as ASS.EVENT;
                    selected.Style = style;
                }
            }
        }

        private void cmiEvents_Click(object sender, RoutedEventArgs e)
        {
            if (lvItems.Items.Count <= 0) return;
            if (lvItems.SelectedItems.Count <= 0) return;

            var items = new object[lvItems.SelectedItems.Count];
            lvItems.SelectedItems.CopyTo(items, 0);
            items = items.OrderBy(i => lvItems.Items.IndexOf(i)).ToArray();

            if (items.Count() > 0) MakeBackup();

            //var symbols = SplitCharSymbols.ToList().Distinct().ToArray();
            var pattern = string.Join("|", SplitCharSymbols.ToList().Distinct().Select(c => $"\\{c}")) + "|\\\\n";

            e.Handled = false;
            if (sender == cmiEventsAdd)
            {
                //e.Handled = true;
            }
            else if (sender == cmiEventsDel)
            {
                foreach (var item in items) { ass.Events.Remove(item as ASS.EVENT); events.Remove(item as ASS.EVENT); }
                e.Handled = true;
            }
            else if (sender == cmiEventsMerge)
            {
                var kvs = items.Select(i => new KeyValuePair<int, ASS.EVENT>(lvItems.Items.IndexOf(i), i as ASS.EVENT));
                var last_idx = -1;
                var groups = new List<List<KeyValuePair<int, ASS.EVENT>>>();
                //if (groups.Count == 0) groups.Add(new List<KeyValuePair<int, ASS.EVENT>>());

                foreach (var kv in kvs)
                {
                    var k = kv.Key;
                    var v = kv.Value;

                    if (last_idx < 0 || k - last_idx > 1) groups.Add(new List<KeyValuePair<int, ASS.EVENT>>());
                    groups.Last().Add(kv);
                    last_idx = k;
                }
                foreach (var group in groups)
                {
                    if (group.Count > 1)
                    {
                        var first = group.First().Value;
                        var last = group.Last().Value;
                        first.End = last.End;
                        first.Text = Regex.Replace(string.Join(MergeString, group.Select(evt => evt.Value.Text).Distinct()), $@"({pattern})+", "$1", RegexOptions.IgnoreCase);
                        foreach (var item in group.Skip(1))
                        {
                            ass.Events.Remove(item.Value);
                            events.Remove(item.Value);
                        }
                    }
                }                
                e.Handled = true;
            }
            else if (sender == cmiEventsSplit)
            {
                var shift = Keyboard.Modifiers == ModifierKeys.Shift;
                var split_mode = SplitCharMode;
                var time_fmt = "HH:mm:ss.ff";

                switch (SplitCharMode)
                {
                    case SplitMode.ByCount:
                        split_mode = shift ? SplitMode.BySymbol : SplitMode.ByCount;
                        break;
                    case SplitMode.BySymbol:
                        split_mode = shift ? SplitMode.ByCount : SplitMode.BySymbol;
                        break;
                }

                foreach (var item in items.Reverse())
                {
                    var done = false;
                    var evt = item as ASS.EVENT;
                    var idx = Convert.ToInt32(evt.ID) - 1;

                    var evts = new List<ASS.EVENT>();

                    if (split_mode == SplitMode.ByCount)
                    {
                        var length = evt.Text.Length;
                        if (length <= SplitCharCount) continue;
                        var total = length % SplitCharCount == 0 ? evt.Text.Length / SplitCharCount : evt.Text.Length / SplitCharCount + 1;
                        if (total <= 1) continue;
                        var times = evt.EndTime - evt.StartTime;
                        if (times.TotalMilliseconds <= 0) continue;
                        var times_ms = times.TotalMilliseconds / total;

                        for (var i = 0; i < total; i++)
                        {
                            var start = evt.StartTime;
                            var evt_new  = evt.Clone();

                            evt_new.Text = evt.Text.Substring(i * SplitCharCount, Math.Min(length - i * SplitCharCount, SplitCharCount));
                            evt_new.Translated = evt.Translated;
                            evt_new.Start = (start + TimeSpan.FromMilliseconds(i * times_ms)).ToString(time_fmt);
                            evt_new.End = (start + TimeSpan.FromMilliseconds((i + 1) * times_ms - 10)).ToString(time_fmt);
                            evts.Add(evt_new);
                        }
                        done = true;
                    }
                    else if (split_mode == SplitMode.BySymbol)
                    {
                        var line  = Regex.Replace(evt.Text, $@"({pattern})", "$1\n", RegexOptions.IgnoreCase);
                        var lines = line.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(l => !l.Trim().Equals("\\n", StringComparison.CurrentCultureIgnoreCase)).ToList();
                        var total = lines.Count();
                        if (total > 1)
                        {
                            var times = evt.EndTime - evt.StartTime;
                            if (times.TotalMilliseconds <= 0) continue;
                            var times_ms = times.TotalMilliseconds / total;

                            for (var i = 0; i < total; i++)
                            {
                                var start = evt.StartTime;
                                var evt_new  = evt.Clone();

                                evt_new.Text = Regex.Replace(lines[i], @"\\n$", "", RegexOptions.IgnoreCase);
                                evt_new.Translated = evt.Translated;
                                evt_new.Start = (start + TimeSpan.FromMilliseconds(i * times_ms)).ToString(time_fmt);
                                evt_new.End = (start + TimeSpan.FromMilliseconds((i + 1) * times_ms - 10)).ToString(time_fmt);
                                evts.Add(evt_new);
                            }
                            done = true;
                        }
                    }

                    if (done)
                    {
                        ass.Events.InsertRange(idx, evts);
                        ass.Events.Remove(evt);
                        evts.Reverse();
                        foreach (var ev in evts) events.Insert(idx, ev);
                        events.Remove(evt);
                    }
                }
                e.Handled = true;
            }
            else if (sender == cmiEventsClear)
            {
                foreach (var item in items)
                {
                    var evt = item as ASS.EVENT;
                    var idx = Convert.ToInt32(evt.ID) - 1;
                    if (idx < 0 || idx >= ass.Events.Count) break;

                    evt.Translated = string.Empty;
                    ass.Events[idx].Translated = string.Empty;
                }
                e.Handled = true;
            }
            if (e.Handled) { for (var i = 0; i < ass.Events.Count; i++) { events[i].ID = $"{i + 1}"; ass.Events[i].ID = $"{i + 1}"; }; }
            if (e.Handled && _last_find_replace_option_ is FindReplaceOptions)
            {
                _last_find_replace_option_.FindResult = 0;
                if (_last_find_replace_option_.ReplaceResult is List<bool>) _last_find_replace_option_.ReplaceResult.Clear();
            }
        }

    }
}
