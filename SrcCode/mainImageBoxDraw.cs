//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/21
// Description: hand recognition interface for palm or fist recognition
// version: v0.2 
// type: class, implement for interface
//////////////////////////////////////////////////////////////////////////

namespace FesianXu.KinectGestureControl
{
    using System;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Windows;

    enum HandInFront { LeftHand, RightHand, SamePlane, NoDetect };
    enum ImageFrameTypes { ColorFrame, DepthFrame, SkeletonFrame };

    class mainImageBoxDraw : SkeletonShow, Drawing
    {
        private Skeleton skel; // the skeleton
        private DrawingContext drawingContext;
        private KinectSensor sensor;
        private static Pen direction_pen = new Pen(Brushes.Red, 3); // the driving direction pen
        private static Pen wheel_pen = new Pen(Brushes.Yellow, 3); // the steering wheel pen
        private static Pen horizon_pen = new Pen(Brushes.LightGreen, 3); // the horizon pen

        private static readonly Pen trackedBonePen = new Pen(Brushes.Green, 6); //Pen used for drawing bones that are currently tracked
        private static readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1); //Pen used for drawing bones that are currently inferred
        // Brush used for drawing joints that are currently tracked                                                                    
        private static readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
        // Brush used for drawing joints that are currently inferred
        private static readonly Brush inferredJointBrush = Brushes.Yellow;
        // Brush used to draw skeleton center point
        private static readonly Brush centerPointBrush = Brushes.Red;

        // Thickness of drawn joint lines
        private const double JointThickness = 3;
        // Thickness of body center ellipse
        private const double BodyCenterThickness = 10;

        private static float RenderWidth ;
        private static float RenderHeight;
        private static double ClipBoundsThickness;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="s"></param>
        public mainImageBoxDraw(ref Skeleton skeleton, ref KinectSensor s) : base(ref s)
        {
            skel = skeleton;
            sensor = s;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        public void addDrawingContext(ref DrawingContext dc)
        {
            drawingContext = dc;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="skeleton"></param>
        public void updateSkeleton(ref Skeleton skeleton)
        {
            skel = skeleton;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="thickness"></param>
        public void getRenderAndClipBounds(float width, float height, double thickness)
        {
            RenderWidth = width;
            RenderHeight = height;
            ClipBoundsThickness = thickness;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="jointType0"></param>
        /// <param name="jointType1"></param>
        /// <param name="ftype"></param>
        private void drawBoneInImage(JointType jointType0,
            JointType jointType1,
            ImageFrameTypes ftype = ImageFrameTypes.ColorFrame)
        {
            Joint joint0 = skel.Joints[jointType0];
            Joint joint1 = skel.Joints[jointType1];
            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }
            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = trackedBonePen;
            }

            if (ftype == ImageFrameTypes.ColorFrame)
                drawingContext.DrawLine(drawPen,
                    this.SkeletonPointToScreen_ColorImage(joint0.Position),
                    this.SkeletonPointToScreen_ColorImage(joint1.Position));
            else if (ftype == ImageFrameTypes.DepthFrame)
                drawingContext.DrawLine(drawPen,
                    this.SkeletonPointToScreen_DepthImage(joint0.Position),
                    this.SkeletonPointToScreen_DepthImage(joint1.Position));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ftype"></param>
        private void drawBonesAndJoints(ImageFrameTypes ftype = ImageFrameTypes.ColorFrame)
        {
            // Render Torso
            this.drawBoneInImage(JointType.Head, JointType.ShoulderCenter, ftype);
            this.drawBoneInImage(JointType.ShoulderCenter, JointType.ShoulderLeft, ftype);
            this.drawBoneInImage(JointType.ShoulderCenter, JointType.ShoulderRight, ftype);
            this.drawBoneInImage(JointType.ShoulderCenter, JointType.Spine, ftype);
            this.drawBoneInImage(JointType.Spine, JointType.HipCenter, ftype);
            this.drawBoneInImage(JointType.HipCenter, JointType.HipLeft, ftype);
            this.drawBoneInImage(JointType.HipCenter, JointType.HipRight, ftype);

            // Left Arm
            this.drawBoneInImage(JointType.ShoulderLeft, JointType.ElbowLeft, ftype);
            this.drawBoneInImage(JointType.ElbowLeft, JointType.WristLeft, ftype);
            this.drawBoneInImage(JointType.WristLeft, JointType.HandLeft, ftype);

            // Right Arm
            this.drawBoneInImage(JointType.ShoulderRight, JointType.ElbowRight, ftype);
            this.drawBoneInImage(JointType.ElbowRight, JointType.WristRight, ftype);
            this.drawBoneInImage(JointType.WristRight, JointType.HandRight, ftype);

            // Left Leg
            this.drawBoneInImage(JointType.HipLeft, JointType.KneeLeft, ftype);
            this.drawBoneInImage(JointType.KneeLeft, JointType.AnkleLeft, ftype);
            this.drawBoneInImage(JointType.AnkleLeft, JointType.FootLeft, ftype);

            // Right Leg
            this.drawBoneInImage(JointType.HipRight, JointType.KneeRight, ftype);
            this.drawBoneInImage(JointType.KneeRight, JointType.AnkleRight, ftype);
            this.drawBoneInImage(JointType.AnkleRight, JointType.FootRight, ftype);

            // Render Joints
            foreach (Joint joint in skel.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    if (ftype == ImageFrameTypes.ColorFrame)
                        drawingContext.DrawEllipse(drawBrush, null,
                            this.SkeletonPointToScreen_ColorImage(joint.Position), JointThickness, JointThickness);
                    else if (ftype == ImageFrameTypes.DepthFrame)
                        drawingContext.DrawEllipse(drawBrush, null,
                            this.SkeletonPointToScreen_DepthImage(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ftype"></param>
        private void drawEllipse(ImageFrameTypes ftype = ImageFrameTypes.ColorFrame)
        {
            if (ftype == ImageFrameTypes.ColorFrame)
                drawingContext.DrawEllipse(centerPointBrush,
                    null,
                    SkeletonPointToScreen_ColorImage(skel.Position),
                    BodyCenterThickness,
                    BodyCenterThickness);
            else if (ftype == ImageFrameTypes.DepthFrame)
                drawingContext.DrawEllipse(centerPointBrush,
                    null,
                    SkeletonPointToScreen_DepthImage(skel.Position),
                    BodyCenterThickness,
                    BodyCenterThickness);
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="RenderWidth"></param>
        /// <param name="RenderHeight"></param>
        /// <param name="ClipBoundsThickness"></param>
        private void RenderClippedEdges()
        {
            if (skel.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skel.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skel.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skel.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }



        public void drawBackgraoud()
        {
            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
        }



        public int drawMatchStickMen(ref Skeleton[] skeleton_list)
        {
            if (skeleton_list.Length != 0)
            {
                for (int i = 0; i < skeleton_list.Length; i++)
                {
                    var skeltmp = skeleton_list[i];
                    updateSkeleton(ref skeltmp);
                    RenderClippedEdges();
                    if (skeltmp.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        drawBonesAndJoints();
                    }
                    else if (skeltmp.TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        drawEllipse();
                    }
                }
            }
            return skeleton_list.Length;
        }



        public void drawSteeringWheel(Point lh, Point rh)
        {
            Point pos_center2d = new Point((lh.X + rh.X) / 2, (lh.Y + rh.Y) / 2);
            double radius = Math.Sqrt(Math.Pow((lh.X - rh.X), 2) + Math.Pow((lh.Y - rh.Y), 2)) / 2;
            Point hori_left = new Point(pos_center2d.X - radius, pos_center2d.Y);
            Point hori_right = new Point(pos_center2d.X + radius, pos_center2d.Y);
            drawingContext.DrawLine(direction_pen, lh, rh);
            drawingContext.DrawLine(horizon_pen, hori_left, hori_right);
            drawingContext.DrawEllipse(null,
                wheel_pen,
                pos_center2d,
                radius,
                radius
                );
        }


        public void drawSingleHandInWheel(Point shand, Point center2d, HandsEnum whichHand)
        {
            double radius = distance2d(shand, center2d);
            if (whichHand == HandsEnum.leftHand)
            {
                Point hori_left = new Point(center2d.X - radius, center2d.Y);
                Point hori_right = new Point(center2d.X + radius, center2d.Y);
                drawingContext.DrawLine(direction_pen, shand, center2d);
                drawingContext.DrawLine(horizon_pen, hori_left, hori_right);
                drawingContext.DrawEllipse(null,
                    wheel_pen,
                    center2d,
                    radius,
                    radius
                    );
            }
            else if (whichHand == HandsEnum.rightHand)
            {
                Point hori_left = new Point(center2d.X - radius, center2d.Y);
                Point hori_right = new Point(center2d.X + radius, center2d.Y);
                drawingContext.DrawLine(direction_pen, shand, center2d);
                drawingContext.DrawLine(horizon_pen, hori_left, hori_right);
                drawingContext.DrawEllipse(null,
                    wheel_pen,
                    center2d,
                    radius,
                    radius
                    );
            }
        }





    }
}
