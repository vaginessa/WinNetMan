using System;

using System.Windows;

namespace WinNetMan
{

    public partial class AddNetConf : Window
    {
        public NetworkConfiguration finalConfig
        {
            get
            {
                return (NetworkConfiguration)DataContext;
            }
        }

        public bool isValidated { get; private set; }

        public AddNetConf()
        {
            DataContext = new NetworkConfiguration();
            isValidated = false;
            InitializeComponent();
        }

        private void validateButton_Click(object sender, RoutedEventArgs e)
        {
            isValidated = true;
            Close();
        }
    }
}
