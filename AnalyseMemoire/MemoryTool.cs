using System;
using System.Linq;
using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Modules;
using Binarysharp.MemoryManagement.Assembly;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Native;
using Binarysharp.MemoryManagement.Threading;
using Binarysharp.MemoryManagement.Windows;
using Binarysharp.MemoryManagement.Patterns;
using System.Diagnostics;

namespace AnalyseMemoire
{
    class MemoryTool
    {
        private MemorySharp sharp;
        private Process process;

        public MemoryTool(String processName)
        {
            this.process = Process.GetProcessesByName(processName).First();
            this.sharp = new MemorySharp(process);
        }

        public MemoryTool(int processID)
        {
            this.process = Process.GetProcessById(processID);
            this.sharp = new MemorySharp(process);
        }

        public int getBaseAddressFromModule(String moduleName)
        {
            int count = 0;
            foreach (RemoteModule module in sharp.Modules.RemoteModules)
            {
                if (module.Name.Equals(moduleName))
                    return module.BaseAddress.ToInt32();
                  count++;
            }
            throw new Exception("There is any module with this name.");
        }

        public Address<T> Read<T>(int address, bool isRelative)
        {
            try
            {
                IntPtr addressFinal = new IntPtr(address);
                return new Address<T>(addressFinal, this.sharp.Read<T>(addressFinal, isRelative));
            }
            catch (TypeInitializationException)
            {
                Console.WriteLine("Error while trying to retrieve the specified value.\nPlease check the value type.");
                return new Address<T>();
            }
        }

        public Address<T> Read<T>(int address, int[] offsets, bool isRelative)
        {
            try
            {
                int currentAddress = this.Read<int>(address, isRelative).getValue;

                for(int i=0; i<offsets.Length-1; i++)
                {
                    currentAddress = this.Read<int>(currentAddress+offsets[i], isRelative).getValue;
                }

                Address<T> finalAddress = new Address<T>(new IntPtr(currentAddress+offsets[offsets.Length-1]), this.Read<T>(currentAddress + offsets[offsets.Length - 1], isRelative).getValue);
                return finalAddress;
            }
            catch (TypeInitializationException)
            {
                Console.WriteLine("Error while trying to retrieve the specified value.\nPlease check the value type.");
                return new Address<T>();
            }
        }

        public void Write<T>(int address, T value, bool isRelative)
        {
            try
            {
                this.sharp.Write<T>(new IntPtr(address), value, isRelative);
            }
            catch (TypeInitializationException)
            {
                Console.WriteLine("Error while trying to write to the specified address.\nPlease check the value type.");

            }
        }

        public void temp(IntPtr address)
        {
            sharp.Assembly.Inject(new String[]{"push eax", "mov eax, 0x123F123F", "mov [eax], ecx", "pop eax"},address);
        }
    }
}
