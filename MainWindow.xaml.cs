using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Shell;
using wnmp.tools;

namespace wnmp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebSite ws;
        private List<SiteConf> SiteList;
        private bool NginxStopping = false;
        private bool MysqlStopping = false;
        private bool PHPStopping = false;
        private bool AppQuit = false;
        private long LastClickTime;//防重复点击
        private int CheckTime = 5*1000;//监控间隔时间
        public MainWindow()
        {
            InitializeComponent();
            //异步加载
            new Thread(() => {
                this.Dispatcher.Invoke(new Action(() => {
                    loadList(false);
                }));
            }).Start();
            //nginx 监控
            checkNginx();
            //mysql 监控
            checkMysql();
            //PHP 监控
            checkPHP();
            //日志菜单
            logBox.ContextMenu = new ContextMenu();
            Label logClean = new Label()
            {
                Content = "清除日志",

            };
            logClean.MouseDown += logClean_MouseDown;
            logBox.ContextMenu.Items.Add(logClean);

            //检测Mysql配置
            Tools.CheckMysqlINI();
        }

        private void logClean_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //清除日志
            logBox.Text = "";
        }

        private void addWebSIteBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //添加新网站并打开站点管理窗口

            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 3)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }

            SiteConf sc = new SiteConf();
            sc.domainName = "www.test.xyz";
            sc.staticRoot = "/";
            sc.siteRoot = "C:\\Users";
            sc.sitePort = "80";
            sc.siteFile = "index.html";
            sc.IsNew = true;
            openSiteWindow(sc);

        }

        /// <summary>
        /// 加载站点
        /// </summary>
        private void loadList(bool force)
        {
            siteListBox.Items.Clear();
            Log("正在加载网站列表...");
            //加载站点列表
            if (SiteList == null || force)
            {
                SiteList = null;
                //加载loading
                Label cc = new Label()
                {
                    Width = 588,
                    Height = 250,
                    Padding = new Thickness(250,105,0,0)
                };
                siteListBox.Items.Add(cc);
                SiteList = Site.GetSiteList();
                siteListBox.Items.Clear();
            }
            foreach (var item in SiteList)
            {
                Canvas c = new Canvas();
                c.Width = 588;
                c.Height = 35;
                //域名
                Label l1 = new Label
                {
                    Content = item.domainName,
                    Width = 135,
                    Height = 25,
                    Margin = new Thickness(10, 5, 0, 0),
                    ToolTip = "打开此网站"
                };
                l1.Cursor = Cursors.Hand;
                l1.MouseDown += openUrl_MouseDown;
                c.Children.Add(l1);
                //端口
                Label l2 = new Label
                {
                    Content = item.sitePort,
                    Width = 50,
                    Height = 25,
                    Margin = new Thickness(175, 5, 0, 0)
                };
                c.Children.Add(l2);
                //路径
                Label l3 = new Label
                {
                    Content = item.siteRoot,
                    Width = 200,
                    Height = 25,
                    Margin = new Thickness(255, 5, 0, 0)
                };
                c.Children.Add(l3);
                //状态
                Label l4 = new Label
                {
                    Content = item.remake,
                    Width = 40,
                    Height = 25,
                    Margin = new Thickness(450, 5, 0, 0)
                };
                c.Children.Add(l4);
                //按钮
                Label l5= new Label
                {
                    Content = "管 理",
                    Width = 40,
                    Height = 25,
                    Margin = new Thickness(520, 5, 0, 0),
                    Style = FindResource("defautBtnLable") as Style,
                    
                };
                l5.DataContext = item.domainName;
                l5.MouseDown += siteListBtn_mouseDown;
                c.Children.Add(l5);
                siteListBox.Items.Add(c);
                
            }
        }

        /// <summary>
        /// 修改管理站点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void siteListBtn_mouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 3)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }

            Label l = (Label)sender;
            foreach (var item in SiteList)
            {
                if(item.domainName== l.DataContext.ToString())
                {
                    openSiteWindow(item);
                }
            }
            
        }

        private void openUrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //打开默认浏览器
            Label l = (Label)sender;
            string domainName = l.Content.ToString().Trim();
            if (domainName != "")
            {
                foreach (var item in SiteList)
                {
                    if (item.domainName == domainName)
                    {
                        string url = "http://" + domainName + ":"+item.sitePort;
                        Process.Start("explorer", url);
                    }
                }
                
            }
            
        }
        /// <summary>
        /// 打开站点管理窗口
        /// </summary>
        /// <param name="sc"></param>
        private void openSiteWindow(SiteConf sc)
        {
            //
            if (ws == null)
            {
                ws = new WebSite(sc,this);
                ws.Owner = this;
                ws.Show();
            }
            else
            {
                try
                {
                    ws.WindowState = WindowState.Normal;
                    ws.Activate();
                    ws.Show();
                }
                catch
                {
                    ws = null;
                    ws = new WebSite(sc,this);
                    ws.Owner = this;
                    ws.Show();
                }

            }
        }

        private void refreshWebSIteBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 3)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }
            LoadList();
        }
        public void LoadList()
        {
            //异步加载
            new Thread(() => {
                this.Dispatcher.Invoke(new Action(() => {
                    loadList(true);
                }));
            }).Start();
        }

        private void nginxRunBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 3)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }
            //控制nginx
            var btn = (TextBlock)sender;
            var str = btn.Text;
            if(str=="启 动")
            {
                Log("正在启动Nginx...");
                Tools.CmdNginx("start");
                btn.Text = "停 止";
                nginxStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0,255,0));
            }
            else
            {
                
                NginxStopping = true;
                Log("正在停止Nginx...");
                try
                {
                    Tools.CmdNginx("stop");
                    btn.Text = "启 动";
                    nginxStatusImg.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                }
                catch
                {
                    Log("Nginx停止错误");
                }
                
            }
        }
        
        /// <summary>
        /// 监控nginx主进程 每隔5秒
        /// </summary>
        private void checkNginx()
        {
            new Thread(()=> {
                while (true)
                {
                    if (AppQuit)
                    {
                        break;
                    }
                    if (Tools.NginxIsRunning())
                    {
                        Dispatcher.Invoke(new Action(() => {
                            nginxRunBtn.Text = "停 止";

                            nginxStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        }));
                        
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() => {
                            if (NginxStopping)
                            {
                                Log("已停止Nginx");
                            }
                            nginxRunBtn.Text = "启 动";
                            NginxStopping = false;
                            nginxStatusImg.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        }));
                        
                    }
                    Thread.Sleep(CheckTime);
                }

            }).Start();
        }

        private void nginxRestartBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 3)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }
            //重启nginx
            if (Tools.NginxIsRunning())
            {
                Log("正在重启Nginx...");
                Tools.CmdNginx("restart");
                nginxRunBtn.Text = "停 止";
                nginxStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //退出程序
            AppQuit = true;
            if (Tools.NginxIsRunning()){
                Tools.CmdNginx("stop");
            }
            if (Tools.MysqlIsRunning())
            {
                Tools.CmdMysql("stop");
            }

        }
        /// <summary>
        /// 监控Mysql主进程 每隔5秒
        /// </summary>
        private void checkMysql()
        {
            new Thread(() => {
                while (true)
                {
                    if (AppQuit)
                    {
                        break;
                    }
                    if (Tools.MysqlIsRunning())
                    {
                        Dispatcher.Invoke(new Action(() => {
                            mysqlRunBtn.Text = "停 止";

                            mysqlStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        }));

                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() => {
                            if (MysqlStopping)
                            {
                                Log("Mysql已停止");
                            }
                            mysqlRunBtn.Text = "启 动";
                            MysqlStopping = false;
                            mysqlStatusImg.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        }));

                    }
                    Thread.Sleep(CheckTime);
                }

            }).Start();
        }

        private void mysqlRunBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 3)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }
            //控制mysql
            var btn = (TextBlock)sender;
            var str = btn.Text;
            if (str == "启 动")
            {
                Log("正在启动Mysql...");
                Tools.CmdMysql("start");
                btn.Text = "停 止";
                mysqlStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
            else
            {

                MysqlStopping = true;
                try
                {
                    Log("正在停止Mysql...");
                    Tools.CmdMysql("stop");
                    btn.Text = "启 动";
                    mysqlStatusImg.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                }
                catch
                {
                    Log("Mysql停止失败，稍后重试");
                }
               
            }
        }

        private void mysqlRestartBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 3)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }
            //重启mysql
            if (Tools.MysqlIsRunning())
            {
                try
                {
                    Log("正在重启Mysql...");
                    Tools.CmdMysql("restart");
                    mysqlRunBtn.Text = "停 止";
                    mysqlStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                }
                catch
                {
                    Log("重启Mysql失败，稍后重试");
                    MessageBox.Show("重启Mysql失败，稍后重试");
                }
                
            }
        }

        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="txt"></param>
        public void Log(string txt)
        {
            if (logBox.Text != "")
            {
                logBox.Text += "\r" + DateTime.Now.ToString() + " ：" + txt;
            }
            else
            {
                logBox.Text += DateTime.Now.ToString() + " ：" + txt;
            }
            
        }

        private void phpRunBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 3)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }
            //控制PHP
            var btn = (TextBlock)sender;
            var str = btn.Text;
            if (str == "启 动")
            {
                Log("正在启动PHP...");
                Tools.CmdPHP("start");
                btn.Text = "停 止";
                phpStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
            else
            {
                if (PHPStopping)
                {
                    return;
                }
                MysqlStopping = true;
                try
                {
                    Log("正在停止PHP...");
                    Tools.CmdPHP("stop");
                    btn.Text = "启 动";
                    phpStatusImg.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                }
                catch
                {
                    Log("PHP停止失败，稍后重试");
                }

            }
        }
        /// <summary>
        /// 监控PHP主进程 每隔5秒
        /// </summary>
        private void checkPHP()
        {
            new Thread(() => {
                while (true)
                {
                    if (AppQuit)
                    {
                        break;
                    }
                    if (Tools.PHPIsRunning())
                    {
                        Dispatcher.Invoke(new Action(() => {
                            phpRunBtn.Text = "停 止";

                            phpStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        }));

                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() => {
                            if (PHPStopping)
                            {
                                Log("PHP已停止");
                            }
                            phpRunBtn.Text = "启 动";
                            PHPStopping = false;
                            phpStatusImg.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        }));

                    }
                    Thread.Sleep(CheckTime);
                }

            }).Start();
        }
        private void phpRestartBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 3)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }
            //重启php
            if (Tools.PHPIsRunning())
            {
                try
                {
                    Log("正在重启PHP...");
                    Tools.CmdPHP("restart");
                    phpRunBtn.Text = "停 止";
                    phpStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                }
                catch
                {
                    Log("重启PHP失败，稍后重试");
                }

            }
        }
    }
}
