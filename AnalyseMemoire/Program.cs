using Binarysharp.MemoryManagement.Native.Enums;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;
using AnalyseMemoire.Structures;
using System.Threading;

namespace AnalyseMemoire
{
    class Program
    {

        static void Main(String[] args)
        {
            //BattleriteTrainer trainer = new BattleriteTrainer();
            //string[] path = new string[] {"userStatus"};
            //trainer.displayVariable<int>(path);
            //path = new string[] {"allyTeam", "player1", "health" };
            //trainer.displayVariable<float>(path);
            //path = new string[] {"allyTeam", "player2", "health"};
            //trainer.displayVariable<float>(path);
            //path = new string[] {"allyTeam", "player3", "health"};
            //trainer.displayVariable<float>(path);
            //path = new string[] {"enemyTeam", "player1", "health"};
            //trainer.displayVariable<float>(path);
            //path = new string[] {"enemyTeam", "player2", "health"};
            //trainer.displayVariable<float>(path);
            //path = new string[] {"enemyTeam", "player3", "health"};
            //trainer.displayVariable<float>(path);


            //string[] codeCaveInstructions =
            //{
            //    "push ecx",
            //    "push ebx",
            //    "mov ecx, 0xA150000",

            //    "mov ebx, 1h",
            //    "mov [ecx], ebx",

            //    "mov ebx, [eax]", // Write enemyTeam pointer
            //    "mov ebx, [ebx+8]",
            //    "mov [ecx+4], ebx",

            //    "mov ebx, [eax+4]", // Write allyTeam pointer
            //    "mov ebx, [ebx+8]",
            //    "mov [ecx+8], ebx",

            //    "pop ebx",
            //    "pop ecx",

            //    "add esp, 0Ch", // ReWrite original instructions
            //    "sub esp, 0Ch",

            //    "jmp dword 44B1DD3F" // Jump into next instructions
            //};

            //foreach (string code in codeCaveInstructions)
            //{
            //    Console.WriteLine(code);
            //}

            Console.WriteLine(BitConverter.ToString(FasmNet.Assemble(new string[]{ "add esp, 0Ch" })));


        }
    }
}