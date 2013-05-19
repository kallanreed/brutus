using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Brutus
{
    [DataContract]
    public class BrutusAnalyzer
    {
        #region Properties
        [DataMember]
        internal Dictionary<int, int> PositionToCount
        {
            get;
            private set;
        }

        [DataMember]
        internal Dictionary<int, Dictionary<byte, int>> PositionToCharToCount
        {
            get;
            private set;
        }

        [DataMember]
        internal Dictionary<int, Dictionary<byte, Dictionary<byte, int>>> PositionToCharToNextCharToCount
        {
            get;
            private set;
        }

        [DataMember]
        internal Dictionary<byte, Dictionary<byte, int>> CharToNextCharToCount
        {
            get;
            private set;
        }

        [DataMember]
        internal int PasswordCount
        {
            get;
            private set;
        }

        [DataMember]
        internal long TotalPasswordLength
        {
            get;
            private set;
        }
        #endregion

        public BrutusAnalyzer()
        {
            Initialize();
        }

        [OnDeserializing]
        internal void OnDeserialize(StreamingContext ctx)
        {
            Initialize();
        }

        internal void Initialize()
        {
            this.PositionToCount = new Dictionary<int, int>();
            this.PositionToCharToCount = new Dictionary<int, Dictionary<byte, int>>();
            this.PositionToCharToNextCharToCount = new Dictionary<int, Dictionary<byte, Dictionary<byte, int>>>();
            this.CharToNextCharToCount = new Dictionary<byte, Dictionary<byte, int>>();
            this.PasswordCount = 0;
            this.TotalPasswordLength = 0;
        }

        public void DumpStats(string folderPath, string outputPrefix = "Brutus")
        {
            string summaryPath = Path.Combine(folderPath, outputPrefix + "_summary.txt");
            using (StreamWriter writer = new StreamWriter(summaryPath))
            {
                writer.WriteLine("Total Passwords\t{0}", this.PasswordCount);
                writer.WriteLine("Average Length\t{0}", Math.Round((double)this.TotalPasswordLength / this.PasswordCount, 2));
            }

            string freqMap1Path = Path.Combine(folderPath, outputPrefix + "_freqMap1.txt");
            using (StreamWriter writer = new StreamWriter(freqMap1Path))
            {
                foreach (int position in this.PositionToCharToCount.Keys)
                {
                    Dictionary<byte, int> charToCount = this.PositionToCharToCount[position];

                    foreach(byte asciiVal in charToCount.Keys)
                    {
                        char charVal = Convert.ToChar(asciiVal);

                        List<string> lineData = new List<string>();
                        lineData.Add(position.ToString());
                        lineData.Add(asciiVal.ToString());
                        lineData.Add(charVal.ToString());
                        lineData.Add(charToCount[asciiVal].ToString());
                        lineData.Add(Math.Round((double)charToCount[asciiVal] / this.PositionToCount[position], 4).ToString());
                        lineData.Add(Char.IsUpper(charVal).ToString());
                        lineData.Add(Char.IsLower(charVal).ToString());
                        lineData.Add(Char.IsDigit(charVal).ToString());
                        lineData.Add((!Char.IsLetterOrDigit(charVal)).ToString());

                        writer.WriteLine(String.Join("\t", lineData.ToArray()));
                    }
                }
            }

            string char2CharFreqMap1Path = Path.Combine(folderPath, outputPrefix + "_char2CharFreqMap1.txt");
            using (StreamWriter writer = new StreamWriter(char2CharFreqMap1Path))
            {
                foreach (byte asciiVal in this.CharToNextCharToCount.Keys)
                {
                    Dictionary<byte, int> nextCharToCount = this.CharToNextCharToCount[asciiVal];

                    foreach (byte nextAsciiVal in nextCharToCount.Keys)
                    {
                        char charVal = Convert.ToChar(asciiVal);
                        char nextCharVal = Convert.ToChar(nextAsciiVal);

                        List<string> lineData = new List<string>();
                        lineData.Add(asciiVal.ToString());
                        lineData.Add(nextAsciiVal.ToString());
                        lineData.Add(charVal.ToString());
                        lineData.Add(nextCharVal.ToString());
                        lineData.Add(nextCharToCount[nextAsciiVal].ToString());

                        writer.WriteLine(String.Join("\t", lineData.ToArray()));
                    }
                }
            }

            string pos2Char2CharFreqMap1Path = Path.Combine(folderPath, outputPrefix + "_pos2Char2CharFreqMap1.txt");
            using (StreamWriter writer = new StreamWriter(pos2Char2CharFreqMap1Path))
            {
                foreach (byte asciiVal in this.CharToNextCharToCount.Keys)
                {
                    Dictionary<byte, int> nextCharToCount = this.CharToNextCharToCount[asciiVal];

                    foreach (byte nextAsciiVal in nextCharToCount.Keys)
                    {
                        char charVal = Convert.ToChar(asciiVal);
                        char nextCharVal = Convert.ToChar(nextAsciiVal);

                        List<string> lineData = new List<string>();
                        lineData.Add(asciiVal.ToString());
                        lineData.Add(nextAsciiVal.ToString());
                        lineData.Add(charVal.ToString());
                        lineData.Add(nextCharVal.ToString());
                        lineData.Add(nextCharToCount[nextAsciiVal].ToString());

                        writer.WriteLine(String.Join("\t", lineData.ToArray()));
                    }
                }
            }
        }

        public void ProcessPasswordList(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                do
                {
                    string password = reader.ReadLine();

                    if (string.IsNullOrEmpty(password))
                    {
                        continue;
                    }

                    ProcessPassword(password);

                } while (!reader.EndOfStream);
            }
        }

        private void ProcessPassword(string password)
        {
            this.PasswordCount++;
            this.TotalPasswordLength += password.Length;

            for (int i = 0; i < password.Length; i++)
            {
                char currentChar = password[i];
                byte currentByte = Convert.ToByte(currentChar);

                // add one to the position
                this.PositionToCount.InitializeOrIncrement<int>(i);
                
                // add one to the char at position
                Dictionary<byte, int> charToCount = this.PositionToCharToCount.GetOrInitialize<int, byte, int>(i);
                charToCount.InitializeOrIncrement<byte>(currentByte);

                // if there is a nextChar
                if (i < password.Length - 1)
                {
                    char nextChar = password[i + 1];
                    byte nextByte = Convert.ToByte(nextChar);

                    // add one to the nextChar for char
                    Dictionary<byte, int> nextCharToCount = this.CharToNextCharToCount.GetOrInitialize<byte, byte, int>(currentByte);
                    nextCharToCount.InitializeOrIncrement<byte>(nextByte);

                    // add one to the nextChar for the char at position
                    Dictionary<byte, Dictionary<byte, int>> charToNextCharToCount =
                        this.PositionToCharToNextCharToCount.GetOrInitialize<int, byte, Dictionary<byte, int>>(i);
                    nextCharToCount = charToNextCharToCount.GetOrInitialize<byte, byte, int>(currentByte);
                    nextCharToCount.InitializeOrIncrement<byte>(nextByte);
                }
            }
        }

        public static void SerializeToFile(BrutusAnalyzer b, string filename)
        {
            DataContractSerializer serializer = new DataContractSerializer(b.GetType());

            using (FileStream stream = new FileStream(filename, FileMode.OpenOrCreate))
            {
                serializer.WriteObject(stream, b);
            }
        }

        public static BrutusAnalyzer DeserializeFromFile(string filename)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(BrutusAnalyzer));

            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                return (BrutusAnalyzer)serializer.ReadObject(stream);
            }
        }
    }

    public static class DictionaryHelpers
    {
        public static void InitializeOrIncrement<K>(this Dictionary<K, int> dictionary, K key)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = 0;
            }

            dictionary[key]++;
        }

        public static Dictionary<K, V> GetOrInitialize<PK, K, V>(this  Dictionary<PK, Dictionary<K, V>> dictionary, PK key)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = new Dictionary<K, V>();
            }

            return dictionary[key];
        }
    }
}
