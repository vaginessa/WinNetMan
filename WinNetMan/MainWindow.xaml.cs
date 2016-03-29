using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;

namespace WinNetMan
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel
        {
            get
            {
                return (MainWindowViewModel)DataContext;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void addProfileButton_Click(object sender, RoutedEventArgs e)
        {
            AddNetConf anc = new AddNetConf();
            anc.ShowDialog();
            if(anc.isValidated)
            {
                _viewModel.AddNetworkConfiguration(anc.finalConfig);
            }
        }

        private void removeProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NetworkConfiguration selectedNetConfig = (NetworkConfiguration)networkProfilesList.SelectedItem;
            _viewModel.NetworkConfigurations.Remove(selectedNetConfig);
            _viewModel.saveNetworkConfigurations();
        }

        private void setProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NetworkInterface selectedIface = (NetworkInterface)networkIfaceList.SelectedItem;
            NetworkConfiguration selectedNetConfig = (NetworkConfiguration)networkProfilesList.SelectedItem;
            if(selectedIface == null || selectedNetConfig == null)
                return;
            applyingGrid.Visibility = Visibility.Visible;

            new Thread(() => {
                if (selectedNetConfig.UseDHCP)
                {
                    if (!string.IsNullOrEmpty(selectedNetConfig.DNS1))
                    {
                        runNetshAddress($"interface ip set dnsservers \"{selectedIface.Name}\" static {selectedNetConfig.DNS1}");
                        if (!string.IsNullOrEmpty(selectedNetConfig.DNS2))
                        {
                            runNetshAddress($"interface ip set dnsservers \"{selectedIface.Name}\" static {selectedNetConfig.DNS2} 2");
                        }
                    }
                    else
                    {
                        runNetshAddress($"interface ip set dnsservers \"{selectedIface.Name}\" dhcp");
                    }
                    runNetshAddress($"interface ip set address \"{selectedIface.Name}\" dhcp");
                }
                else
                {
                    runNetshAddress($"interface ip set dnsservers \"{selectedIface.Name}\" static {selectedNetConfig.DNS1}");
                    if (!string.IsNullOrEmpty(selectedNetConfig.DNS2))
                    {
                        runNetshAddress($"interface ip set dnsservers \"{selectedIface.Name}\" static {selectedNetConfig.DNS2} 2");
                    }
                    runNetshAddress($"interface ip set address \"{selectedIface.Name}\" static {selectedNetConfig.IPAddress} {selectedNetConfig.Netmask} {selectedNetConfig.Gateway} 1");
                }
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    applyingGrid.Visibility = Visibility.Hidden;
                }));
            }).Start();

        }

        private void runNetshAddress(string toRun)
        {
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo("netsh", toRun);
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
        }
    }
}
