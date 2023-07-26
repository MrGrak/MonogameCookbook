using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StaticThreading
{
    public static class Test_8_Thread
    {
        //notice the duplication of fields and methods happening
        //we tried rolling these into their own system/structure
        //but this was actually slower than just hardcoding them
        //note that this works great on dual core cpus with hyperthreading
        //or on cpus with more than 4 cores

        public static Particle[] ParticlesA;
        public static Particle[] ParticlesB;
        public static Particle[] ParticlesC;
        public static Particle[] ParticlesD;

        public static StringBuilder sbA = new StringBuilder(4096);
        public static StringBuilder sbB = new StringBuilder(4096);
        public static StringBuilder sbC = new StringBuilder(4096);
        public static StringBuilder sbD = new StringBuilder(4096);

        public static int LocalSize = Globals.Size / 4;

        public static Stopwatch St = new Stopwatch();

        //create/initialize particle array
        public static void Constructor()
        {
            ParticlesA = new Particle[LocalSize];
            ParticlesB = new Particle[LocalSize];
            ParticlesC = new Particle[LocalSize];
            ParticlesD = new Particle[LocalSize];
            Reset(ParticlesA);
            Reset(ParticlesB);
            Reset(ParticlesC);
            Reset(ParticlesD); 
        }

        //reset particle array to known starting state
        public static void Reset(Particle[] P)
        {
            for (int i = 0; i < LocalSize; i++)
            {
                P[i].X = 0;
                P[i].Y = 0;
                P[i].State = 0;
                P[i].Id = 0;
            }
        }

        //test system, write log
        public static void RunTest()
        {
            StringBuilder sb = new StringBuilder(4096);
            St.Start();

            //kick off tasks, wait for them to complete
            Task Task1 = Task.Factory.StartNew(() => UpdateA(St));
            Task Task2 = Task.Factory.StartNew(() => UpdateB(St));
            Task Task3 = Task.Factory.StartNew(() => UpdateC(St));
            Task Task4 = Task.Factory.StartNew(() => UpdateD(St));
            Task.WaitAll(Task1, Task2, Task3, Task4);

            St.Stop();
            sb.Append(sbA.ToString());
            sb.Append(sbB.ToString());
            sb.Append(sbC.ToString());
            sb.Append(sbD.ToString());

            sb.Append("\n\ncomplete, elapsed: "
                + St.ElapsedMilliseconds + " ms.\n\n");

            //write output log
            string Dir = Path.Combine(Globals.GetDir(), "Test_8_4Tasks.txt");
            Debug.WriteLine("writing output to: " + Dir);
            using (StreamWriter w = new StreamWriter(Dir))
            { w.WriteLine(sb.ToString()); }
        }

        public static void UpdateA(Stopwatch St)
        {
            for (int g = 1; g < Globals.Iterations; g++)
            {
                for (int i = 0; i < LocalSize; i++)
                {   //update
                    ParticlesA[i].X++;
                    ParticlesA[i].Y++;
                    ParticlesA[i].State++;
                    ParticlesA[i].Id++;
                }
                sbA.Append("\nPos: " + ParticlesA[0].X);
                sbA.Append(" frame " + g + " elapsed: "
                    + St.ElapsedTicks + " ticks.");
            }
        }

        public static void UpdateB(Stopwatch St)
        {
            for (int g = 1; g < Globals.Iterations; g++)
            {
                for (int i = 0; i < LocalSize; i++)
                {   //update
                    ParticlesB[i].X++;
                    ParticlesB[i].Y++;
                    ParticlesB[i].State++;
                    ParticlesB[i].Id++;
                }
                sbB.Append("\nPos: " + ParticlesB[0].X);
                sbB.Append(" frame " + g + " elapsed: "
                    + St.ElapsedTicks + " ticks.");
            }
        }

        public static void UpdateC(Stopwatch St)
        {
            for (int g = 1; g < Globals.Iterations; g++)
            {
                for (int i = 0; i < LocalSize; i++)
                {   //update
                    ParticlesC[i].X++;
                    ParticlesC[i].Y++;
                    ParticlesC[i].State++;
                    ParticlesC[i].Id++;
                }
                sbC.Append("\nPos: " + ParticlesC[0].X);
                sbC.Append(" frame " + g + " elapsed: "
                    + St.ElapsedTicks + " ticks.");
            }
        }

        public static void UpdateD(Stopwatch St)
        {
            for (int g = 1; g < Globals.Iterations; g++)
            {
                for (int i = 0; i < LocalSize; i++)
                {   //update
                    ParticlesC[i].X++;
                    ParticlesC[i].Y++;
                    ParticlesC[i].State++;
                    ParticlesC[i].Id++;
                }
                sbD.Append("\nPos: " + ParticlesC[0].X);
                sbD.Append(" frame " + g + " elapsed: "
                    + St.ElapsedTicks + " ticks.");
            }
        }

    }
}