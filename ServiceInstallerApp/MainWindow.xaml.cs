// A simple Windows Service installer app with WPF GUI
using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;

namespace ServiceInstallerApp
{
    public partial class App : Application { }

    public partial class MainWindow : Window
    {
        private ComboBox serviceComboBox;
        private Button installButton;
        private Button uninstallButton;

        public MainWindow()
        {
            InitializeComponent();
            Title = "Windows Service Installer";
            Width = 400;
            Height = 200;

            var panel = new StackPanel { Margin = new Thickness(10) };

            serviceComboBox = new ComboBox { Margin = new Thickness(0, 0, 0, 10) };
            serviceComboBox.Items.Add("TX.exe");
            serviceComboBox.Items.Add("RX.exe");
           
            serviceComboBox.SelectedIndex = 0;

            installButton = new Button { Content = "Install", Margin = new Thickness(0, 0, 0, 5) };
            installButton.Click += InstallButton_Click;

            uninstallButton = new Button { Content = "Uninstall" };
            uninstallButton.Click += UninstallButton_Click;

            panel.Children.Add(serviceComboBox);
            panel.Children.Add(installButton);
            panel.Children.Add(uninstallButton);

            Content = panel;
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            var servicePath = serviceComboBox.SelectedItem.ToString();
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { servicePath });
                MessageBox.Show("Service installed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            var servicePath = serviceComboBox.SelectedItem.ToString();
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", servicePath });
                MessageBox.Show("Service uninstalled successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }

    
}
