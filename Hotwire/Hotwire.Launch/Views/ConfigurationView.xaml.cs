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
using Hotwire.Launch.ViewModels;
using SJR.Controls;

namespace Hotwire.Launch.Views
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : CustomWindow
    {
        ConfigurationViewModel _viewModel;

        public ConfigurationView(ConfigurationViewModel viewModel)
        {
            this.DataContext = viewModel;
            _viewModel = viewModel;

            viewModel.Close = CloseWindow;

            InitializeComponent();

            DropCanvas.Drop += DropCanvas_Drop;
        }

        void DropCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.Text))
            {
                _viewModel.AddCommand.Execute(e.Data.GetData(System.Windows.DataFormats.Text, true));
            }
        }

        private void CloseWindow(bool? result)
        {
            this.DialogResult = result;
            this.Close();
        }
    }
}
