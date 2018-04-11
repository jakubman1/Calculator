using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        new struct Word
        {
            public TextPointer StartPos;
            public TextPointer EndPos;
            public string Text;
            public int Type; //0=numbers,1=standard operators,2=special operators,3=exponents,4=letters(memory),5=absolute value, -1=error state
        }

        List<Word> words = new List<Word>();

        internal void CheckWords(Run run, string text)
        {
            int startIndex = 0;
            int endIndex = 0;

            

            for(int i = 0; i < text.Length; i++)
            {
                //We found a number
                if ((text[i] >= '0' && text[i] <= '9') || (text[i] == '.' || text[i] == ','))
                {
                    //Check for the whole number
                    while ((text[i] >= '0' && text[i] <= '9') || (text[i] == '.' || text[i] == ','))
                    {
                        i++;
                    }
                    endIndex = i;

                    Word w = new Word();
                    //Check if the number is an exponent
                    if (startIndex > 0 && text[startIndex - 1] == '^')
                    {
                        w.Type = 3;
                    }
                    else
                    {
                        w.Type = 0;
                    }
                    w.StartPos = run.ContentStart.GetPositionAtOffset(startIndex, LogicalDirection.Forward);
                    w.EndPos = run.ContentStart.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward);
                    w.Text = text.Substring(startIndex, endIndex - startIndex + 1);

                    words.Add(w);

                }
                else if(text[i] == '+' || text[i] == '-' || text[i] == '*' || text[i] == '/' || text[i] == '=')
                {
                    endIndex = i + 1;
                    //TODO: TEST THIS!!!!
                    Word w = new Word();
                    w.StartPos = run.ContentStart.GetPositionAtOffset(startIndex, LogicalDirection.Forward);
                    w.EndPos = run.ContentStart.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward);
                    w.Text = text.Substring(startIndex, endIndex - startIndex + 1);
                    w.Type = 1;
                }

                else if (text[i] == '!' || text[i] == '^')
                {
                    endIndex = i + 1;
                    //TODO: TEST THIS!!!!
                    Word w = new Word();
                    w.StartPos = run.ContentStart.GetPositionAtOffset(startIndex, LogicalDirection.Forward);
                    w.EndPos = run.ContentStart.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward);
                    w.Text = text.Substring(startIndex, endIndex - startIndex + 1);
                    w.Type = 2;
                }
                else if((text[i] >= 'A' && text[i] <= 'Z') || (text[i] >= 'a' && text[i] <= 'z'))
                {
                    Word w = new Word();
                    if ((i != 0 && !IsOperator(text[i-1])) || (i != text.Length && !IsOperator(text[i + 1]))) {
                        w.Type = -1;
                    }
                    else
                    {
                        w.Type = 4;
                    }
                }

                startIndex = i + 1;
            }
        }

        bool IsOperator(char c)
        {
            char[] operators = { '+', '-', '*', '/', '=', '!', '^', '|' };
            for(int i = 0; i < operators.Length; i++)
            {
                if(c == operators[i])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// When input text is changed, change colors of text based on syntax
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text;
            //Input is empty, no need to color anything
            if (inputTextBox.Document == null)
            {
                return;
            }
            //Remove event handler to prevent infinite loop.
            inputTextBox.TextChanged -= InputTextBox_TextChanged;

            TextRange documentRange = new TextRange(inputTextBox.Document.ContentStart, inputTextBox.Document.ContentEnd);
            documentRange.ClearAllProperties();

            TextPointer navigator = inputTextBox.Document.ContentStart;
            //Go through the document
            while(navigator.CompareTo(inputTextBox.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if(context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    text = ((Run)navigator.Parent).Text;
                    if(text != "")
                    {
                        CheckWords((Run)navigator.Parent, text);
                    }
                }

                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
      
            }

            //Regex.IsMatch(text,@"regex")
            //Remove all whitespace characters from string
            //Regex.Replace(text, @"\s+", ""); //Cant use this, could lead to wrong positions

            //Setup brushes
            Brush numberBrush = new SolidColorBrush(Color.FromArgb(255, 34, 207, 247));
            Brush operatorBrush = new SolidColorBrush(Color.FromArgb(255, 253, 115, 115));
            Brush specialOperatorBrush = new SolidColorBrush(Color.FromArgb(255, 234, 181, 67));
            Brush exponentBrush = new SolidColorBrush(Color.FromArgb(255, 214, 162, 232));
            Brush letterBrush = new SolidColorBrush(Color.FromArgb(255, 27, 156, 252));
            Brush absBrush = new SolidColorBrush(Color.FromArgb(255, 109, 33, 79));

            //Add back event handler
            inputTextBox.TextChanged += InputTextBox_TextChanged;

        }

       

       /* internal void CreateColorSyntax(RichTextBox textBox)
        {
            ready = false;
            //Setup brushes
            Brush numberBrush = new SolidColorBrush(Color.FromArgb(255, 34, 207, 247));
            Brush operatorBrush = new SolidColorBrush(Color.FromArgb(255, 253, 115, 115));

            //Store content
            TextRange storedTextContent = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            string text = storedTextContent.Text;

            if(text != "")
            {
       
                //Remove content
                inputTextBox.Document.Blocks.Clear();

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] >= '0' && text[i] <= '9')
                    {
                        inputTextBox.Foreground = numberBrush;
                        
                    }
                    else if(text[i] == '+' || text[i] == '=' || text[i] == '-' || text[i] == '*' || text[i] == '/')
                    {
                        inputTextBox.Foreground = operatorBrush;
                    }
                    else
                    {
                        inputTextBox.Foreground = Brushes.Red;
                    }
                    inputTextBox.AppendText(text[i].ToString());
                }
                
              
            }
            ready = true;
    
        }*/

       /* List<Tag> m_tags = new List<Tag>();
        internal void CheckWordsInRun(Run theRun) //do not hightlight keywords in this method
        {
            //How, let's go through our text and save all tags we have to save.               
            int sIndex = 0;
            int eIndex = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | GetSpecials(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | GetSpecials(text[i - 1])))
                    {
                        eIndex = i - 1;
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);
                        if (IsKnownTag(word))
                        {
                            Tag t = new Tag();
                            t.StartPosition = theRun.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                            t.EndPosition = theRun.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                            t.Word = word;
                            m_tags.Add(t);
                        }
                    }
                    sIndex = i + 1;
                }
            }
            //How this works. But wait. If the word is last word in my text I'll never hightlight it, due I'm looking for separators. Let's add some fix for this case
            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (IsKnownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = theRun.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = theRun.ContentStart.GetPositionAtOffset(text.Length, LogicalDirection.Backward); //fix 1
                t.Word = lastWord;
                m_tags.Add(t);
            }
        }

    */
    }
}
