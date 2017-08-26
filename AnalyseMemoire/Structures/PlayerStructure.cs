using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire.Structures
{
    class PlayerStructure : Structure
    {
        public PlayerStructure(bool isBasePointer) : base(isBasePointer) { }

        public PlayerStructure(IntPtr baseAddress, bool isBasePointer) : base(baseAddress, isBasePointer) { }

        public override void initVariables()
        {
            this.variables.Add(new Variable("health", VariableType.Float), 0x18);
            this.variables.Add(new Variable("maxHealth", VariableType.Float), 0x1C);
            this.variables.Add(new Variable("energy", VariableType.Float), 0x3C);
            this.variables.Add(new Variable("maxEnergy", VariableType.Float), 0x40);
            this.variables.Add(new Variable("weaponStacks", VariableType.Float), 0x54);
            this.variables.Add(new Variable("maxWeaponStacks", VariableType.Float), 0x58);
        }
    }
}
