using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire.Structures
{
    abstract class Structure
    {
        public IntPtr baseAddress { get; set; }
        protected Dictionary<IVariable, int> variables;

        public Structure()
        {
            this.variables = new Dictionary<IVariable, int>();
            this.initVariables();
        }

        public Structure(IntPtr baseAddress)
        {
            this.baseAddress = baseAddress;
            this.variables = new Dictionary<IVariable, int>();
            this.initVariables();
        }

        public abstract void initVariables();


        public int getVariableOffset<T>(String name)
        {
            foreach(KeyValuePair<IVariable, int> variable in this.variables)
            {
                if (variable.Key.name.Equals(name))
                    return variable.Value;
            }
            Console.WriteLine("La variable du nom \""+name+"\" n'a pas été retrouvée.");
            return -1;
        }

        public IVariable getVariable(String name)
        {
            foreach (KeyValuePair<IVariable, int> variable in this.variables)
            {
                if (variable.Key.name.Equals(name))
                    return variable.Key;
            }
            Console.WriteLine("La variable du nom \"" + name + "\" n'a pas été retrouvée.");
            return null;
        }

        public IntPtr getVariableAddress(String name)
        {
            foreach (KeyValuePair<IVariable, int> variable in this.variables)
            {
                if (variable.Key.name.Equals(name))
                    return new IntPtr(variable.Value+this.baseAddress.ToInt32());
            }
            Console.WriteLine("La variable du nom \"" + name + "\" n'a pas été retrouvée.");
            return IntPtr.Zero;
        }
    }
}
