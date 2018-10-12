using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SubTitles
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class InputDialog : Window
    {
        public string Text
        {
            get { return edInput.Text; }
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;

        private void InputDialogWindow_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper((Window)sender).Handle;
            var value = GetWindowLong(hwnd, GWL_STYLE);
            //SetWindowLong(hwnd, GWL_STYLE, (int)(value & ~WS_MAXIMIZEBOX));
        }

        public InputDialog(string title="", string defaultValue="")
        {
            InitializeComponent();           
            Icon = Application.Current.MainWindow.Icon;
            lblInput.Content = $"{title}:";
            edInput.Text = defaultValue;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            // Do something with the Input
            this.DialogResult = true;
        }

        private void InputDialogWindow_ContentRendered(object sender, EventArgs e)
        {
            edInput.SelectAll();
            edInput.Focus();
        }
    }
}
