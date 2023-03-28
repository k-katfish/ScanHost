using Microsoft.Management.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanHostLib
{
    public class DiskInfo
    {
        char    _DeviceLetter;
        string  _DeviceName;
        int     _Partitions;
        int     _TotalDiskSize;
        int     _PartitionSize;
        int     _FreeSpace;
        int     _UsedSpace;
        string  _FileSystem;
        string  _VolumeName;
        string  _VolumeSerial;
        string  _DiskName;
        string  _DiskModel;
        string  _DiskSerial;

        public DiskInfo() { _DeviceName = string.Empty; _FileSystem = string.Empty; _VolumeName = string.Empty;
            _VolumeSerial = string.Empty; _DiskModel = string.Empty; _DiskName = string.Empty; _DiskSerial = string.Empty;
        }

        public DiskInfo(char deviceLetter, string deviceName, int partitions, int totalDiskSize, int freeSpace, int usedSpace, string fileSystem, string volumeName, string volumeSerial, string diskName, string diskModel, string diskSerial)
        {
            _DeviceLetter = deviceLetter;
            _DeviceName = deviceName;
            _Partitions = partitions;
            _TotalDiskSize = totalDiskSize;
            _PartitionSize = -1;
            _FreeSpace = freeSpace;
            _UsedSpace = usedSpace;
            _FileSystem = fileSystem;
            _VolumeName = volumeName;
            _VolumeSerial = volumeSerial;
            _DiskName = diskName;
            _DiskModel = diskModel;
            _DiskSerial = diskSerial;
        }

        public char DeviceLetter { get => _DeviceLetter; set => _DeviceLetter = value; }
        public string DeviceName { get => _DeviceName; set => _DeviceName = value; }
        public int Partitions { get => _Partitions; set => _Partitions = value; }
        public int TotalDiskSize { get => _TotalDiskSize; set => _TotalDiskSize = value; }
        public int PartitionSize { get => _PartitionSize; set => _PartitionSize = value; }
        public int FreeSpace { get => _FreeSpace; set => _FreeSpace = value; }
        public int UsedSpace { get => _UsedSpace; set => _UsedSpace = value; }
        public string FileSystem { get => _FileSystem; set => _FileSystem = value; }
        public string VolumeName { get => _VolumeName; set => _VolumeName = value; }
        public string VolumeSerial { get => _VolumeSerial; set => _VolumeSerial = value; }
        public string DiskName { get => _DiskName; set => _DiskName = value; }
        public string DiskModel { get => _DiskModel; set => _DiskModel = value; }
        public string DiskSerial { get => _DiskSerial; set => _DiskSerial = value; }

        /*public override string ToString()
        {
            return $"{_DeviceLetter}:\\ {_DeviceName}\r\n" +
                   $"     {_UsedSpace} Used of {_TotalDiskSize} Total\r\n" +
                   $"     {_FreeSpace} Free. {_Partitions} Partitions\r\n" +
                   $"     Total Disk Size: {_TotalDiskSize}\r\n" +
                   $"  Physical Disk Information:\r\n" +
                   $"     FileSystem: {_FileSystem}";
        }*/
        public override string ToString()
        {
            return "I don't think you appreciate how _hard_ it is to ask for this kind of information from a Windows computer.";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;
            DiskInfo diskInfo = (DiskInfo)obj;
            return _DiskSerial.Equals(diskInfo.DiskSerial);
        }

        public override int GetHashCode()
        {
            return _DiskSerial.GetHashCode();
        }
    }

    public static class DiskInfoHelper
    {
        public static List<DiskInfo> GetAllDiskInfo(CimSession cs)
        {
            string Namespace = $"\\\\{cs.ComputerName}\\root\\cimv2";
            string DIQuery = "SELECT * FROM Win32_DiskDrive";
            string PIQuery = "SELECT * FROM Win32_DiskPartition";
            string LDQuery = "SELECT * FROM Win32_LogicalDisk";

            IEnumerable<CimInstance> DI = cs.QueryInstances(Namespace, "WQL", DIQuery);
            IEnumerable<CimInstance> PI = cs.QueryInstances(Namespace, "WQL", PIQuery);
            IEnumerable<CimInstance> LD = cs.QueryInstances(Namespace, "WQL", LDQuery);

            List<DiskInfo> disks = new List<DiskInfo>();

            foreach (CimInstance DiskInstance in DI)
            {
                string DDtDPQuery = "SELECT * FROM Win32_DiskDriveToDiskPartition";
                IEnumerable<CimInstance> DDtDP = cs.QueryInstances(Namespace, "WQL", DDtDPQuery);
                CimInstance mapper = DDtDP.FirstOrDefault();

                //CimInstance partition = new CimInstance(mapper.CimInstanceProperties["Win32_DiskPartition"].Value.ToString);
            }

            return disks;
        }

        public static DiskInfo GetDiskInfo(CimSession cs)
        {
            return new DiskInfo();
        }
    }
}
