using System;

namespace FesianXu.KinectGestureControl
{
    public class ROIOutOfBoundsException: ApplicationException
    {
        private string error;
        private Exception innerException;

        public ROIOutOfBoundsException()
        {
        }

        public ROIOutOfBoundsException(string msg) : base(msg)
        {
            error = msg;
        }

        public ROIOutOfBoundsException(string msg, Exception innerException) : base(msg)
        {
            this.innerException = innerException;
            this.error = msg;
        }

        public string Error { get { return error; } }


    }
}
