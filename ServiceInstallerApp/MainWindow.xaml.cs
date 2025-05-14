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
            serviceComboBox.Items.Add("TX");
            serviceComboBox.Items.Add("RX");
           
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
            string exeDir;
            string extention = ".exe";
            GetServiceInstalltionPath(out exeDir);
            string servicePath = System.IO.Path.Combine(exeDir, serviceComboBox.SelectedItem.ToString()) + extention;
            //var servicePath = "C:\\Users\\sn198\\source\\repos\\FilesManagmentExcersise\\TX\\bin\\Release\\" + serviceComboBox.SelectedItem.ToString();
            try
            {
                 ManagedInstallerClass.InstallHelper(new string[] { servicePath });
                string serviceName = GetServiceNameFromExecutable(servicePath); // implement this

                // Start the service
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }

                MessageBox.Show("Service installed successfully.");
            }
            catch (Exception ex)
            {
                if(ex.InnerException!=null)
                    MessageBox.Show("Error: " + ex.InnerException.Message);
                else
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private string GetServiceNameFromExecutable(string serviceExePath)
        {
            var asm = Assembly.LoadFrom(serviceExePath);

            foreach (var type in asm.GetTypes())
            {
                if (type.IsSubclassOf(typeof(ServiceBase)))
                {
                    var serviceInstance = Activator.CreateInstance(type) as ServiceBase;
                    return serviceInstance?.ServiceName ?? throw new Exception("ServiceName not found.");
                }
            }

            throw new Exception("No ServiceBase subclass found in assembly.");
        }
        private void GetServiceInstalltionPath(out string exeDir)
        {
            
            exeDir = AppDomain.CurrentDomain.BaseDirectory;
            switch (serviceComboBox.SelectedItem.ToString())
            {

                case "TX":
                    exeDir = System.IO.Path.Combine(exeDir, "TX_Installation") ;
                    break;
                case "RX":
                    exeDir = System.IO.Path.Combine(exeDir, "RX_installation");
                    break;
                default:
                    MessageBox.Show("Invalid service selected.");
                    return;
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            string exeDir;
            string extention = ".exe";
            GetServiceInstalltionPath(out exeDir);
            string servicePath = System.IO.Path.Combine(exeDir, serviceComboBox.SelectedItem.ToString()) + extention;
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
