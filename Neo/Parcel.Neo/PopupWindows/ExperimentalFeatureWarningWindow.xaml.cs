using Parcel.NExT.Interpreter;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Parcel.Neo.PopupWindows
{
    /// <summary>
    /// A window specifically for CodeGen related warnings and provides development instructions
    /// </summary>
    public partial class ExperimentalFeatureWarningWindow : Window
    {
        #region Construction
        public ExperimentalFeatureWarningWindow()
        {
            InitializeComponent();

            // Show notification
            if (!CheckEnvironmentVariablesSet())
            {
                EnvPathNotSetNotification.Visibility = Visibility.Visible;
                AllGoodNotification.Visibility = Visibility.Collapsed;
            }
            PythonModuleFolderRun.Text = PythonRootModuleFolderPath;
            ParcelPackageAssembliesFolderRun.Text = ParcelNExTStandardPackagesPath;
        }
        string PythonRootModuleFolderPath = AssemblyHelper.ParcelNExTDistributionRuntimeDirectory;
        string ParcelNExTStandardPackagesPath = AssemblyHelper.ParcelNExTDistributionRuntimeDirectory;
        private bool CheckEnvironmentVariablesSet()
        {
            if (Environment.GetEnvironmentVariable("PYTHONPATH") == null || Environment.GetEnvironmentVariable("PATH") == null)
                return false;

            string[] currentPYTHONPATHEnvVariable = Environment.GetEnvironmentVariable("PYTHONPATH")?.Split(';') ?? [];
            string[] currentPATHEnvVariable = Environment.GetEnvironmentVariable("PATH")?.Split(';') ?? [];
            if (currentPYTHONPATHEnvVariable.Contains(PythonRootModuleFolderPath) || !currentPATHEnvVariable.Contains(ParcelNExTStandardPackagesPath))
                return false;

            return true;
        }
        #endregion

        #region Events
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            });
            e.Handled = true;
        }
        private void AutomaticSetEnvironmentPathsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Provide cross-platform implementation
            // Change paths
            // Notice we can only set for current process
            if (Environment.GetEnvironmentVariable("PYTHONPATH") == null)
                Environment.SetEnvironmentVariable("PYTHONPATH", PythonRootModuleFolderPath, EnvironmentVariableTarget.Process);
            if (Environment.GetEnvironmentVariable("PATH") == null)
                Environment.SetEnvironmentVariable("PATH", ParcelNExTStandardPackagesPath, EnvironmentVariableTarget.Process); 

            string[] currentPYTHONPATHEnvVariable = Environment.GetEnvironmentVariable("PYTHONPATH")?.Split(';') ?? [];
            string[] currentPATHEnvVariable = Environment.GetEnvironmentVariable("PATH")?.Split(';') ?? [];
            if (currentPYTHONPATHEnvVariable.Contains(PythonRootModuleFolderPath))
                Environment.SetEnvironmentVariable("PYTHONPATH", $"{Environment.GetEnvironmentVariable("PYTHONPATH")};{PythonRootModuleFolderPath}", EnvironmentVariableTarget.Process);
            if (currentPYTHONPATHEnvVariable.Contains(PythonRootModuleFolderPath) || !currentPATHEnvVariable.Contains(ParcelNExTStandardPackagesPath))
                Environment.SetEnvironmentVariable("PATH", $"{Environment.GetEnvironmentVariable("PATH")};{ParcelNExTStandardPackagesPath}", EnvironmentVariableTarget.Process);

            // Update notification
            EnvPathNotSetNotification.Visibility = Visibility.Collapsed;
            AllGoodNotification.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
