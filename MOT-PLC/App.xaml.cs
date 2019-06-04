using constant;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MOT_PLC
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private MySqlConnection conn;

        private void QueryOrder()
        {
            // 查询订单状态
            // 0 未操作
            // 1 需要PLC出货
            while (true)
            {
                if (ConnectionState.Open != conn.State)
                {
                    conn.Open();
                }
                DataRowCollection collection = query(2);

                if (collection.Count >= 1) // 有状态为2，有人来领货
                {
                    // PLC通讯，
                    String orderId = collection[0]["out_id"].ToString();
                    update(orderId, 3);
                }
                else
                {
                    // 查询等待出货的订单
                    DataRowCollection collectionPreOrder = query(0);
                    if (collectionPreOrder.Count >= 1) // 预下单订单 通知PLC出货
                    {
                        // PLC通讯出货
                        String orderId = collectionPreOrder[0]["out_id"].ToString();
                        update(orderId, 1);
                    }
                    else
                    {
                        // PLC心跳
                    }
                }

                Thread.Sleep(500);
            }
        }

        private DataRowCollection query(int state)
        {
            String sql = "select * from out_order where state = " + state;

            MySqlDataAdapter md = new MySqlDataAdapter(sql, conn);

            //MySqlCommand cmd = new MySqlCommand(sql, conn);
            //cmd.ExecuteReader()

            DataSet ds = new DataSet();
            md.Fill(ds, "out_order");
            return ds.Tables["out_order"].Rows;
        }

        private Boolean update(string orderId, int state)
        {
            String sql = String.Format("update out_order set state = {0} where out_id = '{1}'", state, orderId);
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            return null != cmd.ExecuteScalar();
        }

        private void Application_Activated(object sender, EventArgs e)
        {
            conn = new MySqlConnection(Constant.myConnectionString);
            ThreadStart childref = new ThreadStart(QueryOrder);
            Thread query = new Thread(childref);
            query.Start();
        }
    }
}
