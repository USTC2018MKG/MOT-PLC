using constant;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

namespace MOT_PLC.pages
{
    /// <summary>
    /// QueryOrderPage.xaml 的交互逻辑
    /// </summary>
    public partial class QueryOrderPage : Page
    {

        private MySqlConnection conn = new MySqlConnection(Constant.myConnectionString);


        private MySqlConnection conn2 = new MySqlConnection(Constant.myConnectionString);
        // 定时器
        System.Windows.Threading.DispatcherTimer dtimer;

        // 员工号
        private String employeeId;

        String orderId;

        public QueryOrderPage(String employeeId)
        {
            InitializeComponent();
            this.employeeId = employeeId;
            if (dtimer == null)
            {
                dtimer = new System.Windows.Threading.DispatcherTimer();
                dtimer.Interval = TimeSpan.FromSeconds(1);
                dtimer.Tick += dtimer_Tick;
            }
        }

        void dtimer_Tick(object sender, EventArgs e)
        {
            if(conn2.State == ConnectionState.Closed)
            {
                conn2.Open();
            }
            String sql = String.Format("select * from out_order where out_id = '{0}' and state = {1}", orderId, 3);
            MySqlCommand cmd = new MySqlCommand(sql, conn2);
            object o = cmd.ExecuteScalar();

            //MySqlDataAdapter md = new MySqlDataAdapter(sql, conn2);
            //DataSet ds = new DataSet();
            //md.Fill(ds, "out_order");
            //DataRowCollection collection = ds.Tables["out_order"].Rows;
            if (null != o)
            {
                conn2.Close();
                dtimer.Stop();
                MessageBoxResult result = MessageBox.Show("出货成功", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.OK)
                {
                    this.NavigationService.GoBack();
                }
            }
        }

        // 页面加载完成后
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 1.查询是否有关联的已出货的订单
            // 1.1 如果有
            //    1.1.1 修改订单状态为，用户希望取货状态
            //    1.1.2 定时查询订单是否开门成功。
            // 1.2 如果没有，提示无订单信息，点击确认后返回主界面
            try
            {
                conn.Open();
                String sql = String.Format("select * from out_order where employee_id = '{0}' and state = {1} order by out_time DESC", employeeId, 1);
                Console.WriteLine(sql+ "\nsql+++++++++++++++++++");
                MySqlDataAdapter md = new MySqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                Console.WriteLine(ds);
                md.Fill(ds, "out_order");
                Console.WriteLine(md);
                DataRowCollection collection = ds.Tables["out_order"].Rows;

                if (collection.Count >= 1)
                {               
                    DataRow row = collection[0];
                    orderId = row["out_id"].ToString();
                    //row["state"] = "2";

                    // 更新状态
                    String updatesSql = String.Format("update out_order set state = 2 where out_id='{0}'", orderId);
                    MySqlCommand cmd = new MySqlCommand(updatesSql, conn);
                    int result = cmd.ExecuteNonQuery();

                    // 修改订单信息成功，查询是否开门成功
                    if (result == 1)
                    {
                        dtimer.Start();
                    }
                    else
                    {
                        Console.WriteLine("+++++++++no+++++++++++++");
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("未查询到订单", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.OK)
                    {
                        this.NavigationService.GoBack();
                    }
                }

            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
