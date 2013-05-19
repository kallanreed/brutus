using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus.Selectors
{
    public class TopSpotSelector : CharsetSelector
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
            
            Dictionary<byte, int> charToCount = this.Analyzer.PositionToCharToCount[0];
            List<KeyValuePair<byte, int>> sorted = charToCount.OrderByDescending(pair => pair.Value).ToList();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < this.MaxCharsPerSet && i < sorted.Count; i++)
            {
                sb.Append(Convert.ToChar(sorted[i].Key));
            }

            ret.Charset1 = sb.ToString();


            charToCount = this.Analyzer.PositionToCharToCount[(int)Math.Floor(this.TargetMaskLength / 2.0 * 0.5)];
            sorted = charToCount.OrderByDescending(pair => pair.Value).ToList();
            sb = new StringBuilder();

            for (int i = 0; i < this.MaxCharsPerSet && i < sorted.Count; i++)
            {
                sb.Append(Convert.ToChar(sorted[i].Key));
            }

            ret.Charset2 = sb.ToString();


            charToCount = this.Analyzer.PositionToCharToCount[(int)Math.Floor(this.TargetMaskLength / 2.0 * 1.5)];
            sorted = charToCount.OrderByDescending(pair => pair.Value).ToList();
            sb = new StringBuilder();

            for (int i = 0; i < this.MaxCharsPerSet && i < sorted.Count; i++)
            {
                sb.Append(Convert.ToChar(sorted[i].Key));
            }

            ret.Charset3 = sb.ToString();


            charToCount = this.Analyzer.PositionToCharToCount[this.TargetMaskLength - 1];
            sorted = charToCount.OrderByDescending(pair => pair.Value).ToList();
            sb = new StringBuilder();

            for (int i = 0; i < this.MaxCharsPerSet && i < sorted.Count; i++)
            {
                sb.Append(Convert.ToChar(sorted[i].Key));
            }

            ret.Charset4 = sb.ToString();


            return ret;
        }
    }
}
