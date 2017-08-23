using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FesianXu.KinectGestureControl
{
    interface Communicater
    {
        /// <summary>
        /// write the raw strings
        /// </summary>
        /// <param name="str"></param>
        void write(string str);

        /// <summary>
        /// write the raw strings and a new line
        /// </summary>
        /// <param name="str"></param>
        void writeLines(string str);

        /// <summary>
        /// open the communication port
        /// </summary>
        /// <returns></returns>
        bool openPort();

        /// <summary>
        /// close the communication port
        /// </summary>
        void closePort();

        /// <summary>
        /// read the communicator's history setting
        /// </summary>
        /// <returns></returns>
        bool readSerialHistorySetting();

        /// <summary>
        /// create the communicator's history setting file
        /// </summary>
        void createSerialHistorySetting();

        /// <summary>
        /// use the history setting
        /// </summary>
        void useHistorySetting();

        List<string> AvailableSerialPortNames { get; }
        List<int> AvailableBaudRates { get; }
        int baudInUsed { get; set; }
        string portNameInUsed { get; set; }
        bool isPortNameSetted { get; set; }
        bool isPortBaudRateSetted { get; set; }
        SerialPortStatusEnum PortStatus { get; }
        bool IsRecordSerialMessages { get; set; }
        bool IsUsedHistoryParams { get; set; }
        byte[] ReceiveBuf { get; }
    }
}
