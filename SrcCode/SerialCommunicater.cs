﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Text;
using System.Globalization;

namespace FesianXu.KinectGestureControl
{

    public enum SerialPortStatusEnum { Opened, Closed };

    public struct SerialHistoryParameters
    {
        public bool isHadHistory;
        public string serialPortName;
        public int serialBaudRate;
        public DateTime time;
    };


    public class SerialCommunicater : Communicater
    {
        private SerialPort sp = new SerialPort();
        private List<int> availableBaudRateList =
            new List<int> { 9600, 19200, 115200 };
        private List<string> availableSerialPortNameList = new List<string> { };
        private SerialPortStatusEnum portStatus;
        private string historyParamsSavePath = @"../../Setting/SerialPortHistorySetting.ini";

        private static string historySerialPortNameFormat = @"<SerialPortName>=";
        private static string historySerialBaudRateFormat = @"<SerialPortBaudRate>=";
        private static string historySerialSettingTimeFormat = @"<SettingTime>=";
        private static string NewLineFormat = "\r\n";

        private SerialHistoryParameters historyParams;
        public bool isUsedHistoryParams;

        public void send(string str)
        {
            sp.Write(str);
        }

        public SerialCommunicater()
        {
            string[] portName = SerialPort.GetPortNames();
            foreach (var s in portName)
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


        /// <summary>
        /// read the history serial port setting, including the baud rate and serial port
        /// name
        /// The format of the setting file is described following :
        /// <SerialPortName>="COM6"
        /// <SerialPortBaudRate>="115200"
        /// <SettingTime>="Year/Month/Date Hour:Minute:Second"
        /// </summary>
        /// <returns>is the file exist</returns>
        public bool readSerialHistorySetting()
        {
            if (File.Exists(historyParamsSavePath)) // the setting file exist, read it
            {
                FileStream file = new FileStream(historyParamsSavePath, FileMode.Open);
                StreamReader reader = new StreamReader(file, Encoding.Default);

                string strline;
                if ((strline = reader.ReadLine()) != null)
                {
                    // parse the setting contents
                    var index = strline.LastIndexOf("=");
                    string name = strline.Substring(index + 2, strline.Length - index - 3);
                    historyParams.serialPortName = name;
                }
                if ((strline = reader.ReadLine()) != null)
                {
                    // parse the setting contents
                    var index = strline.LastIndexOf("=");
                    string baud = strline.Substring(index + 2, strline.Length - index - 3);
                    historyParams.serialBaudRate = Convert.ToInt32(baud);
                }
                if ((strline = reader.ReadLine()) != null)
                {
                    // parse the setting contents
                    var index = strline.LastIndexOf("=");
                    string time = strline.Substring(index + 2, strline.Length - index - 3);
                    historyParams.time = DateTime.Parse(time);
                }
                file.Close();
                reader.Close();
                return true;

            } // the history serial setting exists
            else
            {
                return false;
            }
        }


        public void createSerialHistorySetting()
        {
            FileStream file = new FileStream(historyParamsSavePath, FileMode.Create, FileAccess.Write);
            string nowSerialName = historySerialPortNameFormat + "\"" + portNameInUsed + "\"";
            nowSerialName += NewLineFormat;
            string nowSerialBaud = historySerialBaudRateFormat + "\"" + baudInUsed.ToString() + "\"";
            nowSerialBaud += NewLineFormat;
            string nowSerialSavingTime = historySerialSettingTimeFormat + "\"" + DateTime.Now.ToString() + "\"";
            nowSerialSavingTime += NewLineFormat;
            byte[] data = Encoding.Default.GetBytes(nowSerialName + nowSerialBaud + nowSerialSavingTime);
            file.Write(data, 0, data.Length);
            file.Close();
        }


        public void useHistorySetting()
        {
            portNameInUsed = historyParams.serialPortName;
            baudInUsed = historyParams.serialBaudRate;
            isUsedHistoryParams = true;
        }



        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }


        // visitors

        public List<string> AvailableSerialPortNames { get { return availableSerialPortNameList; } }
        public List<int> AvailableBaudRates { get { return availableBaudRateList; } }
        public int baudInUsed { get; set; }
        public string portNameInUsed { get; set; }
        public bool isPortNameSetted { get; set; }
        public bool isPortBaudRateSetted { get; set; }
        public SerialPortStatusEnum PortStatus { get { return portStatus; } }

    }
}
