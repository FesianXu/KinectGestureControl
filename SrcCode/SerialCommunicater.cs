using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace FesianXu.KinectGestureControl
{

    public enum SerialPortStatusEnum { Opened, Closed};


    public class SerialCommunicater: Communicater
    {
        private SerialPort sp = new SerialPort();
        private List<int> availableBaudRateList =
            new List<int> { 9600, 19200, 115200};
        private List<string> availableSerialPortNameList = new List<string> { };
        private SerialPortStatusEnum portStatus;


        public void send(string str)
        {
            sp.Write(str);
        }

        public SerialCommunicater()
        {
            string[] portName = SerialPort.GetPortNames();
            foreach(var s in portName)
            {
                availableSerialPortNameList.Add(s);
            }
            portStatus = SerialPortStatusEnum.Closed;
            // get the available port names
        }


        /// <summary>
        /// open the serial port
        /// </summary>
        /// <returns>is successfully open the port</returns>
        public bool openPort()
        {
            sp.PortName = portNameInUsed;
            sp.BaudRate = baudInUsed;
            sp.DataBits = 8;
            sp.StopBits = StopBits.One;
            sp.Encoding = System.Text.Encoding.GetEncoding("GB2312");
            sp.ReceivedBytesThreshold = 1;
            sp.Parity = Parity.None;
            sp.DataReceived += sp_DataReceived; // binding the recieve handle
            try
            {
                sp.Open();
                portStatus = SerialPortStatusEnum.Opened;
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                portStatus = SerialPortStatusEnum.Closed;
                return false;
            }
            
        }

        /// <summary>
        /// close the serial port
        /// </summary>
        public void close()
        {
            portStatus = SerialPortStatusEnum.Closed;
            sp.Close();
        }


        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }


        // visitors

        public List<string> AvailableSerialPortNames{ get { return availableSerialPortNameList; } }
        public List<int> AvailableBaudRates { get { return availableBaudRateList; } }
        public int baudInUsed { get; set; }
        public string portNameInUsed { get; set; }
        public bool isPortNameSetted { get; set; }
        public bool isPortBaudRateSetted { get; set; }
        public SerialPortStatusEnum PortStatus { get { return portStatus; } }

    }
}
