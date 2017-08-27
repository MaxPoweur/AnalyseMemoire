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
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;
using Binarysharp.MemoryManagement.Native.Enums;
using AnalyseMemoire.Structures;

namespace AnalyseMemoire
{
    abstract class MemoryTool
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        protected MemorySharp sharp;
        protected Process process;
        protected int sizePrivateStruct;
        protected PrivateStructure privateStruct;

        public MemoryTool(String processName, int sizePrivateStruct)
        {
            this.process = Process.GetProcessesByName(processName).First();
            this.sharp = new MemorySharp(process);
            this.sizePrivateStruct = sizePrivateStruct;
            this.privateStruct = new PrivateStructure(this.malloc(this.sizePrivateStruct), true);
            Console.WriteLine("Private structure isset to " + this.privateStruct.baseAddress.ToInt32().ToString("X"));
            this.init();
            this.initStructure(this.privateStruct);
        }

        public MemoryTool(int processID, int sizePrivateStruct)
        {
            this.process = Process.GetProcessById(processID);
            this.sharp = new MemorySharp(process);
            this.sizePrivateStruct = sizePrivateStruct;
            this.privateStruct = new PrivateStructure(this.malloc(this.sizePrivateStruct), true);
            this.init();
            this.initStructure(this.privateStruct);
        }

        public String[] getAllModules()
        {
            String[] modules = new String[sharp.Modules.RemoteModules.Count()];
            int count = 0;
            foreach (RemoteModule module in sharp.Modules.RemoteModules)
            {
                Console.WriteLine(module.Name + " => " + this.getBaseAddressFromModule(module.Name).ToString("X") + " - " + this.getEndAddressFromModule(module.Name).ToString("X"));
                modules[count] = module.Name;
                count++;
            }
            return modules;
        }

        public int AOBScan(byte[] pattern, int offset)
        {
            AOBScan aobscan = new AOBScan((uint)this.process.Id);

            return aobscan.AobScan(pattern).ToInt32()==0?0:(aobscan.AobScan(pattern).ToInt32() + offset);
        }

        public String getModuleOfAddress(IntPtr address)
        {
            foreach (RemoteModule module in sharp.Modules.RemoteModules)
            {
                if(this.getBaseAddressFromModule(module.Name)<= address.ToInt32() && this.getEndAddressFromModule(module.Name) >= address.ToInt32())
                {
                    return module.Name;
                }
            }
            return null;
        }

        public int getBaseAddressFromModule(String moduleName)
        {
            foreach (RemoteModule module in sharp.Modules.RemoteModules)
            {
                if (module.Name.Equals(moduleName))
                    return module.BaseAddress.ToInt32();
            }
            throw new Exception("There is any module with this name.");
        }

        public int getEndAddressFromModule(String moduleName)
        {
            foreach (RemoteModule module in sharp.Modules.RemoteModules)
            {
                if (module.Name.Equals(moduleName))
                    return module.BaseAddress.ToInt32() + module.Size;
            }
            throw new Exception("There is any module with this name.");
        }

        public Address<T> Read<T>(IntPtr address, bool isRelative)
        {
            try
            {
                IntPtr addressFinal = address;
                return new Address<T>(address, this.sharp.Read<T>(addressFinal, isRelative));
            }
            catch (TypeInitializationException)
            {
                Console.WriteLine("Error while trying to retrieve the specified value.\nPlease check the value type.");
                return new Address<T>();
            }
        }

        public Address<T> Read<T>(IntPtr address, int[] offsets, bool isRelative)
        {
            try
            {
                int currentAddress = this.Read<int>(address, isRelative).getValue;

                for(int i=0; i<offsets.Length-1; i++)
                {
                    currentAddress = this.Read<int>(new IntPtr(currentAddress+offsets[i]), isRelative).getValue;
                }

                Address<T> finalAddress = new Address<T>(new IntPtr(currentAddress+offsets[offsets.Length-1]), this.Read<T>(new IntPtr(currentAddress + offsets[offsets.Length - 1]), isRelative).getValue);
                return finalAddress;
            }
            catch (TypeInitializationException)
            {
                Console.WriteLine("Error while trying to retrieve the specified value.\nPlease check the value type.");
                return new Address<T>();
            }
        }

        public void Write<T>(IntPtr address, T value, bool isRelative)
        {
            try
            {
                this.sharp.Write<T>(address, value, isRelative);
            }
            catch (TypeInitializationException)
            {
                Console.WriteLine("Error while trying to write to the specified address.\nPlease check the value type.");

            }
        }

        public IntPtr malloc(int size)
        {

            IntPtr processPtr = OpenProcess((int)ProcessAccessFlags.AllAccess, false, this.process.Id);
            IntPtr codeCave = VirtualAllocEx(processPtr, IntPtr.Zero, (uint)size, AllocationType.Commit, MemoryProtection.ExecuteReadWrite);
            return codeCave;
        }

        public void injectCode(string[] toInject, IntPtr address)
        {
            IntPtr processPtr = OpenProcess((int)ProcessAccessFlags.AllAccess, false, this.process.Id);
            this.sharp.Assembly.Inject(toInject, address);
        }

        public IntPtr getVariableAddress(string[] path)
        {
            return this.privateStruct.getVariableAddress(path);
        }

        public IntPtr readPointer(IntPtr address)
        {
            IntPtr processHandle = OpenProcess((int)ProcessAccessFlags.AllAccess, false, this.process.Id);
            byte[] buffer = new byte[4];
            int bytesRead=0;
            ReadProcessMemory((int)processHandle, 0x0046A3B8, buffer, 4, ref bytesRead);
            Console.WriteLine("OUII:: " + BitConverter.ToString(buffer));
            return new IntPtr(BitConverter.ToInt32(buffer, 0));
        }
        
        public Structure getPrivateStruct()
        {
            return this.privateStruct;
        }

        public void initStructure(Structure structure)
        {
            foreach(var variable in structure.variables)
            {
                if(variable.Key.type==VariableType.Structure)
                {
                    ((Structure)variable.Key.value).baseAddress = (((Structure)variable.Key.value).isBaseAddressPointer)?this.readPointer(new IntPtr(structure.baseAddress.ToInt32() + variable.Value)):new IntPtr(structure.baseAddress.ToInt32()+variable.Value);
                    Console.WriteLine(variable.Key.name + (((Structure)variable.Key.value).isBaseAddressPointer) + "[" + (structure.baseAddress.ToInt32() + variable.Value).ToString("X") + ":"+ this.readPointer(new IntPtr(structure.baseAddress.ToInt32() + variable.Value)).ToInt32().ToString("X") + "] > " + ((Structure)variable.Key.value).baseAddress.ToInt32().ToString("X"));
                    this.initStructure(((Structure)variable.Key.value));
                }

            }
        }

        public abstract void init();
    }
}
