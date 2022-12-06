using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public delegate void Do();
public class P2PServer : MonoBehaviour
{
    string strIp = "218.234.62.123";
    int port = 8082;
    Socket listenSock;
    int userCount;
    Socket clientPeer;
    const int MAX_USERCOUNT = 1;
    Do doSomething;
    void Awake()
    {
        userCount = 0;
        clientPeer = null;
        doSomething = null;
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
        GameObject NewCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        NewCube.transform.position = new Vector3(0, 0, 0);
        doSomething -= CreateGameObject;
    }
    void Start()
    {        
        
    }        
    void AcceptCallBack(IAsyncResult ar)
    {
        userCount++;
        clientPeer = listenSock.EndAccept(ar);        
        Debug.Log(clientPeer.RemoteEndPoint + "님이 접속했습니다.");
        doSomething = CreateGameObject;
    }
    void ReceiveCallBack(IAsyncResult ar)
    {        
    }
    void SendCallBack(IAsyncResult ar)
    {       
    }
    void Update()
    {        
        if(doSomething != null)
        {
            doSomething();            
        }
    }
}
