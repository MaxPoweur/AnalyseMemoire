using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire.Structures
{
    class TeamStructure : Structure
    {
        public TeamStructure(bool isBasePointer) : base(isBasePointer) { }

        public TeamStructure(IntPtr baseAddress, bool isBasePointer) : base(baseAddress, isBasePointer) { }

        public override void initVariables()
        {
            this.variables.Add(new Variable("player1", VariableType.Structure, new PlayerStructure(false)), 0x0);
            this.variables.Add(new Variable("player2", VariableType.Structure, new PlayerStructure(false)), 0x88);
            this.variables.Add(new Variable("player2", VariableType.Structure, new PlayerStructure(false)), 0x110);
        }
    }
}
