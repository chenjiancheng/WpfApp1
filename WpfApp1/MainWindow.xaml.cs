using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Common;
using WpfApp1.Model.Bean;
using WpfApp1.Present;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, View.IViewMain
    {
        private BackgroundWorker worker;

        public MainWindow()
        {
            InitializeComponent();

            initView();

            MainControl control = new MainControl(this);
        }

        public void showTable(object o)
        {
        }

        private void initView()
        {
            new LogUtil(this);
        }

        private void Button_Click_Clear(object sender, RoutedEventArgs e)
        {
            getShowLogView().Text = "";
            LogUtil.instance.Log("-------清空日志-------");
        }

        public void showLog(string msg)
        {
            TextBox tb = getShowLogView();
            
            tb.Dispatcher.BeginInvoke((Action)delegate()
            {
                if (tb.LineCount > 100)
                {
                    tb.Clear();

                }
                tb.AppendText(msg + "\r\n");//日志
            });
            
        }

        //显示日志   

        public TextBox getShowLogView()
        {
            return txtLog;//获取显示日志的控件
        }

        /**
         窗口最小化问题
         */

    }
}
