using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProcessExample
{
    class Program
    {
        static void Main(string[] args)
        {
            for(int i = 0; i < 10; i++)
            {
                Process.Start("Z:\\github\\NetworkProgramming\\GameNetworkProgramming_1" +
                "\\AsyncExample_Client\\bin\\Debug\\AsyncExample_Client.exe");
            }            
        }
    }
}
