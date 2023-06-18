using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using wnmp.models;

namespace wnmp.tools
{
    internal class PhpINI
    {
        public ObservableCollection<PhpExtModel> extensions = new ObservableCollection<PhpExtModel>();
        public bool LoadState=true;
        private AppConf appConf;
        public PhpINI(AppConf conf)
        {
            appConf = conf;
            load();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        public void load()
        {
            string path = appConf.PHPRoot+"php.ini";
            if (!File.Exists(path))
            {
                LoadState  = false;
                return;
            }
            FileStream fs = new FileStream(path,FileMode.Open);
            if (!fs.CanRead)
            {
                LoadState = false;
                return;
            }

            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                Match m = Regex.Match(line, @"^;?extension=\w+");
                if (!m.Success)
                {
                    continue;
                }
                line = line.Trim();
                if (line == "")
                {
                    continue;
                }
                PhpExtModel mod = new PhpExtModel();
                
                //开启的扩展
                if (line[0] != ';')
                {
                    string etx = line.Replace("extension=", "");
                    mod.ExtName = etx;
                    mod.Open = true;
                    
                }
                else
                {
                    string etx = line.Replace(";extension=", "");
                    mod.ExtName = etx;
                    mod.Open = false;
                }
                //去掉注释
                string[] tmp = mod.ExtName.Split(';');
                mod.ExtName = tmp[0].Trim();
                extensions.Add(mod);
            }
            
            sr.Close();
            fs.Close();
            
        }

        public void Save()
        {
            string path = appConf.PHPRoot + "php.ini";
            if (!File.Exists(path))
            {
                LoadState = false;
                return;
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            if (!fs.CanRead)
            {
                LoadState = false;
                return;
            }

            StreamReader sr = new StreamReader(fs);
            string[] lines = new string[10000];
            int index = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                lines[index] = line;
                
                Match m = Regex.Match(line, @"^;?extension=\w+");
                if (m.Success)
                {
                    string etx = line.Replace(";extension=", "");
                    if (line[0] != ';')
                    {
                        etx = line.Replace("extension=", "");

                    }
                    foreach (PhpExtModel mod in extensions)
                    {
                        if (mod.ExtName == etx)
                        {
                            if (mod.Open)
                            {
                                lines[index] = "extension=" + etx;
                            }
                            else
                            {
                                lines[index] = ";extension=" + etx;
                            }
                        }
                    }
                }
                index++;

            }
            fs.Close();
            sr.Close();
            
            StreamWriter sw = new StreamWriter(path);
            
            for(int i = 0; i < lines.Length; i++)
            {
                if (i <= index)
                {
                    sw.WriteLine(lines[i]);
                }
            }
            
            sw.Flush();
            sw.Close();
            
        }
    }
}
