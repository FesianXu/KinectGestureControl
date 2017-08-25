//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/24
// Description: The Voice Assistant instance, named Chris
// version: v1.1
// type: class
// inherit from: None
// implement of: VoiceAssistant
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Threading;

namespace FesianXu.KinectGestureControl
{
    /// <summary>
    /// Chris, The Voice Assistant instance
    /// </summary>
    class Chris : VoiceAssistant
    {
        private KinectSensor sensor;
        private string voiceRootFolderPath = @"../../Resources/voices/";
        private Dictionary<string, string> voicesDict = new Dictionary<string, string>();
        private SoundPlayer player = new SoundPlayer();
        private SpeechGrammarManager speechGrammar;


        // the path of the voice wave files
        private string response_Wait = @"response_wait.wav"; // "please wait"
        private string response_Welcome = @"response_welcome.wav"; // "welcome to kinect gesture control system!"
        private string response_AsYouWantMaster = @"response_as_you_want_master.wav"; // "as you want, master"
        private string response_Identifying = @"response_identifying.wav"; // "identifying"
        private string response_AuthorizationAccomplish = @"response_authorization_accomplish.wav"; // "authorization accomplish"
        private string response_guest = @"response_guest.wav"; // "guest"
        private string response_master = @"response_master.wav"; // "master"
        private string response_YourIdentityIs = @"response_your_identity_is.wav"; // "your identity is"
        private string response_KinectClosing = @"response_kinect_closing.wav"; // "Kinect Gesture System closing, wait for you next time"
        private string response_VoiceAssistant = @"response_voice_assistant.wav"; // "here is Chris, your voice assistant"
        private string response_SeeYou = @"response_see_you_next_time.wav"; // "see you next time"
        private string response_WhatUp = @"response_what_up.wav"; // "what's up"
        private string response_AuthorizationFailed = @"response_authorization_failed.wav"; // "authorization failed"
        private string response_danger = @"response_danger.wav"; // "danger"
        private string response_warning = @"response_warning.wav"; // "warning"
        private string response_YesQuestion = @"response_yes_question.wav"; // "yes?"
        private string response_Negative = @"response_negative.wav"; // "negative"
        private string response_OperationFailed = @"response_operation_failed.wav"; // "operation failed"
        private string response_Refuse = @"response_refuse.wav"; // "your authority is limited"
        private string response_Authorizing = @"response_authorizing.wav"; // "authorizing,please speak out your name"
        private string response_LogOut = @"response_logging_out.wav"; // "logging out"
        private string response_WelcomeMyMaster = @"response_welcome_master.wav"; // "welcome my master"
        private string response_YouGuestIdentity = @"response_your_guest_identity.wav"; // "your identity is guest";


        public Chris(bool isused = false)
        {
            // initiation the available voice command now
            voicesDict.Add(nameof(response_Welcome), voiceRootFolderPath + response_Welcome);
            voicesDict.Add(nameof(response_AsYouWantMaster), voiceRootFolderPath + response_AsYouWantMaster);
            voicesDict.Add(nameof(response_Wait), voiceRootFolderPath + response_Wait);
            voicesDict.Add(nameof(response_Identifying), voiceRootFolderPath + response_Identifying);
            voicesDict.Add(nameof(response_AuthorizationAccomplish), voiceRootFolderPath + response_AuthorizationAccomplish);
            voicesDict.Add(nameof(response_guest), voiceRootFolderPath + response_guest);
            voicesDict.Add(nameof(response_master), voiceRootFolderPath + response_master);
            voicesDict.Add(nameof(response_YourIdentityIs), voiceRootFolderPath + response_YourIdentityIs);
            voicesDict.Add(nameof(response_KinectClosing), voiceRootFolderPath + response_KinectClosing);
            voicesDict.Add(nameof(response_VoiceAssistant), voiceRootFolderPath + response_VoiceAssistant);
            voicesDict.Add(nameof(response_SeeYou), voiceRootFolderPath + response_SeeYou);
            voicesDict.Add(nameof(response_WhatUp), voiceRootFolderPath + response_WhatUp);
            voicesDict.Add(nameof(response_AuthorizationFailed), voiceRootFolderPath + response_AuthorizationFailed);
            voicesDict.Add(nameof(response_danger), voiceRootFolderPath + response_danger);
            voicesDict.Add(nameof(response_warning), voiceRootFolderPath + response_warning);
            voicesDict.Add(nameof(response_YesQuestion), voiceRootFolderPath + response_YesQuestion);
            voicesDict.Add(nameof(response_Negative), voiceRootFolderPath + response_Negative);
            voicesDict.Add(nameof(response_OperationFailed), voiceRootFolderPath + response_OperationFailed);
            voicesDict.Add(nameof(response_Refuse), voiceRootFolderPath + response_Refuse);
            voicesDict.Add(nameof(response_Authorizing), voiceRootFolderPath + response_Authorizing);
            voicesDict.Add(nameof(response_LogOut), voiceRootFolderPath + response_LogOut);
            voicesDict.Add(nameof(response_WelcomeMyMaster), voiceRootFolderPath + response_WelcomeMyMaster);
            voicesDict.Add(nameof(response_YouGuestIdentity), voiceRootFolderPath + response_YouGuestIdentity);

            IsUsedVoiceAssistant = isused;

            // initiate the voice recognition
            voiceRecog = new KinectVoiceRecognition();
            
        }


        public void updateKinectSensor(ref KinectSensor s)
        {
            sensor = s;
        }

        public void initVoiceRecog()
        {
            voiceRecog.updateAndInitiate(ref sensor);
        }

        public void updateSpeechGrammar(ref SpeechGrammarManager sgm)
        {
            speechGrammar = sgm;
        }


        public void clearTheVoiceRecognitionResult()
        {
            voiceRecog.RecognizedResult = "No Result";
            voiceRecog.RecognizedResultSemantic = "No Result";
        }

        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// say "welcome to kinect gesture control system!"
        /// </summary>
        public void playWelcome()
        {
            player.SoundLocation = voicesDict[nameof(response_Welcome)];
            player.Load();
            player.Play();
        }


        /// <summary>
        /// say "here is Chris, your voice assistant"
        /// </summary>
        public void playHello()
        {
            player.SoundLocation = voicesDict[nameof(response_VoiceAssistant)];
            player.Load();
            player.Play();
        }


        /// <summary>
        /// say "see you next time"
        /// </summary>
        public void playSeeYou()
        {
            player.SoundLocation = voicesDict[nameof(response_SeeYou)];
            player.Load();
            player.Play();
        }


        /// <summary>
        /// say "Kinect Gesture System closing, wait for you next time"
        /// </summary>
        public void playKinectClosing()
        {
            player.SoundLocation = voicesDict[nameof(response_KinectClosing)];
            player.Load();
            player.Play();
        }


        /// <summary>
        /// say "yes?"
        /// </summary>
        public void playYes()
        {
            if (IsUsedVoiceAssistant)
            {
                player.SoundLocation = voicesDict[nameof(response_YesQuestion)];
                player.Load();
                player.Play();
            }
        }


        /// <summary>
        /// say "what's up"
        /// </summary>
        public void playWhatUp()
        {
            if (IsUsedVoiceAssistant)
            {
                player.SoundLocation = voicesDict[nameof(response_WhatUp)];
                player.Load();
                player.Play();
            }
        }


        public void playMaster()
        {
            if (IsUsedVoiceAssistant)
            {
                player.SoundLocation = voicesDict[nameof(response_master)];
                player.Load();
                player.Play();
            }
        }

        public void playGuest()
        {
            if (IsUsedVoiceAssistant)
            {
                player.SoundLocation = voicesDict[nameof(response_guest)];
                player.Load();
                player.Play();
            }
        }

        public void playAuthorizing()
        {
            if (IsUsedVoiceAssistant)
            {
                player.SoundLocation = voicesDict[nameof(response_Authorizing)];
                player.Load();
                player.Play();
            }
        }

        public void playIdentityMaster()
        {
            if (IsUsedVoiceAssistant)
            {
                player.SoundLocation = voicesDict[nameof(response_WelcomeMyMaster)];
                player.Load();
                player.Play();
            }
        }

        public void playIdentityGuest()
        {
            if (IsUsedVoiceAssistant)
            {
                player.SoundLocation = voicesDict[nameof(response_YouGuestIdentity)];
                player.Load();
                player.Play();
            }
        }

        public void playLogOut()
        {
            if (IsUsedVoiceAssistant)
            {
                player.SoundLocation = voicesDict[nameof(response_LogOut)];
                player.Load();
                player.Play();
            }
        }


        public void playAsYouWant()
        {
            if (IsUsedVoiceAssistant)
            {
                player.SoundLocation = voicesDict[nameof(response_AsYouWantMaster)];
                player.Load();
                player.Play();
            }
        }

        
        public bool IsUsedVoiceAssistant { get; set; } // whether use the voice assistant or not

        public VoiceRecognition voiceRecog { get;  } // voice recognition part
        public SpeechGrammarManager SpeechGrammar { get { return speechGrammar; } }

    }
}
