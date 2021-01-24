using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using wnmp.tools;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace wnmp
{
    /// <summary>
    /// webSite.xaml 的交互逻辑
    /// </summary>
    public partial class WebSite : Window
    {
        private SiteConf conf;//站点配置
        private MainWindow fWindow;//父窗口
        public WebSite(SiteConf sc, MainWindow app)
        {
            InitializeComponent();
            conf = sc;
            fWindow = app;
            if (conf != null)
            {
               
                loadConf();
            }
            
            if (sc.IsNew)
            {
                removeWebSite.Visibility = Visibility.Hidden;
            }
            else
            {
                removeWebSite.Visibility = Visibility.Visible;
            }
            Site.EditHost("test.xyz",null,false);
        }

        /*
         * 添加新网站
         */
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (domainNameInput.Text.Trim() != "")
            {
                try
                {
                    Site.AddSite(GetSiteConfFromForm());
                    fWindow.LoadList();
                    Close();
                }
                catch
                {
                    System.Windows.MessageBox.Show("添加失败", "错误");
                }
                
            }
            else
            {
                System.Windows.MessageBox.Show("网站域名必须填写","错误");
            }
            
        }

        /// <summary>
        /// 从表单读取配置
        /// </summary>
        /// <returns></returns>
       private SiteConf GetSiteConfFromForm()
       {
            SiteConf conf = new SiteConf();
            if (!conf.checkDomain(domainNameInput.Text))
            {
                conf.domainName = "";
            }
            else
            {
                conf.domainName = domainNameInput.Text;
            }
            //端口
            conf.sitePort = sitePortInput.Text;
            //站点目录
            conf.siteRoot = siteRootInput.Text;
            //默认文件
            conf.siteFile = siteFileInput.Text;
            //是否开启为静态
            conf.staticStatus = (bool)staticBox.IsChecked;
            //伪静态路径
            conf.staticRoot = staticInput.Text;
            //压缩
            conf.gzip = (bool)gzipBox.IsChecked;
            //跨域
            conf.cross = (bool)crossBox.IsChecked;
            //同步host
            conf.host = (bool)hostBox.IsChecked;
            return conf;
        }

        /// <summary>
        /// 渲染表单
        /// </summary>
        private void loadConf()
        {
            domainNameInput.Text = conf.domainName;
            sitePortInput.Text = conf.sitePort;
            siteRootInput.Text = conf.siteRoot;
            siteFileInput.Text = conf.siteFile;
            staticBox.IsChecked = conf.staticStatus;
            staticInput.Text = conf.staticRoot;
            gzipBox.IsChecked = conf.gzip;
            crossBox.IsChecked = conf.cross;
            hostBox.IsChecked = conf.host;
        }

        private void removeWebSite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //删除网站
            Site.RemoveSite(conf);
            fWindow.LoadList();
            Close();
        }

        private void Label_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            //选择目录
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            
            string path = fbd.SelectedPath;
            if (path.Trim() == "")
            {
                //System.Windows.MessageBox.Show("必须选择目录", "错误");
                return;
            }
            siteRootInput.Text = path.Replace(@"\",@"\\");
        }

        private void staticInput_LostFocus(object sender, RoutedEventArgs e)
        {
            //伪静态路径检测
            string s = staticInput.Text.Trim();
            if (s == "")
            {
                staticInput.Text = "/";
            }
        }

        private void siteFileInput_LostFocus(object sender, RoutedEventArgs e)
        {
            //默认文件检测
            string s = siteFileInput.Text.Trim();
            if (s == "")
            {
                siteFileInput.Text = "index.html";
            }
        }

        private void sitePortInput_LostFocus(object sender, RoutedEventArgs e)
        {
            //默认文件检测
            string s = sitePortInput.Text.Trim();
            if(!Regex.IsMatch(s, @"^[0-9]{2,4}$"))
            {
                sitePortInput.Text = "80";
                System.Windows.MessageBox.Show("网站端口必须为整数");
            }
           
        }
    }
}
