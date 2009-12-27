using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using Hotwire.Shared;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Hotwire.Shared.Interfaces;
using Microsoft.Practices.Unity;
using Hotwire.Launch.Views;

namespace Hotwire.Launch
{
    class TaskTray
    {
        private const string APP_NAME = "Hotwire.App.Exe";

        private List<Machine> config;

        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.NotifyIcon SysTrayIcon;
        private System.Windows.Forms.ContextMenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;

        private IConfigurationService _configurationService;
        private IAlertService _alertService;
        private IUnityContainer _container;

        public TaskTray(IConfigurationService configurationService, IAlertService alertService, IUnityContainer container)
        {
            this._configurationService = configurationService;
            this._alertService = alertService;
            this._container = container;

            InitializeComponent();

            HookEvents();

            if (!configurationService.ConfigurationAppearsValid(config))
            {
                if (_alertService.ShowAlert(@"Your configuration appears invalid, possibly due to changed hardware. Would you like to reconfigure?", @"Hotwire - Configuration Error", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    ShowConfig();
                else
                    this.SysTrayIcon.ShowBalloonTip(2 * 1000, "Hotwire", "Your configuration appears invalid, possibly due to changed hardware. Click here to reconfigure.", System.Windows.Forms.ToolTipIcon.Info);
            }

            if (config.Count == 0)
            {
                if (_alertService.ShowAlert(@"You currently have no machines defined, would you like to configure them now?", @"Hotwire - Not Configured", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    ShowConfig();
                else
                    this.SysTrayIcon.ShowBalloonTip(2 * 1000, "Hotwire", "You currently have no machines defined, click here to configure them.", System.Windows.Forms.ToolTipIcon.Info);
            }
            
        }

        public void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SysTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.MainMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenu.SuspendLayout();

            this.SysTrayIcon.ContextMenuStrip = this.MainMenu;
            this.SysTrayIcon.Icon = new System.Drawing.Icon(this.GetType(), "hotwire.ico");
            this.SysTrayIcon.Text = "Hotwire";
            this.SysTrayIcon.Visible = true;

            BuildMenu();

            this.MainMenu.ResumeLayout(false);
        }

        private void BuildMenu()
        {
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(114, 82);

            this.MainMenu.Items.Clear();

            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.MainMenu.Items.Add(aboutToolStripMenuItem);

            this.configToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.configToolStripMenuItem.Text = "&Configuration";
            this.MainMenu.Items.Add(configToolStripMenuItem);

            this.MainMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            // Build launch items
            this.config = _configurationService.LoadConfiguration().ToList();
            foreach (var machine in config)
            {
                var item = new System.Windows.Forms.ToolStripMenuItem()
                {
                    Name = String.Format("launch{0}", machine.Identifier),
                    Text = String.Format("Launch {0}", machine.MachineName),
                    Tag = machine.Identifier
                };
                item.Click += LaunchMachine;
                this.MainMenu.Items.Add(item);
            }

            this.MainMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.MainMenu.Items.Add(this.exitToolStripMenuItem);
        }

        void LaunchMachine(object sender, EventArgs e)
        {
            var item = sender as System.Windows.Forms.ToolStripMenuItem;

            if (item != null)
            {
                try
                {
                    Process.Start(APP_NAME, String.Format("\"{0}\"", item.Tag));
                }
                catch (Exception)
                {
                    _alertService.ShowAlert("Unable to execute main application, please check your installation.", "Hotwire - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void HookEvents()
        {
            this.exitToolStripMenuItem.Click += (s, e) => 
                {
                    if (this.SysTrayIcon != null)
                        this.SysTrayIcon.Visible = false;

                    Application.Current.Shutdown(); 
                };

            this.aboutToolStripMenuItem.Click += (s, e) =>
                {
                    ShowAbout();
                };

            this.configToolStripMenuItem.Click += (s, e) =>
                {
                    ShowConfig();
                };

            this.SysTrayIcon.MouseClick += (s, e) =>
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        typeof(System.Windows.Forms.NotifyIcon).GetMethod(@"ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this.SysTrayIcon, null);
                };

            this.SysTrayIcon.BalloonTipClicked += (s, e) =>
                {
                    ShowConfig();
                };
        }

        private void ShowAbout()
        {
            var aboutBox = _container.Resolve<AboutBoxView>();
            aboutBox.ShowDialog();
        }

        private void ShowConfig()
        {
            var configBox = _container.Resolve<ConfigurationView>();
            if (configBox.ShowDialog() == true)
                BuildMenu();
        }
    }
}
