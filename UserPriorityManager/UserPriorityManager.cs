using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FesianXu.KinectGestureControl
{
    enum UserPriorityEnum { Master, Guest, NoAuthorization};

    class UserPriorityManager:SystemControlManager
    {
        private UserPriorityEnum currentPriority;
        private string va_cmd;

        private Chris assistant;

        public UserPriorityManager()
        {
        }

        public void updateAssistant(ref Chris chris)
        {
            assistant = chris;
        }

        public void execute()
        {
            if (assistant.voiceRecog.RecognizedResultSemantic == "Chris" &&
    assistant.voiceRecog.regStatus == SpeechRecognizeStatusEnum.Recognized)
            {
                assistant.playWhatUp();
                assistant.voiceRecog.regStatus = SpeechRecognizeStatusEnum.Rejected;
            }
            if (assistant.voiceRecog.RecognizedResultSemantic == "Stop" &&
                assistant.voiceRecog.regStatus == SpeechRecognizeStatusEnum.Recognized)
            {
                assistant.playMaster();
                assistant.voiceRecog.regStatus = SpeechRecognizeStatusEnum.Rejected;
            }
        }




        public UserPriorityEnum CurrentPriority { get; }
    }
}
