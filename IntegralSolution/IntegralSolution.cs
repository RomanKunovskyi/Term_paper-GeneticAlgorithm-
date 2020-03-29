using System;

namespace IntegralSolution
{
    public abstract class AIntegralSolution
    {
        public double E { get; set; } = 0.00001;
        public int MaxM { get; set; } = 32768;
        public double GetSolution(double a, double b, Func<double, double> Function)
        {
            int m = 2;
            double integral1;
            double integral2;
            do
            {
                integral1 = CalculateMethod(a, b, m, Function);
                m *= 2;
                integral2 = CalculateMethod(a, b, m, Function);
                //Console.WriteLine("1 = " + integral1);
                //Console.WriteLine("2 = " + integral2);
                //Console.WriteLine("m= " + m);
                //Console.WriteLine(Math.Abs(integral2 - integral1) / Math.Abs(integral2));
                //Console.WriteLine(E);
                //Console.WriteLine();
            } while (Math.Abs(integral2 - integral1) / Math.Abs(integral2) > E && m <= MaxM);
            return integral2;
        }
        protected double[] GetNodes(double a, double h, int m)
        {
            double[] nodes = new double[m + 1];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = a + i * h;
            }
            return nodes;
        }
        protected abstract double CalculateMethod(double a, double b, int m, Func<double, double> Function);

    }
}
