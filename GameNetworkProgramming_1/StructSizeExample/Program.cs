using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace StructSizeExample
{
    class Program
    {
        public struct Data
        {            
            public byte d1;
            public short d2;
            public int d3;
            public long d4;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("byte의 크기 = " + sizeof(byte));
            Console.WriteLine("short의 크기 = " + sizeof(short));
            Console.WriteLine("int의 크기 = " + sizeof(int));
            Console.WriteLine("long의 크기 = " + sizeof(long));
            Console.WriteLine("Data 구조체의 크기 = " + Marshal.SizeOf(typeof(Data)));
        }
    }
}
