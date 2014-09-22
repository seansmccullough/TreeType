using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TreeType.autocomplete
{
    class Trie
    {
        private TrieNode root;

        //so we don't have to start traversing from top of tree every time.  Ain't nobody got time for that.
        private TrieNode current;
        public Trie(string text)
        {
            root = new TrieNode();
            current = root;
            string[] rawNodes = text.Split('\n');

            //create TrieNodes here
            for(int i=0; i<10000; i++)
            {
                string[] parameters = rawNodes[i].Split(' ');
                parameters[1] = parameters[1].Substring(0,parameters[1].Length-1);
                put(Convert.ToInt16(parameters[1]), parameters[0]);
            }

        }
        private void put(TrieNode node, Int16 rank, String s, int level)
        {
            if (level == s.Length) return;
            node.topWords.Add(rank, s);
            if(node.next[s[level]-97] == null)
            {
                node.next[s[level]-97] = new TrieNode();
                node.next[s[level]-97].value = s.Substring(0, level+1);
                put(node.next[s[level]-97], rank, s, level + 1);
            }
            else
            {
                put(node.next[s[level]-97], rank, s, level + 1);
            }
        }
        private void put(Int16 rank, String s)
        {
            put(root, rank, s, 0);
        }
        private TrieNode get(TrieNode node, String s, int level)
        {
            if (node == null) return null;
            else if (level == s.Length) return node;
            return get(node.next[s[level]-97], s, level + 1);
        }

        //returns top num words beginning with String s
        public String[] top(String s, int num)
        {
            var words = new String[num];

            if((s.Length > 0) && (s[s.Length-1] >= 97) && (s[s.Length-1] <= 122))
            {
                if(s.Length == 1) current = root;
                var node = get(current, s, 0);
                try
                {
                    var entries = node.topWords.OrderByDescending(entry => entry.Key).Take(num).ToArray();
                    for (int i = 0; i < entries.Length; i++)
                    {
                        words[i] = entries.ElementAt(i).Value;
                    }
                    return words;
                }
                catch
                {
                    //move on
                }
         
            }
            //number or symbol bullshit up in here
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = "";
            }
            return words;
        }
    }
}