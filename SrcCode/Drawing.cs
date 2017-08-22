//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/21
// Description: The interface for drawing in the mainImageBox, mainly include the steeringDrawing, 
// matchstick men drawing and skeleton's joints drawing.
// version: v1.0
// type: interface
//////////////////////////////////////////////////////////////////////////



namespace FesianXu.KinectGestureControl
{
    using System.Windows;
    using Microsoft.Kinect;
    using System.Windows.Media;

    enum HandsEnum { leftHand, rightHand, bothHand, NoHand};

    interface Drawing
    {
        /// <summary>
        /// To draw the background of the mainImageBox which show the matchstick men.
        /// </summary>
        void drawBackgraoud();

        /// <summary>
        /// To draw the matchstick men in the mainImageBox and draw the joints.
        /// </summary>
        /// <param name="skeleton">skeletons reference</param>
        void drawMatchStickMen(ref Skeleton skeleton);

        /// <summary>
        /// To draw the steering wheel in the mainImageBox with the left hand point and the right 
        /// hand point.
        /// </summary>
        /// <param name="lh">lfet hand point</param>
        /// <param name="rh">right hand point</param>
        void drawSteeringWheel(Point lh, Point rh);

        /// <summary>
        /// To draw a single hand driving scene.
        /// </summary>
        /// <param name="shand">single hand point</param>
        /// <param name="center2d">the last time both hand driving center point</param>
        /// <param name="whichHand">which hand is left to drive</param>
        void drawSingleHandInWheel(Point shand, Point center2d, HandsEnum whichHand);


        /// <summary>
        /// add the drawing context, it should be reference or it will alway refresh
        /// </summary>
        /// <param name="dc">the drawing context</param>
        void addDrawingContext(ref DrawingContext dc);

        /// <summary>
        /// make the clip bounds to forbid to render out of canvas
        /// </summary>
        /// <param name="width">the render width</param>
        /// <param name="height">the render height</param>
        /// <param name="thickness">the clip bounds render thickness</param>
        void getRenderAndClipBounds(float width, float height, double thickness);

        /// <summary>
        /// update the skeleton
        /// </summary>
        /// <param name="skeleton">the new skeleton</param>
        void updateSkeleton(ref Skeleton skeleton);

    }
}
