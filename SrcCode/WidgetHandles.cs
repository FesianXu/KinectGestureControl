//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/24
// Description: The partial class of MainWindows, for the widget handles
// version: v1.1
// type: partial class
//////////////////////////////////////////////////////////////////////////


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


        /// <summary>
        /// whether show the color hands or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// open serial port click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        if (comm.IsUsedHistoryParams == false)
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
                    comm.closePort();
                    OpenPort.Content = "Open Port";
                    SerialInfoBox.Text = SerialInfoBox_TextFormat + "Port Close!";
                }
            }
        }


        /// <summary>
        /// comboBox, to select the serial port names
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortNameBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int ind = SerialPortNameBox.SelectedIndex;
            comm.portNameInUsed = comm.AvailableSerialPortNames[ind];
            comm.isPortNameSetted = true;
            comm.IsUsedHistoryParams = false;
        }


        /// <summary>
        /// comboBox, to select the serial port's baud rates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialBaudRateBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int ind = SerialBaudRateBox.SelectedIndex;
            comm.baudInUsed = comm.AvailableBaudRates[ind];
            comm.isPortBaudRateSetted = true;
            comm.IsUsedHistoryParams = false;
        }


        /// <summary>
        /// click to select whether record the serial port sending and receiving 
        /// messages or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickToRecordSerialPort_Checked(object sender, RoutedEventArgs e)
        {
            if (this.ClickToRecordSerialPort.IsChecked.GetValueOrDefault())
            {
                comm.IsRecordSerialMessages = true;
            }
            else
            {
                comm.IsRecordSerialMessages = false;
            }
        }


        /// <summary>
        /// click to use voice assistant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void useVoiceAssistant_Checked(object sender, RoutedEventArgs e)
        {
            if (this.useVoiceAssistant.IsChecked.GetValueOrDefault())
            {
                assistant.IsUsedVoiceAssistant = true;
                initVoiceAssistantThread(); // initiate and start the VA thread
                assistant.playHello();
            }
            else
            {
                assistant.IsUsedVoiceAssistant = false;
                assistant.playSeeYou();
            }
        }


        /// <summary>
        /// the backdoor button for running the kinect gesture control system
        /// without the authorization in voice assistant chris
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackDoorButton_Click(object sender, RoutedEventArgs e)
        {
            if (priorityManager.KinectRunningBeginWay == KinectRunBeginWayEnum.NoRunningNow)
            {
                    priorityManager.KinectRunningBeginWay = KinectRunBeginWayEnum.Button_Init;
                    BackDoorButton.Content = "Stop the Kinect";
            }
            else
            {
                priorityManager.KinectRunningBeginWay = KinectRunBeginWayEnum.NoRunningNow;
                BackDoorButton.Content = "Run the Kinect";
            }
        }




    }
}
