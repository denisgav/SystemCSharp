using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sc_core;
using System.Threading;

namespace SCS_Example
{    
    class Program
    {
        static void Main(string[] args)
        {
            //Thread t = new Thread(SimpleTest.Test);
			Thread t = new Thread(PortTest.Test);
            t.Start();
            t.Join();
            Console.ReadKey();
        }        
    }
}
