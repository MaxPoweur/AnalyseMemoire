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
        public Dictionary<Variable, int> variables { get; set; }
        public bool isBaseAddressPointer { get; set; }

        public Structure(bool isBaseAddressPointer)
        {
            this.variables = new Dictionary<Variable, int>();
            this.isBaseAddressPointer = isBaseAddressPointer;
            this.initVariables();
        }

        public Structure(IntPtr baseAddress, bool isBaseAddressPointer)
        {
            this.baseAddress = baseAddress;
            this.isBaseAddressPointer = isBaseAddressPointer;
            this.variables = new Dictionary<Variable, int>();
            this.initVariables();
        }

        public abstract void initVariables();


        public int getVariableOffset(String name)
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
                if(variable.Key.type == VariableType.Structure)
                    return ((Structure)variable.Key.value).getVariable(name);
            }
            Console.WriteLine("La variable du nom \"" + name + "\" n'a pas été retrouvée.");
            return null;
        }

        public IntPtr getVariableAddress(string[] path)
        {
            //Console.WriteLine("New search : " + String.Join(",", path));
            foreach (var variable in this.variables)
            {
                //Console.WriteLine("Compare " + variable.Key.name + " et " + path[0]);
                if(variable.Key.name.Equals(path[0]) && variable.Key.type != VariableType.Structure && 1 != path.Length)
                {
                    Console.WriteLine("Le chemin d'accès à la variable est incorrect : " + path[0] + " n'est pas une structure et n'est pas en fin de chemin.");
                    return IntPtr.Zero;
                }
                else if (variable.Key.name.Equals(path[0]) && variable.Key.type == VariableType.Structure && 1 != path.Length)
                {
                    string[] newPath = Structure.Slice(path, 1, path.Length);
                    return ((Structure)variable.Key.value).getVariableAddress(newPath);
                }
                else if (variable.Key.name.Equals(path[0]) && variable.Key.type != VariableType.Structure && 1 == path.Length)
                    return new IntPtr(this.baseAddress.ToInt32() + variable.Value);
                else if (variable.Key.name.Equals(path[0]) && variable.Key.type == VariableType.Structure && 1 == path.Length)
                {
                    //Console.WriteLine("New search : " + String.Join(",", path));
                    //Console.WriteLine(((Structure)variable.Key.value).baseAddress.ToInt32().ToString("X"));
                    return ((Structure)variable.Key.value).baseAddress;
                }
            }
            Console.WriteLine("Le chemin d'accès " + string.Join(".", path) + " n'a pas été retrouvé.");
            return IntPtr.Zero;
        }

        public static string[] Slice(string[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            string[] res = new string[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }
    }
}
