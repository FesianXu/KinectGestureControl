using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FesianXu.KinectGestureControl
{
    enum UserPriorityEnum { Master, Guest, NoAuthorization};

    class UserPriorityManager:SystemControlManager
    {
        private UserPriorityEnum currentPriority;
        private string va_cmd;
        private bool isBeginAuthorizationThread = false;
        private bool isAuthorized = false;
        private bool haveCalledIdentity = false;

        private Chris assistant;

        public UserPriorityManager()
        {
            currentPriority = UserPriorityEnum.NoAuthorization;
        }

        public void updateAssistant(ref Chris chris)
        {
            assistant = chris;
        }

        public bool execute()
        {
            if (assistant.IsUsedVoiceAssistant == false)
                return false;

            if (assistant.voiceRecog.RecognizedResultSemantic == assistant.SpeechGrammar.Chris &&
    assistant.voiceRecog.regStatus == SpeechRecognizeStatusEnum.Recognized)
            {
                assistant.playWhatUp();
                assistant.voiceRecog.regStatus = SpeechRecognizeStatusEnum.Rejected;
            } // call chris
            
            if (assistant.voiceRecog.RecognizedResultSemantic == assistant.SpeechGrammar.RequestForAuthorization &&
                assistant.voiceRecog.regStatus == SpeechRecognizeStatusEnum.Recognized)
            {
                exec_Authorization();
                assistant.voiceRecog.regStatus = SpeechRecognizeStatusEnum.Rejected;
            } // request for authorization
            //playIdentityClaim();

            if (assistant.voiceRecog.RecognizedResultSemantic == assistant.SpeechGrammar.LogOut &&
                assistant.voiceRecog.regStatus == SpeechRecognizeStatusEnum.Recognized)
            {
                exec_LogOut();
                assistant.voiceRecog.regStatus = SpeechRecognizeStatusEnum.Rejected;
            } // log out



            if (assistant.voiceRecog.RecognizedResultSemantic == assistant.SpeechGrammar.RunTheKinect &&
                assistant.voiceRecog.regStatus == SpeechRecognizeStatusEnum.Recognized)
            {
                exec_RunTheKinect();
                assistant.voiceRecog.regStatus = SpeechRecognizeStatusEnum.Rejected;
            } // run the kinect

            if (assistant.voiceRecog.RecognizedResultSemantic == assistant.SpeechGrammar.StopTheKinect &&
    assistant.voiceRecog.regStatus == SpeechRecognizeStatusEnum.Recognized)
            {
                exec_StopTheKinect();
                assistant.voiceRecog.regStatus = SpeechRecognizeStatusEnum.Rejected;
            } // stop the kinect



            return true;
        }


        private void exec_StopTheKinect()
        {
            if (currentPriority == UserPriorityEnum.Master)
            {
                assistant.playAsYouWant();
                KinectRunningBeginWay = KinectRunBeginWayEnum.NoRunningNow;
            }
        }


        private void exec_RunTheKinect()
        {
            if (currentPriority == UserPriorityEnum.Master)
            {
                assistant.playAsYouWant();
                KinectRunningBeginWay = KinectRunBeginWayEnum.VA_Init;
            }
        }


        private void exec_LogOut()
        {
            assistant.playLogOut();
            haveCalledIdentity = false;
            isAuthorized = false;
            currentPriority = UserPriorityEnum.NoAuthorization;
        }

        private void playIdentityClaim()
        {
            while (true)
            {
                if (haveCalledIdentity == false && isAuthorized == true)
                {
                    if (currentPriority == UserPriorityEnum.Master)
                        assistant.playIdentityMaster();
                    else if (currentPriority == UserPriorityEnum.Guest)
                        assistant.playIdentityGuest();
                    haveCalledIdentity = true;
                    assistant.clearTheVoiceRecognitionResult();
                }
            }
        }



        private void exec_Authorization()
        {
            assistant.playAuthorizing();
            haveCalledIdentity = false;
            if (isBeginAuthorizationThread == false && isAuthorized == false)
            {
                Thread t_Authorization = new Thread(AuthorizationHandle);
                Thread t_call = new Thread(playIdentityClaim);
                t_Authorization.IsBackground = true;
                t_call.IsBackground = true;
                t_Authorization.Priority = ThreadPriority.Normal;
                isBeginAuthorizationThread = true;
                t_Authorization.Start();
                t_call.Start();
            }
            
        }


        private void AuthorizationHandle()
        {
            while (true)
            {
                if (isAuthorized == false)
                {
                    if (assistant.voiceRecog.RecognizedResultSemantic == assistant.SpeechGrammar.Master)
                    {
                        currentPriority = UserPriorityEnum.Master;
                        isAuthorized = true;
                    }
                    else if (assistant.voiceRecog.RecognizedResultSemantic == assistant.SpeechGrammar.Guest)
                    {
                        currentPriority = UserPriorityEnum.Guest;
                        isAuthorized = true;
                    }
                }
                //if (assistant.voiceRecog.RecognizedResultSemantic != assistant.SpeechGrammar.Master ||
                //   assistant.voiceRecog.RecognizedResultSemantic != assistant.SpeechGrammar.Guest)
                //{
                //    currentPriority = UserPriorityEnum.NoAuthorization;
                //    isAuthorized = false;
                //}
            }
        }





        public UserPriorityEnum CurrentPriority { get; }
    }
}
