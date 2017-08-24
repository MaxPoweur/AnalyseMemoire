using Binarysharp.MemoryManagement.Native.Enums;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace AnalyseMemoire
{
    class Program
    {

        static void Main(String[] args)
        {
            MemoryTool memtool = new MemoryTool("Battlerite");

            //int baseAddress = 0x101F30AC;
            //int[] offsets = {0x3C, 0x10, 0x10, 0x98, 0xB0, 0x28, 0x50, 0x18};
            //Address<float> health = memtool.Read<float>(baseAddress, offsets, false);

            //Console.WriteLine(health);
            
            Console.WriteLine(memtool.AOBScan(PatternDatabase.PlayersInfos, 0xC).ToString("X"));
        }
    }
}