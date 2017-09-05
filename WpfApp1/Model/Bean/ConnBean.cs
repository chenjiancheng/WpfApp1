using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model.Bean
{
    class ConnBean
    {
        /**
         电子秤与报警灯的关联信息
             */
        private int id;
        private string ip;//电子秤ip
        private string alias;
        private double weight;
        private int durtion;
        private string ip2;//报警器ip
        private int port;
        private int addr;
        private int channel;
        private string open;
        private string close;
        private string status;
        private string type;
        private string create_time;
        private string ip3;//读卡器ip
        private string port3;

        public int Id { get => id; set => id = value; }
        public string Ip { get => ip; set => ip = value; }
        public string Alias { get => alias; set => alias = value; }
        public double Weight { get => weight; set => weight = value; }
        public int Durtion { get => durtion; set => durtion = value; }
        public int Port { get => port; set => port = value; }
        public int Channel { get => channel; set => channel = value; }
        public string Open { get => open; set => open = value; }
        public string Close { get => close; set => close = value; }
        public string Status { get => status; set => status = value; }
        public string Type { get => type; set => type = value; }
        public string Create_time { get => create_time; set => create_time = value; }
        public string Ip2 { get => ip2; set => ip2 = value; }
        public int Addr { get => addr; set => addr = value; }
        public string Ip3 { get => ip3; set => ip3 = value; }
        public string Port3 { get => port3; set => port3 = value; }

        public override string ToString()
        {
            return ip+"-"+id+"-"+ip2;
        }
    }
}
