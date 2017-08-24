//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/24
// Description: The voice recognition interface
// version: v1.1
// type: interface
//////////////////////////////////////////////////////////////////////////


using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FesianXu.KinectGestureControl
{
    interface VoiceRecognition
    {
        void updateAndInitiate(ref KinectSensor s);

        string RecognizedResult { get; set; }
        string RecognizedResultSemantic { get; set; }
        SpeechRecognizeStatusEnum regStatus { get; set; }
        bool haveStartedVAThread { get; set; }

    }
}
