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

        private bool ready = true;
        /// <summary>
        /// When input text is changed, change colors of text based on syntax
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ready)
            {
                CreateColorSyntax(inputTextBox);
            }
             
        }

        private void CreateColorSyntax(RichTextBox textBox)
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
                

                /*//Going through all numbers
                for (Match m = r.Match(text); m.Success; m = m.NextMatch())
                {
                    textBox.Foreground = numberBrush;
                    textBox.AppendText(m.Value);
                }*/
            }
            ready = true;

        }


    }
}
