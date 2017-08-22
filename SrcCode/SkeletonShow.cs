//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/22
// Description: the class about showing skeletons
// version: v1.0 
// type: class
//////////////////////////////////////////////////////////////////////////

using Microsoft.Kinect;
using System.Windows;

namespace FesianXu.KinectGestureControl
{
    class SkeletonShow: KinectBasicOperator
    {
        private static KinectSensor sensor;
        public SkeletonShow(ref KinectSensor s)
        {
            sensor = s;
        }

        /// <summary>
        /// mapping 3D Point to 2D Point in the depth image way
        /// </summary>
        /// <param name="skelpoint">3d skeleton point</param>
        /// <returns></returns>
        protected Point SkeletonPointToScreen_DepthImage(SkeletonPoint skelpoint)
        {
            DepthImagePoint depthPoint = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// mapping 3D Point to 2D Point in the color image way
        /// </summary>
        /// <param name="skelpoint">the 3d skeleton point</param>
        /// <returns></returns>
        protected Point SkeletonPointToScreen_ColorImage(SkeletonPoint skelpoint)
        {
            ColorImagePoint colorPoint = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skelpoint, ColorImageFormat.RgbResolution640x480Fps30);
            return new Point(colorPoint.X, colorPoint.Y);
        }
    }
}
