using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

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
        private float perecnt;
        private bool installing;

        private bool wrong;

        public string Config;
        public string Type;

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

        public Visibility EditAble
        {
            get
            {
                if(installed)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }
        public Visibility InstallAble
        {
            get
            {
                if (!installed && !installing)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
            set
            {
                installed = value == Visibility.Hidden;
                OnPropertyChanged("InstallAble");
                OnPropertyChanged("EditAble");
            }
        }
        public bool Installing
        {
            get { return installing; }
        }
        public bool IsEnabled
        {
            get
            {
                return !installing;
            }
        }
        public Visibility InstallingAble
        {
            get
            {
                if (installing || wrong)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
            set
            {
                installing = value == Visibility.Visible;
                OnPropertyChanged("InstallingAble");
                OnPropertyChanged("Installing");
                OnPropertyChanged("IsEnabled");
            }
        }

        public float Percent
        {
            get
            {
                return perecnt;
            }
            set
            {
                perecnt = value;
                OnPropertyChanged("Percent");
            }
        }
        public Brush PercentColoer
        {
            get
            {
                return wrong? Brushes.OrangeRed : Brushes.Green;
            }
            
        }
        public bool Wrong
        {
            set
            {
                wrong = value;
                OnPropertyChanged("PercentColoer");
            }
        }
    }
}
