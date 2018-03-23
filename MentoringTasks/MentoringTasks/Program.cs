using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace MentoringTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            Task3();
            Task4();
        }

        public static void Task4()
        {
            var rnd = new Random();
            Task<int[]>.Factory.StartNew(() =>
                {
                    var ints = new int[10];
                    for (var i = 0; i < 10; i++)
                    {
                        ints[i] = rnd.Next(0, 1000);
                        Console.Write($"{ints[i]} ");
                    }

                    Console.WriteLine();
                    return ints;
                }).ContinueWith(t =>
                {
                    var ints = t.Result;
                    for (var i = 0; i < 10; i++)
                    {
                        ints[i] = ints[i] * rnd.Next(0, 1000);
                        Console.Write($"{ints[i]} ");
                    }
                    Console.WriteLine(); 
                    return ints;
                })
                .ContinueWith(t =>
                {
                    var ints = t.Result;
                    Array.Sort(ints, (i1, i2) => i2.CompareTo(i1));
                    for (var i = 0; i < 10; i++)
                    {
                        Console.Write($"{ints[i]} ");
                    }
                    Console.WriteLine();
                    return ints;
                })
                .ContinueWith(t =>
                {
                    var ints = t.Result;
                    var avg = ints.Average();
                    Console.WriteLine(avg);
                }).Wait();
            Console.ReadKey();
        }

        public static void Task3()
        {
            var tasks = new Task[100];
            for (var i = 0; i < 100; i++)
            {
                var ind = i;
                tasks[i] = new Task(() =>
                {
                    for (var j = 0; j < 1000; j++)
                    {
                        Console.WriteLine($"Task #{ind + 1} - {j + 1} ");
                    }
                });
            }
            Parallel.ForEach(tasks, task => task.Start());
            Task.WaitAll(tasks);
            Console.ReadKey();
        }
    }
}
