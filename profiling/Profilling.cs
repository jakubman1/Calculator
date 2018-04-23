using System;

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
            x++;
        }

        x2 = x;

        //soucet vsech prvku pole
        do while (x2 != 0)
            {
                suma = array[x2] + suma;
                x2--;
            }

        //vypocet prumeru
        prum = suma/x   //tu problem
        x2 = x;

        //vypocet jednotlivych prvku sumy
        do while (x2 != 0)
            {
                suma = (array[x2] - prum)* (array[x2] - prum) + suma //tu problem
                    x2--;
            }

        //vrati odchylku
        return squareroot(suma/x-1)  //tu problem

    }
}
