using System;
using ColaCompiler;
namespace tests
{
    class Program
    {
        private static Parse parse = new Parse();
        static void Main(string[] args)
        {
            Console.WriteLine(parse.ParsePrint("print(\"hello, world\")"));
            Console.ReadLine();
        }
    }
}
