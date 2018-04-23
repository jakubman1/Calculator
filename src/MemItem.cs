namespace Calculator
{
    public partial class MainWindow
    {
        public struct MemItem
        {
            public char Letter;
            public double Number;

            public MemItem(char letter, double number)
            {
                Letter = letter;
                Number = number;
            }
        }
    }
}
