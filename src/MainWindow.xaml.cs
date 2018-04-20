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
using MathLibrary;

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

        struct Word
        {
            public TextPointer StartPos;
            public TextPointer EndPos;
            public string Text;
            public int Type; //0=numbers,1=standard operators,2=special operators,3=exponents,4=letters(memory),5=absolute value, -1=error state
        }

        struct MemItem
        {
            public char Letter;
            public double Value;
        }
        private List<Word> words = new List<Word>();
        private List<MemItem> memory = new List<MemItem>();

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

            //Console.WriteLine(text);
            //Lets go through the whole text, letter by letter
            for(int i = 0; i < text.Length; i++)
            {
                
                //We found a number
                if ((text[i] >= '0' && text[i] <= '9') || (text[i] == '.' || text[i] == ','))
                {
                    //Check for the whole number
                    while (i < text.Length && ((text[i] >= '0' && text[i] <= '9') || (text[i] == '.' || text[i] == ',')))
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

                    //Number can not end with a dot, also you can not divide by zero
                    if((text[endIndex] == '.' || text[endIndex] == ',') || (startIndex > 0 && text[startIndex - 1] == '÷' && Convert.ToDouble(w.Text) == 0) )
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
                        w.Type = 1;
                    }
                    //Operator has two characters - We are replacing the text, if we find ** or ××
                    else if ((endIndex + 1 - startIndex) == 2)
                    {
                        if(w.Text == "^" )
                        {
                            w.Type = 2;
                            //w.EndPos = run.ContentStart.GetPositionAtOffset(endIndex, LogicalDirection.Backward);
                        }
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
                    endIndex = i - 1;
                    i--;
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
                else if (text[i] >= '|')
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
            char[] operators = { '+', '-', '*', '/', '=', '!', '^', '|', '×', '÷', '√' };
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
                    return memory[i].Value;
                }
            }
            return Double.NaN;
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
            if (inputTextBox.Document == null)
            {
                TextBlockResult.Text = "0";
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
                    //TextBlockResult.Text = "Chyba";
                }
                else
                {
                    
                    string result = Solve(WordToNodeList(words));
                    
                    if (result == "" || (result.Length >= 3 && result.Substring(0,3) == "err"))
                    {
                        
                    }
                    else
                    {
                        TextBlockResult.Text = result;
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

        // testovaci funkce, je zapotrebi zavolat ButtenPressedEffect a v eventech tlacitka zavolat ButtonEnter pri MouseLeftButtonUp
        private void Button42Clicked(object sender, MouseButtonEventArgs e)
        {
            ButtonPressedEffect(sender, e);
            //inputTextBox.AppendText("42");
        }


        /// <summary>
        /// Click handler for buttons, that add their content to the input field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimpleButtonClicked(object sender, MouseButtonEventArgs e)
        {
            ButtonEnter(sender, e);
            inputTextBox.AppendText((string)((Label)sender).Content);
        }


        private void buttonClear_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBlockResult.Text = "0";
            inputTextBox.Document.Blocks.Clear();
        }

        private void buttonDelete_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(words.Count() > 0)
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
                
                ExpressionNode en = new ExpressionNode(list[i].Text);
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
        /// "errPow" => pow error (possibly one of operands is wrong
        ///  "" => other error 
        /// </summary>
        /// <returns>Result of an equation as a string. Returns error string on error</returns>
        private string Solve(List<ExpressionNode> list)
        {

            int idx;
            int startAt = 0;

            //We are solving the equation from the most significant operators 
            //we don't have to solve numbers, as they won't create trees and are already nodes.

            //Factorial --------------------------------------------
            while ((idx = GetItemIndex("!", list, startAt)) != -1)
            {
                //To find the next item
                startAt = idx + 1;
                //Create subtree
                if (idx > 0)
                {
                    //Set parent to previous node/subtree
                    list[idx - 1].parent = list[idx];
                    //Set number/subtree as a child of operator node
                    list[idx].left = list[idx - 1];
                    //Change reference, so other operators would detect this whole subtree and use it.
                    FillSubtreeWithNodes(list, list[idx], list[idx - 1]);
                    try
                    {
                        //Calculate the value of the currently created subtree, so we don't have to do it later.
                        list[idx].value = Convert.ToString(MathLibrary.Math.Factorial(Convert.ToInt32(list[idx].left.value)));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return "errFact";
                    }
                   
                }
                else
                {
                    return "errFact"; //Chyba
                }
            }

            //Pow function ----------------------------------------
            while ((idx = GetItemIndex("^", list, startAt)) != -1)
            {
                //To find the next item
                startAt = idx + 1;
                //Create subtree
                if(idx > 0)
                {
                    //Set parent to previous node/subtree
                    list[idx - 1].parent = list[idx];
                    //Set number/subtree as a child of operator node
                    list[idx].left = list[idx - 1];
                    //Change reference, so other operators would detect this whole subtree and use it.
                    FillSubtreeWithNodes(list, list[idx], list[idx - 1]);
                }
                else
                {
                    return "errPow";
                }
                if(idx < list.Count() - 1)
                {
                    //Set parent to previous node/subtree
                    list[idx + 1].parent = list[idx];
                    //Set number/subtree as a child of operator node
                    list[idx].right = list[idx + 1];
                    //Change reference, so other operators would detect this whole subtree and use it.
                    FillSubtreeWithNodes(list, list[idx], list[idx + 1]);
                }
                else
                {
                    return "errPow";
                }
                try
                {
                    list[idx].value = Convert.ToString(MathLibrary.Math.Pow(Convert.ToDouble(list[idx].left.value), Convert.ToInt32(list[idx].right.value)));
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return "errPow";
                }

            } 

            return list[0].value;

        }

        /// <summary>
        /// Changes all nodes in a list that match the node "subtree" to node "to"
        /// </summary>
        /// <param name="list">List to change</param>
        /// <param name="to">Node to replace subtree with</param>
        /// <param name="subtree">Subtree to replace</param>
       private void FillSubtreeWithNodes(List<ExpressionNode> list, ExpressionNode to, ExpressionNode subtree)
        {
            for(int i = 0; i < list.Count(); i++)
            {
                if(list[i] == subtree)
                {
                    list[i] = to;
                }
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
        /// Returns index of a word, that has item as its value parameter
        /// </summary>
        /// <param name="item">Item to find</param>
        /// <param name="list">List to find items from</param>
        /// <param name="startAt">Index to start searching at</param>
        /// <returns>Index in words or -1 if item was not found</returns>
        private int GetItemIndex(string item, List<ExpressionNode> list, int startAt = 0)
        {
            for (int i = startAt; i < words.Count(); i++)
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
            for (int i = 0; i < words.Count(); i++)
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

    }


}
