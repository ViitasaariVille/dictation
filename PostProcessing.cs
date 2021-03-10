using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dictation5
{
    class PostProcessing
    {
        //Text post processing
        public static void PostProcessText(string Text)
        {
            string ProcessedText=" ";

            if (Text.Trim().ToLower().StartsWith("otsikko"))
            {
                ProcessedText += Text.ToLower().Replace("otsikko", "").ToUpper();
            }
            else if (Text.Trim().ToLower().StartsWith("väliotsikko"))
            {
                ProcessedText += Text.ToLower().Replace("väliotsikko", "").ToUpper();
            }
            else if (Text.Trim().ToLower().StartsWith("diagnoosi"))
            {
                ProcessedText += Text.ToLower().Replace("diagnoosi", "");
            }
            else if (Text.Trim().ToLower().StartsWith("taikasana"))
            {
                ProcessedText += Text.ToLower().Replace("taikasana", "SIMSALABIM!");
            }
            else
            {
                ProcessedText = Text;
            }

            if (Form1.AutomElement!=null)
            {
                Form1.InsertText(Form1.AutomElement, ProcessedText);
            }
            else
            {
                const string message ="Valitse hiiren oikealla napilla paikka, jonne haluat sanella.";
                const string caption = "";
                var result = MessageBox.Show(message, caption,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                // If the no button was pressed ...
                if (result == DialogResult.No)
                {
                    // cancel the closure of the form.
                    //e.Cancel = true;
                }
            }
        }

        /*
        //Append text
        private static void AppendText(string text, string textType = "")
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
        */
    }
}
