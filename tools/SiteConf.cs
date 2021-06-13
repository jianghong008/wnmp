using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace wnmp.tools
{
    /// <summary>
    /// 网站配置模型
    /// </summary>
    public class SiteConf
    {
        public string domainName = "";
        public string sitePort = "80";
        public string siteRoot = "c:\\www";
        public string siteFile = "index.html index.php";
        public bool staticStatus = true;
        public string staticRoot = "/";
        public bool gzip = true;
        public bool cross = false;
        public bool host = true;
        public bool IsNew = false;
        public string remake = "正常";
        /// <summary>
        /// 配置源码
        /// </summary>
        public string ConfText = "";

        /// <summary>
        /// 检查域名
        /// </summary>
        /// <param name="domainNameInput">域名</param>
        /// <returns>bool</returns>
        public bool checkDomain(string domainNameInput)
        {
            if (!Regex.IsMatch(domainNameInput, @"[\d\w]+\.[\w\d]+$"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
