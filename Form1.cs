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

namespace dictation5
{
    public partial class Form1 : Form
    {
        //Globals
        #region
        public static int StopFlag = 0;
        public static TaskCompletionSource<int> stopRecognition = new TaskCompletionSource<int>();
        public static AutomationElement AutomElement = null;
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
        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            StopFlag = 0;
            await Speech.ContinuousRecognitionMicrofone();
        }
        
        //Button2 click
        private async void button2_ClickAsync(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
            StopFlag = 1;
        }

        //Find automation element
        private void SetAutomationElement(object sender, EventArgs e)
        {
            System.Drawing.Point MousePoint = System.Windows.Forms.Cursor.Position;
            AutomElement = AutomationElement.FromPoint(new System.Windows.Point(MousePoint.X, MousePoint.Y));
            Console.WriteLine(AutomElement.Current.Name);
        }

        //Insert text to automation element
        public static void InsertText(AutomationElement targetControl, string value)
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
                }
            }
        }
    }
}