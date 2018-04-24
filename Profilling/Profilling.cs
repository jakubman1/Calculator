using System;
using MathLibrary;

public class Profilling
{
    static void Main(string[] args)
    {
        string line;
        int x = 0;
        int x2;
        double[] array;
        double suma = 0;
        double prum;

        //cteni
        while ((line = Console.ReadLine()) != null)
        {
            array[x] = line;
            Add(x, 1);
        }

        x2 = x;

        //soucet vsech prvku pole
        do while (x2 != 0)
            {
                suma = Add(array[x2], suma);
                Sub(x2, 1);
            }

        //vypocet prumeru
        prum = Divide(suma, x)
        x2 = x;

        //vypocet jednotlivych prvku sumy
        do while (x2 != 0)
            {
                suma = Add(Pow(Sub(array[x2], prum), 2), suma);
                Sub(x2, 1);
            }
        x = Sub(x, 1);
        Console.WriteLine(Root(Divide(suma, x), 2));
        //vrati odchylku

        return 

    }
}
