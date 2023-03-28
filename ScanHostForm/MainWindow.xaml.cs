using ScanHost;
using ScanHostLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScanHostForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ComputerInfo host = ScanHostHelper.Scan("localhost");
            PopulateContent(host);
        }

        private void PopulateContent(ComputerInfo computerInfo)
        {
            if (computerInfo == null) { return; }

            RHostNameLabel.Content = computerInfo.OS.ComputerName;

            CPUInfoBox.Text = computerInfo.CPU.ToString();
            OSInfoBox.Text = computerInfo.OS.ToString();
            NIInfoBox.Text = ""; // computerInfo.NIC.ToString();
            foreach (NICInfo NIC in computerInfo.NIC)
            {
                NIInfoBox.Text += NIC.ToString();
                NIInfoBox.Text += "============";
            }
            HWInfoBox.Text = computerInfo.Hardware.ToString();
            DIInfoBox.Text = computerInfo.Disk.ToString();
        }

        private void StartScanButton_Click(object sender, RoutedEventArgs e)
        {
            String computerName = NewRHostNameTB.Text.Trim();

            if (computerName == null) { PopulateContent(ScanHostHelper.Scan("localhost")); return; } 
            else if (computerName.Length <= 2) { PopulateContent(ScanHostHelper.Scan("localhost")); return; }
            else { PopulateContent(ScanHostHelper.Scan(computerName)); return; }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
