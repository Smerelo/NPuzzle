using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;

namespace n_puzzle
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No input file or file is not valid");
                Environment.Exit(0);    

            }
            string text = File.ReadAllText(args[0]);
            Node grid = Tools.FillGrid(text);
            if (grid != null)
            {
                //Search.Solve(grid);
                Search.SolveIDA(grid);
            }

        }
    }
}