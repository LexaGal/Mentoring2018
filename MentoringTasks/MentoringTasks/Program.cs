using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace MentoringTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            Task6();
        }

        public static void Task6()
        {
            var task1 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task1: Started");
                Console.WriteLine($"Thread id: {Thread.CurrentThread.ManagedThreadId}");
            });
            var cont1 = task1.ContinueWith(t =>
            {
                Console.WriteLine($"Cont1.: Status: {t.Status}");
                Console.WriteLine($"Thread id: {Thread.CurrentThread.ManagedThreadId}\n");
            });

            Task.WaitAll(task1, cont1);

            var tokenSource2 = new CancellationTokenSource();
            var token2 = tokenSource2.Token;

            var task2 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task2: Started");
                Console.WriteLine($"Thread id: {Thread.CurrentThread.ManagedThreadId}");
                token2.WaitHandle.WaitOne();
                tokenSource2.Token.ThrowIfCancellationRequested();
            }, token2);

            var cont2 = task2.ContinueWith(t =>
            {
                Console.WriteLine($"Cont2.: Status: {t.Status}");
                Console.WriteLine($"Thread id: {Thread.CurrentThread.ManagedThreadId}\n");
            }, TaskContinuationOptions.NotOnRanToCompletion);

            tokenSource2.Cancel();
            Task.WaitAll(cont2);

            var task3 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task3: Started");
                Console.WriteLine($"Thread id: {Thread.CurrentThread.ManagedThreadId}");
                throw new Exception();
            });

            var cont3 = task3.ContinueWith(t =>
            {
                Console.WriteLine($"Cont3.: Error: {t.Exception?.InnerExceptions.First()?.Message}");
                Console.WriteLine($"Thread id: {Thread.CurrentThread.ManagedThreadId}\n");
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, new CurrentThreadTaskScheduler());
            
            Task.WaitAll(cont3);

            var tokenSource4 = new CancellationTokenSource();
            var token4 = tokenSource4.Token;

            var task4 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task4: Started");
                Console.WriteLine($"Thread id: {Thread.CurrentThread.ManagedThreadId}");
                token4.WaitHandle.WaitOne();
                tokenSource4.Token.ThrowIfCancellationRequested();
            }, token4);

            var cont4 = task4.ContinueWith(t =>
            {
                Console.WriteLine($"Cont4.: Status: {t.Status}");
                Console.WriteLine($"Thread id: {Thread.CurrentThread.ManagedThreadId}\n");
            }, TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning);

            tokenSource4.Cancel();
            Task.WaitAll(cont4);

            Console.ReadKey();
        }

    }
}
