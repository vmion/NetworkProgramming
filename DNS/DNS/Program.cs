using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DNS
{
    class Program
    {
        static void Main(string[] args)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry("www.google.com");
            Console.WriteLine(hostEntry.HostName);
            foreach (IPAddress ip in hostEntry.AddressList)
            {
                Console.WriteLine(ip);
            }

            string hostName = Dns.GetHostName();
            Console.WriteLine(hostName);
            IPHostEntry localhost = Dns.GetHostEntry(hostName);
            foreach (IPAddress ip in localhost.AddressList)
            {
                Console.WriteLine("로컬IP =" + ip);
            }
        }
    }
}
