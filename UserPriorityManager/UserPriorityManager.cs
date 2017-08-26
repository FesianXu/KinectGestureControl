//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/26
// Description: Kinect user priority manager
// version: v1.1
// type: class
// Handle: 
// void AuthorizationThreadHandle()
//////////////////////////////////////////////////////////////////////////

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
                exec_ChrisGreet();
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


        /// <summary>
        /// execute the chris' greeting
        /// </summary>
        private void exec_ChrisGreet()
        {
            if (currentPriority == UserPriorityEnum.NoAuthorization)
            {
                assistant.playWhatUp();
            }
            else if (currentPriority == UserPriorityEnum.Master || 
                     currentPriority == UserPriorityEnum.Guest)
            {
                assistant.playYes();
            }
        }


        /// <summary>
        /// execute stop the kinect
        /// </summary>
        private void exec_StopTheKinect()
        {
            if (currentPriority == UserPriorityEnum.Master)
            {
                assistant.playAsYouWant();
                KinectRunningBeginWay = KinectRunBeginWayEnum.NoRunningNow;
            }
            else if (currentPriority == UserPriorityEnum.Guest || 
                     currentPriority == UserPriorityEnum.NoAuthorization)
            {
                assistant.playILimitedPriority();
            }
        }


        /// <summary>
        /// execute run the kinect
        /// </summary>
        private void exec_RunTheKinect()
        {
            if (currentPriority == UserPriorityEnum.Master)
            {
                assistant.playAsYouWant();
                KinectRunningBeginWay = KinectRunBeginWayEnum.VA_Init;
            }
            else if (currentPriority == UserPriorityEnum.Guest)
            {
                assistant.playILimitedPriority();
                KinectRunningBeginWay = KinectRunBeginWayEnum.NoRunningNow;
            }
        }


        /// <summary>
        /// execute log out
        /// </summary>
        private void exec_LogOut()
        {
            assistant.playLogOut();
            haveCalledIdentity = false;
            isAuthorized = false;
            currentPriority = UserPriorityEnum.NoAuthorization;
        }

        /// <summary>
        /// execute claim your identity
        /// </summary>
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


        /// <summary>
        /// execute authorizing
        /// </summary>
        private void exec_Authorization()
        {
            assistant.playAuthorizing();
            haveCalledIdentity = false;
            if (isBeginAuthorizationThread == false && isAuthorized == false)
            {
                Thread t_Authorization = new Thread(AuthorizationThreadHandle);
                Thread t_call = new Thread(playIdentityClaim);
                t_Authorization.IsBackground = true;
                t_call.IsBackground = true;
                t_Authorization.Priority = ThreadPriority.Normal;
                isBeginAuthorizationThread = true;
                t_Authorization.Start();
                t_call.Start();
            }
            
        }


        /// <summary>
        /// authorizing thread
        /// </summary>
        private void AuthorizationThreadHandle()
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
