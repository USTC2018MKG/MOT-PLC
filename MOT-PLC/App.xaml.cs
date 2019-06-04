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
       

        private void QueryOrder()
        {
            // 查询订单状态
            // 0 未操作
            // 1 需要PLC出货

            // 
            while (true)
            {
                
                Thread.Sleep(1000);

            }
        }

       
        private void Application_Activated(object sender, EventArgs e)
        {
            ThreadStart childref = new ThreadStart(QueryOrder);
            Thread query = new Thread(childref);
            query.Start();
        }
    }
}
