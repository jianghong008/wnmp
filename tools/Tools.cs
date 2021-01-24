using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace wnmp.tools
{
    /// <summary>
    /// 程序工具
    /// </summary>
    public class Tools
    {
        public static string NginxRoot = "wnmp/nginx-1.18.0/";
        public static string MysqlRoot = "wnmp/mysql-5.7.26/";
        public static string PHPRoot = "wnmp/php/php7.3.4nts/";
        /// <summary>
        /// 初始化运行环境
        /// </summary>
        public static void InitApp()
        {

        }

        /// <summary>
        /// 加载运行配置
        /// </summary>
        public static void LoadConf()
        {
            string path = Site.GetRootPath() + "/wnmp.conf";
            if (!File.Exists(path))
            {
                //创建新配置文件
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("");
                sw.Close();
            }
        }

        /// <summary>
        /// 管理nginx
        /// </summary>
        /// <param name="cmd">管理命令 start stop restart </param>
        public static void CmdNginx(string cmd)
        {
            
            string path = Site.GetRootPath()+NginxRoot + "/nginx.exe";
            if (!File.Exists(path))
            {
                MessageBox.Show("Nginx未找到", "错误");
                return;
            }
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;
            Process p = new Process();
            p.StartInfo = info;
            p.Start();
            //跳转nginx工作目录，否则启动失败
            p.StandardInput.WriteLine("cd " + NginxRoot);
            Process[] pp = Process.GetProcessesByName("nginx");
            if (pp.Length > 0)
            {
                //如果已经运行了
                if(cmd== "start")
                {
                    //启动
                    return;
                }else if(cmd == "restart")
                {
                    //重启
                    p.StandardInput.WriteLine("nginx.exe -s reload");
                }
                else
                {
                    //停止
                    //p.StandardInput.WriteLine("nginx.exe -s quit");
                    foreach (var item in pp)
                    {
                        item.Kill();
                    }
                }
            }
            else
            {
                //如果没有运行
                if (cmd == "start")
                {
                    //启动
                    
                    p.StandardInput.WriteLine(" nginx.exe");
                    
                }
                else if (cmd == "restart")
                {
                    //重启
                    
                }
                else
                {
                    //停止
                }
            }
            p.StandardInput.WriteLine("exit");
            //p.WaitForExit();
            p.Close();
            Console.WriteLine(p);
        }

        /// <summary>
        /// 运行cmd命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="path">工作空间</param>
        /// <param name="isReturn">是否等待</param>
        /// <returns>返回字符结果</returns>
        public static string RunCMD(string cmd,string path, bool isReturn)
        {
            Process pro = new Process();
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.CreateNoWindow = true;
            //启动并隐藏窗口
            pro.Start();
            pro.StandardInput.WriteLine("cd "+path);
            pro.StandardInput.WriteLine(cmd);
            pro.StandardInput.WriteLine("exit");
            pro.StandardInput.AutoFlush = true;
            //等待程序执行完退出进程
            if (isReturn)
            {
                //获取cmd窗口的输出信息
                string output = pro.StandardOutput.ReadToEnd();
                pro.WaitForExit();
                pro.Close();
                return output;
            }
   
            pro.Close();
            return "";
        }

        /// <summary>
        /// 检测Nginx是否正在运行
        /// </summary>
        /// <returns></returns>
        public static bool NginxIsRunning()
        {
            Process[] pp = Process.GetProcessesByName("nginx");
            if (pp.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 管理Mysql
        /// </summary>
        /// <param name="cmd">管理命令 start stop restart </param>
        public static void CmdMysql(string cmd)
        {

            string path = Site.GetRootPath() + MysqlRoot + "bin/mysqld.exe";
            if (!File.Exists(path))
            {
                MessageBox.Show("Mysql未找到", "错误");
                return;
            }
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;
            Process p = new Process();
            
            p.StartInfo = info;
            p.Start();
            //跳转Mysql工作目录，否则启动失败
            p.StandardInput.WriteLine("cd " + MysqlRoot+"bin");
            Process[] pp = Process.GetProcessesByName("mysqld");
            if (pp.Length > 0)
            {
                //如果已经运行了
                if (cmd == "start")
                {
                    //启动
                    return;
                }
                else if (cmd == "restart")
                {
                    //重启
                    foreach (var item in pp)
                    {
                        item.Kill();
                    }
                    p.StandardInput.WriteLine(" mysqld.exe");
                }
                else
                {
                    //停止
                    foreach (var item in pp)
                    {
                        item.Kill();
                    }
                }
            }
            else
            {
                //如果没有运行
                if (cmd == "start")
                {
                    //启动

                    p.StandardInput.WriteLine(" mysqld.exe");

                }
                else if (cmd == "restart")
                {
                    //重启
                    
                }
                else
                {
                    //停止
                }
            }
            p.StandardInput.WriteLine("exit");
            //p.WaitForExit();
            p.Close();
            Console.WriteLine(p);
        }

        /// <summary>
        /// 检查Mysql配置并自动修复
        /// </summary>
        public static void CheckMysqlINI()
        {
            string ini = Site.GetRootPath() + MysqlRoot;
            string path = ini + "defaultIni";
            if (File.Exists(path))
            {
                //如果存在则检查工作目录
                
                try
                {
                    StreamReader sr = new StreamReader(path);
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    //删除老配置
                    if (File.Exists(ini + "my.ini"))
                    {
                        File.Delete(ini + "my.ini");
                    }
                    
                    ini = ini.Replace(@"\",@"/");
                    //工作目录
                    txt = Regex.Replace(txt, "basedir=.+", "basedir=" + ini);
                    //数据目录
                    txt = Regex.Replace(txt, "datadir=.+", "datadir=" + ini + "data/");
                    //写入
                    StreamWriter sw = new StreamWriter(ini + "my.ini");
                    sw.Write(txt);
                    sw.Flush();
                    sw.Close();
                    Console.WriteLine(txt);
                }
                catch
                {
                    MessageBox.Show("初始化Mysql失败！", "错误");
                }

            }
            else
            {
                MessageBox.Show("未找到defaultIni文件！","错误");
            }

        }
        /// <summary>
        /// 检测Mysql是否正在运行
        /// </summary>
        /// <returns></returns>
        public static bool MysqlIsRunning()
        {
            Process[] pp = Process.GetProcessesByName("mysqld");
            if (pp.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 管理PHP
        /// </summary>
        /// <param name="cmd">管理命令 start stop restart </param>
        public static void CmdPHP(string cmd)
        {

            string path = Site.GetRootPath() + PHPRoot + "php-cgi.exe";
            if (!File.Exists(path))
            {
                MessageBox.Show("PHP未找到", "错误");
                return;
            }
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;
            Process p = new Process();

            p.StartInfo = info;
            p.Start();
            //跳转PHP工作目录，否则启动失败
            p.StandardInput.WriteLine("cd " + PHPRoot);
            Process[] pp = Process.GetProcessesByName("php-cgi");
            if (pp.Length > 0)
            {
                //如果已经运行了
                if (cmd == "start")
                {
                    //启动
                    return;
                }
                else if (cmd == "restart")
                {
                    //重启
                    foreach (var item in pp)
                    {
                        item.Kill();
                    }
                    p.StandardInput.WriteLine("php-cgi.exe -b 127.0.0.1:9000 -c ./php.ini");
                }
                else
                {
                    //停止
                    foreach (var item in pp)
                    {
                        item.Kill();
                    }
                }
            }
            else
            {
                //如果没有运行
                if (cmd == "start")
                {
                    //启动

                    p.StandardInput.WriteLine("php-cgi.exe -b 127.0.0.1:9000 -c ./php.ini");

                }
                else if (cmd == "restart")
                {
                    //重启

                }
                else
                {
                    //停止
                }
            }
            p.StandardInput.WriteLine("exit");
            //p.WaitForExit();
            p.Close();
            Console.WriteLine(p);
        }
        /// <summary>
        /// 检测PHP是否正在运行
        /// </summary>
        /// <returns></returns>
        public static bool PHPIsRunning()
        {
            Process[] pp = Process.GetProcessesByName("php-cgi");
            
            if (pp.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
