using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeType.autocomplete
{
    class Trie
    {
        public Trie(String filename)
        {

        }
        public String[] top(String arg, int num)
        {
            var strings = new String[num];
            for(int i=0; i<strings.Length; i++)
            {
                strings[i] = "the";
            }
            return strings;
        }
    }
}
