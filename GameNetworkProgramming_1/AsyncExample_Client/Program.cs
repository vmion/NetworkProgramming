using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace AsyncExample_Client
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
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
    }
    class Program
    {
        static Socket sock;
        static int port = 8082;
        static string strip = "218.234.62.112";
        static User user;
        static void Main(string[] args)
        {            
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            user = new User(sock);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strip), port);
            sock.Connect(ip);            
            byte[] tmp = new byte[128];
            sock.Receive(tmp);
            string receiveData = Encoding.Default.GetString(tmp);
            Console.WriteLine(receiveData);
            string line = string.Empty;
            receiveData = string.Empty;
            sock.BeginReceive(user.rBuffer, 0, user.rBuffer.Length,
                                    SocketFlags.None, ReceiveCallBack, user);
            while ((line = Console.ReadLine()) != null)
            {                
                byte[] sends = Encoding.Default.GetBytes(line);
                /*
                string[] lines = new string[128];
                if(lines[0] == "#")
                {                    
                    user.sock.Close();
                }
                Array.Clear(lines, 0, lines.Length);
                */                
                if (line.StartsWith("#"))
                    break;                
                /*
                if (line.Equals("#"))
                    break;
                */
                Array.Copy(sends, user.sBuffer, sends.Length);                
                user.sock.BeginSend(user.sBuffer, 0, user.sBuffer.Length,
                                    SocketFlags.None, SendCallBack, sock);
            }
            sock.Close();
        }
        static void ReceiveCallBack(IAsyncResult ar)
        {
            User user = (User)ar.AsyncState;
            string receiveMessage = Encoding.Default.GetString(user.rBuffer);
            Console.WriteLine(receiveMessage);
            Array.Clear(user.rBuffer, 0, user.rBuffer.Length);            
            user.sock.BeginReceive(user.rBuffer, 0, user.rBuffer.Length,
                                    SocketFlags.None, ReceiveCallBack, user);
        }
        static void SendCallBack(IAsyncResult ar)
        {            
            Array.Clear(user.sBuffer, 0, user.sBuffer.Length);            
        }
    }
}
