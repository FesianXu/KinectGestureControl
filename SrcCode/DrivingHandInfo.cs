//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/24
// Description: The information about the driving hands like angle, angle_rate and
// so on
// version: v1.1
// type: class
// inherit from: SkeletonShow
//////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows;

namespace FesianXu.KinectGestureControl
{
    class DrivingHandInfo: SkeletonShow
    {
        private static Skeleton skel;
        private KinectSensor sensor;

        private Joint jspine_shoulder;
        private Joint jwrist_right;
        private Joint jwrist_left;
        private Joint jleft3d;
        private Joint jright3d;

        private Point jleft2d_color;
        private Point jright2d_color;
        private Point jleft2d_depth;
        private Point jright2d_depth;
        private Point pos_center2d_color;
        private Point horizon_left;
        private Point horizon_right;
        private double l2c_distance2d;
        private double r2c_distance2d;
        private double hands_distance2AxisZ;
        private double radius;
        private int dis2left;
        private int dis2right;

        private HandInFront whichHandInFront;
        // const thresholds
        public const double notInSamePlane_threshold = 0.2f;
        public const double bothHandsRadius_threshold = 50;
        public const double singleHandsAway_threshold = 70;

        //driving hands info
        private double angle;
        private double angle_rate;

        // old data, have to use the static variable or it will change as the new
        // instance created
        static private double old_angle = 0;
        static private double old_radius;
        static private Point old_jleft2d;
        static private Point old_jright2d;
        static private Joint old_jleft3d;
        static private Joint old_jright3d;
        static private Point old_pos_center2d;

        private bool isFirstToNormal = true;

        public DrivingHandInfo(ref KinectSensor sensor):base(ref sensor)
        {
            this.sensor = sensor;
        }


        /// <summary>
        /// update the skeleton
        /// </summary>
        /// <param name="skeleton">the new skeleton</param>
        public void updateSkeleton(ref Skeleton skeleton) {
            skel = skeleton;
        }


        /// <summary>
        /// compute the key joints
        /// </summary>
        public void computeKeyJoints()
        {
            jspine_shoulder = skel.Joints[JointType.ShoulderCenter];
            jwrist_left = skel.Joints[JointType.WristLeft];
            jwrist_right = skel.Joints[JointType.WristRight];
            jleft3d = skel.Joints[JointType.HandLeft];
            jright3d = skel.Joints[JointType.HandRight];
        }


        /// <summary>
        /// compute the key points and information
        /// </summary>
        public void computeKeyPointsAndInfo()
        {
            dis2left = (int)(jwrist_left.Position.Z * 1000);
            dis2right = (int)(jwrist_right.Position.Z * 1000);

            jleft2d_color = SkeletonPointToScreen_ColorImage(jleft3d.Position);
            jright2d_color = SkeletonPointToScreen_ColorImage(jright3d.Position);
            jleft2d_depth = SkeletonPointToScreen_DepthImage(jleft3d.Position);
            jright2d_depth = SkeletonPointToScreen_DepthImage(jright3d.Position);
            pos_center2d_color = new Point((jleft2d_color.X + jright2d_color.X) / 2, (jleft2d_color.Y + jright2d_color.Y) / 2);
            radius = Math.Sqrt(Math.Pow((jleft2d_color.X - jright2d_color.X), 2) 
                             + Math.Pow((jleft2d_color.Y - jright2d_color.Y), 2)) / 2;
            horizon_left = new Point(pos_center2d_color.X-radius, pos_center2d_color.Y);
            horizon_right = new Point(pos_center2d_color.X+radius, pos_center2d_color.Y);
            l2c_distance2d = distance2d(jleft2d_color, pos_center2d_color);
            r2c_distance2d = distance2d(jright2d_color, pos_center2d_color);
            hands_distance2AxisZ = jleft3d.Position.Z - jright3d.Position.Z;
            if (Math.Abs(hands_distance2AxisZ) < notInSamePlane_threshold)
                whichHandInFront = HandInFront.SamePlane;
            else if (Math.Abs(hands_distance2AxisZ) >= notInSamePlane_threshold && hands_distance2AxisZ > 0)
                whichHandInFront = HandInFront.RightHand;
            else if (Math.Abs(hands_distance2AxisZ) >= notInSamePlane_threshold && hands_distance2AxisZ < 0)
                whichHandInFront = HandInFront.LeftHand;

            angle = computeAngle(jleft2d_color, jright2d_color, radius);
            angle_rate = (angle - old_angle);
        }


        /// <summary>
        /// get the ROI of hands
        /// </summary>
        /// <param name="whichHand">which hands' ROI</param>
        /// <returns>the ROI</returns>
        public System.Drawing.Rectangle getROI(HandsEnum whichHand)
        {
            if (whichHand == HandsEnum.leftHand)
            {
                System.Drawing.Rectangle roi = new System.Drawing.Rectangle((int)jleft2d_color.X - hands_size_width / 2, (int)jleft2d_color.Y - hands_size_height / 2,
                                                   hands_size_width, hands_size_height);
                if (isROIOutOfBounds(roi))
                {
                    var excep = new ROIOutOfBoundsException("ROI left hand area is out of the image's bounds!");
                    throw excep;
                }
                return roi;
            }
            else if (whichHand == HandsEnum.rightHand)
            {
                System.Drawing.Rectangle roi = new System.Drawing.Rectangle((int)jright2d_color.X - hands_size_width / 2, (int)jright2d_color.Y - hands_size_height / 2,
                                                   hands_size_width, hands_size_height);
                if (isROIOutOfBounds(roi))
                {
                    var excep = new ROIOutOfBoundsException("ROI right hand area is out of the image's bounds!");
                    throw excep;
                }
                return roi; 
            } 
            else
            {
                return new System.Drawing.Rectangle();
            }
                
        }


        /// <summary>
        /// is ROI out of bounds?
        /// </summary>
        /// <param name="roi">the result</param>
        /// <returns></returns>
        private bool isROIOutOfBounds(System.Drawing.Rectangle roi)
        {
            int x = roi.X;
            int y = roi.Y;
            int width = roi.Width;
            int height = roi.Height;
            if (x < 0 || y < 0)
                return true;
            if (x + width > img_size_width)
                return true;
            if (y + height > img_size_height)
                return true;

            return false;
        }


        /// <summary>
        /// update the old datas
        /// </summary>
        public void updateOldData()
        {
            old_jleft2d = jleft2d_color;
            old_jright2d = jright2d_color;
            old_jleft3d = jleft3d;
            old_jright3d = jright3d;
            old_pos_center2d = pos_center2d_color;
            old_radius = radius;
            old_angle = angle;
        }

        //visitors

        public int handWidth { get { return hands_size_width; } }
        public int handHeight { get { return hands_size_height; } }
        public double Angle { get { return angle; } }
        public double AngleRate { get { return angle_rate; } }
        public Point LeftHandPoint { get { return jleft2d_color; } }
        public Point RightHandPoint { get { return jright2d_color; } }
        public double Radius { get { return radius; } }
        public bool IsFirstToNormal { get { return isFirstToNormal; } set { isFirstToNormal = IsFirstToNormal; } }
        public HandInFront WhichHandInFront { get { return whichHandInFront; } }
        public double Left2CenterDistance { get { return l2c_distance2d; } }
        public double Right2CenterDistance { get { return r2c_distance2d; } }
        public Point OldCenterPosition { get { return old_pos_center2d; } }
    }
}
