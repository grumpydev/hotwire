using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotwire.Shared.Interfaces;
using System.Windows;

namespace Hotwire.Shared.Services
{
    public class AlertService : IAlertService
    {

        #region IAlertService Members

        public System.Windows.MessageBoxResult ShowAlert(string messageText, string title, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon)
        {
            return MessageBox.Show(messageText, title, button, icon);
        }

        #endregion
    }
}
