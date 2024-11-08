using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Diagnostics;
using System.IO;

namespace n_puzzle
{
    class Program
    {

        public enum Heuristic{
            Manhattan,
            Misplaced,
            ManhattanLC

        }

        public  static Heuristic heuristicUsed;
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No input file or file is not valid");
                Environment.Exit(0);    

            }
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            heuristicUsed = Heuristic.ManhattanLC;
            string text = File.ReadAllText(args[0]);
            Node grid = Tools.FillGrid(text);
            if (grid != null)
            {
                Search.Solve(grid);
                stopwatch.Stop();
                Console.WriteLine($"Elapsed time {stopwatch.ElapsedMilliseconds} ms");
            }

        }
    }
}