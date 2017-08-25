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

            trainer.loadPlayersInfos();

            Console.WriteLine("allyTeam : " + trainer.getPrivateStruct().getVariableAddress("allyTeam").ToInt32().ToString("X"));
            //Console.WriteLine("enemyTeam location : " + trainer.getPrivateStruct().getVariableAddress("enemyTeam").ToInt32().ToString("X"));

            //Console.WriteLine(memtool.readPointer(new IntPtr(0x6e0000).ToInt32()).ToString("x"));

            //Console.WriteLine(BitConverter.ToString(FasmNet.Assemble(new string[] { "push eax", "push ebx", "mov eax, 123f123fh", "mov ebx, [ecx]", "mov [eax],[ebx+8]", "pop ebx", "pop eax", "call dword [eax+0Ch]", "add esp,0Ch" })));

        }
    }
}