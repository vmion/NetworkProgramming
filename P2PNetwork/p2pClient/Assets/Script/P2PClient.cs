using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

///////////////////////////////
public enum ePACKET { NONE, PEERINFO = 100, CHARINFO = 101 }
public enum eCHARTYPE { NONE = 0, TROLL, GOLEM }
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
public struct CharInfo
{
    public short header;
    public int Uid;
    public byte charType;
    public byte nameLength;
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
    CharInfo serverChar;
    CharInfo clientChar;
    GameObject sGameObject;
    GameObject cGameObject;
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
        clientPeer.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, clientPeer);
    }
    void Start()
    {       
        
    }
    void ReceiveCallBack(IAsyncResult ar)
    {
        clientPeer = (Socket)ar.AsyncState;
        byte[] tmp = new byte[128];
        Array.Copy(rBuffer, 0, tmp, 0, rBuffer.Length);
        Array.Clear(rBuffer, 0, rBuffer.Length);
        packetQue.Enqueue(tmp);
    }
    void SendCallBack(IAsyncResult ar)
    {
        clientPeer = (Socket)ar.AsyncState;
        byte[] tmp = new byte[128];
        Array.Copy(sBuffer, 0, tmp, 0, sBuffer.Length);
        Array.Clear(sBuffer, 0, sBuffer.Length);
        packetQue.Enqueue(tmp);
    }
    public void CreateGameObject()
    {
        sGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sGameObject.transform.position = new Vector3(0, 0, 0);
        sGameObject.name = peerInfo.sPlayerName;

        cGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cGameObject.transform.position = new Vector3(5, 5, 5);
        cGameObject.name = peerInfo.cPlayerName;
    }    
    public void ChangeServerCharacter()
    {
        serverChar.header = (short)ePACKET.CHARINFO;
        serverChar.Uid = peerInfo.severUid;
        serverChar.charType = (byte)eCHARTYPE.TROLL;
        serverChar.name = "Troll";
        //패킷 생성
        byte[] header = BitConverter.GetBytes(serverChar.header);
        byte[] serverUid = BitConverter.GetBytes(serverChar.Uid);
        byte[] sCharType = new byte[1];
        sCharType[0] = serverChar.charType;
        byte[] sCharNameLength = new byte[1];
        sCharNameLength[0] = (byte)serverChar.name.Length;
        byte[] sCharName = Encoding.Default.GetBytes(serverChar.name);

        Array.Copy(header, 0, sBuffer, 0, header.Length);
        Array.Copy(serverUid, 0, sBuffer, 2, serverUid.Length);
        Array.Copy(sCharType, 0, sBuffer, 6, 1);
        Array.Copy(sCharNameLength, 0, sBuffer, 7, 1);
        Array.Copy(sCharName, 0, sBuffer, 8, sCharNameLength[0]);

        clientPeer.BeginSend(sBuffer, 0, sBuffer.Length, SocketFlags.None, SendCallBack, clientPeer);
    }
    public void ChangeClientCharacter()
    {
        clientChar.header = (short)ePACKET.CHARINFO;
        clientChar.Uid = peerInfo.clientUid;
        clientChar.charType = (byte)eCHARTYPE.GOLEM;
        clientChar.name = "Golem";
        //패킷 생성
        byte[] header = BitConverter.GetBytes(clientChar.header);
        byte[] clientUid = BitConverter.GetBytes(clientChar.Uid);
        byte[] cCharType = new byte[1];
        cCharType[0] = clientChar.charType;
        byte[] cCharNameLength = new byte[1];
        cCharNameLength[0] = (byte)clientChar.name.Length;
        byte[] cCharName = Encoding.Default.GetBytes(clientChar.name);

        Array.Copy(header, 0, sBuffer, 0, header.Length);
        Array.Copy(clientUid, 0, sBuffer, 2, clientUid.Length);
        Array.Copy(cCharType, 0, sBuffer, 6, 1);
        Array.Copy(cCharNameLength, 0, sBuffer, 7, 1);
        Array.Copy(cCharName, 0, sBuffer, 8, cCharNameLength[0]);

        clientPeer.BeginSend(sBuffer, 0, sBuffer.Length, SocketFlags.None, SendCallBack, clientPeer);
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
                    byte[] cPlayerName;
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
                    break;
                }
                case (int)ePACKET.CHARINFO:
                    {
                        Debug.Log("패킷타입이 charInfo");
                        Debug.Log("header = " + header);
                        byte[] Uid = new byte[4];
                        byte[] CharType = new byte[1];
                        byte[] CharNameLength = new byte[1];
                        byte[] CharName;
                        Array.Copy(queueData, 2, Uid, 0, Uid.Length);
                        Array.Copy(queueData, 6, CharType, 0, CharType.Length);
                        Array.Copy(queueData, 7, CharNameLength, 0, CharNameLength.Length);
                        CharName = new byte[CharNameLength[0]];
                        Array.Copy(queueData, 8, CharName, 0, CharNameLength[0]);
                        int id = BitConverter.ToInt32(Uid, 0);
                        string rcName = string.Empty;
                        switch ((int)CharType[0])
                        {
                            case (int)eCHARTYPE.TROLL:
                                rcName = "TrollGiant";
                                break;
                            case (int)eCHARTYPE.GOLEM:
                                rcName = "Golem";
                                break;
                        }
                        GameObject rcGameObject = Resources.Load<GameObject>(rcName);
                        if (id == peerInfo.severUid)
                        {
                            Debug.Log("Uid = " + peerInfo.clientUid);
                            DestroyImmediate(sGameObject);
                            sGameObject = Instantiate<GameObject>(rcGameObject);
                            sGameObject.transform.position = new Vector3(0, 0, 0);
                            sGameObject.name = Encoding.Default.GetString(CharName);
                        }
                        else
                        {                            
                            Debug.Log("Uid = " + clientChar.Uid);
                            DestroyImmediate(cGameObject);
                            cGameObject = Instantiate<GameObject>(rcGameObject);
                            cGameObject.transform.position = new Vector3(5, 5, 5);
                            cGameObject.name = Encoding.Default.GetString(CharName);
                        }
                        break;
                    }

            }
        }
    }
}
