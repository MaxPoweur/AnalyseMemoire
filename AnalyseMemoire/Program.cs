using Binarysharp.MemoryManagement.Native.Enums;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;

namespace AnalyseMemoire
{
    class Program
    {

        static void Main(String[] args)
        {
            MemoryTool memtool = new MemoryTool("Battlerite", 1024);

            //int baseAddress = 0x101F30AC;
            //int[] offsets = {0x3C, 0x10, 0x10, 0x98, 0xB0, 0x28, 0x50, 0x18};
            //Address<float> health = memtool.Read<float>(baseAddress, offsets, false);

            //Console.WriteLine(health);
            memtool.loadPlayersInfos();
            //Console.WriteLine("Private Struct location : " + memtool.getPrivateStruct().baseAddress.ToInt32().ToString());
            Console.WriteLine("allyTeam : " + memtool.getPrivateStruct().getVariableAddress("allyTeam").ToInt32().ToString("X"));
            Console.WriteLine("enemyTeam location : " + memtool.getPrivateStruct().getVariableAddress("enemyTeam").ToInt32().ToString("X"));

            //Console.WriteLine(memtool.readPointer(new IntPtr(0x6e0000).ToInt32()).ToString("x"));

            //Console.WriteLine(BitConverter.ToString(FasmNet.Assemble(new string[] { "push eax", "push ebx", "mov eax, 123f123fh", "mov ebx, [ecx]", "mov [eax],[ebx+8]", "pop ebx", "pop eax", "call dword [eax+0Ch]", "add esp,0Ch" })));
        }
    }
}