using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnalyseMemoire
{
    class BattleriteTrainer : MemoryTool
    {
        public BattleriteTrainer() : base("Battlerite", 1024) { }

        public override void init()
        {
            if (this.newPrivateStructure)
            {
                Thread threadLoadingPlayerStatus = new Thread(new ThreadStart(this.loadUserStatus));
                threadLoadingPlayerStatus.Start(); // userStatus utilisable uniquement lorsque l'user a déjà été horsJeu
            }
            if (!this.isVariableIsset(new string[] { "allyTeam" }))
            {
                Thread threadLoadingPlayersDatas = new Thread(new ThreadStart(this.loadPlayersInfos));
                threadLoadingPlayersDatas.Start();
            }
        }

        public void loadDatas(string name, Pattern[] patterns, string[] codeCaveInstructions, int sizeInstruction)
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

            Console.WriteLine("Found "+name+" pattern at " + pattern.ToInt32().ToString("X"));
            this.injectCode(new string[] { "jmp dword 0x" + codeCave.ToInt32().ToString("X") }, pattern); // codeCave redirection

            //foreach (string code in codeCaveInstructions)
            //{
            //    Console.WriteLine(code);
            //}
            codeCaveInstructions = codeCaveInstructions.Concat(new string[]{"jmp dword 0x" + (pattern.ToInt32() + sizeInstruction).ToString("X")}).ToArray();
             // Jump into next instructions
            this.injectCode(codeCaveInstructions, codeCave);
            Thread.CurrentThread.Abort();
        }

        public void loadUserStatus()
        {
            string[] codeCaveInstructions =
            {
                "push ecx",
                "push ebx",
                "mov ecx, 0x" + this.privateStruct.baseAddress.ToInt32().ToString("X"),

                "mov ebx, 1h", // privateStructure ON
                "mov [ecx], ebx",

                "mov ebx, eax", // Write userStatusPointer
                "mov [ecx+0xC], ebx",

                "pop ebx",
                "pop ecx",

                "mov ecx, [eax]", // ReWrite original instructions
                "mov [ebp-14], ecx",
            };

            this.loadDatas("userStatus", PatternDatabase.UserStatus, codeCaveInstructions, 5);
        }

        public void loadPlayersInfos()
        {
            string[] codeCaveInstructions =
            {
                "push ecx",
                "push ebx",
                "mov ecx, 0x" + this.privateStruct.baseAddress.ToInt32().ToString("X"),

                "mov ebx, 1h", // privateStructure ON
                "mov [ecx], ebx",

                "mov ebx, [eax]", // Write enemyTeam pointer
                "mov ebx, [ebx+8]",
                "mov [ecx+4], ebx",

                "mov ebx, [eax+4]", // Write allyTeam pointer
                "mov ebx, [ebx+8]",
                "mov [ecx+8], ebx",

                "pop ebx",
                "pop ecx",

                "add esp, 0Ch", // ReWrite original instructions
                "sub esp, 0Ch"
            };

            this.loadDatas("playerInfos", PatternDatabase.PlayersInfos, codeCaveInstructions, 6);
        }
    }
}
