//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/24
// Description: The main windows of the whole program, mainly include the Kinect
// AllFrameReadyHandle
// version: v1.1
// type: class
//////////////////////////////////////////////////////////////////////////


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
        private Communicater comm = new SerialCommunicater();

        // driving control
        private DrivingControl drvctl = new DrivingControl();

    
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
            drvctl.updateCommunicater(ref comm);
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

                        } // try to not ROI out of bounds
                        catch (ROIOutOfBoundsException excep)
                        {
                            HandsROIStatusBox.Text = "Hands OutOfBounds";
                        }

                        // driving steering wheel
                        draw.drawSteeringWheel(info.LeftHandPoint, info.RightHandPoint);

                        //if (info.Radius < DrivingHandInfo.bothHandsRadius_threshold
                        //    && info.WhichHandInFront == HandInFront.SamePlane)
                        //{
                        //    draw.drawSteeringWheel(info.LeftHandPoint, info.RightHandPoint);
                        //}
                        //else
                        //{
                        //    if (info.Left2CenterDistance > DrivingHandInfo.singleHandsAway_threshold
                        //        || info.WhichHandInFront == HandInFront.RightHand)
                        //    {
                        //        draw.drawSingleHandInWheel(info.RightHandPoint, info.OldCenterPosition, HandsEnum.rightHand);
                        //    }
                        //    else if (info.Right2CenterDistance > DrivingHandInfo.singleHandsAway_threshold
                        //        || info.WhichHandInFront == HandInFront.LeftHand)
                        //    {
                        //        draw.drawSingleHandInWheel(info.LeftHandPoint, info.OldCenterPosition, HandsEnum.leftHand);
                        //    }
                        //    else
                        //    {
                        //        draw.drawSteeringWheel(info.LeftHandPoint, info.RightHandPoint);
                        //    }
                        //}

                        //communication with the slaver
                        if (comm.PortStatus == SerialPortStatusEnum.Opened)
                        {
                            drvctl.driveYaw(info.Angle);
                            var buf = comm.ReceiveBuf;
                            if (buf != null)
                            {
                                var data2chars = System.Text.Encoding.ASCII.GetChars(buf);
                                string ss = new string(data2chars);
                                serialReceiveDataBox.Text = ss;
                            }
                            
                        }

                        ////////////////////////////////////////////////////////////////////////////
                    }
                }
            }

            // prevent drawing outside of our render area
            this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            dc.Close();
            // calculate the costing time
            DateTime total_after = System.DateTime.Now;
            TimeSpan total_ts = total_after.Subtract(total_before);
            double total_tf = total_ts.TotalMilliseconds;
            TotalCostTimeBox.Text = "TotalCostTime = " + total_tf.ToString() + " ms";
        }

 
    }
}