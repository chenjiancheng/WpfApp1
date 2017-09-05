using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    class DZCBean
    {
        //ctrl+r + e   快速get/set
        private string ip;
        private double weight;
        private string data;
        private string unit;

        public string Ip { get => ip; set => ip = value; }
        public double Weight { get => weight; set => weight = value; }
        public string Data { get => data; set => data = value; }
        public string Unit { get => unit; set => unit = value; }

        public override string ToString()
        {
            return ip+" "+ weight+" "+data;
        }
    }
}
