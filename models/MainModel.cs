using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace wnmp.models
{
    /// <summary>
    /// 主界面数据模型
    /// </summary>
    internal class MainModel : INotifyPropertyChanged
    {
        private string[] appIconBg = { "#FFC80000", "#FF0AA719" };
        private string[] appBtn = { "启 动" ,"停 止"};

        private bool _NginxStopping = true;
        private bool _MysqlStopping = true;
        private bool _PHPStopping = true;

        public bool AppQuit = false;
        public long LastClickTime;//防重复点击
        public int CheckTime = 5 * 1000;//监控间隔时间

        public bool _NginxUserRunning = false;//用户启动Nginx
        public bool _MysqlUserRunning = false;//用户启动mysql
        public bool _PhpUserRunning = false;//用户启动php

        public string Name = "";

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// 主界面---s
        /// </summary>
        public bool NginxStopping
        {
            get
            {
                return _NginxStopping;
            }
            set
            {
                _NginxStopping = value;
                OnPropertyChanged("NginxIconBg");
                OnPropertyChanged("NginxBtn");

            }
        }
        public bool MysqlStopping
        {
            get
            {
                return _MysqlStopping;
            }
            set
            {
                _MysqlStopping = value;
                OnPropertyChanged("MysqlIconBg");
                OnPropertyChanged("MysqlBtn");

            }
        }
        public bool PHPStopping
        {
            get
            {
                return _PHPStopping;
            }
            set
            {
                _PHPStopping = value;
                OnPropertyChanged("PhpIconBg");
                OnPropertyChanged("PhpBtn");

            }
        }
        public string NginxIconBg
        {
            get
            {
                if (NginxStopping)
                {
                    return appIconBg[0];
                }
                else
                {
                    return appIconBg[1];
                }
            }
        }
        public string NginxBtn
        {
            get
            {
                if (NginxStopping)
                {
                    return appBtn[0];
                }
                else
                {
                    return appBtn[1];
                }
            }
            /*
            set
            {
                _NginxStopping = value;
                OnPropertyChanged("NginxStopping");
            }
            */
        }
        ///mysql
        public string MysqlIconBg
        {
            get
            {
                if (MysqlStopping)
                {
                    return appIconBg[0];
                }
                else
                {
                    return appIconBg[1];
                }
            }
        }
        public string MysqlBtn
        {
            get
            {
                if (MysqlStopping)
                {
                    return appBtn[0];
                }
                else
                {
                    return appBtn[1];
                }
            }
        }
        ///php
        public string PhpIconBg
        {
            get
            {
                if (_PHPStopping)
                {
                    return appIconBg[0];
                }
                else
                {
                    return appIconBg[1];
                }
            }
        }
        public string PhpBtn
        {
            get
            {
                if (_PHPStopping)
                {
                    return appBtn[0];
                }
                else
                {
                    return appBtn[1];
                }
            }
        }
    }
}
