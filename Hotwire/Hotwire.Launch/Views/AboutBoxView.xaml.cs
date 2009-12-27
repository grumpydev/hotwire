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
using SJR.Controls;
using System.Reflection;

namespace Hotwire.Launch.Views
{
    /// <summary>
    /// Interaction logic for AboutBoxView.xaml
    /// </summary>
    public partial class AboutBoxView : CustomWindow
    {
        public AboutBoxView()
        {
            InitializeComponent();

            VersionText.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
