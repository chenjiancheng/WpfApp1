using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfApp1.Common;
using WpfApp1.Model;
using WpfApp1.Model.Bean;

namespace WpfApp1.Present
{
    class SocketAlarmImp : IServer
    {
        private MainControl control;
        private static Dictionary<string, Socket> alarmClients;

        public SocketAlarmImp(MainControl control)
        {
            this.control = control;
            alarmClients = new Dictionary<string, Socket>();
        }

        public void start()
        {
            Thread serverThread = new Thread(new ThreadStart(createServer));
            //将窗体线程设置为与后台同步，随着主线程结束而结束  
            serverThread.IsBackground = true;
            serverThread.Start();
        }
        public void createServer()
        {
            LogUtil.instance.Log("-------开始创建报警器服务-------");

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            const int port = 6589;//端口号
            string host = IPUtil.GetLocalIpv4()[0];//获取本机IP
            try
            {
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);//IPEndPoint类是对ip端口做了一层封装的
                server.Bind(ipe);//向操作系统申请一个可用的ip地址和端口号用于通信
                server.Listen(100);//设置最大的连接数
                LogUtil.instance.Log("创建端口:" + port + " 等待报警器客户端连接中...");
            }
            catch (Exception e)
            {
                LogUtil.instance.Log("创建报警器Socket失败");
                throw e;
            }
            
            while (true)
            {
                createClienAcceptThread(server.Accept());//阻塞状态，等待连接,创建线程接收信息
            }
            
        }


        private void createClienAcceptThread(Socket client)
        {
            //创建一个线程      
            ParameterizedThreadStart pts = new ParameterizedThreadStart(clientAccept);
            Thread thread = new Thread(pts);
            thread.IsBackground = true;//设置为后台线程，随着主线程退出而退出     
                                       //启动线程     
            thread.Start(client);
        }
        private void clientAccept(Object socketServer)//保存客户端信息
        {
            Socket client = socketServer as Socket;
            //获取客户端的IP和端口号  
            IPAddress clientIP = (client.RemoteEndPoint as IPEndPoint).Address;
            int clientPort = (client.RemoteEndPoint as IPEndPoint).Port;
            LogUtil.instance.Log("报警器客户端连入" + clientIP + ":" + clientPort);

            string ip = clientIP.ToString();
            bool exist = alarmClients.ContainsKey(ip);
            if (exist)//存在，先移除
            {
                alarmClients.Remove(ip);
            }
            alarmClients.Add(clientIP.ToString(), client);//保存客户端到集合里
            
            while (true)
            {
                //接收报警器客户端的信息
                byte[] clientData = new byte[1024];
                try
                {
                    int revLen = client.Receive(clientData);//接收数据
                    string recMessage = Encoding.UTF8.GetString(clientData, 0, revLen);
                    LogUtil.instance.Log("报警器收到信息：" + recMessage);
                }
                catch (SocketException e)
                {
                    LogUtil.instance.Log("报警器接收信息异常(远程主机关闭连接)，已退出信息接收");
                    break;
                }
            }
        }


        public void send(object ob)
        {
            if (ob == null)
            {
                LogUtil.instance.Log("SocketAlarmImp send 的对象为空");
                return;
            }
            Thread serverThread = new Thread(new ParameterizedThreadStart(sendDataAlarm));
            //将窗体线程设置为与后台同步，随着主线程结束而结束  
            serverThread.IsBackground = true;
            serverThread.Start(ob);
        }

        private void sendDataAlarm(Object ob)
        {
            if (ob.GetType() == typeof(ConnBean))
            {
                ConnBean bean = ob as ConnBean;
                if (bean != null && bean.Ip2 != null)
                {

                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    if (bean.Ip2 == null || client == null)
                    {
                        LogUtil.instance.Log("有值为空"+(bean.Ip2 == null) +"-"+(client == null));
                        return;
                    }
                    alarmClients.TryGetValue(bean.Ip2, out client);//从Dictionary中读取报警灯Client

                    if (client != null)
                    {
                        //Encoding.ASCII.GetBytes();
                        //byte[] byteMsgOn = {  0xFE, 05, 00, 00, 0xFF, 00, 0x98, 53 };
                        //byte[] byteMsgOff = { 0xFE, 05 ,00, 00, 00 ,  00, 0xD9 ,0xC5 };
                        byte[] byteMsgOn = CommandUtil.WriteDO(Convert.ToInt16(bean.Addr),bean.Channel - 1,true);//第几个灯，下标从0开始
                        byte[] byteMsgOff = CommandUtil.WriteDO(Convert.ToInt16(bean.Addr), bean.Channel - 1, false);
                        
                        LogUtil.instance.Log("开始发送开灯指令:"+byteMsgOn.ToArray());
                        foreach (byte b in byteMsgOn)
                        {
                            Console.Write(b + " ");
                        }
                        client.Send(byteMsgOn);//254 5 0 1 255 0 201 245

                        Thread.Sleep(5000);//异常   毫秒数
                        
                        LogUtil.instance.Log("开始发送关灯指令:" + byteMsgOff.ToArray());
                        foreach (byte b in byteMsgOff)
                        {
                            Console.Write(b + " ");
                        }
                        client.Send(byteMsgOff);//254 5 0 1 0 0 136 5

                    }
                    else
                    {
                        LogUtil.instance.Log(bean.Ip+"  对应的报警IP: "+bean.Ip2 + " 未在线");
                    }
                }
                else
                {
                    LogUtil.instance.Log("电子称 "+bean.Ip+" 对应的报警器 "+bean.Ip2 + "不存在");
                }
            }
        }

    }
}
