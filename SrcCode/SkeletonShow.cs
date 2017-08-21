using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        protected Point SkeletonPointToScreen_DepthImage(SkeletonPoint skelpoint)
        {
            DepthImagePoint depthPoint = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        protected Point SkeletonPointToScreen_ColorImage(SkeletonPoint skelpoint)
        {
            ColorImagePoint colorPoint = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skelpoint, ColorImageFormat.RgbResolution640x480Fps30);
            return new Point(colorPoint.X, colorPoint.Y);
        }
    }
}
