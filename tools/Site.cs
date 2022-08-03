using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace wnmp.tools
{
    class Site
    {
        private AppConf appConf;
        public Site(AppConf ac)
        {
            appConf = ac;
        }
        //添加站点
        public void AddSite(SiteConf conf)
        {
            string conf_name = conf.domainName.Split(" ")[0];
            string confPath = GetRootPath()+ appConf.NginxRoot + "conf/vhosts/" + conf_name + ".conf";
            if (File.Exists(confPath))
            {
                File.Delete(confPath);
            }
            //写配置
            FileStream fs = new FileStream(confPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("server {");
            //端口
            sw.WriteLine("\tlisten "+conf.sitePort+";");
            //域名
            sw.WriteLine("\tserver_name " + conf.domainName + ";");
            //目录
            sw.WriteLine("\troot          " + conf.siteRoot + ";");
            //跨域
            if (conf.cross)
            {
                sw.WriteLine("\t#允许跨域请求");
                sw.WriteLine("\tadd_header Access-Control-Allow-Origin *;");
                sw.WriteLine("\tadd_header Access-Control-Allow-Methods 'GET, POST, OPTIONS';");
                sw.WriteLine("\tadd_header Access-Control-Allow-Headers 'DNT,X-Mx-ReqToken,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Authorization';");
            }
            //默认文件
            sw.WriteLine("\t#默认文档");
            sw.WriteLine("\tindex " + conf.siteFile + ";");
            //压缩
            if (conf.gzip)
            {
                sw.WriteLine("\t#开启文件压缩");
                sw.WriteLine("\tgzip  on;");
                sw.WriteLine("\tgzip_min_length 1k;");
                sw.WriteLine("\tgzip_buffers 4 16k;");
                sw.WriteLine("\tgzip_http_version 1.0;");
                sw.WriteLine("\tgzip_comp_level 4;");
                sw.WriteLine("\tgzip_types text/plain application/javascript application/css  text/css application/xml text/javascript application/x-httpd-php image/jpeg image/gif image/png;");
                sw.WriteLine("\tgzip_vary off;");
                sw.WriteLine("\tgzip_disable \"MSIE[1 - 6]\\.\";");
            }
            //http转https
            sw.WriteLine("\t#rewrite ^(.*)$ https://$host$1 permanent;");
            //CGI
            sw.WriteLine("\t#CGI");
            sw.WriteLine("\tlocation ~ \\.php(.*)$ {");
            sw.WriteLine("\t\tfastcgi_pass   127.0.0.1:9000;");
            sw.WriteLine("\t\tfastcgi_index  index.php;");
            sw.WriteLine("\t\tfastcgi_split_path_info  ^((?U).+\\.php)(/?.+)$;");
            sw.WriteLine("\t\tfastcgi_param  SCRIPT_FILENAME  $document_root$fastcgi_script_name;");
            sw.WriteLine("\t\tfastcgi_param  PATH_INFO  $fastcgi_path_info;");
            sw.WriteLine("\t\tfastcgi_param  PATH_TRANSLATED  $document_root$fastcgi_path_info;");
            sw.WriteLine("\t\tinclude        fastcgi_params;");
            sw.WriteLine("\t}");
            //URL重写
            if (conf.staticStatus)
            {
                sw.WriteLine("\t#url重写");
                sw.WriteLine("\tlocation "+conf.staticRoot+" {");
                sw.WriteLine("\t\tif (!-e $request_filename) {");
                sw.WriteLine("\t\t\trewrite  ^(.*)$  /index.php$1  last;");
                sw.WriteLine("\t\t\tbreak; ");
                sw.WriteLine("\t\t}");
                sw.WriteLine("\t}");
            }
            

            sw.WriteLine("}");
            sw.Flush();
            sw.Close();
            fs.Close();
            //同步hosts
            if (conf.host)
            {
                EditHost(conf.domainName, null,true);
            }
        }

        /// <summary>
        /// 删除站点
        /// </summary>
        /// <param name="sc">站点配置</param>
        public void RemoveSite(SiteConf sc)
        {
            //先删除站点配置
            string conf_name = sc.domainName.Split(" ")[0];
            string confPath = GetRootPath() + appConf.NginxRoot+ "conf/vhosts/" + conf_name + ".conf";
            if (File.Exists(confPath))
            {
                try
                {
                    File.Delete(confPath);
                    //再删除hosts
                    EditHost(conf_name, null, false);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message, "错误");
                }
                
            }

        }

        /// <summary>
        /// 编辑hosts文件，不给第二个参数视为删除
        /// </summary>
        /// <param name="doMainName">绑定域名</param>
        /// <param name="newName">新的域名</param>
        /// /// <param name="add">是否新增</param>
        public void EditHost(string doMainName,string newName,bool add)
        {
            string conf_name = doMainName.Split(" ")[0];
            string filePath = "C:\\Windows\\System32\\drivers\\etc/hosts";
            if (!File.Exists(filePath))
            {
                MessageBox.Show("未找到hosts文件！","错误");
                return;
            }
            StreamReader sr = new StreamReader(filePath);
            string text = sr.ReadToEnd();
            sr.Close();
            string reg_str = @"[\r\n]?127.0.0.1\s+" + conf_name;
            if (newName != null)
            {
                //修改
                text = Regex.Replace(text, conf_name, newName);
            }
            else
            {
                //删除
                text = Regex.Replace(text, reg_str, "");
                text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(text));
                Console.WriteLine(text);
            }
            if (add)
            {
                //新增
                text += "\r127.0.0.1 " + conf_name;
            }
            //去掉空行
            text = Regex.Replace(text, @"\n\s", "");
            text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(text));
            //重新写入
            StreamWriter sw = new StreamWriter(filePath);
            sw.Write(text);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 获取工作目录
        /// </summary>
        /// <returns></returns>
        public static String GetRootPath()
        {
            return Environment.CurrentDirectory+"/";
        }
        public static SiteConf GetSiteConfFromFile()
        {
            SiteConf conf = new SiteConf();

            return conf;
        }

        /// <summary>
        /// 获取站点列表
        /// </summary>
        /// <returns>SiteConf</returns>
        public List<SiteConf> GetSiteList()
        {
            string confPath = GetRootPath()+ appConf.NginxRoot + "conf/vhosts/";
            List<SiteConf> ar = new List<SiteConf>();
            try
            {
                string[] file = Directory.GetFiles(confPath,"*.conf");
                for(int i = 0; i < file.Length; i++)
                {
                    //遍历处理
                    FileStream fs = new FileStream(file[i], FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    SiteConf sc = new SiteConf();
                    while (!sr.EndOfStream)
                    {
                        string str = sr.ReadLine();
                        //端口
                        Match m = Regex.Match(str, @"[0-9]{2,5}");
                        if (Regex.IsMatch(str, @"\tlisten.+")&& m.Success)
                        {
                            sc.sitePort = m.Value;
                            Console.WriteLine(12312);
                        }
                        //域名
                        if (Regex.IsMatch(str, @"\tserver_name .+"))
                        {
                            string []s = str.Split(" ",2);
                            if (s.Length >= 2)
                            {
                                sc.domainName = s[1].Replace(";", "").Trim();
                                
                            }
                            else
                            {
                                sc.domainName = "未设置";
                            }
                            
                        }
                        //目录
                        if (Regex.IsMatch(str, @"\troot .+"))
                        {
                            string[] s = str.Split(" ",2);
                            if (s.Length >= 2)
                            {
                                sc.siteRoot = s[1].Replace(";", "").Trim();
                            }
                            else
                            {
                                sc.siteRoot = "未设置";
                            }
                        }
                        //默认文件
                        if (Regex.IsMatch(str, @"\tindex .+"))
                        {
                            string[] s = str.Split(" ", 2);
                            if (s.Length >= 2)
                            {
                                sc.siteFile = s[1].Replace(";", "").Trim();
                            }
                            else
                            {
                                sc.siteFile = "index.html";
                            }
                        }

                    }
                    sr.Close();
                    fs.Close();
                    StreamReader ssr = new StreamReader(file[i]);
                    string text = ssr.ReadToEnd();
                    sc.ConfText = text;
                    ssr.Close();
                    //压缩
                    if (Regex.IsMatch(text, @"\tgzip  on;"))
                    {
                        sc.gzip = true;
                    }
                    else
                    {
                        sc.gzip = false;
                    }
                    //跨域
                    if (Regex.IsMatch(text, @"\tadd_header Access-Control-Allow-Origin \*;"))
                    {
                        sc.cross = true;
                    }
                    else
                    {
                        sc.cross = false;
                    }
                    //URL重写
                    Match mm = Regex.Match(text, @".+#url重写[\d\D]+\tlocation.+");
                    if (Regex.IsMatch(text, @"\t\tif \(!-e \$request_filename\) \{")&&mm.Success)
                    {
                        sc.staticStatus = true;
                        string []ss = Regex.Split(mm.Value, "\r\n");
                        if (ss.Length >= 2)
                        {
                            string[] s = ss[1].Split(" ", 2);
                            if (s.Length >= 2)
                            {
                                sc.staticRoot = s[1].Replace("{", "").Trim();
                            }
                            else
                            {
                                sc.staticRoot = "/";
                            }
                        }
                        
                    }
                    else
                    {
                        sc.staticStatus = false;
                    }
                    //处理自行修改的站点文件
                    if (sc.domainName.Trim() == "")
                    {
                        sc.remake = "异常";
                    }
                    //收集站点
                    ar.Add(sc);
                }
                
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"错误");
            }
            return ar;
        }
    }
}
