//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/21
// Description: hand recognition interface for palm or fist recognition
// version: v0.2 
// type: interface
//////////////////////////////////////////////////////////////////////////



namespace FesianXu.KinectGestureControl
{

    enum HandsTypeEnum { LeftPalm, LeftFist, RightPalm, RightFist};
    interface HandRecognition
    {
        /// <summary>
        /// judge the hands type
        /// </summary>
        /// <param name="img">hands roi color image</param>
        /// <returns>the type of hands</returns>
        HandsTypeEnum judgeHandsType(ref byte[] img, HandsEnum whichHand);
    }
}
