using ConsoleTables;
using System.Diagnostics.Metrics;
namespace LR1MAT
{
    struct Support
    {
        public bool Switch;
        public int index;
        public Support(bool B, int i) { this.Switch = B; this.index = i; }
        private static readonly long NegativeZeroBits =
    BitConverter.DoubleToInt64Bits(-0.0);
        public static bool IsNegativeZero(double x)
        {
            return BitConverter.DoubleToInt64Bits(x) == NegativeZeroBits;
        }
    }
    public class Table
    {
        public double[] c;
        public double[,] A;
        public double[] b;
        protected string[] column;
        protected string[] row;
        protected double[,] simplex_table;
        protected double[] end_args;
        int free_variables;
        int basic_variables;
        public Table(double[] c, double[,] A, double[] b)
        {
            this.c = c;
            this.A = A;
            this.b = b;
            free_variables = c.Length;
            basic_variables = b.Length;
            this.column = new string[basic_variables + 1]; this.column[^1] = "F";
            this.row = new string[free_variables + 1]; this.row[0] = "S";
            simplex_table = new double[(basic_variables + 1), (free_variables + 1)];
            end_args = new double[c.Length];
            for (int i = 0; i < basic_variables; i++)
            {
                this.column[i] = $"X{i + 1 + free_variables}";
            }
            for (int i = 1; i < free_variables + 1; i++)
            {
                this.row[i] = $"X{i}";
            }
            for (int i = 0; i < basic_variables; i++)
            {
                this.simplex_table[i, 0] = b[i];
                for (int j = 1; j < free_variables + 1; j++)
                {
                    this.simplex_table[i, j] = A[i, j - 1];
                }
            }
            this.simplex_table[basic_variables, 0] = 0;
            for (int i = 1; i < free_variables + 1; i++)
            {
                this.simplex_table[basic_variables, i] = c[i - 1];
            }
        }
        void print()
        {
            string[] start = new string[row.Length + 1];
            start[0] = "X";
            for (int i = 1; i < row.Length + 1; i++)
            {
                start[i] = row[i - 1];
            }
            var table = new ConsoleTable(start);
            for (int i = 0; i < (basic_variables + 1); i++)
            {
                string[] trow = new string[row.Length + 1];
                trow[0] = $"{column[i]}";
                for (int j = 0; j < (free_variables + 1); j++)
                {
                    if (Support.IsNegativeZero(simplex_table[i, j])) trow[j + 1] = " 0.00";
                    else
                    {
                        if (simplex_table[i, j] >= 0) trow[j + 1] += " ";
                        trow[j + 1] += $"{simplex_table[i, j]:F2}";
                    }
                }
                table.AddRow(trow);


            }
            table.Write(Format.Default);
            Console.WriteLine();
        }
        Support Negative_in_free_members_column()
        {
            for (int i = 0; i < basic_variables; i++)
            {
                if (simplex_table[i, 0] < 0)
                {
                    return new Support(true, i);
                }
            }
            return new Support(false, 0);
        }
        Support Positive_in_last_row()
        {
            for (int i = 1; i < free_variables + 1; i++)
            {
                if (simplex_table[basic_variables, i] > 0)
                {
                    return new Support(true, i);
                }
            }
            return new Support(false, 0);
        }
        int iter(int x)
        {
            int y = 0;
            double mini = double.MaxValue;
            int counter = 0;
            for (int i = 0; i < basic_variables; i++)
            {
                if (simplex_table[i, x] < 0) counter++;
            }
            if (counter == basic_variables) { return 2; }
            for (int i = 0; i < basic_variables; i++)
            {
                double relation = simplex_table[i, 0] / simplex_table[i, x];
                if (relation < mini && relation >= 0) { mini = relation; y = i; }
            }
            if (mini == double.MaxValue) { return 1; }
            double[,] table2 = new double[(basic_variables + 1), (free_variables + 1)];
            string _ = row[x];
            row[x] = column[y];
            column[y] = _;
            table2[y, x] = 1 / simplex_table[y, x];
            for (int i = 0; i < (free_variables + 1); i++) //Srj
            {
                if (i != x)
                {
                    table2[y, i] = simplex_table[y, i] / simplex_table[y, x];
                }
            }
            for (int i = 0; i < (basic_variables + 1); i++) //Sik
            {
                if (i != y)
                {
                    table2[i, x] = -simplex_table[i, x] / simplex_table[y, x];
                }
            }
            for (int i = 0; i < (basic_variables + 1); i++)
            {
                for (int j = 0; j < (free_variables + 1); j++)
                {
                    if (j != x && i != y)
                    {
                        table2[i, j] = simplex_table[i, j] - ((simplex_table[y, j] * simplex_table[i, x]) / simplex_table[y, x]);
                    }
                }
            }
            simplex_table = table2;
            return 0;
        }
        public virtual void end_print(double F)
        {
            Console.WriteLine($"F = {-F:F2}");
        }
        public int simplex_metod()
        {
            print();
            Support Flag1 = Negative_in_free_members_column();
            while (Flag1.Switch)
            {
                int x = 0;
                int _count = 0;
                for (int i = 1; i < free_variables + 1; i++) if (simplex_table[Flag1.index, i] < 0)
                    {
                        if (_count == 0) x = i;
                    }
                int Error = iter(x);
                if (Error == 1) { return 1; }
                else if (Error == 2) { return 2; }
                    print();
                Flag1 = Negative_in_free_members_column();
            }
            Support Flag2 = Positive_in_last_row();
            while (Flag2.Switch)
            {
                int Error = iter(Flag2.index);
                if (Error == 1) { return 1; }
                else if (Error == 2) { return 2; }
                print();
                Flag2 = Positive_in_last_row();
            }
            for (int i = 0; i < basic_variables; i++)
            {
                int number = (int)this.column[i][1] - (int)'0';
                if (1 <= number && number <= c.Length)
                    end_args[number - 1] = this.simplex_table[i, 0];
            }
            for (int i = 0; i < end_args.Length; ++i)
            {
                Console.Write($"X{i + 1} = {end_args[i]:F2}\t");
            }

            end_print(this.simplex_table[basic_variables, 0]);
            return 0;
        }
    }
}
