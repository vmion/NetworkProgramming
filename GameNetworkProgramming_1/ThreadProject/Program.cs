using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadProject
{
    class Program
    {
        static Thread t1;
        static void Main(string[] args)
        {
            ThreadStart threadStart = new ThreadStart(NewClient);
            t1 = new Thread(threadStart);
            t1.Start();
            int count = 0;
            while (count < 300)
            {
                count++;
                Console.WriteLine("Main함수 = " + count);
            }
            t1.Join(); //인스턴스가 종료될때 까지 호출 스레드를 차단합니다.
            Console.WriteLine("Join");
            t1.Interrupt();
            Console.WriteLine("Interrupt");
        }
        static void NewClient()
        {
            int threadCount = 0;
            while(threadCount < 300)
            {
                threadCount++;
                Console.WriteLine("NewClient" + threadCount);
                Thread.Sleep(10);
            }
        }
    }
}
