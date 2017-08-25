using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire
{
    class Variable<T> : IVariable
    {
        
        T value { get; set; }

        public Variable(String name)
        {
            this.name = name;
        }

        public Variable(String name, T value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
