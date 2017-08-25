using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire.Structures
{
    class PrivateStructure : Structure
    {
        public PrivateStructure() : base() { }

        public PrivateStructure(IntPtr baseAddress) : base(baseAddress) { }

        public override void initVariables()
        {
            this.variables.Add(new Variable<Pointer>("enemyTeam"), 0x0);
            this.variables.Add(new Variable<Pointer>("allyTeam"), 0x4);
        }
    }
}
