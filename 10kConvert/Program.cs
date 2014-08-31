using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _10kConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("../../../TreeType/autocomplete/words10k.txt");
            for(int i=0; i<lines.Length; i++)
            {
                lines[i] += " " + (lines.Length - i);
            }
            File.WriteAllLines("../../../TreeType/autocomplete/words10k.txt", lines);
        }
    }
}
