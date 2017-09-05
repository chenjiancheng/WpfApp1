using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model.Bean
{
    class ResultBean
    {
        private string time;
        private int code;
        private string msg;
        private string msg2;
        private bool success;
        private bool isLow;
        private string value1;
        private string value2;
        private string value3;
        private Object bean;

        public string Time { get => time; set => time = value; }
        public int Code { get => code; set => code = value; }
        public string Msg { get => msg; set => msg = value; }
        public bool Success { get => success; set => success = value; }
        public bool IsLow { get => isLow; set => isLow = value; }
        public string Value1 { get => value1; set => value1 = value; }
        public string Value2 { get => value2; set => value2 = value; }
        public string Value3 { get => value3; set => value3 = value; }
        public string Msg2 { get => msg2; set => msg2 = value; }
        public object Bean { get => bean; set => bean = value; }
    }
}
