using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ExtensionMethods;

namespace dictation5
{
    public static class PostProcessing
    {
        //Luvut
        private static Dictionary<string, int> numbers = new Dictionary<string, int>()
        {
            { "nolla", 0 },
            { "yksi", 1 },
            { "kaksi", 2 },
            { "kaks", 2 },
            { "kolme", 3 },
            { "neljä", 4 },
            { "viisi", 5 },
            { "kuusi", 6 },
            { "kuus", 6 },
            { "seitsemän", 7 },
            { "seittemän", 7 },
            { "kahdeksan", 8 },
            { "kaheksan", 8 },
            { "yhdeksän", 9 },
            { "kymmenen", 10 },
            { "yksitoista", 11 },
            { "ykstoista", 11 },
            {"kaksitoista", 12},
            { "kakstoista", 12}
        };
        //järjestysluvut
        private static Dictionary<string, string> ordinalNumbers = new Dictionary<string, string>()
        {
            {"ensimmäinen",  "1." },
            {"toinen", "2." },
            {"kolmas", "3."},
            {"neljäs", "4."},
            {"viides", "5."},
            {"kuudes", "6."},
            {"seitsemäs", "7."},
            {"kahdeksas", "8."},
            {"yhdeksäs", "9."},
            {"kymmenes", "10."},
            {"yhdestoista", "11."},
            {"kahdestoista", "12."},
            {"kolmastoista", "13."},
            {"kolomastoista", "13."},
            {"neljästoista", "14."},
            {"neljästoist", "14."},
            {"viidestoista", "15."},
            {"viidestoist", "15."},
            {"kuudestoista", "16."},
            {"seitsemästoista", "17."},
            {"seitsemässätoista", "17."},
            {"kahdeksastoista", "18."},
            {"yhdeksästoista", "19."},
            {"yhdeksästoist", "19."},
            {"kahdeskymmenes", "21."},
            {"kahdeskymmenesensimmäinen", "21."},
            {"kahdeskymmenestoinen", "22."},
            {"kahdeskymmeneskolmas", "23."},
            {"kahdeskymmenesneljäs", "24."},
            {"kahdeskymmenesviides", "25."},
            {"kahdeskymmeneskuudes", "26."},
            {"kahdeskymmenesseitsemäs", "27."},
            {"kahdeskymmeneskahdeksas", "28."},
            {"kahdeskymmenesyhdeksäs", "29."},
            {"kolmaskymmenes", "30."},
            {"kolmaskymmenesensimmäinen", "31."}
        };
        //Kuukaudet
        private static Dictionary<string, string> months = new Dictionary<string, string>()
        {
            {"tammikuuta", "1."},
            {"helmikuuta", "2."},
            {"maaliskuuta", "3."},
            {"huhtikuuta", "4."},
            {"toukokuuta", "5."},
            {"kesäkuuta", "6."},
            {"heinäkuuta", "7."},
            {"elokuuta", "8."},
            {"syyskuuta", "9."},
            {"lokakuuta", "10."},
            {"marraskuuta", "11."},
            {"joulukuuta", "12."},
            //Kuukaudet 2
            {"ensimmäistä", "1."},
            {"toista", "2."},
            {"kahdetta", "2."},
            {"kolmatta", "3."},
            {"neljättä", "4."},
            {"viidettä", "5."},
            {"kuudetta", "6."},
            {"seitsemättä", "7."},
            {"kahdeksatta", "8."},
            {"yhdeksättä", "9."},
            {"kymmenettä", "10."},
            {"yhdettätoista", "11."},
            {"kahdettatoista", "12."},
        };

        public static string PostProcessText(string Text)
        {
            string inputText = Text;
            
            //Replace common errors related to years
            inputText = inputText.Replace("2000 kaksikyt 1", "2021").Replace("2000 kakskyt 1", "2021").Replace("2000 kakskyt yksi", "2021").Replace("2000 kaksikyt yksi", "2021").Replace("kaksituhattakaksikymmentä yksi", "2021").Replace("2000, kakskyt 1", "2021").Replace("2000 kaksikymmentäyksi", "2021").Replace("2000 kakskyt yks", "2021");
            inputText = inputText.Replace("2000 kakskyt", "2020").Replace("kakskytkakskyt", "2020").Replace("kakstuhattakakskyt", "2020").Replace("Kakstuhattakakskyt", "2020").Replace("2000 kaksikyt", "2020").Replace("2000, kakskyt", "2020");
            inputText = inputText.Replace("lopeta sanellut", "lopeta sanelu").Replace("Lopeta sanellut", "lopeta sanelu");

            //Loop through each word and fix errors
            string[] iw = inputText.Split(' ');
            List<char> punctuationChars = new List<char>() { '.', ',', ':', ';', '?' };
            for(int i = 0; i < iw.Length; i++)
            {
                //Jos kyseessä vuosiluku
                if (iw[i].ToLower().Contains("tuhatta"))
                {
                    if (punctuationChars.IndexOf( char.Parse(iw[i].Substring(iw[i].Length - 1)) ) > -1)
                    {
                        iw[i] = GetYearFromText(iw[i].Substring(0, iw[i].Length - 1).ToLower()) + iw[i].Substring(iw[i].Length - 1);
                    }
                    else
                    {
                        iw[i] = GetYearFromText(iw[i].ToLower());
                    }
                }
                //Jos teksti näyttä numerolta, niin korvataan se numerolla
                else if (numbers.ContainsKey(iw[i].Replace(".", "").ToLower()))
                {
                    iw[i] = numbers[iw[i].Replace(".", "").ToLower()].ToString();
                }
                //Jos teksti näyttä järjestysluvulta, niin korvataan se järjestysluvulla
                else if (ordinalNumbers.ContainsKey(iw[i].Replace(".", "").ToLower()))
                {
                    iw[i] = ordinalNumbers[iw[i].Replace(".", "").ToLower()].ToString();
                }
                //Jos teksti näyttä kuukaudelta, niin korvataan se järjestysluvulla
                else if (months.ContainsKey(iw[i].Replace(".", "").ToLower()))
                {
                    iw[i] = months[iw[i].Replace(".", "").ToLower()].ToString();
                }
            }

            //Muodostetaan lopullinen teksti
            string outputText = "";
            for (int i = 0; i < iw.Length; i++)
            {
                outputText += iw[i] + " ";
            }
            Console.WriteLine(outputText);

            //Siivotaan vielä päivämääristä ylimääräiset välilyönnit regexillä
            Regex re1 = new Regex(@"(3[01]|[12][0-9]|0?[1-9])\. (1[012]|0?[1-9])\. ((?:19|20)\d{2})", 
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Find matches.
            MatchCollection matches1 = re1.Matches(outputText);

            //Regex re2 = new Regex(@"/(3[01]|[12][0-9]|0?[1-9])\s(1[012]|0?[1-9])\.\s((?:19|20)\d{2})/g", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Find matches.
            //MatchCollection matches2 = re2.Matches(outputText);

            foreach (Match match in matches1)
            {
                Console.WriteLine("match found at " + match.Index);
                string str1 = outputText.Substring(0, match.Index);
                string str2 = outputText.Substring(match.Index).Replace(" ", "").Replace(" ", "");
                outputText = str1 + str2;
            }
           
            /*matches2.forEach((match) => {
                console.log("match found at " + match.index);
                var str1 = text.substring(0, match.index);
                var str2 = text.substring(match.index).replace(" ", ".").replace(" ", "");
                text = str1 + str2;
            });*/

            //Format text
            string ProcessedText = " ";
            if (Text.Trim().ToLower().Replace(".","") == "lopeta sanelu")
            {
                Form1.StopDictating();
            }
            else if (Text.Trim().ToLower().StartsWith("otsikko"))
            {
                ProcessedText += outputText.ToLower().Replace("otsikko", "").ToUpper();
            }
            else if (Text.Trim().ToLower().StartsWith("väliotsikko"))
            {
                ProcessedText += outputText.ToLower().Replace("väliotsikko", "").ToUpper();
            }
            else if (Text.Trim().ToLower().StartsWith("diagnoosi"))
            {
                ProcessedText += outputText.ToLower().Replace("diagnoosi", "");
            }
            else if (Text.Trim().ToLower().StartsWith("taikasana"))
            {
                ProcessedText += outputText.ToLower().Replace("taikasana", "SIMSALABIM!");
            }
            else
            {
                ProcessedText += outputText;
            }

            /*if (Form1.AutomElement != null)
            {
                Form1.InsertText(Form1.AutomElement, ProcessedText);
                //Form1.AppendText2(ProcessedText);
            }
            else
            {
                const string message = "Valitse hiiren oikealla napilla paikka, jonne haluat sanella.";
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
            }*/
            return ProcessedText;
        }

        //Function to convert text to a year
        private static string GetYearFromText(string text)
        {
            string[] res;
            var tuhannet = 0;
            var sadat = 0;
            var kymmenet = 0;
            var ykkoset = 0;

            if (text.ToLower().Contains("tuhatta") && text.ToLower().Contains("kymmentä"))
            {
                res = text.Split("tuhatta");
                tuhannet = numbers[res[0].ToString().ToLower()];

                res = res[1].Split("kymmentä");
                kymmenet = numbers[res[0].ToLower()];
                if (res[1].Length >= 4)
                {
                    ykkoset = numbers[res[1].ToLower()];
                }
            }
            else if (text.ToLower().Contains("tuhatta") && text.ToLower().Contains("toista"))
            {
                res = text.Split("tuhatta");
                tuhannet = numbers[res[0].ToLower()];

                res = res[1].Split("toista");
                ykkoset = numbers[res[0].ToLower()];

                kymmenet = 1;
            }

            return (tuhannet * 1000 + kymmenet * 10 + ykkoset).ToString();
        }


    }
}