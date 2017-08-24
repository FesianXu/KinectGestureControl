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
    class Chris : VoiceAssistant
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

        public bool IsUsedVoiceAssistant { get; set; }




    }
}
