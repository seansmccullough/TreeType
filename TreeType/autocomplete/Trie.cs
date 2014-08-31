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
        public Trie(String textFile)
        {
            root = new TrieNode();
            current = root;
            string line;
            var reader = File.OpenText(textFile);

            //create TrieNodes here
            while ((line = reader.ReadLine()) != null)
            {
                string[] parameters = line.Split(' ');
                put(Convert.ToInt16(parameters[1]), parameters[0]);
            }

            reader.Close();
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

            //number or symbol bullshit up in here
            if(s[s.Length-1] < 97 || s[s.Length-1] > 122)
            {
                for(int i=0; i<words.Length; i++)
                {
                    words[i] = "";
                }
                return words;
            }

            else
            {
                if (s.Length == 1) current = root;
                var node = get(current, s, 0);
                
                var entries = node.topWords.OrderByDescending(entry => entry.Key).Take(num).ToArray();
                for (int i = 0; i < entries.Length; i++)
                {
                    words[i] = entries.ElementAt(i).Value;
                }
                return words;
            }

        }
    }
}