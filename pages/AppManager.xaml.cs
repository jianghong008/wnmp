using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using wnmp.models;
using wnmp.tools;

namespace wnmp.pages
{
    /// <summary>
    /// download.xaml 的交互逻辑
    /// </summary>
    public partial class download : Window
    {
        Tools tool;
        AppConf conf;
        MainWindow win;
        private ObservableCollection<ApplicationModel> PhpVersions = new ObservableCollection<ApplicationModel>();
        private ObservableCollection<ApplicationModel> MysqlVersions = new ObservableCollection<ApplicationModel>();
        private ObservableCollection<ApplicationModel> NginxVersions = new ObservableCollection<ApplicationModel>();

        private int TabsIndex = 0;
        public download(Tools t, AppConf c, MainWindow mw)
        {
            InitializeComponent();
            tool = t;
            conf = c;
            win = mw;

            MysqlListBox.ItemsSource = MysqlVersions;
            PhpListBox.ItemsSource = PhpVersions;
            NginxListBox.ItemsSource = NginxVersions;

            WebSpider.AppsUrl = c.appsUrl;
            GetRemoteApps();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        private void GetRemoteApps()
        {
            Loading.Visibility = Visibility.Visible;
            new Thread(() =>
            {
                tool.getWnmpVersions();
                WebSpider.GetAppsVersions(tool);
                Dispatcher.Invoke(() =>
                {
                    LoadList();
                    Loading.Visibility = Visibility.Hidden;
                });
                
            }).Start();
        }
        private void LoadList()
        {
            PhpVersions.Clear();
            // php
            foreach (var item in WebSpider.resources.php)
            {
 
                ApplicationModel m = new ApplicationModel
                {
                    Title = item.title,
                    Url = item.url,
                    Path = "/wnmp/php/" + item.title,
                    Installed = item.installed,
                    Config = "php.ini",
                    Type = "php"
                };
                PhpVersions.Add(m);
            }
            //mysql
            MysqlVersions.Clear();
            foreach (var item in WebSpider.resources.mysql)
            {
                ApplicationModel m = new ApplicationModel
                {
                    Title = item.title,
                    Url = item.url,
                    Path = "/wnmp/mysql/" + item.title,
                    Installed = item.installed,
                    Config = "my.ini",
                    Type = "mysql"
                };
                MysqlVersions.Add(m);
            }
            //nginx
            NginxVersions.Clear();
            foreach (var item in WebSpider.resources.nginx)
            {

                ApplicationModel m = new ApplicationModel
                {
                    Title = item.title,
                    Url = item.url,
                    Path = "/wnmp/nginx/" + item.title,
                    Installed = item.installed,
                    Config = "conf/nginx.conf",
                    Type = "nginx"
                };
                NginxVersions.Add(m);
            }
        }
        private ApplicationModel GetCurrentModel(string title)
        {
            if (TabsIndex == 0)
            {
                foreach (var item in PhpVersions)
                {
                    if (item.Title.Equals(title))
                    {
                        return item;
                    }
                }
            }else if(TabsIndex == 1)
            {
                foreach (var item in NginxVersions)
                {
                    if (item.Title.Equals(title))
                    {
                        return item;
                    }
                }
            }
            else if (TabsIndex == 2)
            {
                foreach (var item in MysqlVersions)
                {
                    if (item.Title.Equals(title))
                    {
                        return item;
                    }
                }
            }

            return null;
        }
        private void InstallApp(object sender, MouseButtonEventArgs e)
        {
            Label btn = (Label)sender;
            ApplicationModel model = GetCurrentModel(btn.Tag.ToString());
            if (model == null)
            {
                return;
            }
            if (model.Installing)
            {
                return;
            }
            model.InstallingAble = Visibility.Visible;
            model.Percent = 0;
            model.Wrong = false;
            // 安装
            new Thread(() =>
            {
                try
                {
                    WebSpider.DownloadAndExtract(model.Url, tool.RootPath + "/wnmp/"+model.Type, (total, percent) => {
                        Dispatcher.Invoke(() =>
                        {
                            model.Percent = percent;
                        });
                    });
                    Dispatcher.Invoke(() =>
                    {
                        //完成
                        model.InstallingAble = Visibility.Hidden;
                        model.InstallAble = Visibility.Hidden;
                        win.ReloadData();
                    });
                }catch (Exception ex)
                {
                    model.Wrong = true;
                    model.InstallingAble = Visibility.Hidden;
                    
                    Trace.WriteLine(ex.ToString());
                }

            }).Start();

        }
        private void L_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //打开目录
            Label btn = (Label)sender;
            Trace.WriteLine("title:");
            Trace.WriteLine(btn.Tag);
            ExplorePath(tool.RootPath + btn.Tag.ToString());
        }
        private void ChangeTabs(object sender,SelectionChangedEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            TabsIndex = tabControl.SelectedIndex;
        }
        /// <summary>
        /// 浏览文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void ExplorePath(string path)
        {
            try
            {
                Trace.WriteLine(path.Replace("/",@"\"));
                _ = Process.Start("explorer.exe", path.Replace("/", @"\"));
            }
            catch
            {

            }
        }
        private void RmBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //不可移除当前使用的，且必须保留一个版本
            Label btn = (Label)sender;
            ApplicationModel model = GetCurrentModel(btn.Tag.ToString());
            if (model == null || !model.Installed)
            {
                return;
            }
            string path = tool.RootPath + model.Path;
            if (path.IndexOf(conf.phpVersion) >= 0 || path.IndexOf(conf.mysqlVersion) >= 0 || path.IndexOf(conf.nginxVersion) >= 0)
            {
                _ = MessageBox.Show("该版本正在使用，不可移除！", "提示");
                return;
            }
            
            //移除应用
            MessageBoxResult mbr = MessageBox.Show("是否删除该版本？此操作不可逆！", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (mbr == MessageBoxResult.Yes)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(path);
                        di.Delete(true);
                        Dispatcher.Invoke(() =>
                        {
                            win.ReloadData();
                        });
                        GetRemoteApps();
                    }
                    catch(Exception err)
                    {
                        Trace.WriteLine(err);
                        _ = MessageBox.Show("删除失败，请稍后重试！");
                    }           }
                else
                {
                    _ = MessageBox.Show("删除失败，应用不存在！");
                }
                
            }
        }

        private void IniBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //查看配置
            Label btn = (Label)sender;
            ApplicationModel model = GetCurrentModel(btn.Tag.ToString());
            if (model == null || !model.Installed)
            {
                return;
            }
            string path = tool.RootPath + model.Path+"/"+model.Config;
            tool.OpenText(path);
        }

        private void WinodowClosing(object sender, CancelEventArgs e)
        {
            WebSpider.Exit = true;
        }
    }
}
