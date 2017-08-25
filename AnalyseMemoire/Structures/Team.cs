using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire.Structures
{
    class Team : Structure
    {
        public Team() : base() { }

        public Team(IntPtr baseAddress) : base(baseAddress) { }

        public override void initVariables()
        {
            this.variables.Add(new Variable("enemyTeam", VariablesTypes.Structure, this.getVariableAddress("enemyTeam")), 0x0);
            this.variables.Add(new Variable("allyTeam", VariablesTypes.Structure, this.getVariableAddress("enemyTeam")), 0x4);
        }
    }
}
