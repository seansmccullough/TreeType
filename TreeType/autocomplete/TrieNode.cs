using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeType.autocomplete
{
    class TrieNode
    {
        //level rank, TrieNode
        public TrieNode[] next { get; set; }
        public SortedDictionary<Int16,String> topWords { get; set; }
        public String value { get; set; }

        public TrieNode()
        {
            next = new TrieNode[26];
            topWords = new SortedDictionary<Int16, String>();
        }
    }
}
