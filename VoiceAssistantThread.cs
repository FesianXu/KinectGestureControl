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
            t_VoiceAssistant = new Thread(VoiceAssistantHandle);
            t_VoiceAssistant.Start();
        }


        private void VoiceAssistantHandle()
        {
            //regResult.Text = voiceReg.RecognizedResult;
            
        }


    }

}
