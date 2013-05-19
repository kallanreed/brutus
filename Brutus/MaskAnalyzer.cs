using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus
{
    public class MaskAnalyzer
    {
        const int FullCharsetSize = 92;
        const int MegaHashesPerSecond = 7500;

        public Mask Mask
        {
            get;
            set;
        }

        public MaskAnalyzer(Mask mask)
        {
            this.Mask = mask;
        }

        public double TotalSpace
        {
            get
            {
                return Math.Pow(FullCharsetSize, this.Mask.Length);
            }
        }

        public double MaskCoveragePercent
        {
            get
            {
                return this.Mask.MaskSpace * 100.0 / this.TotalSpace;
            }
        }

        public TimeSpan MaskTotalCrackingTime
        {
            get
            {
                return TimeSpan.FromSeconds(
                    ((this.Mask.MaskSpace / 1000000.0) / MegaHashesPerSecond));
            }
        }

        /// <summary>
        /// Test the mask against a password list
        /// </summary>
        /// <param name="filename">Path to a file containing passwords to test the mask against</param>
        public MaskAnalyzerResults AnalyzeMaskCoverage(string filename)
        {
            MaskAnalyzerResults results = new MaskAnalyzerResults();

            using (StreamReader reader = new StreamReader(filename))
            {
                do
                {
                    string password = reader.ReadLine();

                    if (string.IsNullOrEmpty(password))
                    {
                        continue;
                    }

                    AnalyzePassword(password, results);

                } while (!reader.EndOfStream);
            }

            return results;
        }

        public MaskAnalyzerResults AnalyzeMaskCoverage(List<string> passwords)
        {
            MaskAnalyzerResults results = new MaskAnalyzerResults();

            foreach (string password in passwords)
            {
                AnalyzePassword(password, results);
            }

            return results;
        }

        private void AnalyzePassword(string password, MaskAnalyzerResults results)
        {
            results.TotalPasswords++;

            if (this.Mask.IsMatch(password))
            {
                results.MatchedCount++;
            }
        }
    }

    public class MaskAnalyzerResults
    {
        public int TotalPasswords
        {
            get;
            internal set;
        }

        public int MatchedCount
        {
            get;
            internal set;
        }

        public int MissedCount
        {
            get
            {
                return this.TotalPasswords - this.MatchedCount;
            }
        }

        public double MatchedPercent
        {
            get
            {
                return (this.MatchedCount * 100.0 / this.TotalPasswords);
            }
        }

        public double MissedPercent
        {
            get
            {
                return (100.0 - this.MatchedPercent);
            }
        }
    }
}
