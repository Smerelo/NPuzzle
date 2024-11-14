using System.Diagnostics;

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
            if (args.Length != 2)
            {
                Console.WriteLine("usage: ./npuzzle [filename] [heuristic]");
                Environment.Exit(0);    

            }
            switch (args[1])
            {
                case "MH":
                    heuristicUsed = Heuristic.Manhattan;
                break;
                case "MLC":
                    heuristicUsed = Heuristic.ManhattanLC;
                break;
                case "MI":
                    heuristicUsed = Heuristic.Misplaced;
                break;
                default:
                    Console.WriteLine("Heuristic is not valid, options are: MH, MLC, MI");
                    Environment.Exit(0);    
                break;
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string text = null;
            Node grid = null;
            try {
                text = File.ReadAllText(args[0]);
                grid = Tools.FillGrid(text);
            } catch (Exception e) {

                Console.WriteLine($"Failed to read and parse file {args[0]}: {e.Message}");
                Environment.Exit(0);
            }
            if (grid != null)
            {
                Search.Solve(grid);
                stopwatch.Stop();
                Console.WriteLine($"Elapsed time {stopwatch.ElapsedMilliseconds} ms");
            }

        }
    }
}