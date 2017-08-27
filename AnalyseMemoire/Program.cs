using Binarysharp.MemoryManagement.Native.Enums;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;
using AnalyseMemoire.Structures;

namespace AnalyseMemoire
{
    class Program
    {

        static void Main(String[] args)
        {
            BattleriteTrainer trainer = new BattleriteTrainer();

            Console.WriteLine("Private struct address : " + trainer.getPrivateStruct().baseAddress.ToInt32().ToString("X"));
            Console.WriteLine("allyTeam address : " + trainer.getPrivateStruct().getVariableAddress(new string[] { "allyTeam" }).ToInt32().ToString("X"));
            Console.WriteLine("allyTeam.p1 address : " + trainer.getPrivateStruct().getVariableAddress(new string[] { "allyTeam", "player1" }).ToInt32().ToString("X"));
            Console.WriteLine("allyTeam.p1.health address : " + trainer.getPrivateStruct().getVariableAddress(new string[] { "allyTeam", "player1", "health" }).ToInt32().ToString("X"));
            //Process p = Process.GetProcessesByName("battlerite")[0];
            //Console.WriteLine(p.Id);
            //Console.WriteLine(trainer.readPointer(new IntPtr(0x41b9d088)).ToString("x"));

            //Console.WriteLine(BitConverter.ToString(FasmNet.Assemble(new string[] { "push eax", "push ebx", "mov eax, 123f123fh", "mov ebx, [ecx]", "mov [eax],[ebx+8]", "pop ebx", "pop eax", "call dword [eax+0Ch]", "add esp,0Ch" })));

        }
    }
}