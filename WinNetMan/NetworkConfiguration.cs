using System;

namespace WinNetMan
{
    [Serializable()]
    public class NetworkConfiguration
    {
        public bool UseDHCP { get; set; }
        public string IPAddress { get; set; }
        public string Netmask { get; set; }
        public string Gateway { get; set; }
        public string DNS1 { get; set; }
        public string DNS2 { get; set; }
        public NetworkConfiguration() { }
        public string IPConfigSummary
        {
            get
            {
                if (UseDHCP)
                {
                    return "Use DHCP";
                }
                else
                {
                    return $"IP : {IPAddress}/{Netmask}, Gateway : {Gateway}";
                }
            }
        }
        public string DNSSummary
        {
            get
            {
                if (UseDHCP && string.IsNullOrEmpty(DNS1))
                {
                    return "DNS will use DHCP";
                }
                else
                {
                    return $"Primary DNS : {DNS1}, Secondary DNS : {DNS2}";
                }
            }
        }
    }
}
