using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace SocketProgramming_1_Client
{
    class Program
    {
        static Socket sock;
        static int port = 8082;
        static string strip = "218.234.62.112";        
        static void Main(string[] args)
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strip), port); //종단점 설정
            sock.Connect(ip);
            byte[] receive = new byte[128];            
            sock.Receive(receive);
            string receiveData = Encoding.Default.GetString(receive);
            Console.WriteLine(receiveData);
            string line = string.Empty;            
            while((line = Console.ReadLine()) != null)
            {                
                byte [] sends = Encoding.Default.GetBytes(line);
                sock.Send(sends);
                line = string.Empty;
            }            
        }
    }
}
