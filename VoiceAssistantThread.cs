//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/24
// Description: The partial class of MainWindows, for the Voice Assistant's Thread
// version: v1.1
// type: partial class
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FesianXu.KinectGestureControl
{
    partial class MainWindow
    {
        private Thread t_VoiceAssistant;

        private void initVoiceAssistantThread()
        {
            // if the VA thread has not been started, start one
            if (voiceReg.haveStartedVAThread == false)
            {
                t_VoiceAssistant = new Thread(VoiceAssistantHandle);
                t_VoiceAssistant.Start();
                voiceReg.haveStartedVAThread = true;
            }
        }


        private void VoiceAssistantHandle()
        {
            //regResult.Text = voiceReg.RecognizedResult;
            if (voiceReg.RecognizedResultSemantic == "LEFT" && voiceReg.regStatus == SpeechRecognizeStatusEnum.Recognized)
            {
                assistant.playWhatUp();
                voiceReg.regStatus = SpeechRecognizeStatusEnum.Rejected;
            }
        }


    }

}
