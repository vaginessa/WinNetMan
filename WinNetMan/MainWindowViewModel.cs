using System;

using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;

namespace WinNetMan
{
    class MainWindowViewModel
    {
        static XmlSerializer _serializer = new XmlSerializer(typeof(List<NetworkConfiguration>));

        public ObservableCollection<NetworkConfiguration> NetworkConfigurations { get; private set; }
        public List<NetworkInterface> NetworkInterfaces { get; private set; }

        public MainWindowViewModel()
        {
            loadNetworkConfigurations();
            loadNetworkAdapters();
        }

        public void loadNetworkAdapters()
        {
            NetworkInterfaces = new List<NetworkInterface>(NetworkInterface.GetAllNetworkInterfaces());
        }

        private void loadNetworkConfigurations()
        {
            try
            {
                string savedNetConfs = Properties.Settings.Default.NetworkConfigurations;
                using (var reader = new StringReader(savedNetConfs))
                {
                    NetworkConfigurations = 
                        new ObservableCollection<NetworkConfiguration>(
                            (List<NetworkConfiguration>)_serializer.Deserialize(reader)
                        );
                }
            }
            catch
            {
                NetworkConfigurations = new ObservableCollection<NetworkConfiguration>();
            }
        }

        public void saveNetworkConfigurations()
        {
            List<NetworkConfiguration> toSerialize = NetworkConfigurations.ToList();
            using (StringWriter textWriter = new StringWriter())
            {
                _serializer.Serialize(textWriter, toSerialize);
                Properties.Settings.Default.NetworkConfigurations = textWriter.ToString();
                Properties.Settings.Default.Save();
            }
        }

        public void AddNetworkConfiguration(NetworkConfiguration netConf)
        {
            NetworkConfigurations.Add(netConf);
            saveNetworkConfigurations();
        }

    }
}
