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
    /// Interaction logic for MainWindow.xaml.
    /// Provides a UI for editing and saving TransferSettings, including validation and folder browsing.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The current TransferSettings instance being edited.
        /// </summary>
        TransferSettings transferSettings;

        /// <summary>
        /// The path to the TransferSettings JSON file.
        /// </summary>
        string TransferSettingsPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Loads settings on startup.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            BtnLoad_Click(null, null);
        }

        /// <summary>
        /// Loads TransferSettings from file and populates the UI fields.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
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
            else //log file was not found
            {
                txtSource.Text = string.Empty;
                txtDestination.Text = string.Empty;
                chkSysLog.IsChecked = true;
                JsonHelper.JsonHelper.SaveToFile<TransferSettings>(TransferSettingsPath, transferSettings);
            }
        }

        /// <summary>
        /// Handles the Browse button click event for the source folder.
        /// Opens a folder browser dialog and sets the selected path.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Handles the Browse button click event for the destination folder.
        /// Opens a folder browser dialog and sets the selected path.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Handles the Save button click event. Validates and saves the settings to file.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
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

