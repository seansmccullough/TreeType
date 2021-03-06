﻿using System;
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
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth * 0.67;
            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight * 0.25;
            this.Loaded += startup;
            Version version = Helper.GetPublishedVersion();

            versionText.Text = "TreeType "+version.Major+"."+version.Minor+"."+version.Build+"."+version.Revision+"\nCopywrite 2014 Sean McCullough";
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
