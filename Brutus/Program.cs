using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus
{
    class Program
    {
        static void Main(string[] args)
        {
            BrutusAnalyzer b = new BrutusAnalyzer();
            b.ProcessPasswordList(@"C:\Users\Kyle\Desktop\passwords.txt");
            b.DumpStats(@"C:\Users\Kyle\Desktop\");
        }
    }
}
