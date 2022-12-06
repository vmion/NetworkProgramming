using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class P2PClient : MonoBehaviour
{    
    Socket clientPeer;
    string strIp = "218.234.62.123";
    int port = 8082;        
    void Awake()
    {
        clientPeer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(strIp), port);
        clientPeer.Connect(ip);
        Debug.Log("Connet");
        GameObject NewCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        NewCube.transform.position = new Vector3(0, 0, 0);
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
    void Update()
    {        
    }
}
