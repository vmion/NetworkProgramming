using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Network_Test
{
    class Program
    {
        static byte[] strIP = Encoding.Default.GetBytes("192.168.0.1");
        static int port = 8082;
        static void Main(string[] args)
        {
            IPAddress ip = new IPAddress(strIP);
            long ip_ = strIP[0] + (strIP[1] << 8) + (strIP[2] << 16) + (strIP[3] << 24);
            IPAddress ip2 = new IPAddress(ip_);
            Console.WriteLine(ip);
        }
    }
}
