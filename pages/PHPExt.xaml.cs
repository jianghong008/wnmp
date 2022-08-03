using System;
using System.Collections.Generic;
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
    /// PHPExt.xaml 的交互逻辑
    /// </summary>
    public partial class PHPExt : Window
    {
        PhpINI ini;
        public PHPExt(AppConf conf)
        {
            InitializeComponent();
            ini = new PhpINI(conf);
            if (ini.LoadState)
            {
                ExtBox.ItemsSource = ini.extensions;
            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ini.Save();
                MessageBox.Show("保存成功", "信息提示", MessageBoxButton.OK, MessageBoxImage.None);
            }catch(Exception)
            {
                MessageBox.Show("保存失败", "信息提示",MessageBoxButton.OK,MessageBoxImage.Hand);
            }
        }
    }
}
