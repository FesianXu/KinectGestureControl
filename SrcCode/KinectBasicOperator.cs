//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/22
// Description: the kinect basic operators, all kinect related class should directly or indirectly 
// inherit from it.
// version: v1.0 
// type: class
//////////////////////////////////////////////////////////////////////////


using System;
using Microsoft.Kinect;
using System.Windows;

namespace FesianXu.KinectGestureControl
{
    class KinectBasicOperator
    {

        protected int img_size_width = 640;
        protected int img_size_height = 480;
        protected int img_channels = 4;
        protected int hands_size_width = 64;
        protected int hands_size_height = 64;
        protected int hands_channels = 3;


        /// <summary>
        /// compute the angle of the steering wheel
        /// </summary>
        /// <param name="pos_left">left hand position</param>
        /// <param name="pos_right">right hand position</param>
        /// <param name="radius">the radius between both hands</param>
        /// <returns></returns>
        protected double computeAngle(Point pos_left, Point pos_right, double radius)
        {
            double angle = 0;
            double h = 0.5 * (pos_left.Y - pos_right.Y);
            try
            {
                angle = Math.Asin(h / radius) * 180 / Math.PI;
            }
            catch (System.DivideByZeroException ex)
            {
                angle = 0;
            }
            return angle;

        }


        /// <summary>
        /// the distance between both hands in 2D
        /// </summary>
        /// <param name="lh">left hand position</param>
        /// <param name="rh">right hand position</param>
        /// <returns></returns>
        protected double distance2d(Point lh, Point rh)
        {
            return Math.Sqrt(Math.Pow(lh.X - rh.X, 2) + Math.Pow(lh.Y - rh.Y, 2));
        }


        /// <summary>
        /// the distance between both hands in 3D
        /// </summary>
        /// <param name="lh">left hand position</param>
        /// <param name="rh">right hand position</param>
        /// <returns></returns>
        protected double distance3d(Joint lh, Joint rh)
        {
            double disX = lh.Position.X - rh.Position.X;
            double disY = lh.Position.Y - rh.Position.Y;
            double disZ = lh.Position.Z - rh.Position.Z;
            double dis = Math.Sqrt(Math.Pow(disX, 2) + Math.Pow(disY, 2) + Math.Pow(disZ, 2));
            return dis;
        }


        /// <summary>
        /// the mean filter
        /// </summary>
        /// <param name="rawData">raw data</param>
        /// <returns>the mean filted value</returns>
        protected double meanFilter(ref double[] rawData)
        {
            int len = rawData.Length;
            double sum = 0;
            for (int i = 0; i < len; i++)
            {
                sum += rawData[i];
            }
            double mean = 0;
            try
            {
                mean = sum / len;
            }
            catch (System.DivideByZeroException ex)
            {
                mean = 0;
            }
            return mean;
        }




    }
}
