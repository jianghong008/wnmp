using System;
using System.IO;
using System.Net;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace wnmp.tools
{
    /// <summary>
    /// 主程序配置模型
    /// </summary>
    public class AppConf
    {
        /// <summary>
        /// 程序名称
        /// </summary>
        public string appName = "wnmp";
        public string autoUpdate = "0";//0-不更新 1-自动更新
        public string quitType = "1";//0-直接退出 1-隐藏托盘
        /// <summary>
        /// 当前版本
        /// </summary>
        public string appVersion = "1.0.0";
        public string appNewVersion = "1.0.0";

        public string appsUrl = "https://jianghong008.github.io/wnmp/wnmp-apps.json";
        /// <summary>
        /// nignx版本
        /// </summary>
        public string nginxVersion = "";
        /// <summary>
        /// mysql版本
        /// </summary>
        public string mysqlVersion = "";
        /// <summary>
        /// php版本
        /// </summary>
        public string phpVersion = "";

        private string _NginxRoot = "";
        public string MysqlRoot = "";
        public string PHPRoot = "";

        /// <summary>
        /// 检查域名
        /// </summary>
        /// <param name="domainNameInput">域名</param>
        /// <returns>bool</returns>

        public AppConf()
        {
            Load();
            
        }
        public string NginxRoot{
            get
            {
                return _NginxRoot;
            }
            set { _NginxRoot = value; }
        }
        public void Load()
        {
            
            string path = Site.GetRootPath() + "/wnmp.ini";
            if (!File.Exists(path))
            {
                //缺省配置
                Save();
            }
            try
            {
                StreamReader sr = new StreamReader(path);
                while (!sr.EndOfStream)
                {
                    string str = sr.ReadLine();
                    if (str.IndexOf("#") == 0 || str.Trim() == "")
                    {
                        
                        continue;
                    }
                    //软件名称
                    if (str.IndexOf("appName") != -1)
                    {
                        appName = str.Replace("appName=", "").Trim();
                    }

                    //软件版本
                    if (str.IndexOf("appVersion") != -1)
                    {
                        appVersion = str.Replace("appVersion=", "").Trim();
                    }
                    //软件下载地址
                    if (str.IndexOf("appsUrl") != -1)
                    {
                        appsUrl = str.Replace("appsUrl=", "").Trim();
                    }
                    //更新设置
                    if (str.IndexOf("autoUpdate") != -1)
                    {
                        autoUpdate = str.Replace("autoUpdate=", "").Trim();
                    }
                    //退出类型
                    if (str.IndexOf("quitType") != -1)
                    {
                        quitType = str.Replace("quitType=", "").Trim();
                    }
                    //nginx版本
                    if (str.IndexOf("nginxVersion") != -1)
                    {
                        nginxVersion = str.Replace("nginxVersion=", "").Trim();
                    }
                    //mysql版本
                    if (str.IndexOf("mysqlVersion") != -1)
                    {
                        mysqlVersion = str.Replace("mysqlVersion=", "").Trim();
                    }
                    //php版本
                    if (str.IndexOf("phpVersion") != -1)
                    {
                        phpVersion = str.Replace("phpVersion=", "").Trim();
                    }

                }
                sr.Close();
            }
            catch
            {
                MessageBox.Show("主程序配置读取失败！", "错误");
            }
            NginxRoot = "wnmp/nginx/" + nginxVersion + "/";
            MysqlRoot = "wnmp/mysql/" + mysqlVersion + "/";
            PHPRoot = "wnmp/php/" + phpVersion + "/";
        }

        public bool Save()
        {
            string path = Site.GetRootPath() + "/wnmp.ini";
            
            try
            {
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("#wnmp");
                sw.WriteLine("appName="+appName);
                sw.WriteLine("autoUpdate=" + autoUpdate);
                sw.WriteLine("quitType=" + quitType + "\n");
                sw.WriteLine("appVersion=" + appVersion + "\n");
                sw.WriteLine("appsUrl=" + appsUrl + "\n");
                sw.WriteLine("#nginx");
                sw.WriteLine("nginxVersion=" + nginxVersion+"\n");
                sw.WriteLine("#mysql");
                sw.WriteLine("mysqlVersion=" + mysqlVersion + "\n");
                sw.WriteLine("#php");
                sw.WriteLine("phpVersion=" + phpVersion);
                sw.Flush();
                sw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查版本
        /// </summary>
        /// <returns>是否需要更新</returns>
        public bool checkNewVersion(string v)
        {
            if (autoUpdate == "0" || appsUrl.Equals(""))
            {
                return false;
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(appsUrl);
            req.Method = "GET";
            try
            {
                using HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Load();
                StreamReader sr = new StreamReader(res.GetResponseStream());
                string jsonText = sr.ReadToEnd();
                sr.Close();
                LocalAppsResources jo = JsonConvert.DeserializeObject<LocalAppsResources>(jsonText);
                string version = jo.version.ToString();
                bool yes = Version.Parse(version) > Version.Parse(appVersion);
                if (yes)
                {
                    return true;
                }
            }
            catch
            {
                //MessageBox.Show("检查更新失败", "错误");
            }
            return false;

        }
    }
}
