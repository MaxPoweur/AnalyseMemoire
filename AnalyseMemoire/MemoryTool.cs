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
using System.Threading;
using System.IO;

namespace AnalyseMemoire
{
    abstract class MemoryTool
    {
        protected MemorySharp sharp { get; set; }
        protected Process process { get; set; }
        protected int sizePrivateStruct { get; set; }
        protected PrivateStructure privateStruct { get; set; }
        protected bool newPrivateStructure;


        public MemoryTool(String processName, int sizePrivateStruct)
        {
            try
            {
                this.process = Process.GetProcessesByName(processName).First();
                this.sharp = new MemorySharp(process);

                Thread threadCheckingProcess = new Thread(new ThreadStart(this.checkProcess));
                threadCheckingProcess.Start();

                this.sizePrivateStruct = sizePrivateStruct;
                this.allocPrivateStructure();
                Console.WriteLine("Private structure isset to " + this.privateStruct.baseAddress.ToInt32().ToString("X"));

                Thread threadRefreshingStructures = new Thread(new ThreadStart(this.refreshStructures));
                threadRefreshingStructures.Start();
                this.init();
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is InvalidOperationException)
                {
                    Console.WriteLine("Le processus distant est fermé. Merci de l'ouvrir puis de lancer notre programme.");
                    Environment.Exit(1);
                }
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public MemoryTool(int processID, int sizePrivateStruct)
        {
            try
            {
                this.process = Process.GetProcessById(processID);
                this.sharp = new MemorySharp(process);

                Thread threadCheckingProcess = new Thread(new ThreadStart(this.checkProcess));
                threadCheckingProcess.Start();

                this.sizePrivateStruct = sizePrivateStruct;
                this.allocPrivateStructure();
                Console.WriteLine("Private structure isset to " + this.privateStruct.baseAddress.ToInt32().ToString("X"));

                Thread threadRefreshingStructures = new Thread(new ThreadStart(this.refreshStructures));
                threadRefreshingStructures.Start();
                this.init();
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is InvalidOperationException)
                {
                    Console.WriteLine("Le processus distant est fermé. Merci de l'ouvrir puis de lancer notre programme.");
                    Environment.Exit(1);
                }
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
        
        public abstract void init();

        public void checkProcess()
        {
            try
            {
                while (true)
                    Process.GetProcessById(this.process.Id);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is InvalidOperationException)
                {
                    Console.WriteLine("Le processus distant est fermé. Merci de l'ouvrir puis de lancer notre programme.");
                    Environment.Exit(1);
                }
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public void refreshStructures()
        {
            while(true)
                this.setStructure(this.privateStruct);
        }

        public void setStructure(Structure structure)
        {
            if (structure.baseAddress == IntPtr.Zero)
                return;
            foreach (var variable in structure.variables)
            {
                if (variable.Key.type == VariableType.Structure)
                {
                    int position = structure.baseAddress.ToInt32() + variable.Value;
                    IntPtr pointer = this.readPointer(new IntPtr(position));

                    ((Structure)variable.Key.value).baseAddress = (((Structure)variable.Key.value).isBaseAddressPointer) ? pointer : new IntPtr(position);

                    //Console.WriteLine(variable.Key.name + " - " +  (((Structure)variable.Key.value).isBaseAddressPointer) + " [" + (position).ToString("X") + ":"+ pointer.ToInt32().ToString("X") + "] > " + ((Structure)variable.Key.value).baseAddress.ToInt32().ToString("X"));

                    this.setStructure(((Structure)variable.Key.value));
                }
                if (variable.Key.type == VariableType.Pointer)
                {
                    variable.Key.value = this.readPointer(new IntPtr(structure.baseAddress.ToInt32() + variable.Value));
                    //Console.WriteLine("ptr: " + (structure.baseAddress.ToInt32() + variable.Value).ToString("X"));
                }

            }
        }

        public void allocPrivateStructure()
        {
            String path = "settings.ini";
            IntPtr baseAddress;
            if (!File.Exists(path))
            {
                File.CreateText(path);
                baseAddress = this.malloc(this.sizePrivateStruct);
                this.newPrivateStructure = true;
            }
            else
            {
                string[] fileContent = File.ReadAllLines(path);
                if (fileContent.Length == 0)
                {
                    baseAddress = this.malloc(this.sizePrivateStruct);
                    this.newPrivateStructure = true;
                }
                else
                {
                    string lastBaseAddress = fileContent[0];
                    try
                    {
                        if (this.sharp.Read<int>(new IntPtr(Convert.ToInt32(lastBaseAddress, 16)), false) != 1)
                        {
                            baseAddress = this.malloc(this.sizePrivateStruct);
                            this.newPrivateStructure = true;
                        }
                        else
                        {
                            baseAddress = new IntPtr(Convert.ToInt32(lastBaseAddress, 16));
                            this.newPrivateStructure = false;
                        }

                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        baseAddress = this.malloc(this.sizePrivateStruct);
                        this.newPrivateStructure = true;
                    }
                }
            }
                
            this.privateStruct = new PrivateStructure(baseAddress, true);

            this.setStructure(this.privateStruct);

            File.WriteAllLines(path, new string[]{ baseAddress.ToInt32().ToString("X")});
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
                if(this.getBaseAddressFromModule(module.Name).ToInt32()<= address.ToInt32() && this.getEndAddressFromModule(module.Name).ToInt32() >= address.ToInt32())
                {
                    return module.Name;
                }
            }
            return null;
        }

        public IntPtr getBaseAddressFromModule(String moduleName)
        {
            foreach (RemoteModule module in sharp.Modules.RemoteModules)
            {
                if (module.Name.Equals(moduleName))
                    return module.BaseAddress;
            }
            throw new Exception("There is any module with this name.");
        }

        public IntPtr getEndAddressFromModule(String moduleName)
        {
            foreach (RemoteModule module in sharp.Modules.RemoteModules)
            {
                if (module.Name.Equals(moduleName))
                    return new IntPtr(module.BaseAddress.ToInt32() + module.Size);
            }
            throw new Exception("There is any module with this name.");
        }
        
        public void displayModules()
        {
            string[] modules = this.getAllModules();
            foreach(string module in modules)
            {
                Console.WriteLine(module + " => [" + this.getBaseAddressFromModule(module).ToInt32().ToString("X") + "-" + this.getEndAddressFromModule(module).ToInt32().ToString("X") + "]");
            }
        }

        public IntPtr malloc(int size)
        {
            IntPtr codeCave;
            do
            {
                codeCave = VirtualAllocEx(this.process.Handle, IntPtr.Zero, (uint)size, AllocationType.Commit, MemoryProtection.ExecuteReadWrite);
            } while (Char.IsDigit(codeCave.ToInt32().ToString("X"), 0));
            if(codeCave == null)
                System.Console.WriteLine("ERROR VIRTUALALOCEX " + Marshal.GetLastWin32Error());
            return codeCave;
        }

        public void injectCode(string[] toInject, IntPtr address)
        {
            this.sharp.Assembly.Inject(toInject, address);
        }

        public IntPtr getVariableAddress(string[] path)
        {
            return this.privateStruct.getVariableAddress(path);
        }

        public IntPtr readPointer(IntPtr address)
        {
            byte[] buffer = new byte[4];
            int bytesRead;
            if (this.process.Handle==IntPtr.Zero)
                Console.WriteLine("handle error ! " + Marshal.GetLastWin32Error());

            if (!ReadProcessMemory(this.process.Handle, address, buffer, buffer.Length, out bytesRead))
            {
                //Console.WriteLine("ERROR READ " + Marshal.GetLastWin32Error());
            }

            //Console.WriteLine(address.ToInt32().ToString("X") + ":: " + BitConverter.ToString(buffer));

            return new IntPtr(BitConverter.ToInt32(buffer, 0));
        }

        public void displayVariable<T>(string[] path)
        {
            string display = "[";
            foreach (string element in path)
                display += element + ".";
            display = display.Substring(0, display.Length - 1);
            display += "::";
            if (!this.isVariableIsset(path))
                display += "nonInitialisée";
            else
                display += this.getVariable<T>(path);
            display += "]";
            Console.WriteLine(display);
        }
        public bool isVariableIsset(string[] path)
        {
            //Console.WriteLine(path[path.Length - 1] + " ::: " + this.privateStruct.getVariableAddress(path).ToInt32().ToString("X") + " = " + (this.privateStruct.getVariableAddress(path) != IntPtr.Zero));
            return this.privateStruct.getVariableAddress(path) != IntPtr.Zero;
        }

        public T getVariable<T>(string[] path)
        {
            IntPtr address = this.privateStruct.getVariableAddress(path);
            return this.sharp.Read<T>(address, false);
        }

        public void loadDatas(String name, Pattern[] patterns, string[] codeCaveInstructions)
        {
            IntPtr pattern = IntPtr.Zero;
            Pattern p;
            while (pattern == IntPtr.Zero)
            {
                var patternCount = 0;
                while (patternCount < patterns.Length)
                {
                    p = patterns[patternCount];
                    pattern = new IntPtr(this.AOBScan(p.pattern, p.offset)); // Search for pattern of playersInfos pattern
                    if (pattern != IntPtr.Zero)
                        break;
                    patternCount++;
                }
            }

            IntPtr codeCave = this.malloc(100);

            Console.WriteLine("Found " + name + " pattern at " + pattern.ToInt32().ToString("X"));
            this.injectCode(new string[] { "jmp dword 0x" + codeCave.ToInt32().ToString("X") }, pattern); // codeCave redirection

            //foreach (string code in codeCaveInstructions)
            //{
            //    Console.WriteLine(code);
            //}

            codeCaveInstructions = codeCaveInstructions.Concat(new string[] { "jmp dword 0x" + (pattern.ToInt32() + 5).ToString("X") }).ToArray();
            this.injectCode(codeCaveInstructions, codeCave);
            Console.WriteLine(name + " codecave injected!");
            Thread.CurrentThread.Abort();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out()] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

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
    }
}
