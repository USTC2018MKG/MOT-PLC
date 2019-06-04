using Device;
using System;
using System.Collections.Generic;
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
    /// CardPage.xaml 的交互逻辑
    /// </summary>
    public partial class CardPage : Page
    {

        System.Windows.Threading.DispatcherTimer dtimer;

        public CardPage()
        {
            InitializeComponent();
            if (dtimer == null)
            {
                dtimer = new System.Windows.Threading.DispatcherTimer();
                dtimer.Interval = TimeSpan.FromSeconds(1);
                dtimer.Tick += dtimer_Tick;
            }
        }

        void dtimer_Tick(object sender, EventArgs e)
        {
            // 判断设备是否可用，不可用则继续检测
            if (CardDevice.Instance.IsDeviceOk)
            {
                String cardNo = CardDevice.Instance.GetCardNo();
                if (!String.IsNullOrEmpty(cardNo))
                {
                    // 刷卡成功后，蜂鸣下
                    CardDevice.Instance.Beep();
                    //打开新窗口
                    this.NavigationService.Navigate(new QueryOrderPage(cardNo));
                    // 关闭定时器
                    dtimer.Stop();
                }
            }
            else
            {
                CardDevice.Instance.Prepare();
                if (!CardDevice.Instance.IsDeviceOk)
                {
                    labelTip.Content = "未检测到刷卡机!";
                }
                else
                {
                    labelTip.Content = "";
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dtimer.Start();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if(dtimer != null && dtimer.IsEnabled)
            {
                dtimer.Stop();
            }
        }
    }
}
