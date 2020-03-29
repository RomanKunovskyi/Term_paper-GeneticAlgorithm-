using System;

namespace IntegralSolution
{
    public class MethodOfTrapezes : AIntegralSolution
    {
        protected override double CalculateMethod(double a, double b, int m, Func<double, double> Function)
        {
            double h = (b - a) / m;
            double[] nodes = GetNodes(a, h, m);
            double solution = 0;
            solution += Function(nodes[0])/2;
            for(int i = 1;i<nodes.Length-1;i++)
            {
                solution += Function(nodes[i]);
            }
            solution += Function(nodes[m]) / 2;
            return solution*h;
        }
    }
}
