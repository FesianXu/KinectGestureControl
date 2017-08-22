//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/22
// Description: the exception about the ROI is out of bounds
// version: v1.0 
// type: exception class
//////////////////////////////////////////////////////////////////////////

namespace FesianXu.KinectGestureControl
{
    using System;

    public class ROIOutOfBoundsException: ApplicationException
    {
        private Exception innerException;
        public bool ROIerror { get;}
        public string Error { get; }

        public ROIOutOfBoundsException()
        {
            ROIerror = true;
        }

        public ROIOutOfBoundsException(string msg) : base(msg)
        {
            ROIerror = true;
            Error = msg;
        }

        public ROIOutOfBoundsException(string msg, Exception innerException) : base(msg)
        {
            this.innerException = innerException;
            Error = msg;
            ROIerror = true;
        }
    }
}
