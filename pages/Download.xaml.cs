using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        public download(Tools t, AppConf c)
        {
            InitializeComponent();
            tool = t;
            conf = c;
            loadList();
        }

        /// <summary>
        /// 加载列表
        /// </summary>
        private void loadList()
        {
            //先清空
            phpListBox.Items.Clear();
            mysqlListBox.Items.Clear();
            nginxListBox.Items.Clear();
            //php
            for (int i = 0; i < tool.phpVersions.Length; i++)
            {
                Canvas c = new Canvas();
                c.Width = 624;
                c.Height = 50;
                Label l = new Label();
                l.Content = "PHP " + tool.phpVersions[i];
                l.Margin = new Thickness(0, 12, 0, 0);
                l.Width = 350;
                l.MouseDoubleClick += L_MouseDoubleClick;
                l.DataContext = "wnmp/php/" + tool.phpVersions[i];
                Label iniBtn = new Label();
                iniBtn.Content = "配置";
                iniBtn.Width = 35;
                iniBtn.Height = 28;
                iniBtn.Margin = new Thickness(515, 12, 0, 0);
                iniBtn.Style = FindResource("defautBtnLable") as Style;
                iniBtn.MouseUp += IniBtn_MouseUp;
                iniBtn.DataContext = "wnmp/php/" + tool.phpVersions[i] + "/php.ini";
                Label rmBtn = new Label();
                rmBtn.Content = "移除";
                rmBtn.Width = 35;
                rmBtn.Height = 28;
                rmBtn.Margin = new Thickness(560, 12, 0, 0);
                rmBtn.Style = FindResource("dangerBtnLable") as Style;
                rmBtn.DataContext = "wnmp/php/" + tool.phpVersions[i];
                rmBtn.MouseUp += RmBtn_MouseUp;
                _ = c.Children.Add(l);
                _ = c.Children.Add(iniBtn);
                _ = c.Children.Add(rmBtn);

                phpListBox.Items.Add(c);
            }
            //mysql
            for (int i = 0; i < tool.mysqlVersions.Length; i++)
            {
                Canvas c = new Canvas();
                c.Width = 624;
                c.Height = 50;
                Label l = new Label();
                l.Content = "Mysql " + tool.mysqlVersions[i];
                l.Margin = new Thickness(0, 12, 0, 0);
                l.Width = 350;
                l.MouseDoubleClick += L_MouseDoubleClick;
                l.DataContext = "wnmp/mysql/" + tool.mysqlVersions[i];
                Label iniBtn = new Label();
                iniBtn.Content = "配置";
                iniBtn.Width = 35;
                iniBtn.Height = 28;
                iniBtn.Margin = new Thickness(515, 12, 0, 0);
                iniBtn.Style = FindResource("defautBtnLable") as Style;
                iniBtn.MouseUp += IniBtn_MouseUp;
                iniBtn.DataContext = "wnmp/mysql/" + tool.mysqlVersions[i] + "/my.ini";
                Label rmBtn = new Label();
                rmBtn.Content = "移除";
                rmBtn.Width = 35;
                rmBtn.Height = 28;
                rmBtn.Margin = new Thickness(560, 12, 0, 0);
                rmBtn.Style = FindResource("dangerBtnLable") as Style;
                rmBtn.DataContext = "wnmp/mysql/" + tool.mysqlVersions[i];
                rmBtn.MouseUp += RmBtn_MouseUp;
                _ = c.Children.Add(l);
                _ = c.Children.Add(iniBtn);
                _ = c.Children.Add(rmBtn);

                mysqlListBox.Items.Add(c);
            }
            //nginx
            for (int i = 0; i < tool.nginxVersions.Length; i++)
            {
                Canvas c = new Canvas();
                c.Width = 624;
                c.Height = 50;
                Label l = new Label();
                l.Content = "Nginx " + tool.nginxVersions[i];
                l.Margin = new Thickness(0, 12, 0, 0);
                l.Width = 350;
                l.MouseDoubleClick += L_MouseDoubleClick;
                l.DataContext = "wnmp/nginx/" + tool.nginxVersions[i];
                Label iniBtn = new Label();
                iniBtn.Content = "配置";
                iniBtn.Width = 35;
                iniBtn.Height = 28;
                iniBtn.Margin = new Thickness(515, 12, 0, 0);
                iniBtn.Style = FindResource("defautBtnLable") as Style;
                iniBtn.MouseUp += IniBtn_MouseUp;
                iniBtn.DataContext = "wnmp/nginx/" + tool.nginxVersions[i] + "/conf/nginx.conf";
                Label rmBtn = new Label();
                rmBtn.Content = "移除";
                rmBtn.Width = 35;
                rmBtn.Height = 28;
                rmBtn.Margin = new Thickness(560, 12, 0, 0);
                rmBtn.Style = FindResource("dangerBtnLable") as Style;
                rmBtn.DataContext = "wnmp/nginx/" + tool.nginxVersions[i];
                rmBtn.MouseUp += RmBtn_MouseUp;
                _ = c.Children.Add(l);
                _ = c.Children.Add(iniBtn);
                _ = c.Children.Add(rmBtn);

                nginxListBox.Items.Add(c);
            }
        }

        private void L_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //打开目录
            Label btn = (Label)sender;
            ExplorePath(tool.RootPath + btn.DataContext.ToString());
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
            //移除移除
            MessageBoxResult mbr = MessageBox.Show("是否删除该版本？此操作不可逆！", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (mbr == MessageBoxResult.Yes)
            {
                Label btn = (Label)sender;
                string path = tool.RootPath + btn.DataContext.ToString();
                if (Directory.Exists(path))
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(path);
                        di.Delete(true);
                        loadList();
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
            tool.OpenText(tool.RootPath + btn.DataContext.ToString());
        }
    }
}
