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
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathLibrary;
using System.Globalization;

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
            buttonDecimal.Content = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        }

        struct Word
        {
            public TextPointer StartPos;
            public TextPointer EndPos;
            public string Text;
            public int Type; //0=numbers,1=standard operators,2=special operators,3=exponents,4=letters(memory),5=absolute value, -1=error state
        }
        private List<Word> words = new List<Word>();
        public List<MemItem> memory = new List<MemItem>();

        //Setup brushes
        Brush numberBrush = new SolidColorBrush(Color.FromArgb(255, 34, 207, 247));
        Brush operatorBrush = new SolidColorBrush(Color.FromArgb(255, 253, 115, 115));
        Brush specialOperatorBrush = new SolidColorBrush(Color.FromArgb(255, 234, 181, 67));
        Brush exponentBrush = new SolidColorBrush(Color.FromArgb(255, 214, 162, 232));
        Brush letterBrush = new SolidColorBrush(Color.FromArgb(255, 27, 156, 252));
        Brush absBrush = new SolidColorBrush(Color.FromArgb(255, 109, 33, 79));
        Brush bgBrush = new SolidColorBrush(Color.FromArgb(255, 252, 66, 123));

        internal void CheckWords(Run run, string text)
        {
            int startIndex = 0;
            int endIndex = 0;
            //text.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            //text.Replace(".", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            //Console.WriteLine(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            //Console.WriteLine(text);
            //Lets go through the whole text, letter by letter
            for (int i = 0; i < text.Length; i++)
            {
                
                //We found a number
                if ((text[i] >= '0' && text[i] <= '9') || (text[i] == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]))
                {
                    //Check for the whole number
                    while (i < text.Length && ((text[i] >= '0' && text[i] <= '9') || (text[i] == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0])))
                    {
                        i++;
                    }
                    //Go back one item
                    endIndex = i - 1;
                    i--;

                    Word w = new Word
                    {
                        StartPos = run.ContentStart.GetPositionAtOffset(startIndex, LogicalDirection.Forward),
                        EndPos = run.ContentStart.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward),
                        Text = text.Substring(startIndex, endIndex + 1 - startIndex)
                    };

                    //w.Text.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                    //Number can not end with a dot, also you can not divide by zero
                    double tmp = -1;
                    if ((text[endIndex] == '.' || text[endIndex] == ',') || (startIndex > 0 && text[startIndex - 1] == '÷' && !Double.TryParse(w.Text, out tmp) && tmp == 0) )
                    {
                        w.Type = -1;
                    }
                
                    //Check if the number is an exponent
                    else if (startIndex > 0 && text[startIndex - 1] == '^')
                    {
                        w.Type = 3;
                    }
                    else
                    {
                        w.Type = 0;
                    }
                   

                    words.Add(w);

                }
                else if(text[i] == '+' || text[i] == '-' || text[i] == '*' || text[i] == '/' || text[i] == '=' || text[i] == '÷' || text[i] == '×')
                {
                    //Check for multi-character operator
                    while (i < text.Length && (text[i] == '+' || text[i] == '-' || text[i] == '*' || text[i] == '/' || text[i] == '=' || text[i] == '÷' || text[i] == '×'))
                    {
                        i++;
                    }
                    //Go back one item
                    endIndex = i - 1;
                    i--;
                    Word w = new Word
                    {
                        StartPos = run.ContentStart.GetPositionAtOffset(startIndex, LogicalDirection.Forward),
                        EndPos = run.ContentStart.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward),
                        Text = ReplaceOperatorText(text.Substring(startIndex, endIndex + 1 - startIndex)),
                        
                    };
                    //Operator has one character - always correct
                    if((endIndex + 1 - startIndex) == 1)
                    {
                        //= sign must be at second position
                        if (w.Text == "=")
                        {
                            if(startIndex == 1)
                            {
                                w.Type = 1;
                            }
                            else
                            {
                                w.Type = -1;
                            }
                        }
                        else
                        {
                            w.Type = 1;
                        }
                        
                    }
                    //Operator has two characters - We are replacing the text, if we find ** or ××
                    else if ((endIndex + 1 - startIndex) == 2)
                    {
                        if(w.Text == "^" )
                        {
                            w.Type = 2;
                            //w.EndPos = run.ContentStart.GetPositionAtOffset(endIndex, LogicalDirection.Backward);
                        }
                        /*else if(w.Text == "=-")
                        {
                           
                            w.EndPos = run.ContentStart.GetPositionAtOffset(endIndex, LogicalDirection.Backward);
                            w.Text = "=";

                            Word w2 = new Word
                            {
                                StartPos = run.ContentStart.GetPositionAtOffset(startIndex + 1, LogicalDirection.Forward),
                                EndPos = run.ContentStart.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward),
                                Text = "-",
                                Type = 1
                            };
                            words.Add(w);
                            words.Add(w2);
                            continue;
                        }*/
                        else
                        {
                            w.Type = -1;
                        }
                    }
                    else
                    {
                        w.Type = -1;
                    }
                    words.Add(w);
                }

                else if (text[i] == '!' || text[i] == '^' || text[i] == '√')
                {
                    endIndex = i + 1;
                    Word w = new Word
                    {
                        StartPos = run.ContentStart.GetPositionAtOffset(startIndex, LogicalDirection.Forward),
                        EndPos = run.ContentStart.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward),
                        Text = ReplaceOperatorText(text.Substring(startIndex, 1)),
                        Type = 2
                    };
                    words.Add(w);
                }
                else if((text[i] >= 'A' && text[i] <= 'Z') || (text[i] >= 'a' && text[i] <= 'z'))
                {
                    //Check for the whole text
                    while (i < text.Length && ((text[i] >= 'A' && text[i] <= 'Z') || (text[i] >= 'a' && text[i] <= 'z')))
                    {
                        i++;
                    }
                    //Go back one item
                    i--;
                    endIndex = i;
                    Word w = new Word
                    {
                        StartPos = run.ContentStart.GetPositionAtOffset(startIndex, LogicalDirection.Forward),
                        EndPos = run.ContentStart.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward),
                        Text = text.Substring(startIndex, endIndex + 1 - startIndex)
                    };
                    switch(w.Text.ToLower())
                    {
                        case "root":
                        case "sqrt":
                            w.Type = 2;
                            w.Text = "√";
                            break;
                        case "pow":
                            w.Type = 2;
                            w.Text = "^";
                            break;   
                        default:
                            if ((i != 0 && !IsOperator(text[i - 1])) || (i < text.Length - 1 && !IsOperator(text[i + 1])) || (!IsInMemory(text[i]) && ((i != 0 && text[i - 1] != '=') || (i < text.Length - 1 && text[i + 1] != '='))))
                            {
                                w.Type = -1;
                            }
                            else
                            {
                                w.Type = 4;
                            }
                            break;
                    }
                    
                    words.Add(w);
                }
                else if (text[i] == '|' || text[i] == '(' || text[i] == ')')
                {
                    endIndex = i + 1;
                    Word w = new Word
                    {
                        StartPos = run.ContentStart.GetPositionAtOffset(startIndex, LogicalDirection.Forward),
                        EndPos = run.ContentStart.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward),
                        Text = text.Substring(startIndex, 1),
                        Type = 5
                    };
                    words.Add(w);
                }
                //Other characters
                else
                {
                    endIndex = i + 1;
                    Word w = new Word
                    {
                        StartPos = run.ContentStart.GetPositionAtOffset(startIndex, LogicalDirection.Forward),
                        EndPos = run.ContentStart.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward),
                        Text = text.Substring(startIndex, endIndex - startIndex),
                        Type = -1
                    };
                    words.Add(w);
                }

                startIndex = i + 1;
            }
        }

        /// <summary>
        /// Checks if character is a valid operator
        /// </summary>
        /// <param name="c"></param>
        /// <returns>True if it is, false if it is not</returns>
        bool IsOperator(char c)
        {
            char[] operators = { '+', '-', '*', '/', '=', '!', '^', '|', '×', '÷', '√', '(', ')' };
            for (int i = 0; i < operators.Length; i++)
            {
                if(c == operators[i])
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if letter is a number in memory
        /// </summary>
        /// <param name="c">Letter to check</param>
        /// <returns>True if letter is an item form memory, false otherwise</returns>
        bool IsInMemory(char c)
        {
            for(int i = 0; i < memory.Count; i++)
            {
                if (c == memory[i].Letter)
                {
                    return true;
                }
            }
            return false;
        }

        private int AddToMemory(char name, double value)
        {
            int index;
            if((index = GetMemoryIndex(name)) != -1)
            {
                memory[index] = new MemItem(name, value);
            }
            else
            {
                memory.Add(new MemItem(name, value));
            }
            return 0;
        }

        private int GetMemoryIndex(char c)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                if (c == memory[i].Letter)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets value from memory by a letter.
        /// </summary>
        /// <param name="c">Letter to find</param>
        /// <returns>Value of memory item. NaN if memory item was not found</returns>
        double GetFromMemory(char c)
        {
            for(int i = 0; i < memory.Count; i++)
            {
                if (c == memory[i].Letter)
                {
                    return memory[i].Number;
                }
            }
            return Double.NaN;
        }

        private void SolveMemory(List<ExpressionNode> list)
        {
            for(int i = 0; i < list.Count(); i++)
            {
                if (IsLetter(list[i].value))
                {
                    double value = GetFromMemory(list[i].value[0]);
                    if(!Double.IsNaN(value) && !((i < list.Count() - 1) && list[i + 1].value == "="))
                    {
                        list[i].value = Convert.ToString(value);
                    } 
                }
            }
        }
        private bool IsLetter(string s)
        {
            return s.Length == 1 && ((s[0] <= 'z' && s[0] >= 'a') || (s[0] <= 'Z' && s[0] >= 'A'));
        }
        /// <summary>
        /// Replaces user written input into correct characters
        /// </summary>
        /// <param name="inText"></param>
        /// <returns></returns>
        string ReplaceOperatorText(string inText)
        {
            switch(inText)
            {
                case "*":
                    return "×";
                case "/":
                    return "÷";
                case "×*":
                case "××":
                case "**":
                    return "^";
                default:
                    return inText;
            }
        }
        /// <summary>
        /// When input text is changed, change colors of text based on syntax
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = "";
            //Input is empty, no need to color anything
            if (inputTextBox.Document == null || inputTextBox.Document.Blocks.Count == 0)
            {
                return;
                
            }
            //Remove event handler to prevent infinite loop.
            inputTextBox.TextChanged -= InputTextBox_TextChanged;
            
            words.Clear();

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
                    if(text != "" && text.Length != 0)
                    {
                        Run parentRun = (Run)navigator.Parent;
                        CheckWords(parentRun, text);
                        
                    }
                }

                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
      
            }
            
            RedrawInput();

            if(words.Count() > 0)
            {

                if (GetItemIndex(-1, words) != -1)
                {
                    //There was an error in the input, equation can not be solved
                }
                else
                {
                    
                    string result = Solve(WordToNodeList(words));

                    if (result == "" || (result.Length >= 3 && result.Substring(0,3) == "err"))
                    {
                        textBlockResult.Text = "Neplatny vyraz";
                    }
                    else
                    {
                        textBlockResult.Text = result;
                    }
                    textBlockMemory.Text = "";
                    for(int i = 0; i < memory.Count(); i++)
                    {
                        if(i % 3 == 2)
                        {
                            textBlockMemory.Text += memory[i].Letter + "=" + Convert.ToString(memory[i].Number) + "\n";
                        }
                        else
                        {
                            textBlockMemory.Text += memory[i].Letter + "=" + Convert.ToString(memory[i].Number) + "\t";
                        }
                       
                    }
                    
                }
            }
            


            //Add back event handler
            inputTextBox.TextChanged += InputTextBox_TextChanged;

        }

        /// <summary>
        /// Redraws input field formatting
        /// </summary>
        private void RedrawInput()
        {
            for (int i = 0; i < words.Count; i++)
            {
                try
                {
                    TextRange range = new TextRange(words[i].StartPos, words[i].EndPos);

                    switch (words[i].Type)
                    {
                        //Numbers
                        case 0:
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, numberBrush);
                            break;
                        //Operators
                        case 1:
                            if (range.Text == "*")
                            {
                                range.Text = "×";
                            }
                            else if (range.Text == "/")
                            {
                                range.Text = "÷";
                            }
                           
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, operatorBrush);
                            break;
                        //Special operators
                        case 2:
                            
                            if (words[i].Text == "^")
                            {
                                if (range.Text == "**" || range.Text == "××" || range.Text == "×*" || range.Text.ToLower() == "pow")
                                {
                                    range.Text = "^";
                                }
                                range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                                range.ApplyPropertyValue(TextElement.FontSizeProperty, 10.0);
                                range.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                            }
                            else if(words[i].Text == "√")
                            {
                                if(range.Text.ToLower() == "sqrt" || range.Text.ToLower() == "root")
                                {
                                    range.Text = "√";    
                                }
                                range.ApplyPropertyValue(TextElement.ForegroundProperty, specialOperatorBrush);
                            }
                            else
                            {
                                range.ApplyPropertyValue(TextElement.ForegroundProperty, specialOperatorBrush);
                            }
                            break;
                        //exponents
                        case 3:
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, exponentBrush);
                            range.ApplyPropertyValue(TextElement.FontSizeProperty, 30.0);
                            range.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                            break;
                        //letters
                        case 4:
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, letterBrush);
                            break;
                        //absolute value
                        case 5:
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, absBrush);
                            break;
                        default:
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                            range.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Baseline);
                            break;
                    }
                }
                catch { }
            }
       }

        /// <summary>
        /// Changes the color of the button's background to "Sasquatch Socks" when mouse enters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonEnter(object sender, MouseEventArgs e)
        {
            ((Label)sender).Background = bgBrush;
        }

        /// <summary>
        /// Changes the color of the button's background to "Ship's Officer" when mouse leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlackButtonLeave(object sender, MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            ((Label)sender).Background = (Brush)bc.ConvertFrom("#2C3A47");
        }

        /// <summary>
        /// Changes the color of the button's background to "Bluebell" when mouse enters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PurpleButtonLeave(object sender, MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            ((Label)sender).Background = (Brush)bc.ConvertFrom("#3B3B98");
        }

        /// <summary>
        /// Changes the color of the button's background to darker "Sasquatch Socks" when the mouse is clicked to make an effect that the button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPressedEffect(object sender, MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            ((Label)sender).Background = (Brush)bc.ConvertFrom("#CB1C51");
        }

        /// <summary>
        /// Click handler for buttons, that add their content to the input field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimpleButtonClicked(object sender, MouseButtonEventArgs e)
        {
            ButtonEnter(sender, e);
            inputTextBox.CaretPosition.InsertTextInRun((string)((Label)sender).Content);
            inputTextBox.Focus();
            try
            {
                inputTextBox.CaretPosition = inputTextBox.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
                //inputTextBox.CaretPosition = inputTextBox.CaretPosition.GetNextContextPosition(LogicalDirection.Forward);
            }
            catch (Exception ecx)
            {
                Console.WriteLine(ecx.Message);
            }

            //inputTextBox.AppendText((string)((Label)sender).Content);
            //inputTextBox.CaretPosition = inputTextBox.CaretPosition.DocumentEnd;
        }


        private void buttonClear_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ButtonEnter(sender, e);
            textBlockResult.Text = "0";
            inputTextBox.Document.Blocks.Clear();
        }

        private void buttonDelete_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ButtonEnter(sender, e);
            if (words.Count() > 0)
            {
                inputTextBox.TextChanged -= InputTextBox_TextChanged;
                buttonDelete.MouseUp -= buttonDelete_MouseUp;
                inputTextBox.Document.Blocks.Clear();
                words.RemoveAt(words.Count() - 1);
                for(int i = 0; i < words.Count; i++)
                {
                    inputTextBox.AppendText(words[i].Text);
                }
                RedrawInput();
                buttonDelete.MouseUp += buttonDelete_MouseUp;
                inputTextBox.TextChanged += InputTextBox_TextChanged;
                inputTextBox.CaretPosition = inputTextBox.CaretPosition.DocumentEnd;
            }
            
        }

        /// <summary>
        /// Convert list of words into list of strings, where list.Text will be inserted into result list.
        /// </summary>
        /// <param name="list">List of words to convert</param>
        /// <returns>List of strings</returns>
        private List<ExpressionNode> WordToNodeList(List<Word> list)
        {
            List<ExpressionNode> result = new List<ExpressionNode>();
            for(int i = 0; i < list.Count(); i++)
            {
                
                ExpressionNode en = new ExpressionNode(list[i].Text, i);
                result.Add(en);
            }

            return result;
        }

        /// <summary>
        /// Solve equation from a list of strings. Each string should represent one operator or operand.
        /// Error strings signify, that something happened during computing. If you see no exception in stdout, 
        /// one of operands was probably wrong.
        /// Error strings (check stdout for detailed error):
        /// "errFact" => factorial error (possibly overflow or wrong operand)
        /// "errPow" => pow error (possibly one of operands is wrong)
        ///  "" => other error 
        /// </summary>
        /// <returns>Result of an equation as a string. Returns error string on error</returns>
        private string Solve(List<ExpressionNode> list)
        {
            int idx;
            int startAt = 0;

            //Change letters from memory into numbers
            SolveMemory(list);

            //We are solving the equation from the most significant operators 
            //we don't have to solve numbers themeselves, as they won't create trees and are already nodes.

            //Brackets ---------------------------------------
            while ((idx = GetItemIndex("(", list)) != -1)
            {
                int idx2 = -1;
                if ((idx2 = GetItemIndexFromBack(")", list)) != -1 && (idx != idx2 + 1))
                {
                    //We found another absolute value sign, solve the expression inside of it. 
                    List<ExpressionNode> insideList = new List<ExpressionNode>();
                    for (int i = idx + 1; i < idx2; i++)
                    {
                        insideList.Add(list[i]);
                        /*Console.Write("Adding:");
                        Console.WriteLine(list[i].value);*/
                    }
                    if (insideList.Count() != 0)
                    {
                        string result = Solve(insideList);
                        if (result.Length >= 3 && result.Substring(0, 3) == "err")
                        {
                            //Exression inside brackets was invalid.
                            return "errBrackets";
                        }
                        else
                        {

                            list[idx].value = result;
                            for (int i = idx + 1; i <= idx2; i++)
                            {
                                list[i] = list[idx];
                            }
                        }
                    }
                    else
                    {
                        //There was nothing in the absolute value.
                        return "errBrackets";
                    }
                }
                else
                {
                    //We did not find another absolute value sing, expression is invalid
                    //Todo - check this in syntax highlightning
                    return "errBrackets";
                }
            }

            //Absolute value ---------------------------------------
            while ((idx = GetItemIndex("|", list)) != -1)
            {
                int idx2 = -1;
                if((idx2 = GetItemIndex("|", list, idx + 1)) != -1 && (idx != idx2 + 1)) {
                    //We found another absolute value sign, solve the expression inside of it. 
                    List<ExpressionNode> insideList = new List<ExpressionNode>();
                    for(int i = idx + 1; i < idx2; i++)
                    {
                        insideList.Add(list[i]);
                        /*Console.Write("Adding:");
                        Console.WriteLine(list[i].value);*/
                    }
                    if(insideList.Count() != 0)
                    {
                        string result = Solve(insideList);
                        if(result.Length >= 3 && result.Substring(0,3) == "err")
                        {
                            //Exression inside absolute value was invalid.
                            return "errAbs";
                        }
                        else
                        {
                            if(Double.TryParse(result, out double dres))
                            {
                                list[idx].value = Convert.ToString(MathLibrary.Math.Abs(dres));
                                for (int i = idx + 1; i <= idx2; i++)
                                {
                                    list[i] = list[idx];
                                }
                            }
                            else
                            {
                                return "errAbs";
                            }
                            
                        }
                    }
                    else
                    {
                        //There was nothing in the absolute value.
                        return "errAbs";
                    }
                }
                else
                {
                    //We did not find another absolute value sing, expression is invalid
                    //Todo - check this in syntax highlightning
                    return "errAbs";
                }
            }

            //Factorial --------------------------------------------
            startAt = 0;
            while ((idx = GetItemIndex("!", list, startAt)) != -1)
            {
                startAt = idx + 1;
                if(Double.IsNaN(SolveOperator(ref list, idx, f: MathLibrary.Math.Factorial)))
                {
                    return "errFact";
                }
            }

            //Pow function ----------------------------------------
            startAt = 0;
            while ((idx = GetItemIndex("^", list, startAt)) != -1)
            {
                startAt = idx + 1;
                if(Double.IsNaN(SolveOperator(ref list, idx, f: MathLibrary.Math.Pow)))
                {
                    return "errPow";
                }
            }

            startAt = 0;
            //Root function ----------------------------------------
            while ((idx = GetItemIndex("√", list, startAt)) != -1)
            {
                startAt = idx + 1;
                if (Double.IsNaN(SolveOperator(ref list, idx, f: MathLibrary.Math.Root, reverseOrder: true)))
                {
                    return "errRoot";
                }

            }

            startAt = 0;
            //Divide function ----------------------------------------
            while ((idx = GetItemIndex("÷", list, startAt)) != -1)
            {
                startAt = idx + 1;
                if (Double.IsNaN(SolveOperator(ref list, idx, f: MathLibrary.Math.Divide)))
                {
                    return "errDivide";
                }
            }

            startAt = 0;
            //Multiply function ----------------------------------------
            while ((idx = GetItemIndex("×", list, startAt)) != -1)
            {
                startAt = idx + 1;
                if (Double.IsNaN(SolveOperator(ref list, idx, f: MathLibrary.Math.Multiply)))
                {
                    return "errMultiply";
                }
            }

            startAt = 0;
            //Sub function ----------------------------------------
            while ((idx = GetItemIndex("-", list, startAt)) != -1)
            {
                startAt = idx + 1;
                if (idx == 0 || !Double.TryParse(list[idx - 1].value, out double tmp))
                {
                    if(idx < list.Count() - 1 && Double.TryParse(list[idx + 1].value, out double val))
                    {
                        val = -val;
                        list[idx].value = Convert.ToString(val);
                        list[idx + 1] = list[idx];
                    }
                } 
                else if (Double.IsNaN(SolveOperator(ref list, idx, f: MathLibrary.Math.Sub)))
                {
                    return "errSub";
                }
            }

            startAt = 0;
            //Add function ----------------------------------------
            while ((idx = GetItemIndex("+", list, startAt)) != -1)
            {
                startAt = idx + 1;
                if (Double.IsNaN(SolveOperator(ref list, idx, f: MathLibrary.Math.Add)))
                {
                    return "errAdd";
                }
            }

            //Add variable to memory (operator =) ----------------
            //At least 3 items must exist
            if(list.Count() >= 3)
            {
                //Second character is =, first should be a character
                if (list[1].value == "=")
                {
                    if (IsLetter(list[0].value) && Double.TryParse(list[2].value, out double num))
                    {
                        AddToMemory(list[0].value[0], num);
                        //Console.WriteLine("Added to memory");
                        FillSubtreeWithNodes(ref list, list[2], list[1]);
                        FillSubtreeWithNodes(ref list, list[2], list[0]);

                    }
                }
            }
            

            /*Console.WriteLine("Debug list:");
            for(int i = 0; i < list.Count(); i++)
            {
                Console.Write(list[i].value);
                Console.Write(",");
            }
            Console.WriteLine("END");*/

            return list[0].value;
        }

        /// <summary>
        /// Changes all nodes in a list that match the node "subtree" to node "to"
        /// </summary>
        /// <param name="list">List to change</param>
        /// <param name="to">Node to replace subtree with</param>
        /// <param name="subtree">Subtree to replace</param>
       private void FillSubtreeWithNodes(ref List<ExpressionNode> list, ExpressionNode to, ExpressionNode subtree)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            for (int i = 0; i < list.Count(); i++)
            {
                if(list[i].id == subtree.id)
                {
                    list[i] = to;
                }
            }
        }

        /// <summary>
        /// Solves a single part of input expression
        /// </summary>
        /// <param name="list">Reference to a list of items</param>
        /// <param name="idx">Index in the list, where the operator can be found</param>
        /// <param name="f">Function to call on operands</param>
        /// <returns>Result of an operation on success, NaN on error</returns>
        private double SolveOperator(ref List<ExpressionNode> list ,int idx, Func<double, double, double> f)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            //Create subtree
            if (idx > 0 && idx < (list.Count() - 1))
            {
                //Set parent to previous node/subtree
                list[idx - 1].parent = list[idx];
                //Set number/subtree as a child of operator node
                list[idx].left = list[idx - 1];

                //Set parent to next node/subtree
                list[idx + 1].parent = list[idx];
                //Set number/subtree as a child of operator node
                list[idx].right = list[idx + 1];

            }
            else
            {
                return Double.NaN;
            }

            try
            {
                //Calculate the result with given function
                double result = f(Convert.ToDouble(list[idx].left.value), Convert.ToDouble(list[idx].right.value));
                //Set node value to th result
                list[idx].value = Convert.ToString(result);
                //Change reference, so other operators would detect this whole subtree and use it.
                FillSubtreeWithNodes(ref list, list[idx], list[idx - 1]);
                FillSubtreeWithNodes(ref list, list[idx], list[idx + 1]);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Double.NaN;
            }
        }

        /// <summary>
        /// Solves a single part of input expression
        /// </summary>
        /// <param name="list">Reference to a list of items</param>
        /// <param name="idx">Index in the list, where the operator can be found</param>
        /// <param name="f">Function to call on operands</param>
        /// <param name="reverseOrder">If true, the number in front of operand will be interpreted as int and sent to function as second parameter</param>
        /// <returns>Result of an operation on success, NaN on error</returns>
        private double SolveOperator(ref List<ExpressionNode> list, int idx, Func<double, int, double> f, bool reverseOrder = false)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            //Create subtree
            if (idx > 0 && idx < (list.Count() - 1))
            {
                //Set parent to previous node/subtree
                list[idx - 1].parent = list[idx];
                //Set number/subtree as a child of operator node
                list[idx].left = list[idx - 1];

                //Set parent to next node/subtree
                list[idx + 1].parent = list[idx];
                //Set number/subtree as a child of operator node
                list[idx].right = list[idx + 1];
            }
            else
            {
                return Double.NaN;
            }
            try
            {
                double result;
                //Calculate the result with given function
                if (reverseOrder)
                {
                    result = f(Convert.ToDouble(list[idx].right.value), Convert.ToInt32(list[idx].left.value));
                }
                else
                {
                    result = f(Convert.ToDouble(list[idx].left.value), Convert.ToInt32(list[idx].right.value));
                }
                
                //Set node value to th result
                list[idx].value = Convert.ToString(result);
                //Change reference, so other operators would detect this whole subtree and use it.
                FillSubtreeWithNodes(ref list, list[idx], list[idx - 1]);
                FillSubtreeWithNodes(ref list, list[idx], list[idx + 1]);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Double.NaN;
            }
        }

        /// <summary>
        /// Solves a single part of input expression
        /// </summary>
        /// <param name="list">Reference to a list of items</param>
        /// <param name="idx">Index in the list, where the operator can be found</param>
        /// <param name="f">Function to call on operands</param>
        /// <returns>Result of an operation on success, NaN on error</returns>
        private double SolveOperator(ref List<ExpressionNode> list, int idx, Func<int, double> f)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            //Create subtree
            if (idx > 0)
            {
                //Set parent to previous node/subtree
                list[idx - 1].parent = list[idx];
                //Set number/subtree as a child of operator node
                list[idx].left = list[idx - 1];
                
            }
            else
            {
                return Double.NaN;
            }

            try
            {
                //Calculate the result with given function
                double result = f(Convert.ToInt32(list[idx].left.value));
                //Set node value to th result
                list[idx].value = Convert.ToString(result);
                //Change reference, so other operators would detect this whole subtree and use it.
                FillSubtreeWithNodes(ref list, list[idx], list[idx - 1]);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Double.NaN;
            }
        }

        /// <summary>
        /// Returns index of a word, that has item as its Text parameter
        /// </summary>
        /// <param name="item">Item to find</param>
        /// <param name="list">List to find items from</param>
        /// <param name="startAt">Index to start searching at</param>
        /// <returns>Index in words or -1 if item was not found</returns>
        private int GetItemIndex(string item, List<string> list, int startAt = 0)
        {
            for (int i = startAt; i < words.Count(); i++)
            {
                if (list[i] == item)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns index of an Expression Node, that has item as its value parameter
        /// </summary>
        /// <param name="item">Item to find</param>
        /// <param name="list">List to find items from</param>
        /// <param name="startAt">Index to start searching at</param>
        /// <returns>Index in words or -1 if item was not found</returns>
        private int GetItemIndexFromBack(string item, List<ExpressionNode> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].value == item)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns index of a word, that has item as its value parameter
        /// </summary>
        /// <param name="item">Item to find</param>
        /// <param name="list">List to find items from</param>
        /// <param name="startAt">Index to start searching at</param>
        /// <returns>Index in words or -1 if item was not found</returns>
        private int GetItemIndex(string item, List<ExpressionNode> list, int startAt = 0)
        {
            for (int i = startAt; i < list.Count(); i++)
            {
                if (list[i].value == item)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetItemIndex(string item, List<Word> list)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Text == item)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns index of a first word, that has type as its Type parameter
        /// </summary>
        /// <param name="type">Type to find</param>
        /// <param name="list">List to find items from</param>
        /// <returns>Index of first occurence of item with given type</returns>
        private int GetItemIndex(int type, List<Word> list)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Type == type)
                {
                    return i;
                }
            }
            return -1;
        }

        private void FactorialButtonClicked(object sender, MouseButtonEventArgs e)
        {
            ButtonEnter(sender, e);
            inputTextBox.AppendText("!");
            inputTextBox.CaretPosition = inputTextBox.CaretPosition.DocumentEnd;
        }

        private void PowerButtonClicked(object sender, MouseButtonEventArgs e)
        {
            ButtonEnter(sender, e);
            inputTextBox.CaretPosition.InsertTextInRun("^");
            inputTextBox.Focus();
            try
            {
                inputTextBox.CaretPosition = inputTextBox.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            catch (Exception ecx)
            {
                Console.WriteLine(ecx.Message);
            }
        }

        private void RootButtonClicked(object sender, MouseButtonEventArgs e)
        {
            ButtonEnter(sender, e);
            inputTextBox.CaretPosition.InsertTextInRun("√");
            inputTextBox.Focus();
            try
            {
                inputTextBox.CaretPosition = inputTextBox.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            catch (Exception ecx)
            {
                Console.WriteLine(ecx.Message);
            }
        }

        private void AbsButtonClicked(object sender, MouseButtonEventArgs e)
        {
            ButtonEnter(sender, e);
            inputTextBox.AppendText("||");
            inputTextBox.Focus();
            inputTextBox.CaretPosition = inputTextBox.Document.ContentEnd;
            inputTextBox.CaretPosition = inputTextBox.CaretPosition.GetNextInsertionPosition(LogicalDirection.Backward);
        }

        private void BracketsButtonClicked(object sender, MouseButtonEventArgs e)
        {
            ButtonEnter(sender, e);
            inputTextBox.AppendText("()");
            inputTextBox.Focus();
            inputTextBox.CaretPosition = inputTextBox.Document.ContentEnd;
            inputTextBox.CaretPosition = inputTextBox.CaretPosition.GetNextInsertionPosition(LogicalDirection.Backward);
        }

        private void MenuItemKonecClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItemNapovedaClick(object sender, RoutedEventArgs e)
        {
            help help = new help();
            help.Show();
        }

        private void MenuItemOProgramuClick(object sender, RoutedEventArgs e)
        {
            about about = new about();
            about.Show();
        }
    }
}
