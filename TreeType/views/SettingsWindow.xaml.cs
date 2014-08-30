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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        
        private const int silderTick = 2;
        public SettingsWindow()
        {
            InitializeComponent();
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - System.Windows.SystemParameters.FullPrimaryScreenWidth / 3;
            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight / 4;
            this.Loaded += startup;
            SensitivitySlider.Minimum = Constant.silderMin;
            SensitivitySlider.Maximum = Constant.silderMax;
            SensitivitySlider.TickFrequency = silderTick;

            //Properties.Settings.Default.Sensitivity maps directly to SenesitivitySlider.Value
            SensitivitySlider.Value = Properties.Settings.Default.Sensitivity;
        }
        protected void startup(Object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_STYLE, 
                NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_STYLE) & ~NativeMethods.WS_SYSMENU);
        }
        void okListener(object sender, RoutedEventArgs e)
        {
            Constant.threshold = (int)((((double)(SensitivitySlider.Value)) / 100)
                * NativeMethods.GetSystemMetrics(NativeMethods.Y_SCREEN));
            Properties.Settings.Default.Sensitivity = (int)SensitivitySlider.Value;
            Properties.Settings.Default.Save();
            ToggleWindow.settings = false;
            MainWindow.toggleWindow.Show();
            this.Close();
        }
        //theses are inverted!  Higher percentage means less sensitive
        void minusListener(object sender, RoutedEventArgs e)
        {
            if (SensitivitySlider.Value < Constant.silderMax)
            {
                SensitivitySlider.Value++;
            }
        }
        void plusListener(object sender, RoutedEventArgs e)
        {

            if (SensitivitySlider.Value > Constant.silderMin)
            {
                SensitivitySlider.Value--;
            }
        }
    }
}
