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
namespace TransferSettingsUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        TransferSettings transferSettings;
        string TransferSettingsPath;
        public MainWindow()
        {
            InitializeComponent();
            BtnLoad_Click(null, null);
        }
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            // string TransferSettingsPath = "settings.json";
            TransferSettingsPath = string.Empty;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TransferSettingsPath"]))
                TransferSettingsPath = ConfigurationManager.AppSettings["TransferSettingsPath"];

            transferSettings = JsonHelper.JsonHelper.LoadFromFile<TransferSettings>(TransferSettingsPath);

            if (transferSettings != null)
            {

                txtSource.Text = transferSettings.SourceFolder;
                txtDestination.Text = transferSettings.DestinationFolder;
                chkSysLog.IsChecked = transferSettings.SysLog;
            }
            else
            {
                txtSource.Text = string.Empty;
                txtDestination.Text = string.Empty;
                chkSysLog.IsChecked = true;
                JsonHelper.JsonHelper.SaveToFile<TransferSettings>(TransferSettingsPath, transferSettings);
            }
        }
        private void BtnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtSource.Text = dialog.SelectedPath;
                }
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
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (string.IsNullOrEmpty(txtSource.Text) || string.IsNullOrEmpty(txtDestination.Text))
                {
                    System.Windows.MessageBox.Show("Please select both source and destination folders.");
                    return;
                }
                transferSettings.SourceFolder = txtSource.Text;
                transferSettings.DestinationFolder = txtDestination.Text;
                transferSettings.SysLog = chkSysLog.IsChecked == true;
                JsonHelper.JsonHelper.SaveToFile<TransferSettings>(TransferSettingsPath, transferSettings);
                System.Windows.MessageBox.Show("Settings saved.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving settings: {ex.Message}");
            }
        }
    }
}

