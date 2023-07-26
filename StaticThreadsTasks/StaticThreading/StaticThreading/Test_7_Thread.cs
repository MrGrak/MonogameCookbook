using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StaticThreading
{
    public class ParticleSystem
    {
        public Particle[] Particles;
        public StringBuilder sb = new StringBuilder(4096);

        public int LocalSize = Globals.Size / 4;

        public ParticleSystem()
        {
            Particles = new Particle[LocalSize];
            Reset(Particles);
        }

        public void Reset(Particle[] P)
        {
            for (int i = 0; i < LocalSize; i++)
            {
                P[i].X = 0;
                P[i].Y = 0;
                P[i].State = 0;
                P[i].Id = 0;
            }
        }

        public void Update(Stopwatch St)
        {
            for (int g = 1; g < Globals.Iterations; g++)
            {
                for (int i = 0; i < LocalSize; i++)
                {   //update
                    Particles[i].X++;
                    Particles[i].Y++;
                    Particles[i].State++;
                    Particles[i].Id++;
                }
                sb.Append("\nPos: " + Particles[0].X);
                sb.Append(" frame " + g + " elapsed: "
                    + St.ElapsedTicks + " ticks.");
            }
        }

    }

    public static class Test_7_Thread
    {
        //note: we expect dividing the work into 4 instead of 3 would
        //be faster, but it's not - due to how we restructured data

        //split particle system aos into 4 groups
        public static ParticleSystem PS1;
        public static ParticleSystem PS2;
        public static ParticleSystem PS3;
        public static ParticleSystem PS4;
        
        public static int LocalSize = Globals.Size / 4;

        public static Stopwatch St = new Stopwatch();

        //create/initialize particle array
        public static void Constructor()
        {
            PS1 = new ParticleSystem();
            PS2 = new ParticleSystem();
            PS3 = new ParticleSystem();
            PS4 = new ParticleSystem();
        }
        
        //test system, write log
        public static void RunTest()
        {
            StringBuilder sb = new StringBuilder(4096);
            St.Start();

            //kick off tasks, wait for them to complete
            Task Task1 = Task.Factory.StartNew(() => PS1.Update(St));
            Task Task2 = Task.Factory.StartNew(() => PS2.Update(St));
            Task Task3 = Task.Factory.StartNew(() => PS3.Update(St));
            Task Task4 = Task.Factory.StartNew(() => PS4.Update(St));
            Task.WaitAll(Task1, Task2, Task3, Task4);

            St.Stop();
            sb.Append(PS1.sb.ToString());
            sb.Append(PS2.sb.ToString());
            sb.Append(PS3.sb.ToString());
            sb.Append(PS4.sb.ToString());

            sb.Append("\n\ncomplete, elapsed: "
                + St.ElapsedMilliseconds + " ms.\n\n");

            //write output log
            string Dir = Path.Combine(Globals.GetDir(), "Test_7_4Tasks.txt");
            Debug.WriteLine("writing output to: " + Dir);
            using (StreamWriter w = new StreamWriter(Dir))
            { w.WriteLine(sb.ToString()); }
        }
        
    }
}