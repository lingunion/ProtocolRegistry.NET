using System;
using System.IO;
using System.Linq;

namespace RegisterWindowTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string result = String.Join(",", args);
            Uri uri = new Uri(args[0]);
            Console.WriteLine(uri);
            File.WriteAllText(Path.GetFullPath(Path.Join($"{uri.AbsolutePath}", $"{uri.Scheme}.txt")), "OK");
        }
    }
}
