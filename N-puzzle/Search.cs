using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Numerics;

namespace n_puzzle
{
    class Search
    {
        private static List<Node> nextNodes;
        private  static List<int> possibleMoves;
        private static List<int> solutionMoves;
        private static Dictionary<int, Node>dictionary;
        
        public static void Solve(Node initialState)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            dictionary = new Dictionary<int, Node>();
            possibleMoves = new List<int>();
            nextNodes = new List<Node>{new Node(), new Node(), new Node(), new Node()};
            PriorityQueue<int, int> openList = new PriorityQueue<int, int>();
          
            dictionary.Add(initialState.key, initialState);
            openList.Enqueue(initialState.key, initialState.f);
            while (openList.Count != 0)
            {
                int key = openList.Dequeue();
                if (!dictionary.TryGetValue(key, out Node currentNode))
                {
                    return;
                }

                if (currentNode.h == 0)
                {
                    stopwatch.Stop();
                    Console.WriteLine("Goal Reached");
                    Console.WriteLine(currentNode.g) ;
                    Console.WriteLine($"Elapsed time {stopwatch.ElapsedMilliseconds} ms");
                    Console.WriteLine("0");
                    Tools.DisplayGrid(initialState.grid);
                    
                    ShowFullPath(currentNode);
                    break;
                }
                GetNextNodes(currentNode);
                currentNode.IsClosed = true;
                foreach (Node node in  nextNodes)
                {
                    key = node.key;
                    Node tmp;
                    if (node.grid == null)
                    {
                        continue;
                    }
                    if (dictionary.TryGetValue(key, out tmp) && tmp.IsClosed)
                    {
                        continue;
                    }
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, node);
                        openList.Enqueue(key, node.f);
                    }
                    else
                    {
                        if (dictionary.TryGetValue(key, out  tmp) && node.g < tmp.g)
                        {
                            tmp.g = node.g;
                            tmp.f = node.f;
                            tmp.parent = node.parent;
                            openList.Enqueue(key, tmp.f);
                        }
                    }
                }
            }

        }

        private static void ShowFullPath(Node currentNode)
        {
            List<Node> path = new List<Node>();
            Node tmp = currentNode; 
            path.Add(tmp);
            while (currentNode.parent.g >= 0)
            {
                path.Add(currentNode);
                if(currentNode.parent.g == 0)
                    break;
                currentNode = currentNode.parent;
            }

            int j = 1;
            for (int i = path.Count - 1; i > 0; i--)
            {
                Console.WriteLine(j);
                j++;
                Tools.DisplayGrid(path[i].grid);
            }

        }


        public static int SolveIDA(Node start)
        {
            Node s = new Node();
            s.CloneNode(start);
            int threshold = start.h;
            int tmp;
            nextNodes = new List<Node>{new Node(), new Node(), new Node(), new Node()};
            possibleMoves = new List<int>();
            solutionMoves = new List<int>();
            while (true)
            {
                solutionMoves.Clear();
                tmp = Find(s, 0, threshold, -1);
                if (tmp == -1)
                {
                    ShowPath(start);
                    return 1;
                }
                threshold = tmp;
            }
        }

        

        private static int Find(Node node, int g, int threshold, int move)
        {
            int f = g + node.h;
            if (solutionMoves.Count < g + 1 )
            {
                solutionMoves.Add(move);
            }
            else
            {
                solutionMoves[g] = move;
            }
            if (f> threshold)
            {
                return f;
            }
            if (node.h == 0)
            {
                return -1; 
            }
            int min = int.MaxValue;
            GetNextNodes(node);
            
            Node tempNode = new Node(node.pos, node.grid);

            for (int i = 0; i < possibleMoves.Count; i++)
            {
                tempNode.CloneNode(nextNodes[i]);
                int tmp = Find(tempNode, g + 1, threshold, possibleMoves[i]);
                if (tmp == -1)
                {
                    return -1;
                }
                if (tmp < min)
                {
                    min = tmp;
                }
            }
            return min;
        }

        private static  void ShowPath(Node start)
        {
            Tools.DisplayGrid(start.grid);
            for (int i = 1; i < solutionMoves.Count; i++)
            {
                start.grid = start.MovePos(solutionMoves[i], start.grid, start.pos);
                Tools.DisplayGrid(start.grid);
            }
        }
        
        private static void GetNextNodes(Node currentState)
        {
            GetPossibleMoves(currentState.pos);
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                Node tmp = new Node();
                tmp.CopyNode(currentState, possibleMoves[i]);
                nextNodes[i] = tmp;
            }
        }

        private static void GetPossibleMoves(Point pos)
        {
            possibleMoves.Clear();
            if (pos.Y != Tools.n - 1)
            {
                possibleMoves.Add(0);

            }
            if (pos.X != Tools.n -1)
            {
                possibleMoves.Add(1);
            }  
            if (pos.Y != 0)
            {
                possibleMoves.Add(2);

            }
            if (pos.X != 0)
            {
                possibleMoves.Add(3);
            }
        }

      
    }
}
