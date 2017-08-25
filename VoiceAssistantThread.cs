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
                t_VoiceAssistant = new Thread(VoiceRecognitionThreadHandle);
                t_VoiceAssistant.IsBackground = true;
                // set it to background thread and it will be killed after the main thread dead
                t_VoiceAssistant.Priority = ThreadPriority.Highest;
                // assistant have the highest thread priority
                t_VoiceAssistant.Start();
                assistant.voiceRecog.haveStartedVAThread = true;
            }

        }


        private void VoiceRecognitionThreadHandle()
        {
            while (true)
            {
                // recognize the words and show them in UI
                VoiceRecognitionResultBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
                    new Action(updateVoiceRecognitionResultBox));
                // recognize the words and execute
                priorityManager.execute();
            }

        }

        private void updateVoiceRecognitionResultBox()
        {
            VoiceRecognitionResultBox.Text = assistant.voiceRecog.RecognizedResult;
        }



    }

}
