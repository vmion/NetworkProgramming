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
    public short sPlayerNameLength;
    public string sPlayerName;
    public short cPlayerNameLength;
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
    string strIp = "218.234.62.128";
    int port = 10002;
    Queue<byte[]> packetQue;    
    //CharInfo serverChar;
    //CharInfo clientChar;    
    public User serUser;
    public User cliUser;
    void Awake()
    {      
    }
    IEnumerator Start()
    {
        CreateGameObject();
        packetQue = new Queue<byte[]>();
        cliUser.USERSOCK = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strIp), port);
        cliUser.USERSOCK.Connect(ip);        
        Debug.Log("Connet");
        cliUser.USERSOCK.Receive(cliUser.RBUFFER);
        byte[] tmp = new byte[128];
        Array.Copy(cliUser.RBUFFER, tmp, cliUser.RBUFFER.Length);
        Array.Clear(cliUser.RBUFFER, 0, cliUser.RBUFFER.Length);
        packetQue.Enqueue(tmp);
        cliUser.USERSOCK.BeginReceive(cliUser.RBUFFER, 0, cliUser.RBUFFER.Length, SocketFlags.None, ReceiveCallBack, cliUser);
        yield return null;        
    }
    void ReceiveCallBack(IAsyncResult ar)
    {
        User user = (User)ar.AsyncState;
        byte[] tmp = new byte[128];
        Array.Copy(user.RBUFFER, 0, tmp, 0, user.RBUFFER.Length);
        Array.Clear(user.RBUFFER, 0, user.RBUFFER.Length);
        packetQue.Enqueue(tmp);
        cliUser.USERSOCK.BeginReceive(cliUser.RBUFFER, 0, cliUser.RBUFFER.Length, SocketFlags.None, ReceiveCallBack, cliUser);
    }
    void SendCallBack(IAsyncResult ar)
    {
        User user = (User)ar.AsyncState;
        byte[] tmp = new byte[128];
        Array.Copy(user.SBUFFER, 0, tmp, 0, user.SBUFFER.Length);
        Array.Clear(user.SBUFFER, 0, user.SBUFFER.Length);
        packetQue.Enqueue(tmp);
    }
    public short GetHeader(byte[] _buffer)
    {
        byte[] _tmp = new byte[2];
        for (int i = 0; i < 2; i++)
        {
            _tmp[i] = _buffer[i];
        }
        return BitConverter.ToInt16(_tmp);
    }
    public void CreateGameObject()
    {
        //serUser.selectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //serUser.selectedGameObject.transform.position = new Vector3(0, 0, 0);
        serUser.selectedGameObject.name = serUser.NAME;

        //cliUser.selectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cliUser.selectedGameObject.transform.position = new Vector3(5, 5, 5);
        cliUser.selectedGameObject.name = cliUser.NAME;

        serUser.selectedGameObject.GetComponent<MeshRenderer>().enabled = false;
        cliUser.selectedGameObject.GetComponent<MeshRenderer>().enabled = false;
    }
    /*
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
    */    
    void Update()
    {        
        if (packetQue.Count > 0)
        {            
            byte[] queueData = packetQue.Dequeue();
            short header = GetHeader(queueData);
            switch ((int)header)
            {
                case (int)ePACKET.PEERINFO:
                {
                    cliUser.CURINDEX = (int)ePACKETMARKER.INITIALIZE + 2;
                    cliUser.READCOUNT = (int)ePACKETMARKER.INITIALIZE;
                    cliUser.WORKBUFFER = queueData;
                    PeerInfo peerInfo;
                    peerInfo.severUid = BitConverter.ToInt32(cliUser.GETINT, 0);
                    peerInfo.clientUid = BitConverter.ToInt32(cliUser.GETINT, 0);

                    cliUser.READCOUNT = BitConverter.ToInt16(cliUser.GETSHORT, 0);
                    peerInfo.sPlayerNameLength = (short)cliUser.READCOUNT;
                    peerInfo.sPlayerName = Encoding.Default.GetString(cliUser.GETBYTES);

                    cliUser.READCOUNT = BitConverter.ToInt16(cliUser.GETSHORT, 0);
                    peerInfo.cPlayerNameLength = (short)cliUser.READCOUNT;
                    peerInfo.cPlayerName = Encoding.Default.GetString(cliUser.GETBYTES);

                    serUser.selectedGameObject.name = peerInfo.sPlayerName;
                    cliUser.selectedGameObject.name = peerInfo.cPlayerName;
                    serUser.selectedGameObject.GetComponent<MeshRenderer>().enabled = true;
                    cliUser.selectedGameObject.GetComponent<MeshRenderer>().enabled = true;

                    Debug.Log("패킷타입이 PeerInfo");
                    Debug.Log("header = " + header);
                    Debug.Log("serverUid = " + peerInfo.severUid);
                    Debug.Log("clientUid = " + peerInfo.clientUid);
                    Debug.Log("sPlayerName = " + peerInfo.sPlayerName);
                    Debug.Log("cPlayerName = " + peerInfo.cPlayerName);                                       
                }
                break;
                    /*
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
                    */
            }
        }
    }
}
