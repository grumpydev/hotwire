using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Hotwire.Shared;
using System.Collections.ObjectModel;
using Hotwire.Shared.Interfaces;
using System.Windows.Input;

namespace Hotwire.Launch.ViewModels
{
    public class ConfigurationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            var evt = PropertyChanged;

            if (evt != null)
                evt.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Machine> _Machines;
        public ObservableCollection<Machine> Machines
        {
            get
            {
                return _Machines;
            }
            private set
            {
                _Machines = value;
                RaisePropertyChanged("Machines");
            }
        }
        private Machine _SelectedMachine;
        public Machine SelectedMachine
        {
            get
            {
                return _SelectedMachine;
            }
            set
            {
                _SelectedMachine = value;
                RaisePropertyChanged("SelectedMachine");
            }
        }

        private ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _SaveCommand;
            }
            private set
            {
                _SaveCommand = value;
                RaisePropertyChanged("SaveCommand");
            }
        }

        private ICommand _CancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _CancelCommand;
            }
            private set
            {
                _CancelCommand = value;
                RaisePropertyChanged("CancelCommand");
            }
        }

        private ICommand _AddCommand;
        public ICommand AddCommand
        {
            get
            {
                return _AddCommand;
            }
            private set
            {
                _AddCommand = value;
                RaisePropertyChanged("AddCommand");
            }
        }

        private ICommand _DeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                return _DeleteCommand;
            }
            private set
            {
                _DeleteCommand = value;
                RaisePropertyChanged("DeleteCommand");
            }
        }

        public Action<bool?> Close { get; set; }

        private IConfigurationService _configurationService;
        private IAlertService _alertService;

        /// <summary>
        /// Initializes a new instance of the ConfigurationViewModel class.
        /// </summary>
        public ConfigurationViewModel(IConfigurationService configurationService, IAlertService alertService)
        {
            _configurationService = configurationService;
            _alertService = alertService;

            Machines = new ObservableCollection<Machine>(_configurationService.LoadConfiguration());
            SelectedMachine = Machines.FirstOrDefault();

            SaveCommand = new SimpleCommand((o) =>
                {
                    _configurationService.SaveConfiguration(Machines);
                    if (Close != null)
                        Close.Invoke(true);
                });

            CancelCommand = new SimpleCommand((o) =>
                {
                    if (Close != null)
                        Close.Invoke(false);
                });

            AddCommand = new SimpleCommand(AddMachine);

            DeleteCommand = new SimpleCommand(DeleteMachine);
        }

        private void AddMachine(object obj)
        {
            string machineUrl = obj as string;

            if (Uri.IsWellFormedUriString(machineUrl, UriKind.Absolute))
            {
                Machine newMachine = new Machine() { MachineName = "New Machine", Url = machineUrl };
                Machines.Add(newMachine);
                SelectedMachine = newMachine;
            }
            else
            {
                _alertService.ShowAlert("Machine link appears to be malformed.", "Unable To Add", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void DeleteMachine(object obj)
        {
            var machine = obj as Machine;

            if (obj == null)
                return;

            if (_alertService.ShowAlert(String.Format("Are you sure you want to delete '{0}'?", machine.MachineName), "Confirm Deletion", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
            {
                Machines.Remove(machine);
                SelectedMachine = Machines.FirstOrDefault();
            }
        }

    }
}
