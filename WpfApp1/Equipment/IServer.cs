using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Present
{
    interface IServer
    {
        void start();
        void send(Object ob);
    }
}
