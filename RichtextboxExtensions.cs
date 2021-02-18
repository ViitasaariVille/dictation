using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dictation5
{
    public static class RichTextBoxExtensions
    {
        const string NewLine = "\r\n";
        const string NewLine2 = "\r\n************************************\n";
        const string NewLine3 = "\n------------------------------------\n";

        public static void AppendLine(this RichTextBox ed)
        {
            ed.AppendText(NewLine);
        }

        public static void AppendLine(this RichTextBox ed, string s)
        {
            ed.AppendText(s + NewLine);
        }

        public static void AppendSubTitle(this RichTextBox ed, string s)
        {
            int ss = ed.SelectionStart;
            ed.AppendText(NewLine3);
            ed.AppendText(s);
            int sl = ed.SelectionStart - ss + 1;

            Font bold = new Font("Tahoma", 13, FontStyle.Italic);
            
            ed.Select(ss, sl);
            ed.SelectionFont = bold;
            ed.AppendText(NewLine);
        }
        public static void AppendTitle(this RichTextBox ed, string s)
        {
            int ss = ed.SelectionStart;
            //ed.AppendText(NewLine);
            ed.AppendText(NewLine);
            ed.AppendText(NewLine2);
            ed.AppendText(s);
            int sl = ed.SelectionStart - ss + 1;

            //Font bold = new Font(ed.Font, FontStyle.Bold);
            Font nameFont = new Font("Tahoma", 15, FontStyle.Bold);

            ed.Select(ss, sl);
            ed.SelectionFont = nameFont;
            ed.AppendText(NewLine);
        }
    }
}
