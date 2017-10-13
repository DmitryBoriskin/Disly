using System;
using Integration.Frmp.library;

namespace Integration.Frmp.cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Manual Integration");
            Console.ReadKey();

            Export.DoExport();

            Console.WriteLine("Manual Integration complete. Press any key to exit...");
        }
    }
}
