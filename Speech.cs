using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace dictation5
{
    public static class Speech
    {
        //Continuous speech recognition from a microfone
        public static async Task ContinuousRecognitionMicrofone()
        {
            var config = SpeechConfig.FromSubscription(
                Properties.Resources.SubscriptionKey,
                Properties.Resources.Region);
            config.SpeechRecognitionLanguage = "fi-FI";
            config.EnableDictation();

            var stopRecognition = new TaskCompletionSource<int>();

            using (var recognizer = new SpeechRecognizer(config))
            {
                string text;

                // Subscribes to events.
                recognizer.Recognizing += (s, e) =>
                {
                    Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                    if (Form1.StopFlag == 1)
                    {
                        Console.WriteLine("Lopetetaan puheentunnistus");
                        stopRecognition.TrySetResult(0);
                    }
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                        PostProcessing.PostProcessText(e.Result.Text);
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        //Form1.AppendText(" NOMATCH: Speech could not be recognized.");
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

                /*
                // Before starting recognition, add a phrase list to help recognition.
                PhraseListGrammar phraseListGrammar = PhraseListGrammar.FromRecognizer(recognizer);
                phraseListGrammar.AddPhrase("Puollan");
                */

                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                // Waits for completion.
                // Use Task.WaitAny to keep the task rooted.
                Task.WaitAny(new[] { stopRecognition.Task });

                // Stops recognition.
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            }
        }
    }
}
