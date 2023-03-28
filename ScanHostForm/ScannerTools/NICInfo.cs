using Microsoft.Management.Infrastructure;
using Microsoft.VisualBasic;
using System;
//using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ScanHostLib
{
    public class NICInfo
    {
        private string _Name; // = $($_.Description)
        private bool _DHCPEnabled; //= $_.DHCPEnabled
        private DateTime _DHCPLeaseExpires; // = $_.DHCPLeaseExpires
        private DateTime _DHCPLeaseObtained; // = $_.DHCPLeaseObtained
        private string _DHCPServer; // = $_.DHCPServer
        private string _DNSDomain; // = $_.DNSDomain
        private string _DNSHostName; // = $_.DNSHostName
        private bool _FullDNSRegistrationEnabled; // = $_.FullDNSRegistrationEnabled
        private string _IPAddress; // = $_.IPAddress
        private bool _IPEnabled; // = $_.IPEnabled
        private string _DefaultIPGateway; // = $_.DefaultIPGateway
        private string _IPSubnet; // = $_.IPSubnet
        private string _MACAddress; // = $_.MACAddress
        private string _ServiceName; // = $_.ServiceName
        private string _Speed; // = [String](($NetworkAdapter.Speed / (1000 * 1000 * 1000))) + " GB"
        private string _AdapterType; // = $NetworkAdapter.AdapterType
        private string _UID; // = $NetworkAdapter.GUID
        private string _Manufacturer; // = $NetworkAdapter.Manufacturer
        private string _NetConnectionID; // = $NetworkAdapter.NetConnectionID
        private string _NetEnabled; // = $NetworkAdapter.NetEnabled
        private string _ProductName; // = $NetworkAdapter.ProductName
        private DateTime _TimeOfLastReset; // = $NetworkAdapter.TimeOfLastReset

        public NICInfo() { }

        public string Name { get => this._Name; set => this._Name = value; }
        public bool DHCPEnabled { get => this._DHCPEnabled; set => this._DHCPEnabled = value; }
        public DateTime DHCPLeaseExpires { get => this._DHCPLeaseExpires; set => this._DHCPLeaseExpires = value; }
        public DateTime DHCPLeaseObtained { get => this._DHCPLeaseObtained; set => this._DHCPLeaseObtained = value; }
        public string DHCPServer { get => this._DHCPServer; set => this._DHCPServer = value; }
        public string DNSDomain { get => this._DNSDomain; set => this._DNSDomain = value; }
        public string DNSHostName { get => this._DNSHostName; set => this._DNSHostName = value; }
        public bool FullDNSRegistrationEnabled { get => this._FullDNSRegistrationEnabled; set => this._FullDNSRegistrationEnabled = value; }
        public string IPAddress { get => this._IPAddress; set => this._IPAddress = value; }
        public string[] IPAddressObject {
            //get => new string[] { this._IPAddress };
            set
            {
                this._IPAddress = value[0];
            }
        }
        public bool IPEnabled { get => this._IPEnabled; set => this._IPEnabled = value; }
        public string DefaultIPGateway { get => this._DefaultIPGateway; set => this._DefaultIPGateway = value; }
        public string IPSubnet { get => this._IPSubnet; set => this._IPSubnet = value; }
        public string MACAddress { get => this._MACAddress; set => this._MACAddress = value; }
        public string ServiceName { get => this._ServiceName; set => this._ServiceName = value; }
        public string Speed { get => this._Speed; set => this._Speed = value; }
        public string AdapterType { get => this._AdapterType; set => this._AdapterType = value; }
        public string UID { get => this._UID; set => this._UID = value; }
        public string Manufacturer { get => this._Manufacturer; set => this._Manufacturer = value; }
        public string NetConnectionID { get => this._NetConnectionID; set => this._NetConnectionID = value; }
        public string NetEnabled { get => this._NetEnabled; set => this._NetEnabled = value; }
        public string ProductName { get => this._ProductName; set => this._ProductName = value; }
        public DateTime TimeOfLastReset { get => this._TimeOfLastReset; set => this._TimeOfLastReset = value; }

        public override string ToString()
        {
            return $"Name: {_Name}\r\n" +
                   $"IP: {_IPAddress}\r\n" +
                   $"MAC: {_MACAddress}\r\n" +
                   $"DHCP: {_DHCPEnabled}\r\n" +
                   $"Lease Expires: {_DHCPLeaseExpires}\r\n" +
                   $"DHCP Server: {_DHCPServer}\r\n" +
                   $"Domain: {_DNSDomain}\r\n" +
                   $"Speed: {_Speed},\r\n";
        }
    }

    public static class NICInfoHelper
    {
        public static List<NICInfo> GetNICInfo(CimSession cs)
        {
#pragma warning disable CS8600, CS8601, CS8602, CS8604
            string Namespace = $"\\\\{cs.ComputerName}\\root\\cimv2";
            string OSQuery = "SELECT * FROM Win32_NetworkAdapterConfiguration";
            IEnumerable<CimInstance> queryInstance = cs.QueryInstances(Namespace, "WQL", OSQuery);
            IEnumerable<CimInstance> NICs = queryInstance.Where(instance => instance.CimInstanceProperties["IPEnabled"].Value.ToString().Equals("True")); //.FirstOrDefault();

            List<NICInfo> nics = new List<NICInfo>();

            foreach (CimInstance cimInstance in NICs) {

                NICInfo nic = new();

                nic.Name = cimInstance.CimInstanceProperties["Description"].Value.ToString();
                nic.DHCPEnabled = Boolean.Parse(cimInstance.CimInstanceProperties["DHCPEnabled"].Value.ToString());
                nic.DHCPLeaseExpires = DateTime.Parse(cimInstance.CimInstanceProperties["DHCPLeaseExpires"].Value.ToString());
                nic.DHCPLeaseObtained = DateTime.Parse(cimInstance.CimInstanceProperties["DHCPLeaseObtained"].Value.ToString());
                nic.DHCPServer = cimInstance.CimInstanceProperties["DHCPServer"].Value.ToString();
                nic.DNSDomain = cimInstance.CimInstanceProperties["DNSDomain"].Value.ToString();
                nic.DNSHostName = cimInstance.CimInstanceProperties["DNSHostName"].Value.ToString();
                nic.FullDNSRegistrationEnabled = Boolean.Parse(cimInstance.CimInstanceProperties["FullDNSRegistrationEnabled"].Value.ToString());
                //nic.IPAddress = ((IEnumerable<object>)cimInstance.CimInstanceProperties["IPAddress"].Value).Cast<object>().Select(x => x.ToString())[0];
                nic.IPAddressObject = (string[])cimInstance.CimInstanceProperties["IPAddress"].Value;
                nic.DefaultIPGateway = cimInstance.CimInstanceProperties["DefaultIPGateway"].Value.ToString();
                nic.IPSubnet = cimInstance.CimInstanceProperties["IPSubnet"].Value.ToString();
                nic.MACAddress = cimInstance.CimInstanceProperties["MACAddress"].Value.ToString();
                nic.ServiceName = cimInstance.CimInstanceProperties["ServiceName"].Value.ToString();
                //nic.Speed = BigInteger.Parse(cimInstance.CimInstanceProperties["Speed"].Value.ToString()) / 1000 / 1000 / 1000 + " GHz";

                nics.Add(nic);
            }

            return nics;
#pragma warning restore CS8600, CS8601, CS8602, CS8604
        }
    }
}
