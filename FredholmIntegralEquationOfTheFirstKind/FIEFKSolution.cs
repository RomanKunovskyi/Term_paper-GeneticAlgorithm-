using IntegralSolution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FredholmIntegralEquationOfTheFirstKind
{
    class FIEFKSolution
    {
        public double RegularizationParameter { get; set; } = 0.1;

        //Мatr
        private readonly double[,] matr;

        //f(x)
        private readonly double[] arr;

        //                a 
        // |              /                          | 
        // | f(x) - a_i * | ( l_i(y) * K(x,y) )dy    |
        // |              /                          |
        //                b
        public FIEFKSolution(FIEFK Equation, AIntegralSolution IntegralSolutionMethod, int arrLength)  //в констпукторі ми будуємо матрицю чисел // я її назвав Мatr_(m x n) на листку, де я розптсував алгоритм
        {
            matr = new double[Equation.X.Length, arrLength];
            arr = new double[Equation.X.Length];

            int progress = 0;

            for (int i = 0; i < Equation.X.Length; i++)
            {
                arr[i] = Equation.F(Equation.X[i]);
                for (int j = 0; j < arrLength; j++)
                {
                    progress += 1;
                    Console.WriteLine($"{progress}/{matr.Length}");
                    //Console.WriteLine("x = " + Equation.X[i]);
                    //Console.WriteLine("j = " + j);
                    matr[i, j] = IntegralSolutionMethod.GetSolution(Equation.a, Equation.b, (y) => GetL_i(j)(y) * Equation.K(Equation.X[i], y));
                }
            }
            Console.Clear();
        }
        
        private Func<double, double> GetL_i(int i)
        {
            return (y) => Math.Pow(y, i);
        }

        public double GetR_n(double[] a)
        {
            double[] r_n = new double[matr.GetLongLength(0)];
            double sum = 0;

            //console print
            //for (int i = 0; i < Equation.X.Length; i++)
            //{
            //    for (int j = 0; j < a.Length; j++)
            //    {
            //        Console.Write(matr[i, j]+" ");
            //    }
            //    Console.WriteLine();
            //}
            //for (int i = 0; i < Equation.X.Length; i++)
            //{
            //    Console.Write(arr[i]+" ");
            //}
            //Console.WriteLine();

            //////////////////////////////////////////////////////////////////////////////////////  RegularizationParameter*sum(a^2)
            double sum_a = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum_a += Math.Pow(a[i],2);
            }
            sum_a *= RegularizationParameter;
            ////////////////////////////////////////////////////////////////////////////////////// провіряємо на кожній точці  і шукаємо макс похибку
            for (int i = 0; i < matr.GetLongLength(0); i++)
            {
                for (int j = 0; j < a.Length; j++)
                {
                    sum += a[j] * matr[i, j];
                }
                r_n[i] = Math.Pow(arr[i] - sum,2) + sum_a;
                sum = 0;
            }
            return r_n.Max();
        }
    }

    ///class with information about one A
    class A
    {
        public double[] A_i { get; private set; }
        public double R_n { get; private set; }
        public int GenerationId { get; private set; }
        public int CivilizationId { get; private set; }

        public A()
        {

        }
        public A(double[] a_i, double r_n, int generationId, int civilizationId)
        {
            A_i = new double[a_i.Length];
            for (int i = 0; i < a_i.Length; i++)
            {
                A_i[i] = a_i[i];
            }
            R_n = r_n;
            GenerationId = generationId;
            CivilizationId = civilizationId;
        }
    }



    //_____________________________________________________________________________________________
    //____________________________________GeneticAlgorithm_________________________________________
    public class GeneticAlgorithm
    {
        private FIEFKSolution FIEFKSolution;
        public int NumberOfGenerationsBetweenBetterAndCurrent { get; set; } = 20;
        public int NumberOfObjectsInOneGeneration { get; set; } = 1000;
        public (int start, int end) Range { get; set; } = (-3, 3); //Random range
        public int NumberOfDecimalPlaces { get; set; } = 3; //знаків після коми в членів масиву a
        public int ArrLength { get; set; } = 10; // довжина масиву а (довжина шуканого полінома)
        public int NumberOfObjectInTournmet { get; set; } = 2;  
        public double E { get; set; } = 1;

        private A TheBestA;  // краще a серед всіх поколінь


        private readonly Task Creater;
        public GeneticAlgorithm(FIEFK Equation, AIntegralSolution IntegralSolutionMethod)
        {
            Creater = Task.Run(() => FIEFKSolution = new FIEFKSolution(Equation, IntegralSolutionMethod, ArrLength));
            TheBestA = new A();
        }


        public void Run()
        {
            List<A> CurrentGeneration = new List<A>(NumberOfObjectsInOneGeneration);
            List<A> NextGeneration = new List<A>(NumberOfObjectsInOneGeneration);

            List<A> Temp_Winners = new List<A>(NumberOfObjectsInOneGeneration); // winners of tournament in current generation

            int сurrentGenerationId = 0;
            int curentCivilizationId = 0;

            Task[] ACreaters = new Task[NumberOfObjectsInOneGeneration];

            Creater.Wait(); //wait for created FIEFKSolution

            //create first generation in first Civilization
            CreateNewCivilization(CurrentGeneration, ACreaters, сurrentGenerationId, curentCivilizationId);

            ////set start info         
            TheBestA = new A(CurrentGeneration[0].A_i, CurrentGeneration[0].R_n, CurrentGeneration[0].GenerationId, CurrentGeneration[0].CivilizationId);

            //для підрахунку, як прога зарандомила
            int numberOfTournmets;
            int numberOfCrossovers;
            int numberOfMutations;
            int numberOfTheBestAInCurrentGeneration = 0; // для підрахунку, яку частину покоління складають TheBestA

            List<Task> CreaterOfNewGeneration = new List<Task>(); //for faster creating

            A TheBestAInCurrentGeneration = new A();


            int helper = 0; // ми маємо різні цивиліщації...     ця змінна рахує Number Of Generations Between Better And Current без урахування зміни цивилізаціїї

  
            do
            {
                //trigger for creating a new civilization
                if (numberOfTheBestAInCurrentGeneration > NumberOfObjectsInOneGeneration * 0.5)// TheBestA складає 50% покоління
                {
                    сurrentGenerationId = 0;
                    curentCivilizationId += 1;
                    CurrentGeneration.Clear();
                    CreateNewCivilization(CurrentGeneration, ACreaters, сurrentGenerationId, curentCivilizationId);
                }

                //обнуляємо шчоччики
                numberOfTournmets = 0;
                numberOfCrossovers = 0;
                numberOfMutations = 0;
                numberOfTheBestAInCurrentGeneration = 0;

                helper += 1;
                сurrentGenerationId += 1;

                //find the best in current
                TheBestAInCurrentGeneration = new A(CurrentGeneration[0].A_i, CurrentGeneration[0].R_n, CurrentGeneration[0].GenerationId, CurrentGeneration[0].CivilizationId);
                foreach (A obj in CurrentGeneration)
                {
                    if (TheBestAInCurrentGeneration.R_n > obj.R_n)
                    {
                        TheBestAInCurrentGeneration = new A(obj.A_i, obj.R_n, obj.GenerationId, obj.CivilizationId);
                    }
                }
                //може the best in current є найкращим серед всіх поколінь
                if (TheBestA.R_n > TheBestAInCurrentGeneration.R_n)
                {
                    TheBestA = new A(TheBestAInCurrentGeneration.A_i, TheBestAInCurrentGeneration.R_n, TheBestAInCurrentGeneration.GenerationId, TheBestAInCurrentGeneration.CivilizationId);
                    helper = 0;
                }

                //input in txt
                AddToTxtBestR_n(TheBestA.R_n);
                AddToTxtBestR_nInCurrentGeneration(TheBestAInCurrentGeneration.R_n);

                //get num of identical the best A in current generation
                numberOfTheBestAInCurrentGeneration = CurrentGeneration.FindAll((A) => A.R_n == TheBestAInCurrentGeneration.R_n).Count;

                //start//tournament///////////////////////////////////////////////////////////////////////////////////////

                // ACreaters[i] create 9970/10000 for 0.036

                for (int i = 0; i < NumberOfObjectsInOneGeneration; i++)
                {
                    ACreaters[i] = Task.Run(() =>
                    {
                        Random Random = new Random();
                        numberOfTournmets += 1;
                        List<A> TournamentParticipantes = new List<A>();
                        for (int i = 0; i < NumberOfObjectInTournmet; i++)
                        {
                            TournamentParticipantes.Add(CurrentGeneration[Random.Next(0, CurrentGeneration.Count)]);
                        }
                        Temp_Winners.Add(Tournmet(TournamentParticipantes));
                    });
                }
                Task.WaitAll(ACreaters);

                // Parallel.For create 8570/10000 for 0.019
                //Parallel.For(0, NumberOfObjectsInOneGeneration, (a) =>
                //{
                //    numberOfTournmets += 1;
                //    List<A> TournamentParticipantes = new List<A>();
                //    for (int i = 0; i < NumberOfObjectInTournmet; i++)
                //    {
                //        TournamentParticipantes.Add(CurrentGeneration[Random.Next(0, CurrentGeneration.Count)]);
                //    }
                //    Temp_Winners.Add(Tournmet(TournamentParticipantes));
                //});

                //end//tournament//////////////////////////////////////////////////////////////////////////////////////////

                //mutation and crossover
                while (numberOfCrossovers * 2 + numberOfMutations < NumberOfObjectsInOneGeneration) //з numberOfCrossovers ми получажмо 2 члени нового покоління, а з мутації 1
                {
                    CreaterOfNewGeneration.Add(Task.Run(() =>
                    {
                        Random Random = new Random();
                        switch (Random.Next() % 100)
                        {
                            case int k when (k < 10): //10%
                                                      //Mutation
                                numberOfMutations += 1;
                                NextGeneration.Add(Mutation(Temp_Winners[Random.Next() % Temp_Winners.Count], Random));
                                break;
                            case int k when (k >= 10):  //90%
                                                        //Crossover                                                    
                                numberOfCrossovers += 1;
                                (A Child1, A Child2) = Crossover(Temp_Winners[Random.Next() % Temp_Winners.Count],
                                                                                Temp_Winners[Random.Next() % Temp_Winners.Count], Random);
                                NextGeneration.Add(Child1);
                                NextGeneration.Add(Child2);
                                break;
                        }
                    }));
                }
                Task.WaitAll(CreaterOfNewGeneration.ToArray());

                //console print
                ConsolePrintProcess(TheBestAInCurrentGeneration, CurrentGeneration.Count, numberOfCrossovers, numberOfMutations, numberOfTournmets, numberOfTheBestAInCurrentGeneration);

                //die
                CreaterOfNewGeneration.Clear();
                Temp_Winners.Clear();
                CurrentGeneration.Clear();
                foreach (var el in NextGeneration)
                {
                    CurrentGeneration.Add(el);
                }
                NextGeneration.Clear();


            } while (helper - сurrentGenerationId + 1 <= NumberOfGenerationsBetweenBetterAndCurrent || TheBestA.R_n > E);

            ConsolePrintAnswer();
        }



        private A Mutation(A Mutant, Random Random)
        {
            Mutant.A_i[Random.Next(0, Mutant.A_i.Length)] =
                (double)Random.Next(Range.start * (int)Math.Pow(10, NumberOfDecimalPlaces), Range.end * (int)Math.Pow(10, NumberOfDecimalPlaces)) / Math.Pow(10, NumberOfDecimalPlaces);

            return new A(Mutant.A_i, FIEFKSolution.GetR_n(Mutant.A_i), Mutant.GenerationId, Mutant.CivilizationId);
        }

        private (A, A) Crossover(A Perent1, A Perent2, Random Random)
        {
            int index = Random.Next(0, Perent1.A_i.Length);
            double[] temp = new double[index + 1];
            for (int i = 0; i <= index; i++)
            {
                temp[i] = Perent1.A_i[i];
                Perent1.A_i[i] = Perent2.A_i[i];
                Perent2.A_i[i] = temp[i];
            }
            return (new A(Perent1.A_i, FIEFKSolution.GetR_n(Perent1.A_i), Perent1.GenerationId, Perent1.CivilizationId),
                    new A(Perent1.A_i, FIEFKSolution.GetR_n(Perent1.A_i), Perent1.GenerationId, Perent1.CivilizationId));
        }
        private A Tournmet(List<A> TournamentParticipantes)
        {
            A Winner = new A(TournamentParticipantes[0].A_i, TournamentParticipantes[0].R_n, TournamentParticipantes[0].GenerationId + 1, TournamentParticipantes[0].CivilizationId);
            foreach (A Participante in TournamentParticipantes)
            {
                if (Winner.R_n > Participante.R_n)
                {
                    Winner = new A(Participante.A_i, Participante.R_n, Participante.GenerationId + 1, Participante.CivilizationId); //GenerationId+1 (the obj for next generation)
                }
            }
            return Winner;
        }


        private void ConsolePrintProcess(A TheBestAInCurrentGeneration, int numberOfObject, int numberOfCrossovers, int numberOfMutations, int numberOfTournmets, int numberOfThebestAInCurrentGeneration)
        {
            Console.WriteLine("____________________________________________________");
            Console.WriteLine("Current civilization                  : " + TheBestAInCurrentGeneration.CivilizationId);
            Console.WriteLine("Current generation                    : " + TheBestAInCurrentGeneration.GenerationId);
            Console.WriteLine("The best r_n(a) in current generation : " + TheBestAInCurrentGeneration.R_n);
            Console.WriteLine("Number of objects                     : " + numberOfObject);
            Console.WriteLine("Number of crossovers                  : " + numberOfCrossovers);
            Console.WriteLine("Number of mutations                   : " + numberOfMutations);
            Console.WriteLine("Number of tournments                  : " + numberOfTournmets);
            Console.WriteLine("Number of identical the best A        : " + numberOfThebestAInCurrentGeneration);
            Console.Write("The best a_n in current generation    :");
            foreach (var el in TheBestAInCurrentGeneration.A_i)
            {
                Console.Write(el + "  ");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("№ of civilization of the best object  : " + TheBestA.CivilizationId);
            Console.WriteLine("№ of generation of the best object    : " + TheBestA.GenerationId);
            Console.WriteLine("The best r_n(a) ever                  : " + TheBestA.R_n);
            Console.WriteLine("____________________________________________________");
        }

        private void ConsolePrintAnswer()
        {
            Console.WriteLine("Answer______________________________________________");
            Console.WriteLine("№ of the best generation              : " + TheBestA.GenerationId);
            Console.WriteLine("The best r_n(a) ever                  : " + TheBestA.R_n);
            Console.Write("The best a_n ever                     :");
            foreach (var el in TheBestA.A_i)
            {
                Console.Write(el + "  ");
            }
            Console.WriteLine();
            Console.WriteLine("____________________________________________________");
            Console.Write($"({TheBestA.A_i[0].ToString().Replace(',', '.')})*y^{0}");
            for (int i = 1; i < TheBestA.A_i.Length; i++)
            {
                Console.Write($" + ({TheBestA.A_i[i].ToString().Replace(',', '.')}*y^{i})");
            }
            Console.WriteLine();
            Console.WriteLine("____________________________________________________");
        }


        private void CreateNewCivilization(List<A> CurrentGeneration, Task[] ACreaters, int currentGenerationId, int curentCivilizationId)
        {
            double[] a_temp = new double[ArrLength];
            for (int i = 0; i < NumberOfObjectsInOneGeneration; i++)
            {
                ACreaters[i] = Task.Run(() =>
                {
                    Random Random = new Random();
                    a_temp = GetRandomArr(Random);
                    CurrentGeneration.Add(new A(a_temp, FIEFKSolution.GetR_n(a_temp), currentGenerationId, curentCivilizationId));
                });
            }
            Task.WaitAll(ACreaters);
        }

        //get random a
        private double[] GetRandomArr(Random Random)
        {
            double[] a = new double[ArrLength];
            for (int i = 0; i < ArrLength; i++)
            {
                //get double
                //a[i] = Random.NextDouble() * (Range.end - Range.start) + (Range.start); 

                //get double with current numberOfDecimalPlaces
                a[i] = ((double)Random.Next(Range.start * (int)Math.Pow(10, NumberOfDecimalPlaces), Range.end * (int)Math.Pow(10, NumberOfDecimalPlaces))) / Math.Pow(10, NumberOfDecimalPlaces);
            }
            return a;
        }

        //print in txt
        //щоб глянути як міняється загальний кращий резудьтат поколінь
        private readonly StreamWriter Fs1 = new StreamWriter("StateBestR_n.txt");
        private void AddToTxtBestR_n(double value)
        {
            Fs1.Write(value.ToString().Replace(",", ".") + " ");
        }

        //щоб глянути як міняється кращий резудьтат в кожному з поколінь
        private readonly StreamWriter Fs2 = new StreamWriter("StateBestR_nInCurrentGeneration.txt");
        private void AddToTxtBestR_nInCurrentGeneration(double value)
        {
            Fs2.Write(value.ToString().Replace(",", ".") + " ");
        }
    }
}
