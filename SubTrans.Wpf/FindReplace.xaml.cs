using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SubTrans
{
    /// <summary>
    /// FindReplace.xaml 的交互逻辑
    /// </summary>
    public partial class FindReplace : Window
    {
        public FindReplaceOptions Options { get; set; } = new FindReplaceOptions();
        public string TextToFind
        {
            get { return (FindText is TextBox ? FindText.Text : string.Empty); }
            set { if (FindText is TextBox) FindText.Text = value; }
        }
        public string TextToReplace
        {
            get { return (ReplaceText is TextBox ? ReplaceText.Text : string.Empty); }
            set { if (ReplaceText is TextBox) ReplaceText.Text = value; }
        }

        public Action<FindReplaceOptions> UpdateOptionAction { get; set; } = null;
        public Action<FindReplaceOptions> FindTextAction { get; set; } = null;
        public Action<FindReplaceOptions> ReplaceTextAction { get; set; } = null;
        public Func<string> FindReplaceResultFunc { get; set; } = null;

        public FindReplace()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //FindText.Text = TextToFind;
            //ReplaceText.Text = TextToReplace;
            FindReplaceOptionsIgnoreCase.IsChecked = Options.IgnoreCase;
            FindReplaceOptionsUseRegex.IsChecked = Options.UseRegex;

            FindText.Focus();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.SystemKey == Key.Escape)
            {
                if (!(Options is FindReplaceOptions)) Options = new FindReplaceOptions();
                Options.IgnoreCase = FindReplaceOptionsIgnoreCase.IsChecked ?? false;
                Options.UseRegex = FindReplaceOptionsUseRegex.IsChecked ?? false;
                Options.TextToFind = FindText.Text;
                Options.TextToReplace = ReplaceText.Text;
                if (UpdateOptionAction is Action<FindReplaceOptions>) UpdateOptionAction.Invoke(Options);
                Close();
            }
        }

        private void FindReplace_Click(object sender, RoutedEventArgs e)
        {
            if (!(Options is FindReplaceOptions)) Options = new FindReplaceOptions();
            Options.IgnoreCase = FindReplaceOptionsIgnoreCase.IsChecked ?? false;
            Options.UseRegex = FindReplaceOptionsUseRegex.IsChecked ?? false;
            Options.TextToFind = FindText.Text;
            Options.TextToReplace = ReplaceText.Text;

            if (sender == FindInAll && FindTextAction is Action<FindReplaceOptions> && FindReplaceResultFunc is Func<string>)
            {
                Options.Mode = FindReplaceMode.Find;
                Options.Range = FindReplaceRangeMode.All;
                FindTextAction.Invoke(Options);
                FindReplaceStatus.Text = FindReplaceResultFunc.Invoke();
            }
            else if (sender == FindInSelected && FindTextAction is Action<FindReplaceOptions> && FindReplaceResultFunc is Func<string>)
            {
                Options.Mode = FindReplaceMode.Find;
                Options.Range = FindReplaceRangeMode.Selected;
                FindTextAction.Invoke(Options);
                FindReplaceStatus.Text = FindReplaceResultFunc.Invoke();
            }
            else if (sender == ReplaceCurrent && ReplaceTextAction is Action<FindReplaceOptions> && FindReplaceResultFunc is Func<string>)
            {
                Options.Mode = FindReplaceMode.Replace;
                Options.Range = FindReplaceRangeMode.Current;
                ReplaceTextAction.Invoke(Options);
                FindReplaceStatus.Text = FindReplaceResultFunc.Invoke();
            }
            else if (sender == ReplaceInAll && ReplaceTextAction is Action<FindReplaceOptions> && FindReplaceResultFunc is Func<string>)
            {
                Options.Mode = FindReplaceMode.Replace;
                Options.Range = FindReplaceRangeMode.All;
                ReplaceTextAction.Invoke(Options);
                FindReplaceStatus.Text = FindReplaceResultFunc.Invoke();
            }
            else if (sender == ReplaceInSelected && ReplaceTextAction is Action<FindReplaceOptions> && FindReplaceResultFunc is Func<string>)
            {
                Options.Mode = FindReplaceMode.Replace;
                Options.Range = FindReplaceRangeMode.Selected;
                ReplaceTextAction.Invoke(Options);
                FindReplaceStatus.Text = FindReplaceResultFunc.Invoke();
            }
        }

    }
}
