using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.View;

namespace WpfApp1.Common
{
    class LogUtil
    {
        private static IViewMain view;
        public static LogUtil instance;
        public LogUtil(IViewMain view)
        {
            LogUtil.view = view;
            instance = this;
        }
        public void Log(string msg)
        {
            view.showLog(TimeUtil.GetCurrentTime()+"\r\n"+msg);
            Console.WriteLine(msg);
        }
    }
}
