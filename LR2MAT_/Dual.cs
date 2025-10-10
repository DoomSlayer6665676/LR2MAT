using LR1MAT;
namespace LR2MAT
{
    public class Dual : Table
    {
        static double[,] reverse_A(double[,] A)
        {
            int row = A.GetLength(1);
            int col = A.GetLength(0);
            double[,] A_ = new double[row, col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    A_[i, j] = -A[j, i];
                }
            }
            return A_;
        }
        static double[] reverse_c(double[] c)
        {
            double[] c_ = new double[c.GetLength(0)];
            for (int i = 0; i < c.GetLength(0); i++) c_[i] = -c[i];
            return c_;
        }
        static double[] reverse_b(double[] b)
        {
            double[] b_ = new double[b.GetLength(0)];
            for (int i = 0; i < b.GetLength(0); i++) b_[i] = -b[i];
            return b_;
        }
        public Dual(double[] c, double[,] A, double[] b) : base(reverse_b(b), reverse_A(A), reverse_c(c)) { }
        public override void end_print(double F)
        {
            Console.WriteLine($"F = {F}");
        }
    }
}
