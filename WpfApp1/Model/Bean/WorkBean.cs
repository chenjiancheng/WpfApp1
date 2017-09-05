using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model.Bean
{
    class WorkBean
    {
        private int userId;
        private string name;
        private string card;
        private string userIp;
        private string weiIp;
        private double weight;
        private string data;
        private string unit;

        public int UserId { get => userId; set => userId = value; }
        public string Name { get => name; set => name = value; }
        public string Card { get => card; set => card = value; }
        public string UserIp { get => userIp; set => userIp = value; }
        public string WeiIp { get => weiIp; set => weiIp = value; }
        public double Weight { get => weight; set => weight = value; }
        public string Data { get => data; set => data = value; }
        public string Unit { get => unit; set => unit = value; }
    }
}
