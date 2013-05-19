using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutus
{
    public abstract class CharsetSelector
    {
        public virtual Mask GetMask()
        {
            return null;
        }
    }
}
