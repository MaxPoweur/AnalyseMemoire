﻿using System;

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

            //AOBScan newscan = new AOBScan(15924);
            //String adress = newscan.AobScan(new byte[] {0x85, 0xC0, 0x0F, 0x85, 0x80, 0x00, 0x00, 0x00, 0x8D, 0x46, 0x2C, 0x8B, 0x38, 0x85, 0xFF, 0x0F, 0x85, 0x60, 0x00, 0x00, 0x00, 0x8D, 0x46, 0x10, 0x8B, 0x18, 0x85, 0xDB, 0x74, 0x28, 0x8D, 0x46, 0x08, 0x8B, 0x00, 0x8D, 0x4D, 0xD0, 0x83, 0xEC, 0x08, 0x53, 0x51, 0xFF, 0xD0, 0x83, 0xC4, 0x0C, 0x8B, 0x45, 0x08, 0x8B, 0x4D, 0xD0, 0x89, 0x08, 0x8B, 0x4D, 0xD4, 0x89, 0x48, 0x04, 0x8B, 0x4D, 0xD8, 0x89, 0x48, 0x08, 0xEB, 0x25, 0x8D, 0x46, 0x08, 0x8B, 0x00, 0x8D, 0x4D, 0xDC, 0x83, 0xEC, 0x0C, 0x51, 0xFF, 0xD0, 0x83, 0xC4, 0x0C, 0x8B, 0x45, 0x08, 0x8B, 0x4D, 0xDC, 0x89, 0x08, 0x8B, 0x4D, 0xE0, 0x89, 0x48, 0x04, 0x8B, 0x4D, 0xE4, 0x89, 0x48, 0x08, 0x8D, 0x65, 0xF4, 0x5E, 0x5F, 0x5B, 0xC9, 0xC2, 0x04, 0x00, 0x8D, 0x45, 0xE8, 0x83, 0xEC, 0x08, 0x57, 0x50, 0x90, 0x90, 0x90, 0xFF, 0x57, 0x0C, 0x83, 0xC4, 0x0C, 0xEB, 0x8D, 0xE8, 0x59, 0x54, 0xF6, 0xC9, 0xE9, 0x76, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x55, 0x8B, 0xEC, 0x57, 0x83, 0xEC, 0x04, 0x8B, 0x7D, 0x08, 0x8B, 0x47, 0x08, 0x8B, 0xC8, 0x8B, 0x49, 0x0C, 0x83, 0xEC, 0x04, 0x51, 0x6A, 0x00, 0x50, 0xE8, 0xEA, 0x1F, 0xFF, 0xC9, 0x83, 0xC4, 0x10, 0xC7, 0x47, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x47, 0x10, 0x40, 0x89, 0x47, 0x10, 0x8D, 0x65, 0xFC, 0x5F, 0xC9, 0xC3, 0x00, 0x00, 0x00, 0x55, 0x8B, 0xEC, 0x57, 0x83, 0xEC, 0x04, 0x8B, 0x7D, 0x08, 0x8B, 0x47, 0x08, 0x8B, 0xC8, 0x8B, 0x49, 0x0C, 0x83, 0xEC, 0x04, 0x51, 0x6A, 0x00, 0x50, 0xE8, 0xB2, 0x1F, 0xFF, 0xC9, 0x83, 0xC4, 0x10, 0xC7, 0x47, 0x0C, 0x00, 0x00, 0x00, 0x00}).ToInt32().ToString("X");
            //Console.WriteLine(adress);
            int addressToInject = 0x473F4355;
            memtool.temp(new IntPtr(addressToInject));
        }
    }
}