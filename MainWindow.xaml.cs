using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using wnmp.pages;
using wnmp.tools;

namespace wnmp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebSite ws;
        private download DownloadPage;
        private List<SiteConf> SiteList;
        private bool NginxStopping = false;
        private bool MysqlStopping = false;
        private bool PHPStopping = false;
        private bool AppQuit = false;
        private long LastClickTime;//防重复点击
        private int CheckTime = 5*1000;//监控间隔时间
        private bool UIisLoaded = false;//加载完成

        private bool NginxUserRunning = false;//用户启动Nginx
        private bool MysqlUserRunning = false;//用户启动mysql
        private bool PhpUserRunning = false;//用户启动php
        
        private System.Windows.Forms.NotifyIcon TrayIcon;
        //主程序配置
        private AppConf appConf;
        private Tools tool;
        public MainWindow()
        {
            InitializeComponent();

            //初始化
            InitApp();
        }
        private void InitApp()
        {
            //加载配置
            appConf = new AppConf();
            
            tool = new Tools(appConf);
            //多开限制
            if (tool.CheckAppIsRunning())
            {
                MessageBox.Show("程序已在运行中，可在任务栏查看！", "错误");
                Environment.Exit(0);
                return;

            }
            tool.getWnmpVersions();
            //异步加载
            new Thread(() => {

                //配置检查
                if (appConf.checkNewVersion(appConf.appNewVersion))
                {
                    Dispatcher.Invoke(new Action(() => {
                        MessageBoxResult mbr = MessageBox.Show("有新版本可用，是否立即更新？", "更新提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (mbr == MessageBoxResult.Yes)
                        {
                            UpdateVersion();
                        }

                    }));
                }
                Dispatcher.Invoke(new Action(() => {
                    //设置UI
                    initUI();
                    //加载站点
                    loadList(false);
                    //加载完成
                    UIisLoaded = true;
                    //检测Mysql配置
                    tool.CheckMysqlINI();
                    //检测php配置
                    tool.CheckPhpINI();
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
            MenuItem logClean = new MenuItem();
            logClean.Header = "清除日志";
            logClean.Click += logClean_click;
            logBox.ContextMenu.Items.Add(logClean);

        }
        /// <summary>
        /// 初始化UI
        /// </summary>
        private void initUI(bool loadList=true)
        {
            Title = appConf.appName + "集成管理";
            windowsTitle.Text = Title +" "+ appConf.appVersion;
            phpVersionLabel.Content = "PHP " + appConf.phpVersion;
            mysqlVersionLabel.Content = "Mysql " + appConf.mysqlVersion;
            nginxVersionLabel.Content = "Nginx " + appConf.nginxVersion;
            if (appConf.autoUpdate=="1")
            {
                autoUpdate.IsChecked = true;
            }
            else
            {
                autoUpdate.IsChecked = false;
            }
            if (appConf.quitType == "1")
            {
                quitType.IsChecked = true;
            }
            else
            {
                quitType.IsChecked = false;
            }
            if (!loadList)
            {
                return;
            }
            //获取可以软件版本
            tool.getWnmpVersions();
            for (int i = 0; i < tool.nginxVersions.Length; i++)
            {
                nginxVersionSelect.Items.Add(tool.nginxVersions[i]);
                if (tool.nginxVersions[i] == appConf.nginxVersion)
                {
                    nginxVersionSelect.SelectedIndex = i;
                }   
            }
            for (int i = 0; i < tool.mysqlVersions.Length; i++)
            {
                mysqlVersionSelect.Items.Add(tool.mysqlVersions[i]);
                if (tool.mysqlVersions[i] == appConf.mysqlVersion)
                {
                    mysqlVersionSelect.SelectedIndex = i;
                }
            }
            for (int i = 0; i < tool.phpVersions.Length; i++)
            {
                phpVersionSelect.Items.Add(tool.phpVersions[i]);
                if (tool.phpVersions[i] == appConf.phpVersion)
                {
                    phpVersionSelect.SelectedIndex = i;
                }
            }
            //检测Mysql配置
            tool.CheckMysqlINI();
            //检测php配置
            tool.CheckPhpINI();
        }
        /// <summary>
        /// 更新版本
        /// </summary>
        private void UpdateVersion()
        {
            try
            {
                //加载更新程序
                string exe = Site.GetRootPath() + "/wnmpUpdate.exe";
                Process.Start(exe);
            }
            catch
            {

            }
        }
        private void logClean_click(object sender, RoutedEventArgs e)
        {
            //清除日志
            logBox.Text = "";
        }

        private void addWebSIteBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //添加新网站并打开站点管理窗口

            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 1)
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
            sc.siteFile = "index.html index.php";
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
                SiteList = (new Site(appConf)).GetSiteList();
                siteListBox.Items.Clear();
            }
            foreach (SiteConf item in SiteList)
            {
                Canvas c = new Canvas();
                c.Width = 588;
                c.Height = 35;
                //域名
                Label l1 = new Label
                {
                    Content = item.domainName.Split(" ")[0],
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
                    Margin = new Thickness(255, 5, 0, 0),
                    ToolTip = item.siteRoot
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
            if (LastClickTime > time - 1)
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
            string domainName = l.Content.ToString().Trim().Split(" ")[0];
            if (domainName != "")
            {
                foreach (var item in SiteList)
                {
                    if (item.domainName.IndexOf(domainName)!=-1)
                    {
                        string url = "http://" + domainName + ":"+item.sitePort;
                        Process.Start("explorer", url);
                        break;
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
                ws = new WebSite(sc,this,appConf);
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
                    ws = new WebSite(sc,this,appConf);
                    ws.Owner = this;
                    ws.Show();
                }

            }
        }

        private void refreshWebSIteBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 1)
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

        /// <summary>
        /// 启动nginx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nginxRunBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 1)
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
                NginxUserRunning = true;
                Log("启动Nginx...");
                tool.CmdNginx("start");
                btn.Text = "停 止";
                nginxStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0,255,0));
            }
            else
            {
                NginxUserRunning = false;
                if (NginxStopping)
                {
                    return;
                }
                NginxStopping = true;
                Log("正在停止Nginx...");
                try
                {
                    tool.CmdNginx("stop");
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
                    if (tool.NginxIsRunning())
                    {
                        Dispatcher.Invoke(new Action(() => {
                            nginxRunBtn.Text = "停 止";
                            nginxStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                            if (NginxStopping)
                            {
                                NginxStopping = false;
                                Log("Nginx停止失败");
                            }
                        }));
                        
                    }
                    else
                    {
                        if (NginxUserRunning)
                        {
                            //如果用户启动，则守护该进程
                            Log("Nginx意外停止，正在重启...");
                            tool.CmdNginx("start");
                        }
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
            if (LastClickTime > time - 1)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }
            //重启nginx
            if (tool.NginxIsRunning())
            {
                Log("重启Nginx...");
                tool.CmdNginx("restart");
                nginxRunBtn.Text = "停 止";
                nginxStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            
        }
        private void show_windows(object sender, EventArgs e)
        {
            //从任务栏重新显示界面
            Show();
            _ = Activate();
            
        }
        private void close_windows(object sender, EventArgs e)
        {
            //退出程序
            AppQuit = true;
            if (tool.NginxIsRunning())
            {
                tool.CmdNginx("stop");
            }
            if (tool.MysqlIsRunning())
            {
                tool.CmdMysql("stop");
            }
            Environment.Exit(0);
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
                    if (tool.MysqlIsRunning())
                    {
                        Dispatcher.Invoke(new Action(() => {
                            mysqlRunBtn.Text = "停 止";
                            mysqlStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                            if (MysqlStopping)
                            {
                                MysqlStopping = false;
                                Log("Msql停止失败");
                            }
                        }));

                    }
                    else
                    {
                        if (MysqlUserRunning)
                        {
                            //如果用户启动，则守护该进程
                            Log("Mysql意外停止，正在重启...");
                            tool.CmdMysql("start");
                        }
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

        /// <summary>
        /// 启动MySQL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mysqlRunBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 1)
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
                MysqlUserRunning = true;
                Log("启动Mysql...");
                tool.CmdMysql("start");
                btn.Text = "停 止";
                mysqlStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
            else
            {
                MysqlUserRunning = false;
                if (MysqlStopping)
                {
                    return;
                }
                MysqlStopping = true;
                
                try
                {
                    Log("正在停止Mysql...");
                    tool.CmdMysql("stop");
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
            if (LastClickTime > time - 1)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }
            //重启mysql
            if (tool.MysqlIsRunning())
            {
                try
                {
                    Log("重启Mysql...");
                    tool.CmdMysql("restart");
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
            Dispatcher.Invoke(new Action(() => {
                if (logBox.Text != "")
                {
                    logBox.Text += "\r" + DateTime.Now.ToString() + " ：" + txt;
                    logboxwarp.ScrollToEnd();
                }
                else
                {
                    logBox.Text += DateTime.Now.ToString() + " ：" + txt;
                }
            }));   
        }

        /// <summary>
        /// 启动php
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void phpRunBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 1)
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
                PhpUserRunning = true;
                Log("启动PHP...");
                tool.CmdPHP("start");
                btn.Text = "停 止";
                phpStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                Trace.WriteLine(0);
            }
            else
            {
                PhpUserRunning = false;
                if (PHPStopping)
                {
                    return;
                }
                
                try
                {
                    Log("正在停止PHP...");
                    PHPStopping = true;
                    tool.CmdPHP("stop");
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
                    if (tool.PHPIsRunning())
                    {
                        Dispatcher.Invoke(new Action(() => {
                            phpRunBtn.Text = "停 止";
                            phpStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                            if (PHPStopping)
                            {
                                PHPStopping = false;
                                Log("PHP停止失败");
                            }
                        }));

                    }
                    else
                    {
                        if (PhpUserRunning)
                        {
                            //如果用户启动，则守护该进程
                            Log("PHP意外停止，正在重启...");
                            tool.CmdPHP("start");
                        }
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
            if (LastClickTime > time - 1)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }
            //重启php
            if (tool.PHPIsRunning())
            {
                try
                {
                    Log("重启PHP...");
                    tool.CmdPHP("restart");
                    phpRunBtn.Text = "停 止";
                    phpStatusImg.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                }
                catch
                {
                    Log("重启PHP失败，稍后重试");
                }

            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //查看nginx配置
            tool.OpenConf("nginx");
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //查看mysql配置
            tool.OpenConf("mysql");
        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            //查看php配置
            tool.OpenConf("php");
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //手动更新
            UpdateVersion();
        }

        private void Label_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void close_windows(object sender, MouseButtonEventArgs e)
        {
            if (appConf.quitType == "0")
            {
                //退出程序
                AppQuit = true;
                if (tool.NginxIsRunning())
                {
                    tool.CmdNginx("stop");
                }
                if (tool.MysqlIsRunning())
                {
                    tool.CmdMysql("stop");
                }
                if (tool.PHPIsRunning())
                {
                    tool.CmdPHP("stop");
                }
                Environment.Exit(0);
            }
            else
            {
                this.Hide();
                //退出到任务栏
                if (TrayIcon == null)
                {
                    TrayIcon = new System.Windows.Forms.NotifyIcon();
                    TrayIcon.Icon = new System.Drawing.Icon("logo.ico");
                    TrayIcon.BalloonTipText = appConf.appName + appConf.appVersion + "正在运行";
                    TrayIcon.Text = appConf.appName + appConf.appVersion + "正在运行";

                    TrayIcon.MouseDoubleClick += show_windows;
                    TrayIcon.ShowBalloonTip(1000);

                    TrayIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
                    TrayIcon.ContextMenuStrip.Items.Add("显示窗口");
                    TrayIcon.ContextMenuStrip.Items.Add("退出程序");

                    TrayIcon.ContextMenuStrip.Items[0].Click += show_windows;
                    TrayIcon.ContextMenuStrip.Items[1].Click += close_windows;
                }
                TrayIcon.Visible = true;
            }
            
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void quitType_Click(object sender, RoutedEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 1)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }

            if ((bool)quitType.IsChecked)
            {
                appConf.quitType = "1";
            }
            else
            {
                appConf.quitType = "0";
            }
            
            appConf.Save();
        }

        private void autoUpdate_Click(object sender, RoutedEventArgs e)
        {
            //防重复点击
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (LastClickTime > time - 1)
            {
                return;
            }
            else
            {
                LastClickTime = time;
            }

            if ((bool)autoUpdate.IsChecked)
            {
                appConf.autoUpdate = "1";
            }
            else
            {
                appConf.autoUpdate = "0";
            }

            appConf.Save();
        }
        /// <summary>
        /// 切换PHP程序版本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void phpVersionSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = phpVersionSelect.SelectedIndex;
            if (UIisLoaded)
            {
                if (!tool.PHPIsRunning())
                {
                    appConf.phpVersion = tool.phpVersions[i];
                    appConf.Save();
                    initUI(false);
                }
                else
                {
                    //phpVersionSelect.SelectedIndex = (new ArrayList(tool.phpVersions)).IndexOf(appConf.phpVersion);
                    MessageBox.Show("请先停止PHP", "错误");
                }
                
            }
        }

        private void nginxVersionSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = nginxVersionSelect.SelectedIndex;
            if (UIisLoaded)
            {
                if (!tool.NginxIsRunning())
                {
                    appConf.nginxVersion = tool.nginxVersions[i];
                    appConf.Save();
                    initUI(false);
                }
                else
                {
                    MessageBox.Show("请先停止Nginx", "错误");
                }
                
            }
        }

        private void mysqlVersionSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = mysqlVersionSelect.SelectedIndex;
            if (UIisLoaded)
            {
                if (!tool.MysqlIsRunning())
                {
                    appConf.mysqlVersion = tool.mysqlVersions[i];
                    appConf.Save();
                    initUI(false);
                }
                else
                {
                    MessageBox.Show("请先停止Mysql", "错误");
                }

            }
        }

        private void refreshWebSIteBtn_Copy_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //打开软件下载界面
            if (DownloadPage == null)
            {
                DownloadPage = new download(tool,appConf);
                DownloadPage.Show();
            }
            else
            {
                try
                {
                    DownloadPage.Activate();
                    DownloadPage.Show();
                }
                catch
                {
                    DownloadPage = null;
                    DownloadPage = new download(tool,appConf);
                    DownloadPage.Show();
                }
                
            }

        }
    }
}
