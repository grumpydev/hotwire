using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Hotwire.Shared.Interfaces
{
    public interface IAlertService
    {
        MessageBoxResult ShowAlert(string messageText, string title, MessageBoxButton button, MessageBoxImage icon);
    }
}
