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
    /// EventDetail.xaml 的交互逻辑
    /// </summary>
    public partial class EventDetail : Window
    {
        public ASS.EVENT Event { get; set; } = null;

        public EventDetail()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Event is ASS.EVENT)
            {
                OriginalContent.Text = Event.Text;
                TransContent.Text = Event.Translated;
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (e.Key == Key.Escape || e.SystemKey == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter || e.SystemKey == Key.Enter) DialogResult = true;
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
            DialogResult = false;
            //Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (Event is ASS.EVENT)
            {
                Event.Translated = TransContent.Text;
                DialogResult = true;
            }
            //Close();
        }

        private string SpeechText = string.Empty;
        private void EventContent_GotFocus(object sender, RoutedEventArgs e)
        {
            if (OriginalContent.IsFocused) SpeechText = OriginalContent.Text;
            else if (TransContent.IsFocused) SpeechText = TransContent.Text;
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
            else if(sender == btnSpeechCHT)
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
        }

    }
}
