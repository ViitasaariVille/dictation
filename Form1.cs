using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CognitiveServices.Speech;

namespace dictation5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            await ContinuousRecognitionWithFileAsync();
        }

   

        // Continuous speech recognition.
        public async Task ContinuousRecognitionWithFileAsync()
        {  
            // <recognitionContinuousWithFile>
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("86c28870596f4ec6af89c88c6fd56328", "northeurope");
            config.SpeechRecognitionLanguage = "fi-FI";
            config.EnableDictation();

            var stopRecognition = new TaskCompletionSource<int>();

            // Creates a speech recognizer using file as audio input.
            // Replace with your own audio file name.
            using (var recognizer = new SpeechRecognizer(config))
            {
                // Subscribes to events.
                recognizer.Recognizing += (s, e) =>
                {
                    //Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                    //AppendText(" " + e.Result.Text);
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        //Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                        if (e.Result.Text.Trim().ToLower().StartsWith("otsikko"))
                        {
                            AppendText(e.Result.Text.ToLower().Replace("otsikko","").ToUpper(), "title");
                        }
                        else if (e.Result.Text.Trim().ToLower().StartsWith("väliotsikko"))
                        {
                            AppendText(e.Result.Text.ToLower().Replace("väliotsikko", "").ToUpper(), "subtitle");
                        }
                        else if (e.Result.Text.Trim().ToLower().StartsWith("diagnoosi"))
                        {
                            AppendText(e.Result.Text.ToLower().Replace("diagnoosi", "ICD10: "));
                        }
                        else if (e.Result.Text.Trim().ToLower().StartsWith("taikasana"))
                        {
                            AppendText(e.Result.Text.ToLower().Replace("taikasana", "SIMSALABIM!"), "title");
                        }
                        else
                        {
                            AppendText(e.Result.Text);
                            //AppendBoldText(" " + e.Result.Text);
                        }
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        //Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        AppendText(" NOMATCH: Speech could not be recognized.");
                    }
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"CANCELED: Reason={e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }

                    stopRecognition.TrySetResult(0);
                };

                recognizer.SessionStarted += (s, e) =>
                {
                    Console.WriteLine("\n    Session started event.");
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\n    Session stopped event.");
                    Console.WriteLine("\nStop recognition.");
                    stopRecognition.TrySetResult(0);
                };

                // Before starting recognition, add a phrase list to help recognition.
                PhraseListGrammar phraseListGrammar = PhraseListGrammar.FromRecognizer(recognizer);
                phraseListGrammar.AddPhrase("Puollan");

                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                // Waits for completion.
                // Use Task.WaitAny to keep the task rooted.
                Task.WaitAny(new[] { stopRecognition.Task });

                // Stops recognition.
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            }
            // </recognitionContinuousWithFile>
        }

        private void AppendText(string text, string textType = "")
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)delegate { this.AppendText(text, textType); });
                return;
            }

            if (textType=="title")
            {
                richTextBox1.AppendTitle(text);
            }
            else if (textType=="subtitle")
            {
                richTextBox1.AppendSubTitle(text);
            }
            else
            {
                richTextBox1.AppendLine(text);
            }
            
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
