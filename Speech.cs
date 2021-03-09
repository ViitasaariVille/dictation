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
                targetControl.SetFocus();

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
                    ((ValuePattern)valuePattern).SetValue(value);
                }
            }
        }
    }
}
