//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/24
// Description: The partial class of MainWindows, for the sensors and other class 
// initiation.including sensor loading and closing
// version: v1.1
// type: partial class
//////////////////////////////////////////////////////////////////////////


using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TensorFlow;
using System.Media;

namespace FesianXu.KinectGestureControl
{

    partial class MainWindow
    {
        private bool isKinectOpened;
        private bool isKinectVoiceBeginAndEnd = true;
        private Chris assistant = new Chris();
        private UserPriorityManager priorityManager = new UserPriorityManager();

        //private KinectVoiceRecognition voiceReg = new KinectVoiceRecognition();

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {          
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            mainImageShow.Source = this.imageSource;

            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();
                this.sensor.ColorStream.Enable();
                this.sensor.DepthStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.AllFramesReady += this.AllFrameReadyHandle;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                    this.sensor.ElevationAngle = 6;
                    isKinectOpened = true;
                    // load CNN models and initiation
                    lhand_global_graph = new TFGraph();
                    rhand_global_graph = new TFGraph();

                    lhand_cnn_model = File.ReadAllBytes(lhand_pb_model_path);
                    rhand_cnn_model = File.ReadAllBytes(rhand_pb_model_path);
                    lhand_global_graph.Import(lhand_cnn_model, "");
                    rhand_global_graph.Import(rhand_cnn_model, "");
                    lhand_global_sess = new TFSession(lhand_global_graph);
                    rhand_global_sess = new TFSession(rhand_global_graph);

                    // get the available serial port names and update the box list
                    var portNames = comm.AvailableSerialPortNames;
                    foreach (var name in portNames)
                    {
                        SerialPortNameBox.Items.Insert(portNames.IndexOf(name), name);
                    }
                    // read the history serial setting
                    if (comm.readSerialHistorySetting())
                    {
                        comm.useHistorySetting();
                        SerialPortNameBox.Text = comm.portNameInUsed;
                        SerialBaudRateBox.Text = comm.baudInUsed.ToString() + " bps";
                    }

                    // load the voice assistant and initiate it
                    assistant.updateKinectSensor(ref sensor);
                    assistant.initVoiceRecog();
                    // play the welcome voice message
                    if(isKinectVoiceBeginAndEnd)
                        assistant.playWelcome();

                    //load reg
                    //voiceReg.updateAndInitiate(ref sensor);
                    priorityManager.updateAssistant(ref assistant);

                }
                catch (IOException)
                {
                    this.sensor = null;
                    isKinectOpened = false;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                try
                {
                    this.sensor.Stop();
                    isKinectOpened = false;
                    lhand_global_sess.Dispose(true);
                    rhand_global_sess.Dispose(true);
                    if (comm.PortStatus == SerialPortStatusEnum.Opened)
                        comm.closePort();
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    if (assistant.voiceRecog.haveStartedVAThread)
                    {
                        // here we should close the Voice Assistant Thread properly
                        
                    }
                    if (isKinectVoiceBeginAndEnd)
                    {
                        assistant.playKinectClosing();
                        System.Threading.Thread.Sleep(4200);
                    }  
                }
            }
        }










    }
}
