using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TreeType
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    public class Tree
    {
        public QuadNode root { get; private set; }
        public bool isShifted { get; private set; }

        public int maxDepth { get; private set; }

        private QuadNode m_current;

        public List<QuadNode> autoCompletes {get; private set;}

        public QuadNode current {
            get { return m_current; }
            private set
            {
                if (value != null)
                {
                    if (m_current != null && m_current.visualNode != null)
                    {
                        m_current.visualNode.toggleSelected();
                        m_current = value;
                        m_current.visualNode.toggleSelected();
                    }
                    else
                    {
                        m_current = value;
                    }
                }
            }   
        }

        Dictionary<string, QuadNode> nodes = new Dictionary<string, QuadNode>();

        public void loadFromFile(string textFile)
        {
            string line;
            var reader = File.OpenText(textFile);
            maxDepth = 0;
 
            autoCompletes = new List<QuadNode>();
            //create QuadNodes here
            while ((line = reader.ReadLine()) != null)
            {
                /*
                 * 0 name
                 * 1 lowercase content
                 * 2 uppercase content
                 * 3 up
                 * 4 right
                 * 5 down
                 * 6 left
                 * 7 type (letter, symbol, number, special)
                 * 8 width
                 * 9 height
                 * 10 snap
                 * 11 keycode
                 * 12 shift (true/false)
                 */
                string[] parameters = line.Split(' ');

                //create new QuadNode and populate directional string fields (upString, rightString, etc.)
                QuadNode newNode = new QuadNode();
                
                newNode.content = parameters[1];
                newNode.contentShift = parameters[2];
                newNode.strings[Direction.Up] = parameters[3];
                newNode.strings[Direction.Right] = parameters[4];
                newNode.strings[Direction.Down] = parameters[5];
                newNode.strings[Direction.Left] = parameters[6];

                if (parameters[7] == "number") newNode.type = QuadNode.Type.number;
                else if (parameters[7] == "auto") 
                {
                    newNode.type = QuadNode.Type.auto;
                    autoCompletes.Add(newNode);
                }
                    
                else if (parameters[7] == "special") newNode.type = QuadNode.Type.special;
                else if (parameters[7] == "symbol") newNode.type = QuadNode.Type.symbol;
                else newNode.type = QuadNode.Type.letter;

                newNode.width = Convert.ToDouble(parameters[8]);
                newNode.height = Convert.ToDouble(parameters[9]);
                String snap = parameters[10];
                if (snap.Equals("1")) newNode.snap = true;
                else newNode.snap = false;
                newNode.keyCode = Convert.ToByte(parameters[11]);
                String shift = parameters[12];
                if (shift.Equals("na")) newNode.shift = QuadNode.Shift.na;
                else if (shift.Equals("shift")) newNode.shift = QuadNode.Shift.shift;
                else if (shift.Equals("noShift")) newNode.shift = QuadNode.Shift.noShift;

                nodes[parameters[0]] = newNode;

                //assume Root is first node in text file
                if (root == null)
                {
                    root = newNode;
                }
            }
            reader.Close();

            //populate up, right, left, and down properties here. 
            foreach (QuadNode e in nodes.Values)
            {
                foreach (Direction direction in (Direction[])Enum.GetValues(typeof(Direction)))
                {
                    string nodeName = e.strings[direction]; 
                    if (nodeName.Equals("null"))
                    {
                        e[direction] = null;
                    }
                    else
                    {
                        e[direction] = nodes[nodeName];
                    }
                }
                if((e[Direction.Down] == null && e[Direction.Up] == null) 
                    ||(e[Direction.Left] == null && e[Direction.Right] == null))
                {
                    e.passThroughNode = true;
                }
            }

            this.current = root;
            QuadNode.calDepth(this.root);
            if (current.depth > maxDepth) maxDepth = current.depth;
        }

        public void clearRendered()
        {
            foreach (QuadNode e in nodes.Values)
            {
                e.rendered = false;
            }
        }

        public Boolean enter()
        {
            if (current.snap == true)
            {
                current = root;
            }
            return true;
        }

        public void up()
        {
            current = current[Direction.Up];
        }
        public void right()
        {
            current = current[Direction.Right];   
        }
        public void down()
        {
            current = current[Direction.Down];
        }
        public void left()
        {
            current = current[Direction.Left];
        }
        public void toggleShift()
        {
            isShifted = !isShifted;
            foreach (QuadNode e in nodes.Values)
            {
                e.visualNode.toggleShift();
            }
        }
    }
}
