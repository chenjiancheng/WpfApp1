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

namespace WpfApp1.Present
{
    class SocketImp : IServer
    {
        private MainControl control;
        private static Dictionary<string, Socket> weighterClients;
        public SocketImp()
        {
        }

        public SocketImp(MainControl control)
        {
            this.control = control;
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
            LogUtil.instance.Log("-------开启创建电子秤服务-------");
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            const int port = 6547;//端口号
            string host = IPUtil.GetLocalIpv4()[0];//获取本机IP
            try
            {
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);//IPEndPoint类是对ip端口做了一层封装的
                server.Bind(ipe);//向操作系统申请一个可用的ip地址和端口号用于通信
                server.Listen(100);//设置最大的连接数
                LogUtil.instance.Log("创建端口:" + port + " 等待电子秤客户端连接中...");
            }
            catch (Exception e)
            {
                LogUtil.instance.Log("-------创建电子秤Socket失败-------");
                throw e;
            }

            while (true)
            {
                createClienAcceptThread(server.Accept());//阻塞状态，等待连接,创建线程接收信息
            }

        }


        private void createClienAcceptThread(Socket client)
        {
            //创建一个通信线程      
            ParameterizedThreadStart pts = new ParameterizedThreadStart(clientAccept);
            Thread thread = new Thread(pts);
            thread.IsBackground = true;//设置为后台线程，随着主线程退出而退出     
                                       //启动线程     
            thread.Start(client);
        }
        private void clientAccept(Object socketServer)
        {
            Socket client = socketServer as Socket;
            while (true)
            {
                //获取客户端的IP和端口号  
                IPAddress clientIP = (client.RemoteEndPoint as IPEndPoint).Address;
                int clientPort = (client.RemoteEndPoint as IPEndPoint).Port;
                LogUtil.instance.Log("电子秤客户端连入  " + clientIP + " : " + clientPort);//CJC应提示或列出报警器是否可用正常

                byte[] clientData = new byte[1024];
                //try
                //{
                    int revLen = client.Receive(clientData);
                //对数据进行判断
                if (revLen == 8)//先8字节判断  && clientData[revLen-1] == 13
                {
                    byte[] data = new byte[revLen];
                    Array.Copy(clientData, data, revLen);//转存另一个数组
                    //在这里把数据整理
                    DZCBean bean = new DZCBean();
                    bean.Ip = clientIP.ToString();
                    bean.Weight = DataUtil.byteWeightDataToDouble(data);//重量数据
                    bean.Data = DataUtil.byteWeightDataToStringSplit(data);//byte[]转string，用逗号分隔

                    LogUtil.instance.Log("电子秤 " + clientIP + " 当前重量: " + bean.Weight);

                    control.receiveData(bean,null);//回传数据
                }
                else
                {
                    LogUtil.instance.Log("数据不是电子秤重量数据，不发送");
                }
                    
                string recMessage = Encoding.ASCII.GetString(clientData, 0, revLen);
                for(int i = 0; i < revLen; i++)
                {
                    Console.Write(clientData[i]+" ");
                }
                    Console.WriteLine("电子秤收到信息：" + recMessage);
                //}
                //catch (SocketException e)
                //{
               //     LogUtil.Log("电子秤远程主机强迫关闭了一个现有的连接");
               // }
                


            }
        }

        private void sendData()
        {

        }

        public void send(object ob)
        {

        }
    }
}
