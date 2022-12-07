using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

///////////////////////////////
public enum ePACKET { NONE, PEERINFO = 100 }
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
public class P2PClient : MonoBehaviour
{    
    Socket clientPeer;
    string strIp = "218.234.62.103";
    int port = 8082;
    byte[] sBuffer;  
    byte[] rBuffer;
    Queue<byte[]> packetQue;
    PeerInfo peerInfo;
    void Awake()
    {        
        sBuffer = new byte[128];
        rBuffer = new byte[128];
        packetQue = new Queue<byte[]>();
        clientPeer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strIp), port);
        clientPeer.Connect(ip);
        Debug.Log("Connet");
        clientPeer.Receive(rBuffer);
        byte[] tmp = new byte[128];
        Array.Copy(rBuffer, tmp, rBuffer.Length);
        Array.Clear(rBuffer, 0, rBuffer.Length);
        packetQue.Enqueue(tmp);
    }
    void Start()
    {       
        
    }
    void ReceiveCallBack(IAsyncResult ar)
    {
        
    }
    void SendCallBack(IAsyncResult ar)
    {
        
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
    void Update()
    {        
        if (packetQue.Count > 0)
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
