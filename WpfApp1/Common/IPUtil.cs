using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Common
{
    class IPUtil
    {
        void ShowIP()
        {
            //ipv4地址也可能不止一个  
            foreach (string ip in GetLocalIpv4())
            {
                Console.WriteLine("本机IP地址："+ip.ToString());
            }
            return;
        }
        //获取本机ipv4地址
        public static string[] GetLocalIpv4()
        {
            //事先不知道ip的个数，数组长度未知，因此用StringCollection储存  
            IPAddress[] localIPs;
            localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            StringCollection IpCollection = new StringCollection();
            foreach (IPAddress ip in localIPs)
            {
                //根据AddressFamily判断是否为ipv4,如果是InterNetWorkV6则为ipv6  
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    IpCollection.Add(ip.ToString());
            }
            string[] IpArray = new string[IpCollection.Count];
            IpCollection.CopyTo(IpArray, 0);
            return IpArray;
        }
    }
}
