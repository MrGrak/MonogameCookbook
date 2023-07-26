using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace StaticThreading
{ 
    public static class Globals
    {
        public static int Size = 4096; //try: 512, 1024, 2048, 4096, 8192
        public static int Iterations = 60 * 10;

        public static string GetDir()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName;
        }
    }
}