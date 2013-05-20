using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus
{
    public abstract class DictionaryBuilder
    {
        public int TargetWordSize
        {
            get;
            set;
        }

        public long MaxOutputSizeInBytes
        {
            get;
            set;
        }

        public PasswordAnalyzer Analyzer
        {
            get;
            set;
        }

        public abstract void GenerateDictionary(string filename);
    }
}
