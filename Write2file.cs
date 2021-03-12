using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace dictation5
{
    class Write2file
    {
        public static void WriteText(string FileName, string text)
        {
            File.WriteAllText(FileName, text);
        }
    }
}
