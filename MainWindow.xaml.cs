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
            }
        }

        //mainImageBoxDraw draw = new mainImageBoxDraw();
        Skeleton skeltmp;

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void AllFrameReadyHandle(object sender, AllFramesReadyEventArgs e)
        {
            DateTime total_before = DateTime.Now;

            Skeleton[] skeletons = new Skeleton[0];
            mainImageBoxDraw draw = new mainImageBoxDraw(ref skeltmp, ref sensor);
            DrivingHandInfo info = new DrivingHandInfo(ref sensor);
            RawByteProcess byteprocess = new RawByteProcess();
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


            // open a drawing context and need to close it manully
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
                        System.Drawing.Rectangle lroi = info.getROI(HandsEnum.leftHand);
                        System.Drawing.Rectangle rroi = info.getROI(HandsEnum.rightHand);
                        byte[] lhand_p = byteprocess.mapColorImageROI(ref rawColorPixels, lroi);
                        byte[] rhand_p = byteprocess.mapColorImageROI(ref rawColorPixels, rroi);
                        BitmapSource lhand_bs = WriteableBitmap.Create(info.handWidth, info.handHeight,
                            96, 96, PixelFormats.Bgr32, null, lhand_p, info.handWidth*4);
                        BitmapSource rhand_bs = WriteableBitmap.Create(info.handWidth, info.handHeight,
                             96, 96, PixelFormats.Bgr32, null, rhand_p, info.handWidth * 4);




                        left_hand_color_box.Source = lhand_bs;
                        right_hand_color_box.Source = rhand_bs;
                    }
                }
            }


            // prevent drawing outside of our render area
            this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            dc.Close();


            DateTime total_after = System.DateTime.Now;
            TimeSpan total_ts = total_after.Subtract(total_before);
            double total_tf = total_ts.TotalMilliseconds;
            TotalCostTimeBox.Text = "TotalCostTime = "+total_tf.ToString()+" ms";
        }

        

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
    }
}