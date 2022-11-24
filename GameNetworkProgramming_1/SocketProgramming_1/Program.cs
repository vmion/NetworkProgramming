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
            byte[] tmp = Encoding.Default.GetBytes(data);
            client.Send(tmp);
            Console.WriteLine(client.RemoteEndPoint + "님께서 접속했습니다.");
            
            byte[] receiveBuffer = new byte[128];
            byte[] sendBuffer = new byte[128];            
            while (true)
            {
                try
                {
                    if(client.Connected)
                    {
                        client.Receive(receiveBuffer);
                        Array.Copy(receiveBuffer, sendBuffer, receiveBuffer.Length);
                        client.Send(sendBuffer);
                        string receive = Encoding.Default.GetString(receiveBuffer);
                        Console.WriteLine("채팅 : " + receive);
                        Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
                        Array.Clear(sendBuffer, 0, sendBuffer.Length);
                    }                    
                }
                catch(SocketException e)
                {
                    //소켓에 대한 예외
                    Console.WriteLine(e.Message);
                    client.Close();
                }
                catch(ObjectDisposedException e)
                {
                    //삭제된 개체를 사용할시 발생되는 예외
                    Console.WriteLine(e.Message);
                }
            }            
        }
    }
}
