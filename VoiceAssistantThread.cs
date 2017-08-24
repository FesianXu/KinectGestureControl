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
            if (assistant.voiceRecog.haveStartedVAThread == false)
            {
                t_VoiceAssistant = new Thread(VoiceRecognitionHandle);
                t_VoiceAssistant.IsBackground = true; 
                // set it to background thread and it will be killed after the main thread dead
                t_VoiceAssistant.Start();
                assistant.voiceRecog.haveStartedVAThread = true;
            }

        }


        private void VoiceRecognitionHandle()
        {
            //regResult.Text = voiceReg.RecognizedResult;
            while (true)
            {
                if (assistant.voiceRecog.RecognizedResultSemantic == "LEFT" && 
                    assistant.voiceRecog.regStatus == SpeechRecognizeStatusEnum.Recognized)
                {
                    assistant.playWhatUp();
                    assistant.voiceRecog.regStatus = SpeechRecognizeStatusEnum.Rejected;
                }
                // recognize the words and show them in UI

                // recognize the words and execute

            
            }

        }







    }

}
