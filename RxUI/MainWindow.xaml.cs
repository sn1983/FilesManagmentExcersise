using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassHelpers;
using JsonHelper;

namespace RxUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {

        RxSettings rxSettings;
        string rxSettingsPath;
        public MainWindow()
        {
            InitializeComponent();
            BtnLoad_Click(null, null);
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {

            rxSettingsPath = string.Empty;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["rxSettingsPath"]))
                rxSettingsPath = ConfigurationManager.AppSettings["rxSettingsPath"];
            rxSettings = JsonHelper.JsonHelper.LoadFromFile<RxSettings>(rxSettingsPath);
            if (rxSettings != null)
            {

                txtDestination.Text = rxSettings.DestinationFolder;
                chkSysLog.IsChecked = rxSettings.SysLog;
                txtSmtpServer.Text = rxSettings.SmtpServer;
                txtPort.Text = rxSettings.Port.ToString();
                chkUseSsl.IsChecked = rxSettings.UseSsl;
                txtSenderEmail.Text = rxSettings.SenderEmail;
                txtSenderName.Text = rxSettings.SenderName;
                txtUsername.Text = rxSettings.Username;
                txtPassword.Password = rxSettings.Password;
                txtLogFolder.Text = rxSettings.logFolder;
            }
            else
            {
                txtDestination.Text = string.Empty;
                chkSysLog.IsChecked = false;
                txtSmtpServer.Text = string.Empty;
                txtPort.Text = string.Empty;
                chkUseSsl.IsChecked = false;
                txtSenderEmail.Text = string.Empty;
                txtSenderName.Text = string.Empty;
                txtUsername.Text = string.Empty;
                txtPassword.Password = string.Empty;
                txtLogFolder.Text = string.Empty;
                JsonHelper.JsonHelper.SaveToFile<RxSettings>(rxSettingsPath, rxSettings);
            }
        }

        bool ValidateBeforeSave()
        {  // Basic validation
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(txtDestination.Text))
                errors.Add("Destination Folder is required.");

            if (string.IsNullOrWhiteSpace(txtSmtpServer.Text))
                errors.Add("SMTP Server is required.");

            if (!int.TryParse(txtPort.Text, out int port) || port < 1 || port > 65535)
                errors.Add("Port must be a valid number between 1 and 65535.");

            if (string.IsNullOrWhiteSpace(txtSenderEmail.Text))
                errors.Add("Sender Email is required.");
            else if (!txtSenderEmail.Text.Contains("@") || !txtSenderEmail.Text.Contains("."))
                errors.Add("Sender Email is not a valid email address.");

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
                errors.Add("Username is required.");

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
                errors.Add("Password is required.");
            if (errors.Any())
            {
                System.Windows.MessageBox.Show(string.Join(Environment.NewLine, errors), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateBeforeSave())
            {
                rxSettings.DestinationFolder = txtDestination.Text;
                rxSettings.SysLog = chkSysLog.IsChecked == true;
                rxSettings.SmtpServer = txtSmtpServer.Text;
                rxSettings.Port = int.TryParse(txtPort.Text, out int port) ? port : 587;
                rxSettings.UseSsl = chkUseSsl.IsChecked == true;
                rxSettings.SenderEmail = txtSenderEmail.Text;
                rxSettings.SenderName = txtSenderName.Text;
                rxSettings.Username = txtUsername.Text;
                rxSettings.Password = txtPassword.Password;
                rxSettings.logFolder = txtLogFolder.Text;

                JsonHelper.JsonHelper.SaveToFile<RxSettings>(rxSettingsPath, rxSettings);
                System.Windows.MessageBox.Show("Settings saved.");
            }
        }

        private void BtnBrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtDestination.Text = dialog.SelectedPath;
                }
            }
        }

        private void BtnBrowseLogFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtLogFolder.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
