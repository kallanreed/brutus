using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus
{
    public class Mask
    {
        private List<string> maskPositionToCharset;
        private List<int> maskCharsetCount;

        public string Charset1
        {
            get;
            set;
        }

        public string Charset2
        {
            get;
            set;
        }

        public string Charset3
        {
            get;
            set;
        }

        public string Charset4
        {
            get;
            set;
        }

        private string _maskString;
        public string MaskString
        {
            get
            {
                return this._maskString;
            }
            set
            {
                this.maskCharsetCount = new List<int>() { 0, 0, 0, 0 };
                this.maskPositionToCharset = new List<string>();

                foreach (char c in value)
                {
                    switch (c)
                    {
                        case '1':
                            this.maskCharsetCount[0]++;
                            this.maskPositionToCharset.Add(this.Charset1);
                            break;
                        case '2':
                            this.maskCharsetCount[1]++;
                            this.maskPositionToCharset.Add(this.Charset2);
                            break;
                        case '3':
                            this.maskCharsetCount[2]++;
                            this.maskPositionToCharset.Add(this.Charset3);
                            break;
                        case '4':
                            this.maskCharsetCount[3]++;
                            this.maskPositionToCharset.Add(this.Charset4);
                            break;
                        default:
                            throw new ArgumentException("Invalid charset specifier: " + c.ToString());
                    }
                }

                this._maskString = value;
            }
        }

        public int Length
        {
            get
            {
                return this.MaskString.Length;
            }
        }

        public double MaskSpace
        {
            get
            {
                // this assumes all of the charsets are valid
                
                return (
                    Math.Pow(this.Charset1.Length, this.maskCharsetCount[0]) *
                    Math.Pow(this.Charset2.Length, this.maskCharsetCount[1]) *
                    Math.Pow(this.Charset3.Length, this.maskCharsetCount[2]) *
                    Math.Pow(this.Charset4.Length, this.maskCharsetCount[3])
                    );
            }
        }

        public bool IsMatch(string password)
        {
            if (password.Length != this.Length)
            {
                return false;
            }

            for (int i = 0; i < password.Length; i++)
            {
                if (!this.maskPositionToCharset[i].Contains(password[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.MaskString);
            sb.AppendLine(this.Charset1);
            sb.AppendLine(this.Charset2);
            sb.AppendLine(this.Charset3);
            sb.AppendLine(this.Charset4);

            return sb.ToString();
        }
    }
}
