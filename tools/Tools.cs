﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
        public string NginxRoot = "";
        public string MysqlRoot = "";
        public string PHPRoot = "";

        public string[] nginxVersions = { };
        public string[] mysqlVersions = { };
        public string[] phpVersions = { };

        private AppConf appConf;
        public string RootPath = "";
        [DllImport("User32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        /// <summary>
        /// 初始化运行环境
        /// </summary>
        public Tools(AppConf ac)
        {
            RootPath = Site.GetRootPath();
            appConf = ac;
            
        }
        /// <summary>
        /// 重新加载配置
        /// </summary>
        public void loadPath()
        {
            NginxRoot = "wnmp/nginx/" + appConf.nginxVersion + "/";
            MysqlRoot = "wnmp/mysql/" + appConf.mysqlVersion + "/";
            PHPRoot = "wnmp/php/" + appConf.phpVersion + "/";

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
        /// 获取当前可用版本
        /// </summary>
        public void getWnmpVersions()
        {
            try
            {
                //nginx
                nginxVersions = Directory.GetDirectories(RootPath + "wnmp/nginx");
                for (int i = 0; i < nginxVersions.Length; i++)
                {
                    nginxVersions[i] = Path.GetFileName(nginxVersions[i]);
                }
                //mysql
                mysqlVersions = Directory.GetDirectories(RootPath + "wnmp/mysql");
                for (int i = 0; i < mysqlVersions.Length; i++)
                {
                    mysqlVersions[i] = Path.GetFileName(mysqlVersions[i]);
                }
                //php
                phpVersions = Directory.GetDirectories(RootPath + "wnmp/php");
                for (int i = 0; i < phpVersions.Length; i++)
                {
                    phpVersions[i] = Path.GetFileName(phpVersions[i]);
                }
            }
            catch
            {

            }
            
        }
        /// <summary>
        /// 记事本打开配置文件
        /// </summary>
        /// <param name="confType">配置名称 nginx,mysql,php</param>
        public void OpenConf(string confType)
        {
            loadPath();
            string path = NginxRoot+"conf/nginx.conf";
            switch (confType)
            {
                case "nginx":
                    path = NginxRoot + "conf/nginx.conf";
                    break;
                case "mysql":
                    path = MysqlRoot+"my.ini";
                    break;
                case "php":
                    path = PHPRoot+"php.ini";
                    break;
                case "hosts":
                    path = "C:/Windows/System32/drivers/etc/hosts";
                    break;
            }
            try
            {
                _ = Process.Start("notepad.exe", path);
            }
            catch
            {

            }
        }

        /// <summary>
        /// 打开文本
        /// </summary>
        /// <param name="fileName"></param>
        public void OpenText(string fileName)
        {
            try
            {
                _ = Process.Start("notepad.exe", fileName);
            }
            catch
            {

            }
        }
        /// <summary>
        /// 管理nginx
        /// </summary>
        /// <param name="cmd">管理命令 start stop restart </param>
        public void CmdNginx(string cmd)
        {
            loadPath();
            string path = Site.GetRootPath()+NginxRoot + "/nginx.exe";
            if (!File.Exists(path))
            {
                _ = MessageBox.Show("Nginx未找到", "错误");
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
        public string RunCMD(string cmd,string path, bool isReturn)
        {
            loadPath();
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
        public bool NginxIsRunning()
        {
            loadPath();
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
        public void CmdMysql(string cmd)
        {
            loadPath();
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
        public void CheckMysqlINI()
        {
            loadPath();
            string ini = Site.GetRootPath() + MysqlRoot;
            string path = ini + "my.ini";
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

                    ini = ini.Replace(@"\", @"/");
                    //工作目录
                    txt = Regex.Replace(txt, "basedir=.+", "basedir = \"" + ini+"\"");
                    //数据目录
                    txt = Regex.Replace(txt, "datadir=.+", "datadir = \"" + ini + "data/\"");
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
                MessageBox.Show(path+"不存在！", "错误");
            }

        }
        /// <summary>
        /// 检查php配置并自动修复
        /// </summary>
        public void CheckPhpINI()
        {
            loadPath();
            string ini = Site.GetRootPath() + PHPRoot;
            string path = ini + "php.ini";
            if (File.Exists(path))
            {
                //如果存在则检查工作目录
                
                try
                {
                    StreamReader sr = new StreamReader(path);
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    //删除老配置
                    if (File.Exists(ini + "php.ini"))
                    {
                        File.Delete(ini + "php.ini");
                    }
                    
                    ini = ini.Replace(@"\",@"/");
                    //扩展目录
                    txt = Regex.Replace(txt, "extension_dir.+", "extension_dir = \"" + ini+"ext\"");
                    //数据目录
                    txt = Regex.Replace(txt, "session.save_path=.+", "session.save_path = \"" + ini + "tmp/tmp\"");
                    //写入
                    StreamWriter sw = new StreamWriter(ini + "php.ini");
                    sw.Write(txt);
                    sw.Flush();
                    sw.Close();
                    Console.WriteLine(txt);
                }
                catch
                {
                    MessageBox.Show("初始化PHP失败！", "错误");
                }

            }
            else
            {
                MessageBox.Show(path+"不存在！","错误");
            }

        }
        /// <summary>
        /// 检测Mysql是否正在运行
        /// </summary>
        /// <returns></returns>
        public bool MysqlIsRunning()
        {
            loadPath();
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
        public void CmdPHP(string cmd)
        {
            loadPath();
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
        public bool PHPIsRunning()
        {
            loadPath();
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

        /// <summary>
        /// 检查程序是否已在运行
        /// </summary>
        /// <returns></returns>
        public bool CheckAppIsRunning()
        {
            loadPath();
            try
            {
                Process[] ps = Process.GetProcessesByName("wnmp");
                Process p = Process.GetCurrentProcess();
                foreach (var item in ps)
                {
                    if (p.Id != item.Id)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
