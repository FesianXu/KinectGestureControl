using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace FesianXu.KinectGestureControl
{
    /// <summary>
    /// Chris, The Voice Assistant instance
    /// </summary>
    class Chris : KinectVoiceRecognition, VoiceAssistant
    {
        private string voiceRootFolderPath = @"../../Resources/voices/";
        private Dictionary<string, string> voicesDict = new Dictionary<string, string>();
        private SoundPlayer player = new SoundPlayer();

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


        public Chris()
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

            IsUsedVoiceAssistant = false;
        }

        public void playWelcome()
        {
            player.SoundLocation = voicesDict[nameof(response_Welcome)];
            player.Load();
            player.Play();
        }

        public void playHello()
        {
            player.SoundLocation = voicesDict[nameof(response_VoiceAssistant)];
            player.Load();
            player.Play();
        }

        public void playSeeYou()
        {
            player.SoundLocation = voicesDict[nameof(response_SeeYou)];
            player.Load();
            player.Play();
        }

        public void playKinectClosing()
        {
            player.SoundLocation = voicesDict[nameof(response_KinectClosing)];
            player.Load();
            player.Play();
        }

        public bool IsUsedVoiceAssistant { get; set; }




    }
}
