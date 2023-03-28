using Microsoft.Management.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanHostLib
{
    public class OSInfo
    {
        private string OSComputerName;
        private string OSCaption;
        private string OSVersion;
        private DateTime OSLocalDateTime;
        private DateTime OSLastBootTime;
        private TimeSpan OSUptime;
        private DateTime OSInstallDate;
        private string OSRegisteredUser;
        private string OSOrganization;
        //       private string OSAvailableRAM;

        public OSInfo() { }
        public OSInfo(string ComputerName, string Caption, string Version, DateTime LocalDateTime, DateTime LastBootTime, TimeSpan Uptime, DateTime InstallDate, string RegisteredUser, string Organization)
        {
            OSComputerName = ComputerName;
            OSCaption = Caption;
            OSVersion = Version;
            OSLocalDateTime = LocalDateTime;
            OSLastBootTime = LastBootTime;
            OSUptime = Uptime;
            OSInstallDate = InstallDate;
            OSRegisteredUser = RegisteredUser;
            OSOrganization = Organization;
        }

        public OSInfo(string ComputerName, string Caption, string Version, DateTime LocalDateTime, DateTime LastBootTime, DateTime InstallDate, string RegisteredUser, string Organization)
        {
            OSComputerName = ComputerName;
            OSCaption = Caption;
            OSVersion = Version;
            OSLocalDateTime = LocalDateTime;
            OSLastBootTime = LastBootTime;
            CalculateUptime();
            OSInstallDate = InstallDate;
            OSRegisteredUser = RegisteredUser;
            OSOrganization = Organization;
        }

        public string ComputerName { get => this.OSComputerName; set => this.OSComputerName = value; }
        public string Caption { get => this.OSCaption; set => this.OSCaption = value; }
        public string Version { get => this.OSVersion; set => this.OSVersion = value; }
        public DateTime LocalDateTime { get => this.OSLocalDateTime; set => this.OSLocalDateTime = value; }
        //public string LocalDateTimeS { get => this.OSLocalDateTime.ToString(); set => this.OSLocalDateTime = DateTime.Parse(value); }
        public DateTime LastBootTime { get => this.OSLastBootTime; set => this.OSLastBootTime = value; }
        public TimeSpan Uptime { get => this.OSUptime; set => this.OSUptime = value; }
        public DateTime InstallDate { get => this.OSInstallDate; set => this.OSInstallDate = value; }
        public string RegisteredUser { get => this.OSRegisteredUser; set => this.OSRegisteredUser = value; }
        public string Organization { get => this.OSOrganization; set => this.OSOrganization = value; }
        //public string AvailableRAM { get => this.AvailableRAM; set => this.AvailableRAM = value; }

        public string LocalDateTimeString
        {
            get => this.OSLocalDateTime.ToString();
            set => this.OSLocalDateTime = DateTime.Parse(value);
        }
        public string LastBootTimeString
        {
            get => this.OSLastBootTime.ToString();
            set => this.OSLastBootTime = DateTime.Parse(value);
        }
        public string UptimeString
        {
            get => this.OSUptime.ToString();
        }
        public void CalculateUptime()
        {
            try { this.OSUptime = this.OSLocalDateTime - this.OSLastBootTime; }
            catch (Exception) { throw new InvalidOperationException("OSInfo.LocalDateTime and OSInfo.LastBootTime must be set before calling OSInfo.CalculateUptime()"); }
        }
        public bool Equals(OSInfo otherOsInfo)
        {
            return this.OSComputerName.Equals(otherOsInfo.OSComputerName) && this.OSRegisteredUser.Equals(otherOsInfo.RegisteredUser) && this.OSOrganization.Equals(otherOsInfo.OSOrganization);
        }

        public override string ToString()
        {
            return $"{OSComputerName}\r\n" +
                   $"{OSCaption}\r\n" +
                   $"{OSVersion}\r\n" +
                   $"Current Time: {OSLocalDateTime}\r\n" +
                   //$"Boot Time: {OSLastBootTime}\r\n" +
                   $"Uptime: {OSUptime}\r\n" +
                   $"Install Date: {OSInstallDate}\r\n" +
                   $"Registered to: {OSRegisteredUser},\r\n" +
                   $"{OSOrganization}";
        }
    }

    public static class OSInfoHelper
    {
        public static OSInfo GetOSInfo(CimSession cs)
        {
#pragma warning disable CS8600, CS8601, CS8602, CS8604
            string Namespace = $"\\\\{cs.ComputerName}\\root\\cimv2";
            string OSQuery = "SELECT * FROM Win32_OperatingSystem";
//            CimSession mySession = CimSession.Create("cs");
            IEnumerable<CimInstance> queryInstance = cs.QueryInstances(Namespace, "WQL", OSQuery);
            CimInstance cimInstance = queryInstance.FirstOrDefault();

            OSInfo os = new();
            os.ComputerName = cimInstance.CimInstanceProperties["CSName"].Value.ToString();
            os.Caption = cimInstance.CimInstanceProperties["Caption"].Value.ToString();
            os.Version = cimInstance.CimInstanceProperties["Version"].Value.ToString();
            os.LocalDateTime = DateTime.Parse(cimInstance.CimInstanceProperties["LocalDateTime"].Value.ToString());
            os.LastBootTime = DateTime.Parse(cimInstance.CimInstanceProperties["LastBootUpTime"].Value.ToString());
            os.CalculateUptime();
            os.InstallDate = DateTime.Parse(cimInstance.CimInstanceProperties["InstallDate"].Value.ToString());
            os.RegisteredUser = cimInstance.CimInstanceProperties["RegisteredUser"].Value.ToString();
            os.Organization = cimInstance.CimInstanceProperties["Organization"].Value.ToString();

            return os;
#pragma warning restore CS8600, CS8601, CS8602, CS8604

        }

        public static OSInfo GetOSInfo(string? ComputerName)
        {
#pragma warning disable CS8600, CS8601, CS8602, CS8604
            string Namespace = $"\\\\{ComputerName}\\root\\cimv2";
            string OSQuery = "SELECT * FROM Win32_OperatingSystem";
            CimSession cs = CimSession.Create(ComputerName);
            IEnumerable<CimInstance> queryInstance = cs.QueryInstances(Namespace, "WQL", OSQuery);
            CimInstance cimInstance = queryInstance.FirstOrDefault();

            OSInfo os = new OSInfo();
            os.ComputerName = cimInstance.CimInstanceProperties["CSName"].Value.ToString();
            os.Caption = cimInstance.CimInstanceProperties["Caption"].Value.ToString();
            os.Version = cimInstance.CimInstanceProperties["Version"].Value.ToString();
            os.LocalDateTime = DateTime.Parse(cimInstance.CimInstanceProperties["LocalDateTime"].Value.ToString());
            os.LastBootTime = DateTime.Parse(cimInstance.CimInstanceProperties["LastBootUpTime"].Value.ToString());
            os.CalculateUptime();
            os.InstallDate = DateTime.Parse(cimInstance.CimInstanceProperties["InstallDate"].Value.ToString());
            os.RegisteredUser = cimInstance.CimInstanceProperties["RegisteredUser"].Value.ToString();
            os.Organization = cimInstance.CimInstanceProperties["Organization"].Value.ToString();
//            os.AvailableRAM = $"{int.Parse(cimInstance.CimInstanceProperties["FreePhysicalMemory"].Value.ToString()) / 1024 / 1024 / 1024} GB";

            return os;
#pragma warning restore CS8600, CS8601, CS8602, CS8604
        }
    }
}
