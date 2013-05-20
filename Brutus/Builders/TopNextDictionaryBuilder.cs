using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus.Builders
{

    public class TopNextDictionaryBuilder : DictionaryBuilder
    {
        public override void GenerateDictionary(string filename)
        {
            long bytesWritten = 0;
            int newlineLen = Environment.NewLine.Length;

            using (StreamWriter writer = new StreamWriter(filename))
            {
                // this is nice because if either the enumerator
                // will exit when the dicionary is depleted or
                // the output size is met
                foreach (string word in FullDictionary())
                {
                    writer.WriteLine(word);
                    bytesWritten += word.Length + newlineLen;

                    if (bytesWritten > this.MaxOutputSizeInBytes)
                    {
                        break;
                    }
                }
            }
        }

        public IEnumerable<string> FullDictionary()
        {
            // special case to make this work when you enter 0 as target length
            if (this.TargetWordSize == 0)
            {
                yield break;
            }

            List<KeyValuePair<byte, int>> sorted = this.Analyzer.PositionToCharToCount[0].OrderByDescending(pair => pair.Value).ToList();

            foreach (KeyValuePair<byte, int> pair in sorted)
            {
                foreach (string s in FullDictionary(pair.Key, 1, Convert.ToChar(pair.Key).ToString()))
                {
                    yield return s;
                }
            }
        }

        // SO NASTY, but I can't seem to make StringBuilder make sense right now.
        // need to pre-sort the data... generally gross.
        public IEnumerable<string> FullDictionary(byte current, int position, string val)
        {
            if (position >= this.TargetWordSize)
            {
                // we have a value that should be the right size
                yield return val;
            }
            else
            {
                //Dictionary<byte, int> nextChar2Count = this.Analyzer.PositionToCharToNextCharToCount[position][current];
                Dictionary<byte, int> nextChar2Count = this.Analyzer.CharToNextCharToCount[current];

                if (nextChar2Count == null)
                {
                    // no next data for the current character, this is a dead end
                    yield break;
                }

                List<KeyValuePair<byte, int>> sorted = nextChar2Count.OrderByDescending(pair => pair.Value).ToList();

                foreach (KeyValuePair<byte, int> pair in sorted)
                {
                    foreach (string s in FullDictionary(pair.Key, ++position, val + Convert.ToChar(pair.Key)))
                    {
                        yield return s;
                    }
                }
            }
        }
    }
}
