using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus.Selectors
{
    public class TopNextSpotSelector : CharsetSelector
    {
        public int MaxCharsPerSet
        {
            get;
            set;
        }

        public int TargetMaskLength
        {
            get;
            set;
        }

        public PasswordAnalyzer Analyzer
        {
            get;
            set;
        }

        public override Mask GetMask()
        {
            // LOTS of possible null refs here

            Mask ret = new Mask();

            ret.Charset1 = GetTopCharset(0);
            ret.Charset2 = GetTopCharsetWithNext(1);
            ret.Charset3 = GetTopCharsetWithNext(this.TargetMaskLength / 2 + 1);
            ret.Charset4 = GetTopCharsetWithNext(this.TargetMaskLength - 2);

            return ret;
        }

        private string GetTopCharset(int position)
        {
            Dictionary<byte, int> charToCount = this.Analyzer.PositionToCharToCount[position];
            List<KeyValuePair<byte, int>> sorted = charToCount.OrderByDescending(pair => pair.Value).ToList();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < (this.MaxCharsPerSet) && i < sorted.Count; i++)
            {
                sb.Append(Convert.ToChar(sorted[i].Key));
            }

            return sb.ToString();
        }

        private string GetTopCharsetWithNext(int position)
        {
            Dictionary<byte, int> charToCount = this.Analyzer.PositionToCharToCount[position];
            List<KeyValuePair<byte, int>> sorted = charToCount.OrderByDescending(pair => pair.Value).ToList();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < (this.MaxCharsPerSet / 2) && i < sorted.Count; i++)
            {
                sb.Append(Convert.ToChar(sorted[i].Key));

                Dictionary<byte, int> nextCharToCount = this.Analyzer.PositionToCharToNextCharToCount[position][sorted[i].Key];
                foreach (KeyValuePair<byte, int> pair in nextCharToCount.OrderByDescending(pair => pair.Value))
                {
                    if (!sb.ToString().Contains(Convert.ToChar(pair.Key)))
                    {
                        sb.Append(Convert.ToChar(pair.Key));
                        break;
                    }
                }
            }

            return sb.ToString();
        }
    }
}
