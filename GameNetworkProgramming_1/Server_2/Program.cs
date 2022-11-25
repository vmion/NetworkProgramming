using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Server_2
{
    class Program
    {
        static Thread t1;
        static Socket listenSock;
        static int port = 8082;
        static string strip = "218.234.62.112";
        static List<UserClass> userList;
        static void Main(string[] args)
        {
            listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strip), port); 
            listenSock.Bind(ip);
            Console.WriteLine("Bind");
            
            ThreadStart threadStart = new ThreadStart(NewClient);
            t1 = new Thread(threadStart);
            t1.Start();                        

            byte[] receiveBuffer = new byte[128];
            byte[] sendBuffer = new byte[128];
            userList = new List<UserClass>();
            /*
            for(int i = 0; i < userList.Count; i++)
            {
                userList[i].userSock.Receive(userList[i].receiveBuffer);
                Array.Copy(userList[i].receiveBuffer, userList[i].sendBuffer,
                    userList[i].receiveBuffer.Length);
                userList[i].userSock.Send(userList[i].sendBuffer);
                Array.Clear(userList[i].receiveBuffer, 0, userList[i].receiveBuffer.Length);
                Array.Clear(userList[i].sendBuffer, 0, userList[i].sendBuffer.Length);
            }
            */
        }
        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket userSock = listenSock.EndAccept(ar);
            UserClass newUser = new UserClass(userSock);
            userList.Add(newUser);
            byte[] tmp = Encoding.Default.GetBytes("Game에 오신 것을 환영합니다.");
            userSock.Send(tmp);
            userSock.BeginReceive(newUser.receiveBuffer, 0, newUser.receiveBuffer.Length,
                SocketFlags.None, ReceiveCallBack, newUser);
        }
        static void ReceiveCallBack(IAsyncResult ar)
        {
            UserClass user = (UserClass)ar.AsyncState;
            Array.Copy(user.receiveBuffer, user.sendBuffer, user.receiveBuffer.Length);
            Array.Clear(user.receiveBuffer, 0, user.receiveBuffer.Length);
            user.userSock.BeginSend(user.sendBuffer, 0, user.sendBuffer.Length, 
                SocketFlags.None, SendCallBack, user);
        }
        static void SendCallBack(IAsyncResult ar)
        {
            UserClass user = (UserClass)ar.AsyncState;
            Array.Clear(user.sendBuffer, 0, user.sendBuffer.Length);
            user.userSock.BeginReceive(user.receiveBuffer, 0, user.receiveBuffer.Length,
                SocketFlags.None, ReceiveCallBack, user);
        }
        static void NewClient()
        {
            while(true)
            {
                listenSock.Listen(1000);
                //Console.WriteLine("Listen");                                          
                listenSock.BeginAccept(AcceptCallBack, null);
                //Console.WriteLine("Accept");                
                //byte[] tmp = Encoding.Default.GetBytes("Game에 오신 것을 환영합니다.");
                //client.Send(tmp);
                Thread.Sleep(10);
            }
        }        
    }
}
