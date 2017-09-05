using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model.Bean
{
    class UserBean
    {
        private int id;
        private int num;
        private string name;
        private string ip;
        private string card;

        public int Num { get => num; set => num = value; }
        public string Name { get => name; set => name = value; }
        public string Ip { get => ip; set => ip = value; }
        public string Card { get => card; set => card = value; }
        public int Id { get => id; set => id = value; }
    }
}
