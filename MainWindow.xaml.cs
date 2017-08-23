using System;
//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace FesianXu.KinectGestureControl
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Windows.Media.Imaging;
    using TensorFlow;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        private byte[] rawColorPixels; // raw color frame pixels
        private int colorFrameStride;
        private int colorFrameWidth;
        private int colorFrameHeight;

        private short[] rawDepthPixels; // raw depth frame pixels
        private byte[] depthInfoPixels; // depth information parse from rawDepthPixels
        private int depthFrameStride;
        private int depthFrameWidth;
        private int depthFrameHeight;
        private int min_depth;
        private int max_depth;

        // CNN initiation
        private string lhand_pb_model_path = @"../../Resources/models/lhand_cnn_models.pb";
        private string rhand_pb_model_path = @"../../Resources/models/rhand_cnn_models.pb";
        private TFGraph lhand_global_graph;
        private TFGraph rhand_global_graph;
        private static TFSession lhand_global_sess;
        private static TFSession rhand_global_sess;
        private byte[] lhand_cnn_model; // cnn models raw byte streams
        private byte[] rhand_cnn_model;
        
        private TFTensor lhand_tensor;
        private TFTensor rhand_tensor;

        // checkbox values
        private bool isShowColorHandsValue = false;

        private Skeleton skeltmp;
        private RawByteMapping byteprocess = new RawByteProcess();


        // serial port initiation
        private SerialCommunicater comm = new SerialCommunicater();

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


                }
                catch (IOException)
                {
                    this.sensor = null;
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
                this.sensor.Stop();
                lhand_global_sess.Dispose(true);
                rhand_global_sess.Dispose(true);
            }
        }


        
        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void AllFrameReadyHandle(object sender, AllFramesReadyEventArgs e)
        {
            DateTime total_before = DateTime.Now;

            Skeleton[] skeletons = new Skeleton[0];
            Drawing draw = new mainImageBoxDraw(ref skeltmp, ref sensor);
            DrivingHandInfo info = new DrivingHandInfo(ref sensor);
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            } // get skeleton frames
            using (ColorImageFrame imgFrame = e.OpenColorImageFrame())
            {
                if (imgFrame == null)
                    return;
                rawColorPixels = new byte[imgFrame.PixelDataLength];
                imgFrame.CopyPixelDataTo(rawColorPixels);
                colorFrameStride = imgFrame.Width * 4;
                colorFrameWidth = imgFrame.Width;
                colorFrameHeight = imgFrame.Height;
            }
            using (DepthImageFrame depFrame = e.OpenDepthImageFrame())
            {
                if (depFrame == null)
                    return;
                rawDepthPixels = new short[depFrame.PixelDataLength];
                depthInfoPixels = new byte[depFrame.PixelDataLength];
                depFrame.CopyPixelDataTo(rawDepthPixels);
                depthFrameStride = depFrame.Width;
                depthFrameWidth = depFrame.Width;
                depthFrameHeight = depFrame.Height;
                min_depth = depFrame.MinDepth;
                max_depth = depFrame.MaxDepth;
            }


            // open a drawing context and need to close it manually
            DrawingContext dc = this.drawingGroup.Open();
            draw.addDrawingContext(ref dc);
            draw.getRenderAndClipBounds(RenderWidth, RenderHeight, ClipBoundsThickness);
            draw.drawBackgraoud();

            if (skeletons.Length != 0)
            {
                for (int id_skel = 0; id_skel < skeletons.Length; id_skel++)
                {
                    skeltmp = skeletons[id_skel];
                    draw.drawMatchStickMen(ref skeltmp);
                    if (skeltmp.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        info.updateSkeleton(ref skeltmp);
                        info.computeKeyJoints();
                        info.computeKeyPointsAndInfo();
                        angleBox.Text = "Angle = " + (float)Math.Round((double)info.Angle, 2) + "°";
                        angleRateBox.Text = "AngleRate = " + (float)Math.Round((double)info.AngleRate, 2) + "°";
                        // parameters update and showing
                        try
                        {
                            System.Drawing.Rectangle lroi = info.getROI(HandsEnum.leftHand);
                            System.Drawing.Rectangle rroi = info.getROI(HandsEnum.rightHand);
                            byte[] lhand_p = byteprocess.mapColorImageROI(ref rawColorPixels, lroi);
                            byte[] rhand_p = byteprocess.mapColorImageROI(ref rawColorPixels, rroi);
                            BitmapSource lhand_bs = WriteableBitmap.Create(info.handWidth, info.handHeight,
                                96, 96, PixelFormats.Bgr32, null, lhand_p, info.handWidth * 4);
                            BitmapSource rhand_bs = WriteableBitmap.Create(info.handWidth, info.handHeight,
                                 96, 96, PixelFormats.Bgr32, null, rhand_p, info.handWidth * 4);
                            byte[] lhand_c3 = byteprocess.removeAlphaChannel(ref lhand_p);
                            byte[] rhand_c3 = byteprocess.removeAlphaChannel(ref rhand_p);
                            var p_l = recognizeHands(HandsEnum.leftHand, ref lhand_c3);
                            if (p_l[0] == 1)
                                LeftHandStatusBox.Text = "Left Hand is palm";
                            else
                                LeftHandStatusBox.Text = "Left Hand is fist";

                            var p_r = recognizeHands(HandsEnum.rightHand, ref rhand_c3);
                            if (p_r[0] == 1)
                                RightHandStatusBox.Text = "Right Hand is palm";
                            else
                                RightHandStatusBox.Text = "Right Hand is fist";


                            if (isShowColorHandsValue)
                            {
                                left_hand_color_box.Source = lhand_bs;
                                right_hand_color_box.Source = rhand_bs;
                            } // checkbox to select whether show the color hand
                            HandsROIStatusBox.Text = "Hands Normal";

                        }
                        catch (ROIOutOfBoundsException excep)
                        {
                            HandsROIStatusBox.Text = "Hands OutOfBounds";
                        }

                        // driving steering wheel
                        if (info.Radius < DrivingHandInfo.bothHandsRadius_threshold
                            && info.WhichHandInFront == HandInFront.SamePlane)
                        {
                            draw.drawSteeringWheel(info.LeftHandPoint, info.RightHandPoint);
                        }
                        else
                        {
                            if (info.Left2CenterDistance > DrivingHandInfo.singleHandsAway_threshold
                                || info.WhichHandInFront == HandInFront.RightHand)
                            {
                                draw.drawSingleHandInWheel(info.RightHandPoint, info.OldCenterPosition, HandsEnum.rightHand);
                            }
                            else if (info.Right2CenterDistance > DrivingHandInfo.singleHandsAway_threshold
                                || info.WhichHandInFront == HandInFront.LeftHand)
                            {
                                draw.drawSingleHandInWheel(info.LeftHandPoint, info.OldCenterPosition, HandsEnum.leftHand);
                            }
                            else
                            {
                                draw.drawSteeringWheel(info.LeftHandPoint, info.RightHandPoint);
                            }
                        }
                        ////////////////////////////////////////////////////////////////////////////
                    }
                }
            }


            // prevent drawing outside of our render area
            this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            dc.Close();


            DateTime total_after = System.DateTime.Now;
            TimeSpan total_ts = total_after.Subtract(total_before);
            double total_tf = total_ts.TotalMilliseconds;
            TotalCostTimeBox.Text = "TotalCostTime = " + total_tf.ToString() + " ms";
        }


        private static TFTensor CreateTensorFromRawTensor(ref TFTensor[] tensor_m, TFGraph graph, bool isFromFile = false)
        {

            TFOutput input, output;
            if (isFromFile)
                input = graph.Placeholder(TFDataType.String);
            else
                input = graph.Placeholder(TFDataType.UInt8);
            if (isFromFile)
                output = graph.Cast(graph.DecodePng(contents: input, channels: 3), DstT: TFDataType.Float);
            else
                output = graph.Cast(x: input, DstT: TFDataType.Float);
            output = graph.ExpandDims(input: output, dim: graph.Const(0));

            using (var sess = new TFSession(graph))
            {
                var nor = sess.Run(
                    inputs: new[] { input },
                    inputValues: tensor_m,
                    outputs: new[] { output }
                    );
                return nor[0];
            }
        }

    }
}