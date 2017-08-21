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
        private const double notInSamePlane_threshold = 0.2f;
        private const double bothHandsRadius_threshold = 50;
        private const double singleHandsAway_threshold = 70;

        //driving hands info
        private double angle;
        private double angle_rate;
        private double old_angle;

        public DrivingHandInfo(ref KinectSensor sensor):base(ref sensor)
        {
        }

        public void updateSkeleton(ref Skeleton skeleton) {
            skel = skeleton;
        }

        public void computeKeyJoints()
        {
            jspine_shoulder = skel.Joints[JointType.ShoulderCenter];
            jwrist_left = skel.Joints[JointType.WristLeft];
            jwrist_right = skel.Joints[JointType.WristRight];
            jleft3d = skel.Joints[JointType.HandLeft];
            jright3d = skel.Joints[JointType.HandRight];
        }

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
            old_angle = angle;
        }


        public System.Drawing.Rectangle getROI(HandsEnum whichHand)
        {
            if (whichHand == HandsEnum.leftHand)
                return new System.Drawing.Rectangle((int)jleft2d_color.X - hands_size_width / 2, (int)jleft2d_color.Y - hands_size_height / 2,
                                                   hands_size_width, hands_size_height);
            else if (whichHand == HandsEnum.rightHand)
                return new System.Drawing.Rectangle((int)jright2d_color.X - hands_size_width / 2, (int)jright2d_color.Y - hands_size_height / 2,
                                   hands_size_width, hands_size_height);
            else
                return new System.Drawing.Rectangle();
        }


        public int handWidth { get { return hands_size_width; } }
        public int handHeight { get { return hands_size_height; } }
        public double Angle { get { return angle; } }
        public double AngleRate { get { return angle_rate; } }

    }
}
