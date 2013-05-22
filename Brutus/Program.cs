using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brutus.Builders;
using Brutus.Optimizers;
using Brutus.Selectors;

namespace Brutus
{
    class Program
    {
        static void Main(string[] args)
        {
            string basePath = @"C:\BRUTUS\";
            int targetPWLen = 12;
            int maxCharset = 35;
            int mbOut = 1;

            if (args.Length >= 3)
            {
                int.TryParse(args[0], out targetPWLen);
                int.TryParse(args[1], out maxCharset);
                int.TryParse(args[2], out mbOut);
            }

            PasswordAnalyzer analyzer = new PasswordAnalyzer();
            analyzer.ProcessPasswordList(basePath + "passwords.txt");
            PasswordAnalyzer.SerializeToFile(analyzer, basePath + "serialized.xml");

            //analyzer = PasswordAnalyzer.DeserializeFromFile(basePath + "serialized.xml");
            analyzer.DumpStats(@basePath);

            DictionaryBuilder builder = new TopNextDictionaryBuilder()
            {
                MaxOutputSizeInBytes = 1024 * 1024 * mbOut,
                Analyzer = analyzer,
                TargetWordSize = targetPWLen
            };

            builder.GenerateDictionary(basePath + "dictionary_out.txt");

            ////Mask testMask = new Mask()
            ////{
            ////    Charset1 = "ABCDSP",
            ////    Charset2 = "aeiouyrstln123",
            ////    Charset3 = "aeiouyrstln1234567890",
            ////    Charset4 = "1234567890!@#$",
            ////    MaskString = "1222233334"
            ////};

            CharsetSelector selector = new TopSpotSelector()
            {
                Analyzer = analyzer,
                MaxCharsPerSet = maxCharset,
                TargetMaskLength = targetPWLen
            };

            Mask testMask = selector.GetMask();

            // run optimizer to select best mask string with given charset
            MaskOptimizer optimizer = new MiddleShiftMaskOptimizer()
            {
                //StartingMaskString = "1222233334",
                //UseAllCharsets = true,
                TargetMaskLength = targetPWLen,
                PasswordListFilename = basePath + "passwords.txt"
            };
            optimizer.OptimizeMask(testMask);

            // dump results of the optimized mask
            MaskAnalyzer maskAnalyzer = new MaskAnalyzer(testMask);
            MaskAnalyzerResults results = maskAnalyzer.AnalyzeMaskCoverage(basePath + "passwords.txt");

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
