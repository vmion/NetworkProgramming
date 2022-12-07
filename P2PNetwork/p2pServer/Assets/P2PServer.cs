using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

///////////////////////////////
public enum ePACKET { NONE, PEERINFO = 100}
//피어 정보
public struct PeerInfo
{
    public short header;
    public int severUid;
    public int clientUid;
    public byte sPlayerNameLength;
    public string sPlayerName;
    public byte cPlayerNameLength;
    public string cPlayerName;
}
//캐릭터 변경
public struct CharChange
{
    public int uid;
    public byte charType;
    public string name;
}
//////////////////////////////
public class P2PServer : MonoBehaviour
{
    string strIp = "218.234.62.103";
    int port = 8082;
    /////////////
    Socket listenSock;
    byte[] ssBuffer; //server의 send
    byte[] srBuffer; //server의 receive
    /////////////  
    Socket clientPeer;
    byte[] csBuffer;  //client의 send
    byte[] crBuffer;  //client의 receive
    ///////////// 

    int userCount;
    
    const int MAX_USERCOUNT = 1;    
    Queue<byte[]> packetQue;
    PeerInfo peerInfo;
    void Awake()
    {
        userCount = 0;
        clientPeer = null;        
        ssBuffer = new byte[128];
        srBuffer = new byte[128];
        csBuffer = new byte[128];
        crBuffer = new byte[128];
        packetQue = new Queue<byte[]>();
        listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strIp), port);
        listenSock.Bind(ip);        
        StartCoroutine(Listen());
    }
    IEnumerator Listen()
    {
        while(userCount< MAX_USERCOUNT)
        {
            listenSock.Listen(4);
            listenSock.BeginAccept(AcceptCallBack, null);
            yield return null;
        }        
    }
    public void CreateGameObject()
    {
        GameObject sCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sCube.transform.position = new Vector3(0, 0, 0);
        sCube.name = peerInfo.sPlayerName;

        GameObject cCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cCube.transform.position = new Vector3(5, 5, 5);
        cCube.name = peerInfo.cPlayerName;
        
    }
    void Start()
    {        
        
    }        
    void AcceptCallBack(IAsyncResult ar)
    {
        userCount++;
        clientPeer = listenSock.EndAccept(ar);        
        Debug.Log(clientPeer.RemoteEndPoint + "님이 접속했습니다.");
        PeerInfoPacket();        
    }
    public void PeerInfoPacket()
    {
        PeerInfo peerInfo;
        peerInfo.header = (short)ePACKET.PEERINFO;
        peerInfo.severUid = (int)listenSock.Handle;
        peerInfo.clientUid = (int)clientPeer.Handle;
        peerInfo.sPlayerName = ((int)listenSock.Handle).ToString();
        peerInfo.cPlayerName = ((int)clientPeer.Handle).ToString();        

        byte[] header = BitConverter.GetBytes(peerInfo.header);
        byte[] serverUid = BitConverter.GetBytes(peerInfo.severUid);
        byte[] clientUid = BitConverter.GetBytes(peerInfo.clientUid);
        byte[] sPlayerName = Encoding.Default.GetBytes(peerInfo.sPlayerName);
        byte[] cPlayerName = Encoding.Default.GetBytes(peerInfo.cPlayerName);
        byte[] sPlayerNameLength = new byte[1];
        byte[] cPlayerNameLength = new byte[1];
        sPlayerNameLength[0] = (byte)sPlayerName.Length;
        cPlayerNameLength[0] = (byte)cPlayerName.Length;

        Array.Copy(header, 0, ssBuffer, 0, header.Length);
        Array.Copy(serverUid, 0, ssBuffer, 2, serverUid.Length);
        Array.Copy(clientUid, 0, ssBuffer, 6, clientUid.Length);        
        Array.Copy(sPlayerNameLength, 0, ssBuffer, 10, 1);
        Array.Copy(sPlayerName, 0, ssBuffer, 11, (byte)sPlayerName.Length);
        Array.Copy(cPlayerNameLength, 0, ssBuffer, 11 + (byte)sPlayerName.Length, 1);
        Array.Copy(cPlayerName, 0, ssBuffer, 11 + (byte)sPlayerName.Length + cPlayerNameLength[0], (byte)cPlayerName.Length);

        clientPeer.Send(ssBuffer);
        byte[] tmp = new byte[128];
        Array.Copy(ssBuffer, 0, tmp, 0, ssBuffer.Length);
        Array.Clear(ssBuffer, 0, ssBuffer.Length);
        packetQue.Enqueue(tmp);
    }
    void ReceiveCallBack(IAsyncResult ar)
    {        
    }
    void SendCallBack(IAsyncResult ar)
    {       
    }
    void Update()
    {    
        if(packetQue.Count > 0)
        {
            byte[] queueData = packetQue.Dequeue();
            byte[] headerType = new byte[2];
            Array.Copy(queueData, headerType, headerType.Length);
            short header = BitConverter.ToInt16(headerType);
            switch ((int)header)
            {
                case (int)ePACKET.PEERINFO:
                {
                        byte[] serverUid = new byte[4];
                        byte[] clientUid = new byte[4];
                        byte[] sPlayerNameLength = new byte[1];
                        byte[] sPlayerName;
                        byte[] cPlayerNameLength = new byte[1];
                        byte[] cPlayerName; ;
                        Array.Copy(queueData, 2, serverUid, 0, serverUid.Length);
                        Array.Copy(queueData, 6, clientUid, 0, clientUid.Length);
                        Array.Copy(queueData, 10, sPlayerNameLength, 0, sPlayerNameLength.Length);
                        sPlayerName = new byte[sPlayerNameLength[0]];
                        Array.Copy(queueData, 11, sPlayerName, 0, sPlayerNameLength[0]);
                        Array.Copy(queueData, 11 + sPlayerNameLength[0], cPlayerNameLength, 0, cPlayerNameLength.Length);
                        cPlayerName = new byte[cPlayerNameLength[0]];
                        Array.Copy(queueData, 11 + sPlayerNameLength[0] + cPlayerNameLength[0], cPlayerName, 0, cPlayerNameLength[0]);
                        peerInfo.severUid = BitConverter.ToInt32(serverUid);
                        peerInfo.clientUid = BitConverter.ToInt32(clientUid);
                        peerInfo.sPlayerName = Encoding.Default.GetString(sPlayerName);
                        peerInfo.cPlayerName = Encoding.Default.GetString(cPlayerName);
                        Debug.Log("패킷타입이 PeerInfo");
                        Debug.Log("header = " + header);
                        Debug.Log("serverUid = " + peerInfo.severUid);
                        Debug.Log("clientUid = " + peerInfo.clientUid);
                        Debug.Log("sPlayerName = " + peerInfo.sPlayerName);
                        Debug.Log("cPlayerName = " + peerInfo.cPlayerName);
                        CreateGameObject();
                    }
                break;
            }
        }
    }
}
