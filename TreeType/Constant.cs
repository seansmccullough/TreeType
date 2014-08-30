using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using VirtualInput;

namespace TreeType
{
    class Constant
    {
        public static SolidColorBrush defaultColor = new SolidColorBrush(Colors.Black);
        public static SolidColorBrush selectedBackgroundColor = new SolidColorBrush(Colors.IndianRed);
        public static SolidColorBrush letterBackgroundColor = new SolidColorBrush(Colors.White);
        public static SolidColorBrush numberBackgroundColor = new SolidColorBrush(Colors.CadetBlue);
        public static SolidColorBrush symbolBackgroundColor = new SolidColorBrush(Colors.LightGreen);
        public static SolidColorBrush specialBackgroundColor = new SolidColorBrush(Colors.Yellow);
        public static SolidColorBrush whiteColor = new SolidColorBrush(Colors.White);
        public static SolidColorBrush transparentColor = new SolidColorBrush(Colors.Transparent);
        public static int defaultFontSize = 17;
        public static int defaultHeight = 25;
        public static int defaultWidth = 25;
        public static int defaultTextOffsetX = 2;
        public static int defaultTextOffsetY = 2;
        public static int defaultLineLength = 22;
        public static int defaultLineOffset = 7;
        public static int lineWidth = 1;
        public static int centerX = (int)(System.Windows.SystemParameters.FullPrimaryScreenWidth * 0.9);
        public static int centerY = (int)(System.Windows.SystemParameters.FullPrimaryScreenHeight * 0.5);
        public static int threshold = 75;
        public const int silderMin = 6;
        public const int silderMax = 24;
    }
}
