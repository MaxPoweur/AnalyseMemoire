using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire
{
    class Address<T>
    {
        private IntPtr address;
        public IntPtr getAddress
        {
            get { return this.address; }
            set { this.address = value; }
        }

        private T value;
        public T getValue
        {
            get { return this.value; }
            set {if(this.address!=default(IntPtr)) this.value = value; }
        }

        public Address()
        {
            this.address = default(IntPtr);
            this.value = default(T);
        }

        public Address(IntPtr address)
        {
            this.address = address;
            this.value = default(T);
        }

        public Address(IntPtr address, T value)
        {
            this.address = address;
            this.value = value;
        }

        public override String ToString()
        {
            return "["+this.address.ToString("X")+":"+this.value+"]";
        }
    }
}
