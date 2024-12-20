using System.Text.RegularExpressions;
using System.Drawing;

namespace n_puzzle
{
    class Tools
    {
        public static int n;
        public static Point[] goalState;
        private static List<List<int>> conflictGraph;
        public static Node FillGrid(string text)
        {
            text += "\n";
            text = RemoveBetween(text, "#", "\n");
            string[] rows = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            n = Int32.Parse(rows[0]);
            Console.WriteLine("Size: " + n);
            if (rows.Length > n + 1|| rows.Length < n + 1 )
            {
                Console.WriteLine("File is not valid");
                foreach (var item in rows)
                {
                    Console.WriteLine(item);
                }
                Environment.Exit(0);    
            }
            goalState = GenerateGoal();
            int [] solution = GenerateSolution();

            int[,] grid = new int[n, n];
            string[] tmp;

            Point pos = new Point();
            for (int i = 0; i < n; i++)
            {
                tmp = rows[i + 1].Split(new char[]{' ', '\t', '\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length != n)
                {
                    Console.WriteLine("File is not valid");
                    foreach (var item in rows)
                      Console.WriteLine(item);
                    Environment.Exit(0);    
                }
                for (int j = 0; j < n; j++)
                {
                    grid[i, j] = int.Parse(tmp[j]);
                    if(grid[i, j] > n * n - 1){

                    Console.WriteLine($"File is not valid (Numbers must be less than {n*n -1})");
                    foreach (var item in rows)
                      Console.WriteLine(item);
                    Environment.Exit(0);    
                    }
                    if (grid[i,j] == 0)
                        pos = new Point { X = i, Y = j };
                }
            }
            for (int i = 0; i < n; i++){
                for (int j = 0; j < n; j++){
                    for(int p = 0; p < n; p++){
                        for(int q = 0; q < n; q++){
                            if ((i != p || j != q) && grid[i,j] == grid[p, q]) {
                                Console.WriteLine($"File is not valid (no duplicates pls!)");
                                foreach (var item in rows)
                                    Console.WriteLine(item);
                                Environment.Exit(0);
                            }
                        }
                    }
                }
            }
            Node initialNode = new Node(pos, grid);
            if (!IsSolvable(initialNode, solution))
            {
                DisplayGrid(grid);
                Console.WriteLine("Puzzle is unsolvable");
                return null;
            }
            return initialNode;

        }


        private static int InvertionCount(int[] tab)
        {
            int y ;
            int x ;
            int inversions = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                x = tab[i];
                for (int j = i + 1; j < tab.Length; j++)
                {
                    y = tab[j];

                    if ( (x != 0 && y != 0) &&  x > y)
                    {
                        inversions++;
                    }
                }
            }
            return inversions;
        }

        private static bool IsSolvable(Node start, int[] solution)
        {
            int t = 0;
            int[] puzzle = new int[n * n ];
            int zeroPos = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (start.grid[i, j] == 0)
                    {
                        zeroPos = i + 1;
                        puzzle[t] = 0;
                        t++;
                        continue;
                    }

                    puzzle[t] = solution[start.grid[i, j] - 1];
                    t++;
                }
            }
            
            var puzzleInvertions = InvertionCount(puzzle);
            if (n % 2 == 0)
            {
                return zeroPos  % 2 != puzzleInvertions % 2;
            }
            return  puzzleInvertions % 2 == 0;
        }

        private static int[] GenerateSolution()
        {
            int[] tmp = new int[n * n];
            int t = 0;
            int posInGoal = 0;
            for (int i = 0; i < n ; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    tmp[t] = GetPoint(new Point { X = i, Y = j });
                    if (tmp[t] == 0)
                    {
                        posInGoal = t;
                    }
                    t++;
                }
            }
            return MoveBlankSpace(tmp, posInGoal);
        }

        private static int[] MoveBlankSpace(int[] tmp, int pos)
        {
            int blankSpace = pos % n;
            int distance = Math.Abs(n - blankSpace) - 1;
            blankSpace = pos + distance;
            int nb = tmp[blankSpace];
            tmp[blankSpace] = 0;
            tmp[pos] = nb;
        
          
         
            tmp[blankSpace] = tmp[tmp.Length -1];
            tmp[tmp.Length - 1]  = 0;
            
           
            return tmp;
        }


        private static int GetPoint(Point p)
        {

            int i = 0;
            foreach (Point point in goalState)
            {
                if (p == point)
                {
                    return i;
                }
                i++;
            }
            return 0;
        }

        public static int GetHValue(int[,] grid)
        {
            int h = 0;
            Point p;
            Point p2;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (grid[i,j] == 0)
                    {
                        continue;
                    }
                    p = new Point { X = i, Y = j }; // pos of current number
                    p2 = goalState[grid[i, j]]; // pos of current number in the goal state
                    if (p != p2)
                    {
                        h += Math.Abs(p2.X - p.X) + Math.Abs(p2.Y - p.Y);
                    }
                }
            }

            return h;
        }

        public static int GetHValueLC(int[,] grid, out int lc)
        {
            int h = 0;
            Point p;
            Point p2;
            conflictGraph = new List<List<int>>();
           
            lc = 0;
            for (int i = 0; i < n*n; i++)
            {
                conflictGraph.Add(new List<int>());
            }
            for (int i = 0; i < n; i++)
            {
                 
                lc += GetHorizontalConflicts(i, grid);
                for (int j = 0; j < n; j++)
                {
                    if (i == 0)
                    {
                        lc += GetVerticalConflicts(j, grid);
                    }

                    if (grid[i, j] == 0)
                    {
                        continue;
                    }

                    p = new Point {X = i, Y = j}; // pos of current number
                    p2 = goalState[grid[i, j]]; // pos of current number in the goal state
                    if (p != p2)
                    {
                        
                        h += Math.Abs(p2.X - p.X) + Math.Abs(p2.Y - p.Y);
                    }
                }
            }
            
            return h + lc;
        }

        public static int GetVerticalConflicts(int col, int[,] grid)
        {
            int lc = 0;
            for (int i = 0; i < n; i++)
            {
                int nb = grid[i, col];
                if (nb == 0)
                    continue;
                Point p1 = goalState[nb];
                for (int j = i + 1; j < n; j++)
                {
                    int nb2 = grid[j, col];
                    if (nb2 == 0)
                        continue;
                    Point p2 = goalState[nb2];
                    if (p2.X <= p1.X)
                    {
                        conflictGraph[nb].Add(nb2);
                        conflictGraph[nb2].Add(nb);
                    }
                }
            }

            while (conflictGraph.SelectMany(x => x).Any())
            {
                int max = conflictGraph.Max(x => x.Count);
                int index = conflictGraph.FindIndex(x => x.Count == max);
                foreach (int neighbour  in conflictGraph[index])
                {
                    conflictGraph[neighbour].Remove(index);
                    lc++;
                }
                conflictGraph[index].Clear();
            }
            return lc * 2;
        }

        public static int GetHorizontalConflicts(int row, int[,] grid)
        {
            int lc = 0;
            for (int i = 0; i < n; i++)
            {
                int nb = grid[row, i];
                if (nb == 0)
                    continue;
                Point p1 = goalState[nb];
                for (int j = i + 1; j < n; j++)
                {
                    int nb2 = grid[row, j];
                    if (nb2 == 0)
                        continue;
                    Point p2 = goalState[nb2];
                    if (p2.Y <= p1.Y)
                    {
                        conflictGraph[nb].Add(nb2);
                        conflictGraph[nb2].Add(nb);
                    }
                }
            }

            while (conflictGraph.SelectMany(x => x).Any())
            {
                int max = conflictGraph.Max(x => x.Count);
                int index = conflictGraph.FindIndex(x => x.Count == max);
                foreach (int neighbour  in conflictGraph[index])
                {
                    conflictGraph[neighbour].Remove(index);
                    lc++;
                }
                conflictGraph[index].Clear();
            }
            return lc * 2;
        }

        private static Point[] GenerateGoal()
        {
            int tilesNb = n * n;
            int[,] tmpGrid = new int[n,n];
            int x = 0, y = 0, direction = 0;
            Point[] p = new Point[tilesNb];
            p[0] =  n % 2 == 0 ?  new Point { X = n  /2, Y = (n - 1) / 2 } : new Point { X = n  / 2, Y = n  / 2 };
            for (int i = 1; i < tilesNb; i++)
            {
                //direction = 0-right, 1-down, 2-left, 3-up
                tmpGrid[x, y] = i;
                p[i] = new Point { X = y, Y = x };
                if (x == n - 1 || tmpGrid[x + 1, y] != 0)
                {
                    direction = 1;
                   

                    if (y == n - 1 || tmpGrid[x, y + 1] != 0)
                    {
                        direction = 2;
                    }
                }
                if (direction == 2  && x != 0 && tmpGrid[x - 1, y ] != 0)
                {
                    direction = 3;
                }
                if (x == 0  && y == n -1)
                {
                    direction = 3;   
                }
                if (direction == 3 && tmpGrid[x , y - 1] != 0)
                {
                    direction = 0;
                }
                switch (direction)
                {
                    case 0:
                        x++;
                        break;
                    case 1:
                        y++;
                        break;
                    case 2:
                        x--;
                        break;
                    case 3:
                        y--;
                        break;
                }
            }
            return p;
        }
        
        public static void DisplayGrid(int[,] grid)
        {

            for (int i = 0; i < n ; i++)
            {
                for (int j = 0; j < n ; j++)
                {
                    Console.Write(grid[i, j] + "  ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        private static string RemoveBetween(string sourceString, string startTag, string endTag)
        {
            var regex = new Regex($"{Regex.Escape(startTag)}(.*?){Regex.Escape(endTag)}", RegexOptions.RightToLeft);
            return regex.Replace(sourceString, endTag);
        }

        public static int GetHValueMisplaced(int[,] grid)
        {
            int misplacedCount = 0;
            int n = grid.GetLength(0);
            Point p;
            Point p2;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    p = new Point { X = i, Y = j };
                    p2 = goalState[grid[i, j]];
                    if (p != p2)
                    {
                        misplacedCount++;
                    }
                }
            }
            return misplacedCount;
        }
    }
}
