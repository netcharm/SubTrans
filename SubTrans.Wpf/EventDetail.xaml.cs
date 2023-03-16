using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// EventDetail.xaml 的交互逻辑
    /// </summary>
    public partial class EventDetail : Window
    {
        public ASS.EVENT Event { get; set; } = null;
        public bool OriginalEditable { get; set; } = false;

        private string LINE_BREAK = @"\n\r|\r\n|\r|\n";

        private void EventChange()
        {
            if (!Event.Text.Equals(OriginalContent.Text)) Event.Text = Regex.Replace(OriginalContent.Text, LINE_BREAK, @"\N", RegexOptions.IgnoreCase);
            Event.Translated = TransContent.Text.Replace(Environment.NewLine, "\\N");
            DialogResult = true;
        }

        private void EventCancel()
        {
            DialogResult = false;
        }

        public EventDetail()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Event is ASS.EVENT)
            {
                chkOriginalEditable.IsChecked = OriginalEditable;
                OriginalContent.IsReadOnly = !OriginalEditable;
                OriginalContent.Text = Regex.Replace(Event.Text, @"\\N", Environment.NewLine, RegexOptions.IgnoreCase);
                TransContent.Text = Regex.Replace(Event.Translated, @"\\N", Environment.NewLine, RegexOptions.IgnoreCase); 
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (Keyboard.Modifiers == ModifierKeys.Control && (e.Key == Key.Enter || e.SystemKey == Key.Enter)) EventChange();
            else if (e.Key == Key.Escape || e.SystemKey == Key.Escape) EventCancel();
        }

        private void Window_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.XButton1)
            {
                DialogResult = false;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            EventCancel();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (Event is ASS.EVENT) EventChange();
        }

        private string SpeechText = string.Empty;
        private void EventContent_GotFocus(object sender, RoutedEventArgs e)
        {
            if (OriginalContent.IsFocused) SpeechText = OriginalContent.SelectionLength > 0 ? OriginalContent.SelectedText : OriginalContent.Text;
            else if (TransContent.IsFocused) SpeechText = TransContent.SelectionLength > 0 ? TransContent.SelectedText : TransContent.Text;
            else SpeechText = string.Empty;
        }

        private void btnSpeech_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnSpeechAuto)
            {
                MainWindow.Text2Voice(SpeechText);
            }
            else if (sender == btnSpeechCHS)
            {
                MainWindow.Text2Voice(SpeechText, "zh-hans");
            }
            else if (sender == btnSpeechCHT)
            {
                MainWindow.Text2Voice(SpeechText, "zh-hant");
            }
            else if (sender == btnSpeechJAP)
            {
                MainWindow.Text2Voice(SpeechText, "ja");
            }
            else if (sender == btnSpeechKOR)
            {
                MainWindow.Text2Voice(SpeechText, "ko");
            }
            else if (sender == btnSpeechENG)
            {
                MainWindow.Text2Voice(SpeechText, "en");
            }
            else if (sender == chkOriginalEditable)
            {
                if (sender is CheckBox)
                {
                    OriginalEditable = (sender as CheckBox).IsChecked ?? false;
                    OriginalContent.IsReadOnly = !OriginalEditable;
                }
            }            
        }

    }
}
