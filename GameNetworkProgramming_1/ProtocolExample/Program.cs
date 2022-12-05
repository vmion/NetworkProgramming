using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProtocolExample
{
    public enum eHEADER
    {
        PEERINFO = 1000
    }
    public struct PeerInfo
    {
        public short Header;
        public int uid;
        public string charName;
        public byte charType;
    }
    class Program
    {
        static PeerInfo peerInfo;
        static byte[] sendBuffer;
        static void Main(string[] args)
        {
            peerInfo.Header = (short)eHEADER.PEERINFO;
            peerInfo.uid = 8627;
            peerInfo.charName = "홍길동";
            peerInfo.charType = 1;
            sendBuffer = new byte[128];
            PeerInfoPacket();
            GetPeerInfo();
        }
        public static void PeerInfoPacket()
        {
            byte[] header = BitConverter.GetBytes(peerInfo.Header);
            byte[] uid = BitConverter.GetBytes(peerInfo.uid);
            byte[] charName = Encoding.Default.GetBytes(peerInfo.charName);
            byte[] charType = BitConverter.GetBytes(peerInfo.charType);
            Array.Copy(header, 0, sendBuffer, 0, header.Length);
            Array.Copy(uid, 0, sendBuffer, 2, uid.Length);
            Array.Copy(charName, 0, sendBuffer, 6, charName.Length);
            Array.Copy(charType, 0, sendBuffer, 30, charType.Length);
        }
        public static void GetPeerInfo()
        {
            //sendbuffer에 있는 내용을 분리하시오.
            byte[] arrHeader = new byte[2];
            byte[] arrUid = new byte[4];
            byte[] arrCharName = new byte[24];
            byte[] arrCharType = new byte[1];
            Array.Copy(sendBuffer, 0, arrHeader, 0, arrHeader.Length);
            Array.Copy(sendBuffer, 2, arrUid, 0, arrUid.Length);
            Array.Copy(sendBuffer, 6, arrCharName, 0, arrCharName.Length);
            Array.Copy(sendBuffer, 30, arrCharType, 0, arrCharType.Length);
            short header = BitConverter.ToInt16(arrHeader, 0);
            int id = BitConverter.ToInt32(arrUid, 0);
            string name = Encoding.Default.GetString(arrCharName);
            byte type = arrCharType[0];
            Console.WriteLine("header = " + header);
            Console.WriteLine("uid = " + id);
            Console.WriteLine("name = " + name);
            Console.WriteLine("type = " + type);
        }
    }
}
