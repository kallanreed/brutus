using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus
{
    public abstract class MaskOptimizer
    {
        public bool UseAllCharsets
        {
            get;
            set;
        }

        public int TargetMaskLength
        {
            get;
            set;
        }

        public string StartingMaskString
        {
            get;
            set;
        }

        public string PasswordListFilename
        {
            get;
            set;
        }

        public virtual void OptimizeMask(Mask mask)
        {
        }
    }
}
