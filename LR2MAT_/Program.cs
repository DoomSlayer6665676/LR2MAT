using LR1MAT;
using LR2MAT;
class Program
{
    static void Main(string[] args)
    {
        double[] c = { 5, 6, 4 };
        double[,] A = { { 1, 1, 1 }, { 1, 3, 0 }, { 0, 0.5, 4 } };
        double[] b = { 7, 8, 6 };
        Console.WriteLine("ПЗ ЛП");
        Console.WriteLine();
        Table table = new(c, A, b);
        table.simplex_metod();
        Console.WriteLine();
        Console.WriteLine("ДЗ ЛП");
        Console.WriteLine();
        Dual rev_table = new(c, A, b);
        rev_table.simplex_metod();
    }
}