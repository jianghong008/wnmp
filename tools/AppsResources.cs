using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace wnmp.tools
{
    public class AppsResourcesItem
    {
        public string url;
        public string title;
        public bool installed;
    }
    public class LocalAppsResources
    {
        public string version = "1.0.0";
        public string url = "";
        public List<AppsResourcesItem> php = new List<AppsResourcesItem>();
        public List<AppsResourcesItem> mysql = new List<AppsResourcesItem>();
        public List<AppsResourcesItem> nginx = new List<AppsResourcesItem>();
    }
    /// <summary>
    /// 软件资源
    /// </summary>
    internal class AppsResources
    {
        public string version = "1.0.0";
        public string url = "";
        public string[] php = new string[] { };
        public string[] mysql = new string[] { };
        public string[] nginx = new string[] { };
    }
}
