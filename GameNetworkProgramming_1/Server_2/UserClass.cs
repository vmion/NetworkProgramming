using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

class UserClass
{
    public Socket userSock;
    public byte[] receiveBuffer;
    public byte[] sendBuffer;
    public UserClass()
    {

    }
    public UserClass(Socket _sock)
    {
        userSock = _sock;
        sendBuffer = new byte[128];
        receiveBuffer = new byte[128];
    }
    public void ClearSendBuffer()
    {
        Array.Clear(sendBuffer, 0, sendBuffer.Length);
    }
    public void ClearReceiveBuffer()
    {
        Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
    }
}

