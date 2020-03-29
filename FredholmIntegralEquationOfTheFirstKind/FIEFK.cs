using System;

namespace FredholmIntegralEquationOfTheFirstKind
{
    //класс представляючий нерозвязаний інтеграл фредгольма 1 роду
    public class FIEFK
    {
        //  a
        //  /
        //  | U(y) K(x,y) dy = F(x)
        //  /
        //  b
        public double a { get; private set; }
        public double b { get; private set; }
        public Func<double, double, double> K { get; private set; }
        public Func<double, double> F { get; private set; }

        // масив точок на яких ми будемо перевіряти похибку
        public double[] X { get; private set; }

        public FIEFK(double a,double b, Func<double,double,double> K,Func<double,double> F,int numbeOfNodes)
        {
            this.a = a;
            this.b = b;
            this.K = K;
            this.F = F;
            X = new double[numbeOfNodes];
            double h = (b - a) / (numbeOfNodes-1);
            for (int i = 0; i < X.Length; i++)
            {
                X[i] = a + i * h;
            }
        }
    }
}
