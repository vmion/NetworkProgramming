using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

public enum ePACKETMARKER { INITIALIZE = 0 };
public class User : MonoBehaviour
{
    public Socket USERSOCK { get; set; }
    public byte[] SBUFFER { get; set; }
    public byte[] RBUFFER { get; set; }
    public byte[] WORKBUFFER { get; set; }
    public int UID { get; set; }
    public string NAME { get; set; }
    public byte CHARTYPE { get; set; }

    public GameObject selectedGameObject;
    public int CURINDEX { get; set; }
    public int READCOUNT { get; set; }

    public byte[] ADDPACKET
    {
        get { return WORKBUFFER; }
        set
        {
            byte[] _value = value;
            for (int i = 0; i < _value.Length; i++)
                WORKBUFFER[CURINDEX++] = _value[i];
        }
    }
    public byte[] GETINT
    {
        get
        {
            byte[] result = new byte[4];
            int j = 0;
            for (int i = CURINDEX; i < CURINDEX + 4; i++)
            {
                result[j++] = WORKBUFFER[i];
            }
            CURINDEX = CURINDEX + 4;
            return result;
        }
    }
    public byte[] GETSHORT
    {
        get
        {
            byte[] result = new byte[2];
            int j = 0;
            for (int i = CURINDEX; i < CURINDEX + 2; i++)
            {
                result[j++] = WORKBUFFER[i];
            }
            CURINDEX = CURINDEX + 2;
            return result;
        }
    }
    public byte[] GETBYTES
    {
        get
        {
            byte[] results = new byte[READCOUNT];
            int j = 0;
            for (int i = CURINDEX; i < CURINDEX + READCOUNT; i++)
            {
                results[j++] = WORKBUFFER[i];
            }
            CURINDEX = CURINDEX + READCOUNT;
            return results;
        }
    }

    void Awake()
    {
        SBUFFER = new byte[128];
        RBUFFER = new byte[128];
        UID = -1;
        NAME = "";
        CHARTYPE = 0;
    }
    public void Close()
    {
        USERSOCK.Close();
    }
}