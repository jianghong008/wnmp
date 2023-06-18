using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using wnmp.models;

namespace wnmp.tools
{
    /// <summary>
    /// 网络工具
    /// </summary>
    internal class WebSpider
    {
        /// <summary>
        /// 软件下载地址
        /// </summary>
        public static string AppsUrl;
        public static AppsResources srcResources;
        public static string ResourceJson = "";

        public static LocalAppsResources resources = new LocalAppsResources();

        public static bool Exit;

        public static void GetAppsVersions(Tools tool)
        {
            Exit = false;
            try
            {
                if (ResourceJson.Equals(""))
                {
                    ResourceJson = DownloadToString(AppsUrl);
                }
                srcResources = JsonConvert.DeserializeObject<AppsResources>(ResourceJson);
                GetLocalResources();
            }
            catch (Exception ex)
            {
                srcResources = new AppsResources();
                Trace.WriteLine(ex.ToString());
                MessageBox.Show("获取网络资源失败！请检查配置");
            }
            // 与本地合并
            // php
            foreach (var version in tool.phpVersions)
            {
                bool installed = false;
                foreach (var item in resources.php)
                {
                    if (version.Equals(item.title))
                    {
                        item.installed = true;
                        installed = true;
                        
                    }
                }
                if (!installed)
                {
                    resources.php.Insert(0,new AppsResourcesItem
                    {
                        title = version,
                        installed = true,
                        url = ""
                    });
                }
            }
            //mysql
            foreach (var version in tool.mysqlVersions)
            {
                bool installed = false;
                foreach (var item in resources.mysql)
                {
                    if (version.Equals(item.title))
                    {
                        item.installed = true;
                        installed = true;
                    }
                }
                if (!installed)
                {
                    resources.mysql.Insert(0,new AppsResourcesItem
                    {
                        title = version,
                        installed = true,
                        url = ""
                    });
                }
            }
            //nginx
            foreach (var version in tool.nginxVersions)
            {
                bool installed = false;
                foreach (var item in resources.nginx)
                {
                    if (version.Equals(item.title))
                    {
                        item.installed = true;
                        installed = true;
                    }
                }
                if (!installed)
                {
                    resources.nginx.Insert(0,new AppsResourcesItem
                    {
                        title = version,
                        installed = true,
                        url = ""
                    });
                }
            }

        }
        /// <summary>
        /// 转化资源
        /// </summary>
        private static void GetLocalResources()
        {
            resources.php.Clear();
            foreach (var item in srcResources.php)
            {
                string[] temp = item.Split("/");
                string title = temp[^1].Replace(".zip", "");
                resources.php.Add(new AppsResourcesItem
                {
                    installed = false,
                    title = title,
                    url = item
                });
            }
            resources.mysql.Clear();
            foreach (var item in srcResources.mysql)
            {
                string[] temp = item.Split("/");
                string title = temp[^1].Replace(".zip", "");
                resources.mysql.Add(new AppsResourcesItem
                {
                    installed = false,
                    title = title,
                    url = item
                });
            }
            resources.nginx.Clear();
            foreach (var item in srcResources.nginx)
            {
                string[] temp = item.Split("/");
                string title = temp[^1].Replace(".zip","");
                resources.nginx.Add(new AppsResourcesItem
                {
                    installed = false,
                    title = title,
                    url = item
                });
            }
        }
        /// <summary>
        /// 下载文本文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string DownloadToString(string url,int timeout=10000)
        {
            HttpWebRequest req = HttpWebRequest.CreateHttp(url);
            req.Method = "Get";
            req.Timeout = timeout;
            WebResponse res = req.GetResponse();
            Stream s = res.GetResponseStream();
            StreamReader readStream = new StreamReader(s, Encoding.UTF8);

            string txt = readStream.ReadToEnd();
            readStream.Close();
            s.Close();
            return txt;
        }
        /// <summary>
        /// 下载zip包并解压
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        public static void DownloadAndExtract(string url,string savePath,Action<long,float> action)
        {
            HttpWebRequest req = HttpWebRequest.CreateHttp(url);
            req.Method = "Get";
            //req.Timeout = timeout;
            MD5 md5 = MD5.Create();
            Regex regex = new Regex(@"[^\w\d]");
            
            string temp = Convert.ToBase64String(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(url))).ToLower();
            temp = regex.Replace(temp,"");

            WebResponse res = req.GetResponse();
            Stream s = res.GetResponseStream();
            byte[] buffer = new byte[1024];
            int size = s.Read(buffer,0,buffer.Length);
            long totalSize = res.ContentLength, downloadSzie = 0;
            // 先缓存本地
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            
            string tempPath = savePath + "/" + temp + ".zip";
            FileStream sw = new FileStream(tempPath, FileMode.Create,FileAccess.Write);
            while (size > 0)
            {
                if (Exit)
                {
                    //停止
                    sw.Flush();
                    sw.Close();
                    s.Close();
                    res.Close();
                    res.Dispose();
                    File.Delete(tempPath);
                    return;
                }
                sw.Write(buffer, 0, size);
                downloadSzie += size;
                float percent = (float)downloadSzie / (float)totalSize * 100 - 6;
                if (percent <= 1)
                {
                    percent = 1;
                }
                
                Trace.WriteLine("write:"+(percent));
                size = s.Read(buffer, 0, buffer.Length);
                action(totalSize, percent);
            }
            sw.Flush();
            sw.Close();
            s.Close();
            res.Close();
            res.Dispose();

            if (ZipFile.IsZipFile(tempPath))
            {

                ZipFile zip = ZipFile.Read(tempPath);
                zip.ExtractAll(savePath,ExtractExistingFileAction.OverwriteSilently);
                //完成
                action(totalSize, 100);
                zip.Dispose();
                File.Delete(tempPath);
                
            }
            else
            {
                
                File.Delete(tempPath);
                //失败
                Trace.WriteLine("文件不是zip");
                throw new Exception("资源解压失败，不是zip文件");
            }
            
        }
    }
}
