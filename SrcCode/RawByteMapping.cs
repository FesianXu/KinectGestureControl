//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/21
// Description: To map the raw byte stream to a Image or byte with ROI(Region of Interest).
// version: v1.0
// type: interface & class
//////////////////////////////////////////////////////////////////////////



namespace FesianXu.KinectGestureControl
{
    using System.Drawing;
    using Microsoft.Kinect;

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

    class RawByteProcess: KinectBasicOperator, RawByteMapping
    {
        /// <summary>
        /// mapping the specific ROI of a color image byte stream   
        /// </summary>
        /// <param name="raw">raw color image stream</param>
        /// <param name="roi">the roi area</param>
        /// <param name="channels">the channel of the raw image, normally set to 4 with BGR and alpha</param>
        /// <returns></returns>
        public byte[] mapColorImageROI(ref byte[] raw, Rectangle roi, int channels = 4)
        {
            int roix = roi.X;
            int roiy = roi.Y;
            int width = roi.Width;
            int height = roi.Height;
            byte[] img = new byte[width * height * 4];
            for (int y = roiy; y < roiy + height; y++)
            {
                for (int x = roix; x < roix + width; x++)
                {
                    for (int c = 0; c < channels; c++)
                    {
                        img[4 * ((y - roiy) * width + (x - roix)) + c] = raw[4 * (img_size_width * y + x) + c];
                    }
                }
            }
            return img;
        }


        /// <summary>
        /// mapping the specific ROI of a depth image byte stream   
        /// </summary>
        /// <param name="raw">raw depth image stream</param>
        /// <param name="roi">the roi area</param>
        /// <param name="lthresh">the left hand threshold</param>
        /// <param name="rthresh">the right hand threshold</param>
        /// <returns></returns>
        public byte[] mapDepthImageROI(ref byte[] raw, Rectangle roi, int lthresh, int rthresh)
        {
            int roix = roi.X;
            int roiy = roi.Y;
            int width = roi.Width;
            int height = roi.Height;
            short[] part = new short[width * height];
            byte[] img = new byte[width * height];
            for (int y = roiy; y < roiy + height; y++)
            {
                for (int x = roix; x < roix + width; x++)
                {
                    part[(y - roiy) * width + (x - roix)] = raw[img_size_width * y + x];
                }
            }
            for (int ind = 0; ind < part.Length; ind++)
            {
                int depth = (short)(part[ind] >> DepthImageFrame.PlayerIndexBitmaskWidth);
                int player = part[ind] & DepthImageFrame.PlayerIndexBitmask;
                depth = (
                    depth > 1000 &&
                    (depth < lthresh || depth < rthresh) &&
                    player > 0
                    ) ? depth : 0;
                img[ind] = (byte)(depth * 255.0f / 4000);
            }
            return img;
        }


        /// <summary>
        /// remove the alpha channel in the 4-channel images
        /// </summary>
        /// <param name="bgra">the raw image byte stream with bgra channels</param>
        /// <returns></returns>
        public byte[] removeAlphaChannel(ref byte[] bgra)
        {
            byte[] bgr = new byte[hands_size_width * hands_size_height * hands_channels];
            for (int y = 0; y < hands_size_height; y++)
            {
                for (int x = 0; x < hands_size_width; x++)
                {
                    for (int c = 0; c < hands_channels; c++)
                    {
                        bgr[3 * (hands_size_width * y + x) + c] = bgra[4 * (hands_size_width * y + x) + c];
                    }
                }
            }
            return bgr;
        }



    }

}
