using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;
using WpfApp1.Model.Bean;

namespace WpfApp1.Present
{
    interface IControl
    {
        void receiveData(DZCBean bean,UserBean user);
    }
}
