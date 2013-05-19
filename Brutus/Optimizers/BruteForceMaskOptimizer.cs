using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus.Optimizers
{
    class BruteForceMaskOptimizer : MaskOptimizer
    {
        public override void OptimizeMask(Mask mask)
        {
            List<string> passwords = new List<string>();
            List<int> maskList = new List<int>();
            string bestMaskString = null;
            double bestMatchPercent = -1;

            // initialize mask list to all 1s
            for (int i = 0; i < this.TargetMaskLength; i++)
            {
                maskList.Add(1);
            }

            // build a memory list of passwords because we hammer on it
            using (StreamReader reader = new StreamReader(this.PasswordListFilename))
            {
                do
                {
                    string password = reader.ReadLine();

                    if (string.IsNullOrEmpty(password))
                    {
                        continue;
                    }

                    passwords.Add(password);

                } while (!reader.EndOfStream);
            }

            do
            {
                mask.MaskString = GetMaskString(maskList);
                MaskAnalyzer ma = new MaskAnalyzer(mask);
                MaskAnalyzerResults res = ma.AnalyzeMaskCoverage(passwords);

                if (res.MatchedPercent > bestMatchPercent)
                {
                    bestMatchPercent = res.MatchedPercent;
                    bestMaskString = mask.MaskString;
                }

            } while (IncrementList(maskList));

            mask.MaskString = bestMaskString;
        }

        private static bool IncrementList(List<int> toIncr)
        {
            // this is terrible, better off using real ints
            // and implementing a base 4 radix
            for (int i = toIncr.Count - 1; i >= 0; i--)
            {
                toIncr[i]++;

                if (toIncr[i] > 4)
                {
                    toIncr[i] = 1;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private string GetMaskString(List<int> maskList)
        {
            StringBuilder sb = new StringBuilder();

            foreach (int i in maskList)
            {
                sb.Append(i.ToString());
            }

            return sb.ToString();
        }
    }
}
