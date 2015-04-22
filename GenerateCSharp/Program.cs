using Microsoft.Fast;
using System;

namespace GenerateCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = args[0];
            var output = args[1];

            Console.WriteLine("CS: {0} -> {1}", input, output);

            var prog = Parser.ParseFromFile(input);
            CsharpGenerator.GenerateCode(prog, output);            
        }
    }
}
