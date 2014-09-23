using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using VirtualInput;

namespace TreeType.views
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - System.Windows.SystemParameters.FullPrimaryScreenWidth / 3;
            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight / 4;
            this.Loaded += startup;
            versionText.Text = "TreeType Copywrite 2014 Sean McCullough";
        }

        protected void startup(Object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_STYLE,
                NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_STYLE) & ~NativeMethods.WS_SYSMENU);
        }

        void okListener(object sender, RoutedEventArgs e)
        {
            MainWindow.toggleWindow.Show();
            this.Close();
        }
    }
}
