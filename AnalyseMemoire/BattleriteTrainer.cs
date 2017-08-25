using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire
{
    class BattleriteTrainer : MemoryTool
    {
        public BattleriteTrainer() : base("Battlerite", 1024) { }

        public void loadPlayersInfos()
        {
            IntPtr pattern = IntPtr.Zero;
            int patternCount = 0;
            Pattern p;
            while (patternCount < PatternDatabase.PlayersInfos.Length)
            {
                p = PatternDatabase.PlayersInfos[patternCount];
                pattern = new IntPtr(this.AOBScan(p.pattern, p.offset)); // Search for pattern of playersInfos pattern
                if (pattern != IntPtr.Zero)
                    break;
                patternCount++;
            }

            if (pattern == IntPtr.Zero)
            {
                Console.WriteLine("No patterns available || Patterns not found in memory");
                return;
            }

            IntPtr codeCave = this.malloc(100);

            Console.WriteLine("Found PlayersDatas pattern at " + pattern.ToInt32().ToString("X"));

            this.injectCode(new string[] { "jmp " + codeCave.ToInt32().ToString("X") + "h", "nop" }, pattern); // codeCave redirection
            string[] codeCaveInstructions =
            {
                "push ecx",
                "push ebx",
                "mov ecx, " + this.privateStruct.baseAddress.ToInt32().ToString("X")+"h",

                "mov ebx, [eax]", // Write enemyTeam pointer
                "mov ebx,[ebx+8]",
                "mov [ecx], ebx",

                "mov ebx, [eax+4]", // Write allyTeam pointer
                "mov ebx,[ebx+8]",
                "mov [ecx+4], ebx",

                "pop ebx",
                "pop ecx",

                "add esp,0Ch", // ReWrite original instructions
                "sub esp,0Ch",

                "jmp " + (pattern.ToInt32() + 6).ToString("X") + "h" // Jump into next instructions
            };
            this.injectCode(codeCaveInstructions, codeCave);
        }
    }
}
