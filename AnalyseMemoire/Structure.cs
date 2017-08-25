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
        protected Dictionary<Variable, int> variables;

        public Structure()
        {
            this.variables = new Dictionary<Variable, int>();
            this.initVariables();
        }

        public Structure(IntPtr baseAddress)
        {
            this.baseAddress = baseAddress;
            this.variables = new Dictionary<Variable, int>();
            this.initVariables();
        }

        public abstract void initVariables();


        public int getVariableOffset<T>(String name)
        {
            foreach(var variable in this.variables)
            {
                if (variable.Key.name.Equals(name))
                    return variable.Value;
            }
            Console.WriteLine("La variable du nom \""+name+"\" n'a pas été retrouvée.");
            return -1;
        }

        public Variable getVariable(String name)
        {
            foreach (var variable in this.variables)
            {
                if (variable.Key.name.Equals(name))
                    return variable.Key;
                if(variable.Key.type == VariablesTypes.Structure)
                    return ((Structure)variable.Key.value).getVariable(name);
            }
            Console.WriteLine("La variable du nom \"" + name + "\" n'a pas été retrouvée.");
            return null;
        }

        public IntPtr getVariableAddress(String name)
        {
            foreach (var variable in this.variables)
            {

                if (variable.Key.name.Equals(name) && variable.Key.type != VariablesTypes.Structure)
                    return new IntPtr(variable.Value+this.baseAddress.ToInt32());
                else if(variable.Key.name.Equals(name) && variable.Key.type == VariablesTypes.Structure && variable.Key.value != null)
                    return ((Structure)variable.Key.value).baseAddress;
                if (variable.Key.type == VariablesTypes.Structure && variable.Key.value!=null)
                    return ((Structure)variable.Key.value).getVariableAddress(name);
            }
            Console.WriteLine("La variable du nom \"" + name + "\" n'a pas été retrouvée.");
            return IntPtr.Zero;
        }
    }
}
