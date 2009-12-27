using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hotwire.App.ViewModels;
using Hotwire.Shared.Interfaces;
using SJR.Controls;

namespace Hotwire.App.Views
{
    enum CurrentStage
	{
        Startup,
        Login,
        RemoteControl,
        Done
	}

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : CustomWindow
    {
        private IAlertService _alertService;

        private MainViewModel _viewModel;

        private CurrentStage _currentStage = CurrentStage.Startup;

        private System.Threading.Timer _timeoutTimer;

        public MainView(MainViewModel viewModel, IAlertService alertService)
        {
            _alertService = alertService;
            _viewModel = viewModel;
            this.DataContext = viewModel;

            this.Loaded += MainView_Loaded;
            InitializeComponent();
        }

        void MainBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ProcessStage();
        }

        void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            MainBrowser.LoadCompleted += MainBrowser_LoadCompleted;
            _timeoutTimer = new System.Threading.Timer(Timeout, null, 30 * 1000, System.Threading.Timeout.Infinite);
            ProcessStage();
        }

        private void Timeout(object state)
        {
            _alertService.ShowAlert("There was a problem enabling remote control. Please check the browser display for more information", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            this.Dispatcher.Invoke(new Action(()=>FinishedProcessing()), null);
        }

        private void FinishedProcessing()
        {
            MainBrowser.LoadCompleted -= MainBrowser_LoadCompleted;
            MainBrowser.Visibility = Visibility.Visible;
            LoadingAnimation.Visibility = Visibility.Collapsed;
        }

        private void ProcessStage()
        {
            switch (_currentStage)
            {
                case CurrentStage.Startup:
                    var startupUri = new Uri(_viewModel.CurrentMachine.Url);
                    _currentStage = CurrentStage.Login;
                    MainBrowser.Navigate(startupUri);
                    break;
                case CurrentStage.Login:
                    try
                    {
                        var currentDocument = (mshtml.HTMLDocument)MainBrowser.Document;

                        currentDocument.getElementById("usr").setAttribute("value", _viewModel.CurrentMachine.Username, 0);
                        currentDocument.getElementById("pwd").setAttribute("value", _viewModel.CurrentMachine.Password, 0);

                        if (!String.IsNullOrEmpty(_viewModel.CurrentMachine.Password))
                        {
                            currentDocument.getElementById("loginButton").click();
                        }

                        _currentStage = CurrentStage.RemoteControl;
                    }
                    catch (Exception ex)
                    {
                        // Swallow any exceptions from browser
                    }
                    break;
                case CurrentStage.RemoteControl:
                    if (MainBrowser.Source.ToString().EndsWith("main.html"))
                    {
                        _currentStage = CurrentStage.Done;
                        MainBrowser.Navigate(new Uri(MainBrowser.Source.ToString().Replace("main.html", "remctrl.html")));
                    }
                    break;
                case CurrentStage.Done:
                    FinishedProcessing();
                    if (_timeoutTimer != null)
                        _timeoutTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    break;
                default:
                    break;
            }
        }
    }
}
