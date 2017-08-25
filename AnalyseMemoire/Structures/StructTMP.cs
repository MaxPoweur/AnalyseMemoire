using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire.Structures
{
    class StructTMP : Structure
    {
        public StructTMP() : base() { }

        public StructTMP(IntPtr baseAddress) : base(baseAddress) { }

        public override void initVariables()
        {
            this.variables.Add(new Variable("tmpValue", VariablesTypes.Integer), 0x10);
        }
    }
}