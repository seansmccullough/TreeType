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

namespace TreeType
{
    /// <summary>
    /// Interaction logic for ToggleWindow.xaml
    /// </summary>
    public partial class ToggleWindow : Window
    {
        public static bool settings = false;
        public ToggleWindow()
        {
            InitializeComponent();
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - System.Windows.SystemParameters.FullPrimaryScreenWidth / 4;
            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight / 4;
            //this.Top = 100;
            this.Loaded += startup;
        }

        protected void startup(Object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_STYLE, 
                NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_STYLE) & ~NativeMethods.WS_SYSMENU);
        }

        void exitListener(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        void settingsListener(object sender, RoutedEventArgs e)
        {
            if(!settings)
            {
                Window settingsWindow = new SettingsWindow();
                settingsWindow.Show();
                settings = true;
                this.Hide();
            }
            
        }
    }
}
