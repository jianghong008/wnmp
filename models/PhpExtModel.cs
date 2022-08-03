using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace wnmp.models
{
    /// <summary>
    /// php扩展数据模型
    /// </summary>
    internal class PhpExtModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string ext_name;
        private bool open;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public string ExtName
        {
            get
            {
                return ext_name;
            }
            set
            {
                ext_name = value;
                OnPropertyChanged("ExtName");
            }
        }
        public bool Open
        {
            get
            {
                return open;
            }
            set
            {
                open = value;
                OnPropertyChanged("Open");
            }
        }
    }
}
