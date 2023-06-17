using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace wnmp.models
{
    /// <summary>
    /// 应用数据模型
    /// </summary>
    internal class ApplicationModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string title;
        private bool installed;
        private bool is_use;
        private string path;

        private string url;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
                OnPropertyChanged("Url");
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                OnPropertyChanged("Path");
            }
        }
        public bool Installed
        {
            get
            {
                return installed;
            }
            set
            {
                installed = value;
                OnPropertyChanged("Installed");
            }
        }
        public bool IsUse
        {
            get
            {
                return is_use;
            }
            set
            {
                is_use = value;
                OnPropertyChanged("IsUse");
            }
        }
    }
}
