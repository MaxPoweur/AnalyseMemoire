using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire
{
    class Variable
    {
        public String name { get; set; }
        public VariablesTypes type { get; set; }
        public Object value { get; set; }

        public Variable(String name, VariablesTypes type)
        {
            this.name = name;
            this.type = type;
        }

        public Variable(String name, VariablesTypes type, Object value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }
    }
}
