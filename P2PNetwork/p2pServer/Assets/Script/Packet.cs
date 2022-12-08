using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public enum ePACKETMARKER { INITIALIZE = 0 };
public class Packet : MonoBehaviour
{

    byte[] sBuffer = new byte[128];
    public int CURINDEX { get; set; }
    //    public int getIndex { get; set; }
    public int READCOUNT { get; set; }

    public byte[] ADDPACKET
    {
        get { return sBuffer; }
        set
        {
            byte[] _value = value;
            for (int i = 0; i < _value.Length; i++)
                sBuffer[CURINDEX++] = _value[i];
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
                result[j++] = sBuffer[i];
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
                result[j++] = sBuffer[i];
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
                results[j++] = sBuffer[i];
            }
            CURINDEX = CURINDEX + READCOUNT;
            return results;
        }
    }


    void Start()
    {
        // 패킷 생성시 /////////////////////////
        CURINDEX = (int)ePACKETMARKER.INITIALIZE;
        READCOUNT = (int)ePACKETMARKER.INITIALIZE;
        ADDPACKET = BitConverter.GetBytes(2134);
        ADDPACKET = BitConverter.GetBytes((short)355);
        byte[] strByteArray = Encoding.Default.GetBytes("안녕하세요");
        ADDPACKET = BitConverter.GetBytes((short)strByteArray.Length);
        ADDPACKET = strByteArray;
        
        // 패킷 파싱 /////////////////////////
        CURINDEX = (int)ePACKETMARKER.INITIALIZE;
        READCOUNT = (int)ePACKETMARKER.INITIALIZE;
        int iV = BitConverter.ToInt32(GETINT);
        short sV = BitConverter.ToInt16(GETSHORT);
        READCOUNT = BitConverter.ToInt16(GETSHORT);
        string str = Encoding.Default.GetString(GETBYTES);
        
        Debug.Log(str);
    }


    void Update()
    {
        
    }
}
