//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/21
// Description: To map the raw byte stream to a Image or byte with ROI(Region of Interest).
// version: v0.2 
// type: interface
//////////////////////////////////////////////////////////////////////////



namespace FesianXu.KinectGestureControl
{
    using System.Drawing;

    interface RawByteMapping
    {
        /// <summary>
        /// mapping the raw color frames byte stream to ROI byte stream with same channels
        /// </summary>
        /// /// <param name="raw">raw color frame byte stream</param>
        /// <param name="roi">region of interest</param>
        /// <param name="channels">the number of channels both in origin and target iamges, normally set
        /// to 4, with BGR and alpha channel</param>
        /// <returns>color roi byte stream</returns>
         
        byte[] mapColorImageROI(ref byte[] raw, Rectangle roi, int channels=4);
        /// <summary>
        /// mapping the raw depth frames byte stream to ROI byte stream with same channels
        /// </summary>
        /// <param name="raw">raw depth frame byte stream</param>
        /// <param name="roi">region of interest</param>
        /// <param name="lthresh">left hand distance threshold</param>
        /// <param name="rthresh">right hand distance threshold</param>
        /// <returns>depth roi byte stream</returns>
        byte[] mapDepthImageROI(ref byte[] raw, Rectangle roi, int lthresh, int rthresh);

        /// <summary>
        /// remove the alpha channel in bgra images
        /// </summary>
        /// <param name="bgra">bgra frame raw byte stream</param>
        /// <returns>bgr byte stream</returns>
        byte[] removeAlphaChannel(ref byte[] bgra);
    }
}
