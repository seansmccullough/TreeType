using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TreeType.autocomplete
{
    class Trie
    {
        public Trie(String textFile)
        {
            string line;
            var reader = File.OpenText(textFile);
 
            //create TrieNodes here
            while ((line = reader.ReadLine()) != null)
            {
            }

            reader.Close();
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
