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
            sock.Connect(ip); //원격 사용 요청(연결하다의 의미)
            byte[] receiveBuffer = new byte[128];
            byte[] sendBuffer = new byte[128];
            byte[] tmp = new byte[128];
            sock.Receive(tmp);
            string receiveData = Encoding.Default.GetString(tmp);
            Console.WriteLine(receiveData);            
            string line = string.Empty;
            receiveData = string.Empty;
            while ((line = Console.ReadLine()) != null)
            {                
                byte [] sends = Encoding.Default.GetBytes(line);
                Array.Copy(sends, sendBuffer, sends.Length);
                sock.Send(sendBuffer);
                sock.Receive(receiveBuffer);
                receiveData = Encoding.Default.GetString(receiveBuffer);
                Console.WriteLine(receiveData);                                
                Array.Clear(sendBuffer, 0, sendBuffer.Length);
                Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
                line = string.Empty;
                receiveData = string.Empty;
            }            
        }
    }
}
