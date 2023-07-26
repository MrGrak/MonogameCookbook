using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Threading;

namespace StaticThreading
{
    //a compact 16 byte struct
    public struct Particle { public int X, Y, State, Id; }

    class Program
    {
        static void Main(string[] args)
        {
            //depending on processor count, choose to divide into 2, 4, 8, etc...
            Console.WriteLine("Processor Count: " + Environment.ProcessorCount);
            Console.WriteLine("Instance size: 16 bytes");
            Console.WriteLine("Test size: " + Globals.Size + " instances");
            Console.WriteLine("Estimated size in cache: " 
                + ((Globals.Size * 16) / 1024) + " kbs"
                + " / " + (Globals.Size * 16) + " bytes");
            Console.WriteLine(" ");

            //construct all test classes/cases
            Test_1_Single.Constructor(); //fastest when values of size <= ~1024
            Test_2_Thread.Constructor();
            Test_3_Thread.Constructor();
            Test_4_Thread.Constructor();
            Test_5_Thread.Constructor();
            Test_6_Thread.Constructor(); //fastest when values of size > ~8192
            Test_7_Thread.Constructor(); //you'd expect this to be faster than 6, but no
            Test_8_Thread.Constructor(); //fastest when values of size between ~1024-8192

            Console.WriteLine("Simulating " + Globals.Iterations + " frames...");

            //1. let's start with a simple aos example (baseline)
            Test_1_Single.RunTest();
            //2. build two threaded example using temp thread
            Thread BkgT = new Thread(new ThreadStart(Test_2_Thread.RunTest));
            BkgT.Start();
            //3. build static two threaded example
            Test_3_Thread.T.Start();
            //4. use task to wait
            Test_4_Thread.RunTest();
            //5. use tasks to wait, split work in two
            Test_5_Thread.RunTest();
            //6. use tasks to wait, split work in threes, hardcoded
            Test_6_Thread.RunTest();
            //7. use tasks, split into 4, more mature design
            Test_7_Thread.RunTest();
            //8. use tasks to wait, splitwork into 4s, hardcoded
            Test_8_Thread.RunTest();

            //write important data to console
            Console.WriteLine(String.Format("{0:0000000}", 
                Test_1_Single.St.ElapsedTicks) + " tx - Test 1 single thread");
            Console.WriteLine(String.Format("{0:0000000}", 
                Test_2_Thread.St.ElapsedTicks) + " tx - Test 2 temp thread");
            Console.WriteLine(String.Format("{0:0000000}", 
                Test_3_Thread.St.ElapsedTicks) + " tx - Test 3 static thread");
            Console.WriteLine(String.Format("{0:0000000}", 
                Test_4_Thread.St.ElapsedTicks) + " tx - Test 4 1 task");
            Console.WriteLine(String.Format("{0:0000000}", 
                Test_5_Thread.St.ElapsedTicks) + " tx - Test 5 2 tasks");
            Console.WriteLine(String.Format("{0:0000000}", 
                Test_6_Thread.St.ElapsedTicks) + " tx - Test 6 3 tasks");
            Console.WriteLine(String.Format("{0:0000000}", 
                Test_7_Thread.St.ElapsedTicks) + " tx - Test 7 4 tasks A");
            Console.WriteLine(String.Format("{0:0000000}", 
                Test_8_Thread.St.ElapsedTicks) + " tx - Test 8 4 tasks B");


            Console.WriteLine(" ");
        }
    }
}
