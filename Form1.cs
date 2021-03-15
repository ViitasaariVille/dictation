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
using System.Windows.Automation;
using System.Threading;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;
using ExtensionMethods;

namespace dictation5
{
    public partial class Form1 : Form
    {
        //Globals
        #region
        public static int StopFlag = 0;
        public TaskCompletionSource<int> stopRecognition = new TaskCompletionSource<int>();
        public static AutomationElement AutomElement = null;
        #endregion

        //Classes
        #region
        //Speech s = new Speech();
        #endregion

        //Form1
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            MouseHook.Start();
            MouseHook.MouseAction += new EventHandler(SetAutomationElement);
        }

        //Button1 click
        public async void button1_ClickAsync(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            StopFlag = 0;
            await ContinuousRecognitionMicrofone();
        }
        
        //Button2 click
        private async void button2_ClickAsync(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
            StopFlag = 1;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            //string[] test = null;
            //string outputFile = "";
            string targetDirectory = @"C:\Users\ville\Documents\puhe\aaniraidat";
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);
                //Console.WriteLine(fileName);
        }

        private async void ProcessFile(string fileName)
        {
            Console.WriteLine(fileName);
            string OutputFile = @"C:\Users\ville\Documents\puhe\TextFiles\" + fileName.Split(@"\")[6] + ".txt";
            await ContinuousRecognitionWithFileAsync(fileName, OutputFile);
        }


        //Find automation element
        private void SetAutomationElement(object sender, EventArgs e)
        {
            System.Drawing.Point MousePoint = System.Windows.Forms.Cursor.Position;
            AutomElement = AutomationElement.FromPoint(new System.Windows.Point(MousePoint.X, MousePoint.Y));
            Console.WriteLine(AutomElement.Current.Name);
        }

        //Continuous speech recognition from a microfone
        public async Task ContinuousRecognitionMicrofone()
        {
            var config = SpeechConfig.FromSubscription(
                Properties.Resources.SubscriptionKey,
                Properties.Resources.Region);
            config.SpeechRecognitionLanguage = "fi-FI";
            config.EnableDictation();
            //config.SetServiceProperty("punctuation", "explicit", ServicePropertyChannel.UriQueryParameter);

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
                        if (AutomElement == null)
                        {
                            AppendText2(PostProcessing.PostProcessText(e.Result.Text));
                        }
                        else
                        {
                            InsertText(AutomElement, PostProcessing.PostProcessText(e.Result.Text));
                        }
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        Console.WriteLine($"NOMATCH: Speech could not be recognized.");
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

        // Continuous speech recognition from a file
        public async Task ContinuousRecognitionWithFileAsync(string InputFile, string OutputFile)
        {
            // <recognitionContinuousWithFile>
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription(
                Properties.Resources.SubscriptionKey,
                Properties.Resources.Region);
            config.SpeechRecognitionLanguage = "fi-FI";
            config.EnableDictation();

            var stopRecognition = new TaskCompletionSource<int>();

            string RecognizedText = "";

            // Creates a speech recognizer using file as audio input.
            // Replace with your own audio file name.
            using (var audioInput = AudioConfig.FromWavFileInput(InputFile))
            {
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {
                    // Subscribes to events.
                    recognizer.Recognizing += (s, e) =>
                    {
                        Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                    };

                    recognizer.Recognized += (s, e) =>
                    {
                        if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                            RecognizedText += " " + e.Result.Text;
                        }
                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
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
                        Write2file.WriteText(OutputFile, RecognizedText);
                        Console.WriteLine("\n    Session stopped event.");
                        Console.WriteLine("\nStop recognition.");
                        stopRecognition.TrySetResult(0);
                    };

                    // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    // Waits for completion.
                    // Use Task.WaitAny to keep the task rooted.
                    Task.WaitAny(new[] { stopRecognition.Task });

                    // Stops recognition.
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                }
            }
            // </recognitionContinuousWithFile>
        }


        //Insert text to automation element
        public void InsertText(AutomationElement targetControl, string value)
        {
            // Validate arguments / initial setup
            if (value == null)
                throw new ArgumentNullException(
                    "String parameter must not be null.");

            if (targetControl == null)
                throw new ArgumentNullException(
                    "AutomationElement parameter must not be null");

            // A series of basic checks prior to attempting an insertion.
            //
            // Check #1: Is control enabled?
            // An alternative to testing for static or read-only controls 
            // is to filter using 
            // PropertyCondition(AutomationElement.IsEnabledProperty, true) 
            // and exclude all read-only text controls from the collection.
            if (!targetControl.Current.IsEnabled)
            {
                throw new InvalidOperationException(
                    "The control is not enabled.\n\n");
            }

            // Check #2: Are there styles that prohibit us 
            //           from sending text to this control?
            /*if (!targetControl.Current.IsKeyboardFocusable)
            {
                throw new InvalidOperationException(
                    "The control is not focusable.\n\n");
            }*/

            // Once you have an instance of an AutomationElement,  
            // check if it supports the ValuePattern pattern.
            object valuePattern = null;

            if (!targetControl.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
            {
                // Elements that support TextPattern 
                // do not support ValuePattern and TextPattern
                // does not support setting the text of 
                // multi-line edit or document controls.
                // For this reason, text input must be simulated.

                // Set focus for input functionality and begin.
                //targetControl.SetFocus();

                // Pause before sending keyboard input.
                Thread.Sleep(100);

                // Delete existing content in the control and insert new content.
                //SendKeys.SendWait("^{HOME}");   // Move to start of control
                //SendKeys.SendWait("^+{END}");   // Select everything
                //SendKeys.SendWait("{DEL}");     // Delete selection
                SendKeys.SendWait(value);
                //SendKeys.Send(value);
            }
            // Control supports the ValuePattern pattern so we can 
            // use the SetValue method to insert content.
            else
            {
                if (((ValuePattern)valuePattern).Current.IsReadOnly)
                {
                    throw new InvalidOperationException(
                        "The control is read-only.");
                }
                else
                {
                    //((ValuePattern)valuePattern).SetValue(value);
                    //targetControl.SetFocus();

                    // Pause before sending keyboard input.
                    Thread.Sleep(100);

                    //SendKeys
                    SendKeys.SendWait(value);
                    //SendKeys.Send(value);
                }
            }
        }

        //Append text
        public void AppendText2(String text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendText2), new object[] { text });
                return;
            }
            this.richTextBox1.Text += text;
        }
        public void AppendText(string text, string textType = "")
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)delegate { this.AppendText(text, textType); });
                return;
            }
            if (textType == "title")
            {
                richTextBox1.AppendTitle(text);
            }
            else if (textType == "subtitle")
            {
                richTextBox1.AppendSubTitle(text);
            }
            else if (textType == "diagnoosi")
            {
                richTextBox1.AppendDiagnosis(text);
            }
            else
            {
                richTextBox1.AppendLine(text);
            }
        }

        //Programmatically close the form
        public static void StopDictating()
        {
            //button2.Enabled = false;
            //button1.Enabled = true;
            StopFlag = 1;
            Console.WriteLine("Lopetetaan sanelu");
        }

       
    }
}