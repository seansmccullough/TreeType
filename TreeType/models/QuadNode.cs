using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TreeType
{
    
    public class QuadNode
    {
        public enum Shift { shift, noShift, na };
        public enum Type { letter, symbol, number, special, auto }
        // Pointers to nearby nodes
        private Dictionary<Direction, QuadNode> neighbors;
        public Dictionary<Direction, string> strings;

        public QuadNode this[Direction direction]
        {
            get { return this.neighbors[direction]; }
            set { this.neighbors[direction] = value; }
        }
        
        public String content { get; set; }
        public String contentShift { get; set; }

        public Boolean visited { get; set; }
        public Boolean path { get; set; }
        //scaling factor for defaultHeight
        public double height { get; set; }
        //scaling factor for defaultHeight
        public double width { get; set; }

        public Type type { get; set; }
        public int depth { get; set; }
        public Boolean snap { get; set; }

        public Boolean rendered { get; set; }
        //used to match VisualNodes with QuadNodes
        public VisualNode visualNode { get; set; }
        public Byte keyCode { get; set; }
        public Shift shift { get; set; }
        //Refers to Node with parent and only one child, on the opposite side of the parent
        public bool passThroughNode { get; set; }

        public QuadNode()
        {
            this.neighbors = new Dictionary<Direction, QuadNode>(Enum.GetValues(typeof(Direction)).Length);
            this.strings = new Dictionary<Direction, string>(Enum.GetValues(typeof(Direction)).Length);
            this.visited = false;
            this.path = false;
            this.rendered = false;
            this.content = "";
            this.width = 1;
            this.height = 1;
            this.depth = 0;
            this.passThroughNode = false;
        }
        /*
         * Returns the depth of current QuadNode and sets its depth field
         * A leaflet has a depth of 0
         */
        public static int calDepth(QuadNode current)
        {
            int maxDepth = 0;

            current.visited = true;

            foreach (Direction direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                if ((current[direction] != null) && (current[direction].visited == false))
                {
                    int dirDepth = calDepth(current[direction]) + 1;
                    maxDepth = (dirDepth > maxDepth) ? dirDepth : maxDepth;
                }
            }

            current.depth = maxDepth;
           
            return current.depth;
        }
    }
}
