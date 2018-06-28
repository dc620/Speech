using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.CognitiveServices.SpeechRecognition;
namespace Speech
{
    public partial class Form1 : Form
    {
        private MicrophoneRecognitionClient micClient;

        private string SubscriptionKey = "f05d9b035d884225b67023c63fa4e0ee";
        private SpeechRecognitionMode Mode = SpeechRecognitionMode.LongDictation;
        private string DefaultLocale = "en-US";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (micClient == null)
                CreateMicrophoneRecoClient();

            micClient.StartMicAndRecognition();
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            micClient.EndMicAndRecognition();
        }
        private void CreateMicrophoneRecoClient()
        {
            micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                Mode,
                DefaultLocale,
                SubscriptionKey);
            micClient.AuthenticationUri = "";

            // Event handlers for speech recognition results
            micClient.OnMicrophoneStatus += OnMicrophoneStatus;
            //  micClient.OnPartialResponseReceived += OnPartialResponseReceivedHandler;
            micClient.OnResponseReceived += OnMicDictationResponseReceivedHandler;
            micClient.OnConversationError += OnConversationErrorHandler;
        }

        private void OnMicrophoneStatus(object sender, MicrophoneEventArgs e)
        {
            //Dispatcher dispatcherUI = Dispatcher.CurrentDispatcher;
            tbLogs .BeginInvoke (new Action(() =>
            {
                if (e.Recording)
                {
                    WriteLine("Please start speaking ... :)");
                }

                WriteLine("");
            }));
        }

        private void OnPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {
            WriteLine("{0}", e.PartialResult);
            WriteLine("");
        }

        private void OnMicDictationResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.EndOfDictation ||
                e.PhraseResponse.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout)
            {
                // Dispatcher dispatcherUI = Dispatcher.CurrentDispatcher;
                tbLogs.BeginInvoke(new Action(() =>
                {
                        micClient.EndMicAndRecognition();
                    }));
            }

            if (e.PhraseResponse.Results.Length > 0)
            {
                WriteLine(e.PhraseResponse.Results.OrderByDescending(x => x.Confidence).First().DisplayText);
            }
        }

        private void OnConversationErrorHandler(object sender, SpeechErrorEventArgs e)
        {
            WriteLine("Error: {0}", e.SpeechErrorText);
            WriteLine("");
        }

        private void WriteLine(string format, params object[] args)
        {
            var formattedStr = string.Format(format, args);
            Trace.WriteLine(formattedStr);
            //    Dispatcher dispatcherUI = Dispatcher.CurrentDispatcher;
            tbLogs.BeginInvoke(new Action(() =>
            {
                tbLogs.Text += formattedStr + "\n";
               // tbLogs.ScrollToEnd();
            }));
        }
    }
}
