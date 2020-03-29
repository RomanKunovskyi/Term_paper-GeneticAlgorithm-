using FredholmIntegralEquationOfTheFirstKind;
using IntegralSolution;
using System;
using System.Collections.Generic;

namespace Term_paper
{
    class Program
    {
        static void Main()
        {
            //timer start
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            ////////////////////////////////////////////////////////////////////////////////////////

            //Term_Paper\Term_paper\Term_paper\bin\Debug\netcoreapp3.0\Octave\plotfunc для перевірки функції

            //result x^2
            //FIEFK FIEFK = new FIEFK(
            //    0,
            //    1,
            //    (x, y) => Math.Pow(x, 2) * Math.Pow(y,2),         //K
            //    (x) => 0.4* Math.Pow(x, 2),                       //F
            //    101);


            //result cos x
            FIEFK FIEFK = new FIEFK(
                0,
                Math.PI/2,
                (x, y) => y*x,                     //K
                (x) => -2*x,                       //F
                101);

            GeneticAlgorithm GeneticAlgorithm = new GeneticAlgorithm(FIEFK, new MethodOfTrapezes());
            GeneticAlgorithm.Run();

            ////////////////////////////////////////////////////////////////////////////////////////
            //timer stop
            var resultTime = startTime.Elapsed;
            Console.WriteLine("{0:00}:{1:00}:{2:00}.{3:000}",
                resultTime.Hours,
                resultTime.Minutes,
                resultTime.Seconds,
                resultTime.Milliseconds);
        }
    }
}
