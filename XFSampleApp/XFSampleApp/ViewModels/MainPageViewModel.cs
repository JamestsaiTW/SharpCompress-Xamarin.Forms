using System;
using System.Collections.Generic;
using System.Text;

namespace XFSampleApp.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private bool _isNeedPassword;
        public bool IsNeedPassword
        {
            get { return _isNeedPassword; }
            set
            {
                if (_isNeedPassword != value)
                {
                    _isNeedPassword = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
