using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StaticThreading
{
    public static class Test_5_Thread
    {
        public static Particle[] Particles;
        public static StringBuilder sbA = new StringBuilder(4096);
        public static StringBuilder sbB = new StringBuilder(4096);

        public static Stopwatch St = new Stopwatch();

        //create/initialize particle array
        public static void Constructor()
        {
            Particles = new Particle[Globals.Size];
            Reset();
        }

        //reset particle array to known starting state
        public static void Reset()
        {
            for (int i = 0; i < Globals.Size; i++)
            {
                Particles[i].X = 0;
                Particles[i].Y = 0;
                Particles[i].State = 0;
                Particles[i].Id = 0;
            }
        }
        
        //test system, write log
        public static void RunTest()
        {
            StringBuilder sb = new StringBuilder(4096);
            St.Start();

            //kick off two tasks, wait for them to complete
            Task Task1 = Task.Factory.StartNew(() => UpdateA(St));
            Task Task2 = Task.Factory.StartNew(() => UpdateB(St));
            Task.WaitAll(Task1, Task2);
            
            St.Stop();
            sb.Append(sbA.ToString());
            sb.Append(sbB.ToString());
            sb.Append("\n\ncomplete, elapsed: "
                + St.ElapsedMilliseconds + " ms.\n\n");

            //write output log
            string Dir = Path.Combine(Globals.GetDir(), "Test_5_2Tasks.txt");
            Debug.WriteLine("writing output to: " + Dir);
            using (StreamWriter w = new StreamWriter(Dir))
            { w.WriteLine(sb.ToString()); }
        }
        
        public static void UpdateA(Stopwatch St)
        {
            int size = Globals.Size / 2;
            for (int g = 1; g < Globals.Iterations; g++)
            {
                for (int i = 0; i < size; i++)
                {   //update
                    Particles[i].X++;
                    Particles[i].Y++;
                    Particles[i].State++;
                    Particles[i].Id++;
                }
                sbA.Append("\nPos: " + Particles[0].X);
                sbA.Append(" frame " + g + " elapsed: "
                    + St.ElapsedTicks + " ticks.");
            }
        }

        public static void UpdateB(Stopwatch St)
        {
            int start = Globals.Size / 2;
            for (int g = 1; g < Globals.Iterations; g++)
            {
                for (int i = start; i < Globals.Size; i++)
                {   //update
                    Particles[i].X++;
                    Particles[i].Y++;
                    Particles[i].State++;
                    Particles[i].Id++;
                }
                sbB.Append("\nPos: " + Particles[start].X);
                sbB.Append(" frame " + g + " elapsed: "
                    + St.ElapsedTicks + " ticks.");
            }
        }

    }
}