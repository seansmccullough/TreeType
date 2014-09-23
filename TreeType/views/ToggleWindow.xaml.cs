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
using System.ComponentModel;
using VirtualInput;
using TreeType.views;

namespace TreeType
{
    /// <summary>
    /// Interaction logic for ToggleWindow.xaml
    /// </summary>
    public partial class ToggleWindow : Window
    {
        public ToggleWindow()
        {
            InitializeComponent();
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - System.Windows.SystemParameters.FullPrimaryScreenWidth / 4;
            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight / 4;
            this.Closing += closeListener;
        }
        private void closeListener(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void helpListener(object sender, RoutedEventArgs e)
        {
            Window helpWindow = new HelpWindow();
            helpWindow.Show();
            this.Hide();
        }

        private void settingsListener(object sender, RoutedEventArgs e)
        {
            Window settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            this.Hide();
        }
    }
}
