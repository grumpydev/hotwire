using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Hotwire.Shared;
using Hotwire.Shared.Interfaces;
using Microsoft.Practices.Unity;

namespace Hotwire.App.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            var evt = PropertyChanged;

            if (evt != null)
                evt.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Machine _CurrentMachine;
        public Machine CurrentMachine
        {
            get
            {
                return _CurrentMachine;
            }
            set
            {
                _CurrentMachine = value;
                RaisePropertyChanged("CurrentMachine");
            }
        }

        private IAlertService _alertService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <param name="configurationService"></param>
        /// <param name="container"></param>
        /// <param name="alertService"></param>
        public MainViewModel(IAlertService alertService, Machine currentMachine)
        {
            _alertService = alertService;
            CurrentMachine = currentMachine;
        }
    }
}
