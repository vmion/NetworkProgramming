using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace AsyncExample
{
    public class User
    {
        public Socket sock;
        public byte[] rBuffer;
        public byte[] sBuffer;        
        public User(Socket _sock)
        {
            sock = _sock;
            sBuffer = new byte[128];
            rBuffer = new byte[128];
        }
        public void Close()
        {
            //sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
    }
    class Program
    {
        static string strIp = "218.234.62.112";
        static int port = 8082;
        static Socket listenSock;        
        static Thread t1;
        static List<User> userList;
        static void Main(string[] args)
        {
            listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Parse(strIp), port);
            userList = new List<User>();
            listenSock.Bind(ipEndpoint);
            Console.WriteLine("Bind");

            ThreadStart threadStart = new ThreadStart(NewClient);
            t1 = new Thread(threadStart);
            t1.Start();

        }
        static void NewClient()
        {
            //사용자 접속
            while (true)
            {
                listenSock.Listen(1000);                                                        
                listenSock.BeginAccept(AcceptCallBack, null);                
                Thread.Sleep(10);
            }
        }
        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket userSock = listenSock.EndAccept(ar);
            User user = null;
            try 
            {
                byte[] tmp = Encoding.Default.GetBytes("Game에 오신 것을 환영합니다.");
                userSock.Send(tmp);
                user = new User(userSock);
                userList.Add(user);
                Console.WriteLine(user.sock.RemoteEndPoint + "유저가 접속하였습니다.");
                userSock.BeginReceive(user.rBuffer, 0, user.rBuffer.Length,
                    SocketFlags.None, ReceiveCallBack, user);
            }
            catch(Exception e)
            {
                if(user != null)
                    userList.Remove(user);
                userSock.Close();
            }
        }
        static void ReceiveCallBack(IAsyncResult ar)
        {
            //userSock이 보낸 데이터를 다른 사용자에게 전송
            User user = (User)ar.AsyncState;
            try
            {
                //자신의 메시지를 다른사용자에게 보내기 위하여 rBuffer의 데이터를 sBuffer에 전송            
                for (int i = 0; i < userList.Count; i++)
                {
                    Array.Copy(user.rBuffer, userList[i].sBuffer, user.rBuffer.Length);
                    userList[i].sock.BeginSend(userList[i].sBuffer, 0, userList[i].sBuffer.Length,
                                            SocketFlags.None, SendCallBack, userList[i]);
                }
                Array.Clear(user.rBuffer, 0, user.rBuffer.Length);
            }
            catch(Exception e)
            {                
                Console.WriteLine(user.sock.Handle + "유저가 접속종료하였습니다.");
                userList.Remove(user);
                user.sock.Close();                
            }
        }
        static void SendCallBack(IAsyncResult ar)
        {
            User user = (User)ar.AsyncState;
            try
            {                
                Array.Clear(user.sBuffer, 0, user.sBuffer.Length);
                user.sock.BeginReceive(user.rBuffer, 0, user.rBuffer.Length,
                                        SocketFlags.None, ReceiveCallBack, user);
            }
            catch(Exception e)
            {
                userList.Remove(user);
                user.sock.Close();
            }
        }
    }
}
