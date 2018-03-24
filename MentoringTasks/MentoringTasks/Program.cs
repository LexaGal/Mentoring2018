using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MentoringTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            Task3();
            Task4();
            Task5();
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
                        ints[i] = ints[i]*rnd.Next(0, 1000);
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
        }

        public static async Task Task5()
        {
            var results1 = new List<int>();
            var results2 = new List<int>();

            var queue = new BufferBlock<int>(new DataflowBlockOptions {BoundedCapacity = 10});
            var consumerOptions = new ExecutionDataflowBlockOptions {BoundedCapacity = 1, MaxDegreeOfParallelism = 2};

            var rnd = new Random();
            var consumer1 = new ActionBlock<int>(async x =>
            {
                await Task.Delay(rnd.Next(1000, 3000));
                results1.Add(x);
            }, consumerOptions);

            var consumer2 = new ActionBlock<int>(async x =>
            {
                await Task.Delay(rnd.Next(1000, 3000));
                results2.Add(x);
            }, consumerOptions);

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true}; //true - else no return
            queue.LinkTo(consumer1, linkOptions);
            queue.LinkTo(consumer2, linkOptions);

            var producer = Produce(queue, Enumerable.Range(1, 10));

            await Task.WhenAll(producer, consumer1.Completion, consumer2.Completion);

            Console.WriteLine("Task 1 operations:");
            results1.ForEach(Console.WriteLine);

            Console.WriteLine("Task 2 operations:");
            results2.ForEach(Console.WriteLine);
        }

        private static async Task Produce(BufferBlock<int> queue, IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                await queue.SendAsync(value);
            }
            queue.Complete();
        }
    }
}
