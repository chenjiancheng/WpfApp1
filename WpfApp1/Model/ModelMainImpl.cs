using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Common;
using WpfApp1.Model.Bean;

namespace WpfApp1.Model
{
    class ModelMainImpl:IModel
    {
        private MySqlConnection conn;
        private MySqlCommand cmd;

        public static Dictionary<string, ConnBean> connMap = new Dictionary<string, ConnBean>();//电子秤 、读卡器、 报警器关联表，以电子秤IP为key

        private static Dictionary<string, WorkBean> workMap = new Dictionary<string, WorkBean>();//以读卡器IP为key，

        public ModelMainImpl()
        {
            /*
             * 1、概述

                ado.net提供了丰富的数据库操作，这些操作可以分为三个步骤：

                第一，使用SqlConnection对象连接数据库；
                第二，建立SqlCommand对象，负责SQL语句的执行和存储过程的调用；
                第三，对SQL或存储过程执行后返回的“结果”进行操作。
                对返回“结果”的操作可以分为两类：

                一是用SqlDataReader直接一行一行的读取数据集；
                二是DataSet联合SqlDataAdapter来操作数据库。
                两者比较：

                SqlDataReader时刻与远程数据库服务器保持连接，将远程的数据通过“流”的形式单向传输给客户端，它是“只读”的。由于是直接访问数据库，所以效率较高，但使用起来不方便。
                DataSet一次性从数据源获取数据到本地，并在本地建立一个微型数据库（包含表、行、列、规则、表之间的关系等），期间可以断开与服务器的连接，使用SqlDataAdapter对象操作“本地微型数据库”，
                结束后通过SqlDataAdapter一次性更新到远程数据库服务器。这种方式使用起来更方，便简单。但性能较第一种稍微差一点。（在一般的情况下两者的性能可以忽略不计。）


                 http://www.cnblogs.com/youuuu/archive/2011/06/16/2082730.html
             *
             */

            //创建连接
            try
            {
                string mysqlcon = "database=auto_alarm_schema;Password=6514;User ID=root;server=192.168.3.16";

                conn = new MySqlConnection(mysqlcon);
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    LogUtil.instance.Log("-------数据库连接成功-------");
                }

                cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;//使其只执行SQL语句文本形式
            }
            catch (Exception e)
            {
                LogUtil.instance.Log("-------数据库连接异常-------");
                //CJC需要处理部分  1、ip有误
                throw e;
            }

            readWeigher();//电子秤报警数据

        }

        //保存用户称药数据信息
        public Boolean saveWork(WorkBean b)
        {
            if (b == null) return false;
            //CJC需要处理部分
            LogUtil.instance.Log("-------开始保存数据-------");//"insert into work(weighter_ip) values('127.168.3.78')"
            string sql = "insert into work(user_id,user_name,user_card,user_ip,weighter_ip,weight,data,unit) values("+b.UserId+",'"+b.Name+"','"+b.Card+ "','"+b.UserIp+ "','"+b.WeiIp+ "','"+b.Weight+ "','"+b.Data+ "','"+b.Unit+ "')";

            LogUtil.instance.Log(sql);//日志输出

            cmd.CommandText = sql;
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            Console.WriteLine("数据库保存结果:"+count);
            return true;
        }


        //查询电子秤报警数据并转为实体
        private void readWeigher()
        {
            if (cmd != null)
            {
                
                cmd.CommandText = "select * from conn";

                DataSet ds = new DataSet();
                string sql = "select * from conn";

                Console.WriteLine("查询conn表" + sql);

                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                da.Fill(ds, "table1");

                initWeigherMap(ds.Tables[0]);

                //PrintRows(ds);
            }
        }

        //转为实体
        private void initWeigherMap(DataTable table)
        {
            DataColumn column = table.Columns["ip"];
            foreach (DataRow row in table.Rows)
            {
                connMap.Add(row[column].ToString(), MapUtil.DataRowToModel<ConnBean>(row));
            }

            foreach (string key in connMap.Keys)
            {
                Console.WriteLine("实体在Map中的值:"+ connMap[key]);
            }
        }

        
        public ResultBean insertWeightDataToWorkBean(DZCBean bean)//电子秤读到重量
        {
            ResultBean result = new ResultBean();
            bool success = false;
            string msg = "";

            ConnBean conn = new ConnBean();
            bool connExist = connMap.TryGetValue(bean.Ip, out conn);//通过关联表中找到读卡器IP


            if (conn != null)
            {
                if (conn.Ip3 != null)//读卡器IP
                {
                    WorkBean work = tryGetWorkBeanValue(conn.Ip3);//通过读卡器IP得到已经保存的刷卡信息
                    if (work != null)
                    {
                        work = insertWeightToMap(work, bean);

                        saveWork(work);//保存信息

                        success = true;
                        msg = "数据保存成功";
                        result.IsLow = (bean.Weight - conn.Weight) < 0.00000001;//低重
                        result.Value1 = bean.Weight + "";
                        result.Value2 = conn.Weight + "";
                        result.Msg2 = "低重，发送报警。报警重量:" + conn.Weight + "，当前重量:" + bean.Weight;//在IsLow为true时才正确
                        result.Bean = conn;
                    }
                    else
                    {
                        msg = "电子称 " + conn.Ip + " 未有用户刷卡，重量数据不保存";
                    }
                    
                }
                else
                {
                    msg = "电子称 "+ conn.Ip+" 关联的读卡器 "+conn.Ip3+" 不存在";
                }
                
            }
            else
            {
                msg = "关联表中不存在电子秤IP:" + bean.Ip + "的报警器及读卡器关联信息";
            }
            result.Success = success;
            result.Msg = msg;

            return result;
        }
        public static void insertUserDataToWorkBean(UserBean bean)//读卡器读到卡号
        {
            /***
             接收到一个用户刷卡信息
             1、查看卡号是否存在用户表中(可不做要求cjc)
             2、查看关联表中，是否存在读卡器IP
             3、记录

             一个IP对应一个读卡器,
             查看workMap中是否有
             */

            // TODO 从用户表中通过卡号查找用户信息在给到bean

            WorkBean work = tryGetWorkBeanValue(bean.Ip);

            if (work != null)
            {
                if (!work.Card.Equals(bean.Card))//判断存在卡号是否与当前刷的卡号一致  不一致则移除后新增
                {
                    workMap.Remove(bean.Ip);
                    workMap.Add(bean.Ip, insertUserToMap(bean));
                }
            }
            else
            {
                workMap.Add(bean.Ip, insertUserToMap(bean));
            }

        }

        private static WorkBean insertUserToMap(UserBean bean)
        {
            WorkBean workBean = new WorkBean();
            workBean.UserIp = bean.Ip;
            workBean.UserId = bean.Num;
            workBean.Name = bean.Name;
            workBean.Card = bean.Card;

            return workBean;
        }

        private static WorkBean insertWeightToMap(WorkBean workBean,DZCBean bean)
        {
            workBean.WeiIp = bean.Ip;
            workBean.Weight = bean.Weight;
            workBean.Data = bean.Data;
            workBean.Unit = bean.Unit;

            return workBean;
        }

        private static WorkBean tryGetWorkBeanValue(string ip)
        {
            WorkBean work = new WorkBean();
            bool exist = workMap.TryGetValue(ip, out work);
            return work;
        }

        //打印table
        private void PrintRows(DataSet dataSet)
        {
            //输出DataSet
            // For each table in the DataSet, print the row values.
            foreach (DataTable table in dataSet.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        Console.Write(row[column]+"  ");
                    }
                    Console.WriteLine();
                }
            }
        }

        public void release()
        {
            try
            {
                if (conn != null) {
                    conn.Close();
                }
            }
            catch (SqlException e)
            {
                LogUtil.instance.Log("-------关闭数据库异常-------");
                throw (e);
            }
            
        }
    }
}
