    using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace n_puzzle
{
    class Tools
    {
        public static int n;
        public static Point[] goalState;
        public static Node FillGrid(string text)
        {
            text = RemoveBetween(text, "#", "$");
            string[] rows = text.Split("$", StringSplitOptions.RemoveEmptyEntries);
            n = Int32.Parse(rows[0]);
      
            Console.WriteLine($"Size: {n}");
            goalState = GenerateGoal();
            int [] solution = GenerateSolution();
            int[,] grid = new int[n, n];
            string[] tmp;

            Point pos = new Point();
            for (int i = 0; i < n; i++)
            {
                tmp = rows[i + 1].Split(new char[]{' ', '\t', '\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < n; j++)
                {
                    grid[i, j] = int.Parse(tmp[j]);
                    if (grid[i,j] == 0)
                        pos = new Point { X = i, Y = j };
                }
            }
            //DisplayGrid(grid);
            Node initialNode = new Node(pos, grid);
            if (!IsSolvable(initialNode, solution))
            {
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
            while (tmp[tmp.Length - 1] != 0)
            {
                tmp[blankSpace] = tmp[blankSpace + n];
                tmp[blankSpace + n] = 0;
            }
            
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

        public static int GetHValue(int[,] nodeState)
        {
            int h = 0;
            Point p;
            Point p2;
            for (int i = 0; i < Tools.n; i++)
            {
                for (int j = 0; j < Tools.n; j++)
                {
                    if (nodeState[i,j] == 0)
                    {
                        continue;
                    }
                    p = new Point { X = i, Y = j }; // pos of current number
                    p2 = goalState[nodeState[i, j]]; // pos of current number in the goal state
                    if (p != p2)
                    {
                        h += Math.Abs(p2.X - p.X) + Math.Abs(p2.Y - p.Y);
                    }
                }
            }

            return h;
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


        public  static bool CompareGrids(int[,] g1, int[,] g2)
       {

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (g1[i,j] != g2[i,j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        public static void DisplayGrid(int[,] grid)
        {

            Console.WriteLine("");
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
            return regex.Replace(sourceString, startTag + endTag).Trim('#');
        }
    }
}
