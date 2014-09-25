using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using VirtualInput;
using TreeType.autocomplete;
namespace TreeType
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Dictionary<QuadNode, VisualNode> dictionary;
        private Tree keyboard;

        private double dpiX;
        private double dpiY;
        private int centerX;
        private int centerY;

        //toggle pass through/intercept mode
        private bool passThrough = true;
        
        //if an edge has already been triggered
        //private bool edge = false;
        //toggle mouse/keyboard mode
        private bool mouse = true;

        //private long startRightHold = 0;

        private bool toggledOff = false;

        private System.Windows.Threading.DispatcherTimer timer;

        private Stack<string> stack;
        private Stack<string> tempStack;

        //if auto correct words should be capitalized
        private bool capts = false;

        private Trie trie;

        public static ToggleWindow toggleWindow;
        
        public MainWindow()
        {
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, Constant.rightClickHoldTime);
            timer.Tick += new EventHandler(TimerEvent);
            stack = new Stack<string>();
            tempStack = new Stack<string>();
            this.Loaded += startup;
            InitializeComponent();
            this.Closing += MainWindow_Closing;
            
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth * 0.75 - this.Width * 0.5;
            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight * 0.55 - this.Width * 0.5;
            this.Cursor = System.Windows.Input.Cursors.None;
            try
            {
                trie = new Trie(TreeType.Properties.Resources.words10k);
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show("Unable to load words10k.txt: " + e.Message);
                this.Close();
            }
            keyboard = new Tree();
            toggleWindow = new ToggleWindow();
            toggleWindow.Show();
            this.Hide();
            try
            {
                keyboard.loadFromFile(TreeType.Properties.Resources.mainTreeKeyboard);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Unable to load mainTreeKeyboard.txt: " + e.Message);
                this.Close();
            }
            renderKeyboard(false);
            if (mouse)
            {
                try
                {
                    VirtualInput.VirtualKeyboard.StartMouseInterceptor();
                    VirtualInput.VirtualKeyboard.mouseEventHandler += new VirtualInput.MouseEvent(MouseHandler);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("error rendering keyboard: " + e.Message);
                    this.Close();
                }
            }
            else
            {
                try
                {
                    VirtualInput.VirtualKeyboard.StartKeyboardInterceptor();
                    VirtualInput.VirtualKeyboard.keyPressHandler += new VirtualInput.Win32KeyPress(OnButtonKeyDown);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Error rendering keyboard: " + e.Message);
                    this.Close();
                }
            }
        }
        protected void startup(Object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_STYLE,
                NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_STYLE) & ~NativeMethods.WS_SYSMENU);
            PresentationSource source = PresentationSource.FromVisual(this);
            dpiX = source.CompositionTarget.TransformToDevice.M11;
            dpiY = source.CompositionTarget.TransformToDevice.M22;
            centerX = (int)(System.Windows.SystemParameters.FullPrimaryScreenWidth * 0.75 * dpiX);
            centerY = (int)(System.Windows.SystemParameters.FullPrimaryScreenHeight * 0.55 * dpiY);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VirtualInput.VirtualKeyboard.StopInterceptor();
        }

        private void renderKeyboard(Boolean grid)
        {
            if(grid)
            {

            }
            else
            { 
                keyboard.root.rendered = true;

                VisualNode root = new VisualNode(
                    keyboard.root, 
                    Convert.ToInt32(Canvas.Width / 2), 
                    Convert.ToInt32(Canvas.Height / 2), 
                    null, 
                    Canvas);

                root.toggleSelected();
                //dictionary.Add(keyboard.Root, Root);
                keyboard.root.visualNode = root;

                drawChildren(
                    keyboard.root,
                    Convert.ToInt32(Canvas.Width / 2),
                    Convert.ToInt32(Canvas.Height / 2));
            }
        }
        // current refers to parent node
        private void drawChildren(QuadNode current, int x, int y)
        {
            //calculate line length for this level
            int factor;
            int passThoughFactor;
            if (current.depth == 1 || current.depth == 2) factor = 0;
            else factor = Convert.ToInt32(Math.Pow(2.0, current.depth - 3));
            passThoughFactor = Convert.ToInt32(Math.Pow(2.0, current.depth - 5));
            int passThoughLineLength = Constant.defaultLineLength * passThoughFactor + Constant.defaultLineOffset + (Constant.defaultLineOffset * passThoughFactor);
            int nonPassThoughLineLength = Constant.defaultLineLength * factor + Constant.defaultLineOffset + (Constant.defaultLineOffset * factor);
            if (current.depth == keyboard.maxDepth) nonPassThoughLineLength += 2*Constant.defaultLineLength;
            int lineLength = 0;

            //up
            if((current[Direction.Up] != null) && (!current[Direction.Up].rendered))
            {
                current[Direction.Up].rendered = true;

                if (current[Direction.Up].passThroughNode) lineLength = passThoughLineLength;
                else if (current.passThroughNode) lineLength = nonPassThoughLineLength - Constant.defaultLineLength;
                else lineLength = nonPassThoughLineLength;

                Line newLine = lineBuilder(
                    x,
                    y - Convert.ToInt32(current.height * Constant.defaultHeight) / 2,
                    x,
                    y - lineLength - Convert.ToInt32(current.height * Constant.defaultHeight) / 2);
                

                VisualNode node = new VisualNode(
                    current[Direction.Up],
                    x,
                    y - lineLength - Convert.ToInt32(Constant.defaultHeight * current[Direction.Up].height) / 2 - Convert.ToInt32(Constant.defaultHeight * current.height) / 2, 
                    newLine, 
                    Canvas);
                
                current[Direction.Up].visualNode = node;

                drawChildren(
                    current[Direction.Up], 
                    x,
                    y - lineLength - Convert.ToInt32(Constant.defaultHeight * current[Direction.Up].height) / 2 - Convert.ToInt32(Constant.defaultHeight * current.height) / 2);
            }
            //right
            if ((current[Direction.Right] != null) && (!current[Direction.Right].rendered))
            {
                current[Direction.Right].rendered = true;

                if (current[Direction.Right].passThroughNode) lineLength = passThoughLineLength;
                else if (current.passThroughNode) lineLength = nonPassThoughLineLength - Constant.defaultLineLength;
                else lineLength = nonPassThoughLineLength;

                Line newLine = lineBuilder(
                    x + Convert.ToInt32(current.width * Constant.defaultWidth) / 2,
                    y,
                    x + lineLength + Convert.ToInt32(current.width * Constant.defaultWidth) / 2,
                    y);

                VisualNode node = new VisualNode(
                    current[Direction.Right],
                    x + lineLength + Convert.ToInt32(current.width * Constant.defaultWidth) / 2 + Convert.ToInt32(current[Direction.Right].width * Constant.defaultWidth) / 2,
                    y,
                    newLine, 
                    Canvas);

                current[Direction.Right].visualNode = node;
                drawChildren(
                    current[Direction.Right],  
                    x + lineLength + Convert.ToInt32(current.width * Constant.defaultWidth) / 2 + Convert.ToInt32(current[Direction.Right].width * Constant.defaultWidth) / 2, 
                    y);
            }
            //down
            if ((current[Direction.Down] != null) && (!current[Direction.Down].rendered))
            {
                current[Direction.Down].rendered = true;

                if (current[Direction.Down].passThroughNode) lineLength = passThoughLineLength;
                else if (current.passThroughNode) lineLength = nonPassThoughLineLength - Constant.defaultLineLength;
                else lineLength = nonPassThoughLineLength;

                Line newLine = lineBuilder(
                    x,
                    y + Convert.ToInt32(current.height * Constant.defaultHeight) / 2,
                    x,
                    y + lineLength + Convert.ToInt32(current.height * Constant.defaultHeight) / 2);

                VisualNode node = new VisualNode(
                    current[Direction.Down], 
                    x,
                    y + lineLength + Convert.ToInt32(Constant.defaultHeight * current[Direction.Down].height) / 2 + Convert.ToInt32(Constant.defaultHeight * current.height) / 2, 
                    newLine, 
                    Canvas);

                current[Direction.Down].visualNode = node;
                drawChildren(current[Direction.Down], x, y + lineLength + Convert.ToInt32(Constant.defaultHeight * current[Direction.Down].height) / 2 + Convert.ToInt32(Constant.defaultHeight * current.height) / 2);
            }
            //left
            if ((current[Direction.Left] != null) && (!current[Direction.Left].rendered))
            {
                current[Direction.Left].rendered = true;

                if (current[Direction.Left].passThroughNode) lineLength = passThoughLineLength;
                else if (current.passThroughNode) lineLength = nonPassThoughLineLength - Constant.defaultLineLength;
                else lineLength = nonPassThoughLineLength;

                Line newLine = lineBuilder(
                    x - Convert.ToInt32(current.width * Constant.defaultWidth) / 2, 
                    y,
                    x - lineLength - Convert.ToInt32(current.width * Constant.defaultWidth) / 2 - Convert.ToInt32(current[Direction.Left].width * Constant.defaultWidth) / 2, 
                    y);

                VisualNode node = new VisualNode(
                    current[Direction.Left],
                    x - lineLength - Convert.ToInt32(current.width * Constant.defaultWidth) / 2 - Convert.ToInt32(current[Direction.Left].width * Constant.defaultWidth) / 2, 
                    y, 
                    newLine, 
                    Canvas);

                current[Direction.Left].visualNode = node;
                drawChildren(current[Direction.Left], x - lineLength - Convert.ToInt32(current.width * Constant.defaultWidth) / 2 - Convert.ToInt32(current[Direction.Left].width * Constant.defaultWidth) / 2, y);
            }
        }
        private Line lineBuilder(int x1, int y1, int x2, int y2)
        {
            Line newLine = new Line();
            newLine.Stroke = Constant.defaultColor;
            newLine.StrokeThickness = Constant.lineWidth;
            newLine.X1 = x1;
            newLine.X2 = x2;
            newLine.Y1 = y1;
            newLine.Y2 = y2;
            newLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            newLine.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Canvas.Children.Add(newLine);
            return newLine;
        }
        private void TimerEvent(Object o, EventArgs e)
        {
            timer.Stop();
            togglePassThough();
            toggledOff = true;
        }
        public bool MouseHandler(VirtualInput.MSLLHOOKSTRUCT e, Int32 wParam)
        {
            if (wParam == VirtualInput.NativeMethods.WM_RBUTTONDOWN)
            {
                if (!timer.IsEnabled)
                {
                    timer.Start();
                    toggledOff = false;
                }
                return true;
            }
            else if (wParam == VirtualInput.NativeMethods.WM_RBUTTONUP)
            {
                timer.Stop();
                if (passThrough && !toggledOff) NativeMethods.rightClick();
                return true;
            }
            else if(!passThrough)
            {
                if (wParam == VirtualInput.NativeMethods.WM_LBUTTONDOWN)
                {
                    OnButtonKeyDown(Keys.Enter);
                }
                else if (e.pt.x < (centerX - TreeType.Constant.threshold))
                {
                    keyboard.left();
                }
                else if (e.pt.x > (centerX + TreeType.Constant.threshold))
                {
                    keyboard.right();
                }
                else if (e.pt.y < (centerY - TreeType.Constant.threshold))
                {
                    keyboard.up();
                }
                else if (e.pt.y > (centerY + TreeType.Constant.threshold))
                {
                    keyboard.down();        
                }
                else
                {
                    return false;
                }
                VirtualInput.NativeMethods.moveMouse(centerX, centerY);
                return true;
            }
            return false;
        }

        private void togglePassThough()
        {
            //show keyboard
            if(passThrough)
            {
                NativeMethods.leftClick();
                this.Show();
                toggleWindow.Hide();
                if(!keyboard.isShifted) keyboard.toggleShift();
                stack.Clear();
                Constant.autoSentenceEnd = Properties.Settings.Default.AutoSentenceEnd;
                /*Properties.Settings.Default.Sensitivity is the vertical % of the screen cursor must move
              to trigger an edge event (left, right, up, down).  Note is it an int, so 1 represents 1% or 0.01
             
              Constant.threshold measures number of pixels cursor must move to trigger an edge event
              Constant.threshold is not persistant*/
                Constant.threshold = (int)((NativeMethods.GetSystemMetrics(NativeMethods.Y_SCREEN)
                    * Properties.Settings.Default.Sensitivity / 100));
            }
            //hide keyboard
            else
            {
                clearAutos();
                keyboard.word = "";
                toggleWindow.Show();
                this.Hide();
            }
            passThrough = !passThrough;
        }
        private void auto(string s)
        {
            //if first word being typed, or last character wasn't a letter
            //if ((keyboard.isShifted && stack.Count == 0) || (keyboard.isShifted && stack.Count > 0 && stack.Peek()[stack.Peek().Length - 1] <= 97 && stack.Peek()[stack.Peek().Length - 1] >= 122))
            if(keyboard.isShifted)
            {
                capts = true;
            }
            String[] suggestions = trie.top(s, keyboard.autoCompletes.Count);

            for (int i = 0; i < suggestions.Length; i++)
            {
                if(suggestions[i] != null && suggestions[i] != "")
                {
                    if (capts)
                    {
                        keyboard.autoCompletes.ElementAt(i).content = Char.ToUpper(suggestions[i][0]) + suggestions[i].Substring(1);
                    }
                    else
                    {
                        keyboard.autoCompletes.ElementAt(i).content = suggestions[i];
                    }
                }
                else
                {
                    keyboard.autoCompletes.ElementAt(i).content = "";
                }
                keyboard.autoCompletes.ElementAt(i).visualNode.replace(keyboard.autoCompletes.ElementAt(i).content);
            }
        }
        private void clearAutos()
        {
            capts = false;
            keyboard.clearAutos();
        }
        private bool OnButtonKeyDown(Keys key)
        {
            //enter
            if (key == Keys.Enter)
            {
                //shift 
                if (keyboard.current.content == "shift")
                {
                    keyboard.toggleShift();
                }
                //space 
                else if(keyboard.current.keyCode == 32)
                {
                    if (Constant.autoSentenceEnd && (stack.Count > 2))
                    {
                        string last = stack.Pop();
                        string penultimate = stack.Pop();
                        stack.Push(penultimate);
                        stack.Push(last);
                        if ((last == " ") && (penultimate[penultimate.Length - 1] > 96) && (penultimate[penultimate.Length - 1] < 123))
                        {
                            NativeMethods.KeyPress(8);
                            NativeMethods.KeyPress(190);
                            stack.Push(".");
                            NativeMethods.KeyPress(32);
                            NativeMethods.KeyPress(32);
                            stack.Push(Convert.ToString(Char.ToLower(Convert.ToChar(keyboard.current.keyCode))));
                            stack.Push(Convert.ToString(Char.ToLower(Convert.ToChar(keyboard.current.keyCode))));
                            keyboard.toggleShift();
                        }
                        else
                        {
                            NativeMethods.KeyPress(keyboard.current.keyCode);
                            stack.Push(Convert.ToString(Char.ToLower(Convert.ToChar(keyboard.current.keyCode))));
                        }
                    }
                    else
                    {
                        NativeMethods.KeyPress(keyboard.current.keyCode);
                        stack.Push(Convert.ToString(Char.ToLower(Convert.ToChar(keyboard.current.keyCode))));
                    }
                    keyboard.word = "";
                    capts = false;
                    clearAutos();
                    return true;
                }
                //autocomplete 
                else if(keyboard.current.type == QuadNode.Type.auto)
                {
                    if (keyboard.current.content == "" || keyboard.current.content == "0") return true;
                    string temp = keyboard.current.content.Substring(keyboard.word.Length, keyboard.current.content.Length - keyboard.word.Length);
                    NativeMethods.type(temp);
                    keyboard.word += temp;
                    stack.Push(temp);
                    auto(keyboard.word);

                    if (keyboard.isShifted) keyboard.toggleShift();
                }
                //backspace
                else if(keyboard.current.content == "back")
                {
                    if (stack.Count > 1)
                    {
                        string temp = stack.Pop();
                        for(int i=0; i<temp.Length; i++)
                        {
                            NativeMethods.KeyPress(8);
                        }
                        if(keyboard.word.Length > 0)
                        {
                            keyboard.word = keyboard.word.Substring(0, keyboard.word.Length - temp.Length);
                        }
                        else if(keyboard.word.Length == 0)
                        {
                            tempStack.Clear();
                            temp = stack.Pop();
                            tempStack.Push(temp);
                            while (stack.Count > 0 && temp != " ")
                            {
                                keyboard.word = temp + keyboard.word;
                                temp = stack.Pop();
                                tempStack.Push(temp);
                            }
                            if (stack.Count == 0) keyboard.word = temp + keyboard.word;
                            while(tempStack.Count > 0)
                            {
                                stack.Push(tempStack.Pop());
                            }

                        }
                        auto(keyboard.word);
                    }
                    else
                    {
                        NativeMethods.KeyPress(8);
                        clearAutos();
                    }
                    if (keyboard.isShifted) keyboard.toggleShift();
                }
                //all other characters
                else
                {
                    stack.Push(Convert.ToString(Char.ToLower(Convert.ToChar(keyboard.current.keyCode))));
                    //actual letters
                    if (keyboard.current.type == QuadNode.Type.letter)
                    {
                        //update current word, get top suggestions, put suggestions in auto boxes.
                        keyboard.word += keyboard.current.content;
                        auto(keyboard.word);
                    }
                    //numbers and symbols
                    else
                    {
                        clearAutos();
                        keyboard.word = "";
                    }
                    //shift
                    if (keyboard.isShifted)
                    {
                        if (keyboard.current.shift == QuadNode.Shift.na || keyboard.current.shift == QuadNode.Shift.shift)
                        {
                            NativeMethods.KeyDown((char)16);
                            NativeMethods.KeyPress(keyboard.current.keyCode);
                            NativeMethods.KeyUp((char)16);
                        }
                        else
                        {
                            NativeMethods.KeyPress(keyboard.current.keyCode);
                        }
                        keyboard.toggleShift();
                    }
                    //not shifted
                    else
                    {
                        if(keyboard.current.shift == QuadNode.Shift.shift)
                        {
                            NativeMethods.KeyDown((char)16);
                            NativeMethods.KeyPress(keyboard.current.keyCode);
                            NativeMethods.KeyUp((char)16);
                        }
                        else NativeMethods.KeyPress(keyboard.current.keyCode);
                    }
                        
                }
                keyboard.enter();
                return true;
            }
            //left
            else if (key == Keys.Left)
            {
                keyboard.left();
                return true;
            }
            //right
            else if (key == Keys.Right)
            {
                keyboard.right();
                return true;
            }
            //up
            else if (key == Keys.Up)
            {
                keyboard.up();
                return true;
            }
            //down
            else if (key == Keys.Down)
            {
                keyboard.down();
                return true;
            }
            return false;
        }
    }
}
