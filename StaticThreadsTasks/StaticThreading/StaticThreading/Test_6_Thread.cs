using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StaticThreading
{
    public static class Test_6_Thread
    {
        //notice the duplication of fields and methods happening
        //we could roll these into their own class instance, but for
        //this test we'll just write it all out, hardcoded
        //this works really well for single core hyperthreaded, or dual
        //core single threaded cpus

        public static Particle[] ParticlesA;
        public static Particle[] ParticlesB;
        public static Particle[] ParticlesC;

        public static StringBuilder sbA = new StringBuilder(4096);
        public static StringBuilder sbB = new StringBuilder(4096);
        public static StringBuilder sbC = new StringBuilder(4096);

        public static int LocalSize = Globals.Size / 3;

        public static Stopwatch St = new Stopwatch();

        //create/initialize particle array
        public static void Constructor()
        {
            ParticlesA = new Particle[Globals.Size / 3];
            ParticlesB = new Particle[Globals.Size / 3];
            ParticlesC = new Particle[Globals.Size / 3];
            Reset(ParticlesA);
            Reset(ParticlesB);
            Reset(ParticlesC);
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
            Task.WaitAll(Task1, Task2, Task3);

            St.Stop();
            sb.Append(sbA.ToString());
            sb.Append(sbB.ToString());
            sb.Append(sbC.ToString());

            sb.Append("\n\ncomplete, elapsed: "
                + St.ElapsedMilliseconds + " ms.\n\n");

            //write output log
            string Dir = Path.Combine(Globals.GetDir(), "Test_6_3Tasks.txt");
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

    }
}