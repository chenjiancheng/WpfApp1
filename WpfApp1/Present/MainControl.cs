using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Common;
using WpfApp1.Model;
using WpfApp1.Model.Bean;
using WpfApp1.View;

namespace WpfApp1.Present
{
    class MainControl:IControl
    {
        private IServer server;
        private IServer alarmServer;
        private IViewMain view;
        private ModelMainImpl model;
        private static WorkBean workBean = new WorkBean();

        public MainControl(IViewMain view)
        {
            this.view = view;

            initServer();

            initMode();
            

        }

        private void initServer()
        {
            server = new SocketImp(this);//电子秤服务器，等待电子秤连接和接收数据
            server.start();

            alarmServer = new SocketAlarmImp(this);//报警器
            alarmServer.start();
        }

        private void initMode()
        {
            model = new ModelMainImpl();//数据库，保存数据
        }

        public void receiveData(DZCBean bean,UserBean user)
        {
            Console.WriteLine("我这这里页面接收到了数据: "+bean.Ip);
            //把数据给前台

            if (bean != null)//电子秤数据
            {

                ResultBean result = model.insertWeightDataToWorkBean(bean);//结合数据,保存数据，判断低重

                LogUtil.instance.Log(result.Msg);

                if (result.Success && result.IsLow)
                {
                    LogUtil.instance.Log(result.Msg2);
                    alarmServer.send(result.Bean);
                }
                
            }else if (user != null)//用户刷卡的
            {

            }
            view.showTable(null);
        }
    }
}
