using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FesianXu.KinectGestureControl
{
    partial class MainWindow
    {

        private string SerialInfoBox_TextFormat = "SerialPortInfo:";

        /// <summary>
        /// Handles the checking or unchecking of the seated mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != this.sensor)
            {
                if (this.checkBoxSeatedMode.IsChecked.GetValueOrDefault())
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                else
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                }
            }
        }

        private void backgroundInMainWindow_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void isShowColorHands_Checked(object sender, RoutedEventArgs e)
        {
            if (null != this.sensor)
            {
                if (this.isShowColorHands.IsChecked.GetValueOrDefault())
                {
                    isShowColorHandsValue = true;
                }
                else
                {
                    isShowColorHandsValue = false;
                }
            }
        }

        private void OpenPort_Click(object sender, RoutedEventArgs e)
        {
            if (comm.PortStatus == SerialPortStatusEnum.Closed)
            {
                if (comm.isPortBaudRateSetted && comm.isPortNameSetted)
                {
                    if (comm.openPort())
                    {
                        OpenPort.Content = "Close Port";
                        SerialInfoBox.Text = SerialInfoBox_TextFormat + "Port Open Success!";
                        if (comm.isUsedHistoryParams == false)
                        {
                            comm.createSerialHistorySetting();
                        }
                    }
                    else
                    {
                        // open fail as the port has been opened by other process yet
                        SerialInfoBox.Text = SerialInfoBox_TextFormat + "Open failed! some process have been opened it yet";
                    }
                }
                else
                {
                    SerialInfoBox.Text = SerialInfoBox_TextFormat + "Please select the port name and baud rate!";
                }
            }
            else if (comm.PortStatus == SerialPortStatusEnum.Opened)
            {
                if (comm.isPortBaudRateSetted && comm.isPortNameSetted)
                {
                    comm.close();
                    OpenPort.Content = "Open Port";
                    SerialInfoBox.Text = SerialInfoBox_TextFormat + "Port Close!";
                }
            }
        }


        private void SerialPortNameBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int ind = SerialPortNameBox.SelectedIndex;
            comm.portNameInUsed = comm.AvailableSerialPortNames[ind];
            comm.isPortNameSetted = true;
            comm.isUsedHistoryParams = false;
        }

        private void SerialBaudRateBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int ind = SerialBaudRateBox.SelectedIndex;
            comm.baudInUsed = comm.AvailableBaudRates[ind];
            comm.isPortBaudRateSetted = true;
            comm.isUsedHistoryParams = false;
        }
    }
}
