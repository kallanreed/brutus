using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus.Optimizers
{
    class MiddleShiftMaskOptimizer : MaskOptimizer
    {
        int toChange;

        public override void OptimizeMask(Mask mask)
        {
            List<string> passwords = new List<string>();
            List<int> maskList = new List<int>();
            string bestMaskString = null;
            double bestMatchPercent = -1;

            // initialize mask list to all 2s
            for (int i = 0; i < this.TargetMaskLength; i++)
            {
                maskList.Add(2);
            }

            // set first and last two to 1 and 4
            maskList[0] = 1;
            maskList[maskList.Count - 1] = 4;
            maskList[maskList.Count - 2] = 4;
            this.toChange = maskList.Count - 3;

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

        private bool IncrementList(List<int> toIncr)
        {
            if (toChange < 2)
            {
                // don't let it change the first two elements
                return false;
            }

            toIncr[toChange] = 3;
            toChange--;

            return true;
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
