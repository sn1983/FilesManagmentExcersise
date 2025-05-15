// A simple Windows Service installer app with WPF GUI
using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;

namespace ServiceInstallerApp
{
    /// <summary>
    /// Interaction logic for the application.
    /// </summary>
    public partial class App : Application { }

    /// <summary>
    /// Main window for the Windows Service Installer application.
    /// Allows the user to install or uninstall TX and RX services.
    /// </summary>
    public partial class MainWindow : Window
    {
        private ComboBox serviceComboBox;
        private Button installButton;
        private Button uninstallButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Sets up the UI and event handlers.
        /// </summary>
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

        /// <summary>
        /// Handles the Install button click event.
        /// Installs the selected service using InstallUtil.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            string exeDir;
            string extention = ".exe";
            GetServiceInstalltionPath(out exeDir);
            string servicePath = System.IO.Path.Combine(exeDir, serviceComboBox.SelectedItem.ToString()) + extention;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { servicePath });
                //string serviceName = GetServiceNameFromExecutable(servicePath);

                // Optionally start the service after installation
                //using (ServiceController sc = new ServiceController(serviceName))
                //{
                //    sc.Start();
                //    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                //}

                MessageBox.Show("Service installed successfully.");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    MessageBox.Show("Error: " + ex.InnerException.Message);
                else
                    MessageBox.Show("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the service name from the executable by searching for a ServiceBase subclass.
        /// </summary>
        /// <param name="serviceExePath">The path to the service executable.</param>
        /// <returns>The service name.</returns>
        //private string GetServiceNameFromExecutable(string serviceExePath)
        //{
        //    var asm = Assembly.LoadFrom(serviceExePath);

        //    foreach (var type in asm.GetTypes())
        //    {
        //        if (type.IsSubclassOf(typeof(ServiceBase)))
        //        {
        //            var serviceInstance = Activator.CreateInstance(type) as ServiceBase;
        //            return serviceInstance?.ServiceName ?? throw new Exception("ServiceName not found.");
        //        }
        //    }

        //    throw new Exception("No ServiceBase subclass found in assembly.");
        //}

        /// <summary>
        /// Determines the installation path for the selected service.
        /// </summary>
        /// <param name="exeDir">The output parameter for the executable directory.</param>
        private void GetServiceInstalltionPath(out string exeDir)
        {
            exeDir = AppDomain.CurrentDomain.BaseDirectory;
            switch (serviceComboBox.SelectedItem.ToString())
            {
                case "TX":
                    exeDir = System.IO.Path.Combine(exeDir, "TX_Installation");
                    break;
                case "RX":
                    exeDir = System.IO.Path.Combine(exeDir, "RX_installation");
                    break;
                default:
                    MessageBox.Show("Invalid service selected.");
                    return;
            }
        }

        /// <summary>
        /// Handles the Uninstall button click event.
        /// Uninstalls the selected service using InstallUtil.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
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
