using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SystemCSharp.Fibers;
using System.Diagnostics;

namespace FiberTest
{
    class Program
    {
        
        //very simple countdown for test purposes.
        public class Countdown
        {
            private ManualResetEvent wait = new ManualResetEvent(false);
            private int count;
            private int initialCount;

            public Countdown(int count) { this.initialCount = count; this.count = count; }

            public void StartCounting() { wait.WaitOne(); }

            public void Decrement()
            {
                if (Interlocked.Decrement(ref count) == 0)
                    wait.Set();
            }

            public void Reset() { this.count = initialCount; wait.Reset(); }
        }


        static Countdown cnt = new Countdown(100);
        
        static int smallLoop = 100000;
        static int largeLoop = 900000;

        static IEnumerable<FiberStatus> F()
        {
            string s = string.Empty;

            for (int i = 0; i < smallLoop; i++)
            {
                s = i.ToString();
                yield return FiberStatus.Yield();
            }
            cnt.Decrement();
        }

        static IEnumerable<FiberStatus> F2()
        {
            string s = string.Empty;

            for (int i = 0; i < largeLoop; i++)
            {
                s = i.ToString();
                yield return FiberStatus.Yield();
            }
            cnt.Decrement();
        }

        static void TestFibers()
        {
            Fiber[] fibers = new Fiber[100];
            Stopwatch w = new Stopwatch();

            for (int i = 0; i < 100; i++)
            {
                if (i % 2 == 0)
                    fibers[i] = new Fiber(F);
                else
                    fibers[i] = new Fiber(F2);
            }

            for (int k = 0; k < 2; k++)
            {
                w.Start();

                for (int i = 0; i < 100; i++)
                {
                    fibers[i].Run();
                }

                cnt.StartCounting();
                w.Stop();
                Console.WriteLine("Fibers took: {0}", w.ElapsedMilliseconds);
            }
        }

        static void Main(string[] args)
        {
            TestFibers();
        }
    }
}
