using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire.Structures
{
    class PrivateStructure : Structure
    {
        public PrivateStructure(bool isBasePointer) : base(isBasePointer) { }

        public PrivateStructure(IntPtr baseAddress, bool isBasePointer) : base(baseAddress, isBasePointer) { }

        public override void initVariables()
        {
            this.variables.Add(new Variable("allyTeam", VariableType.Structure, new TeamStructure(true)), 0x4);
            this.variables.Add(new Variable("enemyTeam", VariableType.Structure, new TeamStructure(true)), 0x8);
            this.variables.Add(new Variable("userStatus", VariableType.Pointer, new IntPtr()), 0xC);
        }
    }
}
