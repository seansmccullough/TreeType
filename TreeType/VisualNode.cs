using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TreeType
{
    public class VisualNode
    {
        private Boolean selected;
        private SolidColorBrush defaultColor;
        private Rectangle box;
        private TextBlock text;
        //VisualNode owns parent just to toggle it.  VisualNode doesn't render it
        public Line parent { get; set; }
        private QuadNode quadnode;
        private int x;
        private int y;
        private Boolean shift;
        public VisualNode()
        {
        }
        //x and y are the center of the new node!!!
        public VisualNode(QuadNode quadnode, int x, int y, Line parent, Canvas canvas)
        {
            this.quadnode = quadnode;
            shift = false;
            //quadnode.rendered = true;
            selected = false;
            this.x = x;
            this.y = y;
            
            this.parent = parent;

            //box
            box = new Rectangle();
            box.Width = Constant.defaultWidth * this.quadnode.width;
            box.Height = Constant.defaultHeight * this.quadnode.height;
            box.Stroke = Constant.defaultColor;
            Canvas.SetLeft(box, x - Constant.defaultWidth * this.quadnode.width / 2);
            Canvas.SetTop(box, y - Constant.defaultHeight * this.quadnode.height / 2);
            canvas.Children.Add(box);

            //box background color
            switch(quadnode.type)
            {
                case "letter":
                    box.Fill = Constant.letterBackgroundColor;
                    defaultColor = Constant.letterBackgroundColor;
                    break;
                case "symbol":
                    box.Fill = Constant.symbolBackgroundColor;
                    defaultColor = Constant.symbolBackgroundColor;
                    break;
                case "number":
                    box.Fill = Constant.numberBackgroundColor;
                    defaultColor = Constant.numberBackgroundColor;
                    break;
                case "special":
                    box.Fill = Constant.specialBackgroundColor;
                    defaultColor = Constant.specialBackgroundColor;
                    break;
                default:
                    box.Fill = Constant.letterBackgroundColor;
                    defaultColor = Constant.letterBackgroundColor;
                    break;
            }

            //text
            this.text = new TextBlock();
            text.TextAlignment = TextAlignment.Center;
            this.text.Text = quadnode.content;
            this.text.FontSize = Constant.defaultFontSize;
            this.text.Foreground = Constant.defaultColor;
            Canvas.SetLeft(this.text, x);
            Canvas.SetTop(this.text, y);
            canvas.Children.Add(this.text);
            text.AddHandler(TextBlock.LoadedEvent, new RoutedEventHandler(textBoxLoaded));
        }
        //centers TextBlock in Rectangle after the TextBlock has been loaded
        private static void textBoxLoaded(object sender, RoutedEventArgs e)
        {
            TextBlock temp = sender as TextBlock;
            Canvas.SetLeft(temp, Canvas.GetLeft(temp) - temp.ActualWidth/2);
            Canvas.SetTop(temp, Canvas.GetTop(temp) - temp.ActualHeight / 2);
        }
        public void toggleSelected()
        {
            if(selected)
            {
                //parent.Stroke = Constant.defaultColor;

                //box.Stroke = Constant.defaultColor;
                box.Fill = defaultColor;
                //text.Foreground = Constant.defaultColor;
                selected = false;
            }
            else
            {
                //parent.Stroke = Constant.defaultColor;
                //box.Stroke = Constant.selectedBackgroundColor;
                //text.Foreground = Constant.selectedColor;
                box.Fill = Constant.selectedBackgroundColor;
                selected = true;
            }
        }
        public void toggleShift()
        {
            if (shift) text.Text = quadnode.content;
            else text.Text = quadnode.contentShift;
            shift = !shift;
        }
    }
}
