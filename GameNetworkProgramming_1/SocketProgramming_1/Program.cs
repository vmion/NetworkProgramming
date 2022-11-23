using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace SocketProgramming_1
{    
    class Program
    {
        static Socket listenSock;
        static int port = 8082;
        static string strip = "218.234.62.112";
        static void Main(string[] args)
        {
            listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strip), port); //종단점 설정
            listenSock.Bind(ip);
            Console.WriteLine("Bind");
            listenSock.Listen(1000);
            Console.WriteLine("Listen");
            Socket client = listenSock.Accept();
            Console.WriteLine("Accept");
            string data = "Game에 오신 것을 환영합니다.";
            byte[] sendBuffer = Encoding.Default.GetBytes(data);
            client.Send(sendBuffer);
            Console.WriteLine(client.RemoteEndPoint + "님께서 접속했습니다.");
            
            byte[] receiveBuffer = new byte[128];
            while (true)
            {                
                client.Receive(receiveBuffer);
                string receive = Encoding.Default.GetString(receiveBuffer);
                Console.WriteLine("채팅 : " + receive);
                Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
            }            
        }
    }
}
