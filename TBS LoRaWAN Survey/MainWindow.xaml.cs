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
using System.IO.Ports;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TBS_LoRaWAN_Survey
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort;
        int count = 0;
        string filename;

        public MainWindow()
        {
            InitializeComponent();
            serialPort = new SerialPort("COM11");
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.BaudRate = 115200;
            serialPort.Open();
            filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "-Survey.csv";
            try
            {
                System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite);
                fileStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string result = serialPort.ReadExisting();
            var newString = string.Join(" ", Regex.Split(result, @"(?:\r\n|\n|\r)"));
            var fmtString = newString.Replace(",", ";");
            this.Dispatcher.Invoke(new Action(() => {
                var finalString = DateTime.Now.ToLongTimeString() + " , " + Location.Text + " , " + fmtString.Replace("  ", ",");
                Debug.Write(count.ToString());
                Debug.Write(" ");
                Debug.WriteLine(finalString);
                resultsList.Items.Add(finalString);
                try
                {
                    System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(fileStream, System.Text.Encoding.Default);
                    streamWriter.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                    streamWriter.Write(finalString);
                    streamWriter.Write(",");
                    streamWriter.WriteLine(this.Notes.Text);
                    streamWriter.Close();
                    fileStream.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }));
            
            count++;
        }

        private void btnJoinNetwork_Click(object sender, RoutedEventArgs e)
        {
            serialPort.WriteLine("AT+JOIN");
        }

        private void btnRequestAck_Click(object sender, RoutedEventArgs e)
        {
            serialPort.WriteLine("AT+ACK=1");
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            serialPort.WriteLine("AT+SEND=1");
        }

        private void btnConnectivity_Click(object sender, RoutedEventArgs e)
        {
            serialPort.WriteLine("AT+NJS");
        }

        private void btnPingTest_Click(object sender, RoutedEventArgs e)
        {
            serialPort.WriteLine("AT+PING");
        }

        private void btnLinkTest_Click(object sender, RoutedEventArgs e)
        {
            serialPort.WriteLine("AT+NLC");
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            serialPort.WriteLine("AT+NJS=?");
            serialPort.WriteLine("AT+PING=?");
            serialPort.WriteLine("AT+NLC=?");
        }
    }
}
