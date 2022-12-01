﻿using System;
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
        public int handle;
        public byte[] rBuffer;
        public byte[] sBuffer;        
        public User(Socket _sock)
        {
            sock = _sock;
            handle = (int)_sock.Handle;
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
        static List<User> deleteList;
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
        static void RemoveUser()
        {
            if(deleteList.Count > 0)
            {
                for(int i = 0; i < deleteList.Count; i++)
                {
                    userList.Remove(deleteList[i]);
                    deleteList[i].Close();
                }
                deleteList.Clear();
            }
        }
        static void NewClient()
        {
            //사용자 접속
            while (true)
            {                
                listenSock.Listen(1000);                                                        
                listenSock.BeginAccept(AcceptCallBack, null);                
                Thread.Sleep(10);
                if(deleteList != null)
                    RemoveUser();
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
                Console.WriteLine(user.handle + "유저가 접속하였습니다.");
                userSock.BeginReceive(user.rBuffer, 0, user.rBuffer.Length,
                    SocketFlags.None, ReceiveCallBack, user);
            }
            catch(Exception e)
            { 
                //if(user != null)                
                deleteList.Add(user);                     
            }
        }
        static void ReceiveCallBack(IAsyncResult ar)
        {           
            User user = (User)ar.AsyncState;
            try
            {
                if (user.rBuffer[0] == 35) //#을 전송했을 경우
                    throw new Exception(user.handle + "님이 접속종료했습니다.");
                
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
                Console.WriteLine(user.handle + "유저가 접속종료하였습니다.");
                //deleteList.Add(user);                                                                                                
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
                //if(user != null)
                //deleteList.Add(user);                
            }
        }
    }
}