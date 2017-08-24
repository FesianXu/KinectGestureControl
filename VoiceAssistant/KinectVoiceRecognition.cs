using System;
using System.Text;
using System.IO;
using Microsoft.Kinect;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;

namespace FesianXu.KinectGestureControl
{

    enum SpeechRecognizeStatusEnum { Recognized, Rejected };

    class KinectVoiceRecognition: VoiceRecognition
    {
        private KinectSensor sensor;
        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;
        private string SpeechGrammarFilePath = @"../../Resources/SpeechGrammars/SpeechGrammar.xml";
        

        public KinectVoiceRecognition()
        {
        }

        public void updateAndInitiate(ref KinectSensor s)
        {
            sensor = s;
            RecognizerInfo reginfo = GetKinectRecognizer();
            if (reginfo != null)
            {
                speechEngine = new SpeechRecognitionEngine(reginfo.Id);
            }
            string text = System.IO.File.ReadAllText(SpeechGrammarFilePath);
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(text)))
            {
                var g = new Grammar(memoryStream);
                speechEngine.LoadGrammar(g);
            }
            speechEngine.SpeechRecognized += SpeechRecognized;
            speechEngine.SpeechRecognitionRejected += SpeechRejected;
            speechEngine.SetInputToAudioStream(
            sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            speechEngine.RecognizeAsync(RecognizeMode.Multiple);


        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private RecognizerInfo GetKinectRecognizer()
        {
            var t = SpeechRecognitionEngine.InstalledRecognizers();
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }
            return null;
        }


        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;
            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                RecognizedResult = e.Result.Text;
                RecognizedResultSemantic = e.Result.Semantics.Value.ToString();
                regStatus = SpeechRecognizeStatusEnum.Recognized;
            }
            else
            {
                regStatus = SpeechRecognizeStatusEnum.Rejected;
            }
        }


        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            RecognizedResult = "No Result";
            RecognizedResultSemantic = "No Result";
            regStatus = SpeechRecognizeStatusEnum.Rejected;
        }

        public string RecognizedResult { get; set; }
        public string RecognizedResultSemantic { get; set; }
        public SpeechRecognizeStatusEnum regStatus { get; set; }
        public bool haveStartedVAThread { get; set; }
    }
}
