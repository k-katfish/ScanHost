using Microsoft.Management.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScanHostLib
{
    public class HardwareInfo
    {
        private string _Name;
        private string _DNSName;
        private bool   _OnDomain;
        private string _Domain;
        private string _Workgroup;
        private string _Manufacturer;
        private string _Model;
        private string _Serial;
        private string _HWUUID;
        private string _InstalledRAM;

        public HardwareInfo() {
            _Name = string.Empty; _DNSName = string.Empty; _OnDomain = false; _Domain = string.Empty;
            _Workgroup = string.Empty; _Manufacturer = string.Empty; _Model = string.Empty; _Serial = string.Empty;
            _HWUUID = string.Empty; _InstalledRAM = string.Empty;
        }

        public HardwareInfo(string Name, string DNSName, bool OnDomain, string DomWkgName, string Manufacturer, string Model, string Serial, string UUID, string InstalledRAM)
        {
            _Name = Name;
            _DNSName = DNSName;
            _OnDomain = OnDomain;
            if (OnDomain) { _Domain = DomWkgName; _Workgroup = ""; }
            else { _Workgroup =  DomWkgName; _Domain = ""; }
            _Manufacturer = Manufacturer;
            _Model = Model;
            _Serial = Serial;
            _HWUUID = UUID;
            _InstalledRAM = InstalledRAM;
        }

        public string Name { get => _Name; set => _Name = value; }
        public string DNSName { get => _DNSName; set => _DNSName = value; }
        public bool OnDomain { get => _OnDomain; set => _OnDomain = value; }
        public string Domain { get => _Domain; set => _Domain = value; }
        public string Workgroup { get => _Workgroup; set => _Workgroup = value; }
        public string Manufacturer { get => _Manufacturer; set => _Manufacturer = value; }
        public string Model { get => _Model; set => _Model = value; }
        public string Serial { get => _Serial; set => _Serial = value; }
        public string UUID { get => _HWUUID; set => _HWUUID = value; }
        public string RAM { get => _InstalledRAM; set => _InstalledRAM = value; }

        public override bool Equals(Object? other)
        {
            if (other == null) return false;
            if (other.GetType() != this.GetType()) return false;
            HardwareInfo otherInfo = (HardwareInfo)other;
            return this.Name.Equals(otherInfo.Name) && this.Model.Equals(otherInfo.Model);
        }

        public override string ToString()
        {
            if (_OnDomain)
            {
                return $"{_DNSName}\r\n" +
                       $"{_Domain}\r\n" +
                       $"Make  : {_Manufacturer}\r\n" +
                       $"Model : {_Model},\r\n" +
                       $"Serial: {_Serial}\r\n" +
                       $"UUID  : {_HWUUID}\r\n" +
                       $"RAM: {_InstalledRAM}";
            } else
            {
                return $"{_Name}\r\n" +
                       $"{_Workgroup}\r\n" +
                       $"Make  : {_Manufacturer}\r\n" +
                       $"Model : {_Model},\r\n" +
                       $"Serial: {_Serial}\r\n" +
                       $"UUID  : {_HWUUID}\r\n" +
                       $"RAM: {_InstalledRAM}";
            }
        }

        public override int GetHashCode()
        { return _HWUUID.GetHashCode(); }
    }

    public static class HardwareInfoHelper
    {
        public static HardwareInfo GetHWInfo(CimSession cs)
        {
#pragma warning disable CS8600, CS8602, CS8604
            string Namespace = $"\\\\{cs.ComputerName}\\root\\cimv2";
            string SEQuery = "SELECT * FROM Win32_SystemEnclosure";
            string CSQuery = "SELECT * FROM Win32_ComputerSystem";
            string CSPQuery = "SELECT * FROM Win32_ComputerSystemProduct";
            IEnumerable<CimInstance> SE = cs.QueryInstances(Namespace, "WQL", SEQuery);
            CimInstance SystemEnclosure = SE.FirstOrDefault();

            IEnumerable<CimInstance> CS = cs.QueryInstances(Namespace, "WQL", CSQuery);
            CimInstance ComputerSystem = CS.FirstOrDefault();

            IEnumerable<CimInstance> CSP = cs.QueryInstances(Namespace, "WQL", CSPQuery);
            CimInstance ComputerSYstemProduct = CSP.FirstOrDefault();

            HardwareInfo hardwareInfo = new HardwareInfo(
                ComputerSystem.CimInstanceProperties["Name"].Value.ToString(),
                ComputerSystem.CimInstanceProperties["DNSHostName"].Value.ToString(),
                Boolean.Parse(ComputerSystem.CimInstanceProperties["PartOfDomain"].Value.ToString()),
                ComputerSystem.CimInstanceProperties["Domain"].Value.ToString(),
                ComputerSystem.CimInstanceProperties["Manufacturer"].Value.ToString(),
                ComputerSystem.CimInstanceProperties["Model"].Value.ToString(),
                SystemEnclosure.CimInstanceProperties["SerialNumber"].Value.ToString(),
                ComputerSYstemProduct.CimInstanceProperties["UUID"].Value.ToString(),
                Math.Round(Double.Parse(ComputerSystem.CimInstanceProperties["TotalPhysicalMemory"].Value.ToString()) / (1024 * 1024 * 1024)) + " GB"
                );

            return hardwareInfo;
#pragma warning restore CS8600, CS8602, CS8604
        }
        
        public static HardwareInfo GetHWInfo(string? ComputerName)
        {
            CimSession cs = CimSession.Create(ComputerName);
            return GetHWInfo(cs);
        }
    }
}
