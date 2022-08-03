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
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
