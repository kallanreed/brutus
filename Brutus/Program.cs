﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brutus.Optimizers;
using Brutus.Selectors;

namespace Brutus
{
    class Program
    {
        static void Main(string[] args)
        {
            PasswordAnalyzer b = new PasswordAnalyzer();
            //b.ProcessPasswordList(@"C:\Users\Kyle\Desktop\passwords.txt");
            //PasswordAnalyzer.SerializeToFile(b, @"C:\Users\Kyle\Desktop\serialized.xml");
         
            b = PasswordAnalyzer.DeserializeFromFile(@"C:\Users\Kyle\Desktop\serialized.xml");
            b.DumpStats(@"C:\Users\Kyle\Desktop\");

            //Mask testMask = new Mask()
            //{
            //    Charset1 = "ABCDSP",
            //    Charset2 = "aeiouyrstln123",
            //    Charset3 = "aeiouyrstln1234567890",
            //    Charset4 = "1234567890!@#$",
            //    MaskString = "1222233334"
            //};

            CharsetSelector selector = new TopSpotSelector()
            {
                Analyzer = b,
                MaxCharsPerSet = 30,
                TargetMaskLength = 12
            };

            Mask testMask = selector.GetMask();

            // run optimizer to select best mask string with given charset
            MaskOptimizer optimizer = new MiddleShiftMaskOptimizer()
            {
                //StartingMaskString = "1222233334",
                //UseAllCharsets = true,
                TargetMaskLength = 12,
                PasswordListFilename = @"C:\Users\Kyle\Desktop\passwords.txt"
            };
            optimizer.OptimizeMask(testMask);

            // dump results of the optimized mask
            MaskAnalyzer maskAnalyzer = new MaskAnalyzer(testMask);
            MaskAnalyzerResults results = maskAnalyzer.AnalyzeMaskCoverage(@"C:\Users\Kyle\Desktop\passwords.txt");

            Console.WriteLine(testMask.ToString());
            Console.WriteLine("Mask Space Coverage {0}%", Math.Round(maskAnalyzer.MaskCoveragePercent, 8).ToString(".#########################"));
            Console.WriteLine("Mask Total Cracking Time {0}", maskAnalyzer.MaskTotalCrackingTime.ToString());
            Console.WriteLine("Matched {0}", results.MatchedCount);
            Console.WriteLine("Missed {0}", results.MissedCount);
            Console.WriteLine("Matched {0}%", Math.Round(results.MatchedPercent, 4));
            Console.WriteLine("Missed {0}%", Math.Round(results.MissedPercent, 4));
        }
    }
}
