using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfApp1.Model.Bean;

namespace WpfApp1.View
{
    interface IViewMain
    {
        void showTable(Object o);

        void showLog(string msg);//日志
        TextBox getShowLogView();//获取显示日志的控件
    }
}
