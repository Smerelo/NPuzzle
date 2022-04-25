using System;
using System.Collections.Generic;
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
        private static Dictionary<int, Node>buckets;
        public static void Solve(Node initialState)
        {
            buckets = new Dictionary<int, Node>();
            possibleMoves = new List<int>();
            nextNodes = new List<Node>{new Node(), new Node(), new Node(), new Node()};
            PriorityQueue<int, int> openList = new PriorityQueue<int, int>();
          
            buckets.Add(initialState.key, initialState);
            openList.Enqueue(initialState.key, initialState.f);
            while (openList.Count != 0)
            {
                int key = openList.Dequeue();
                if (!buckets.TryGetValue(key, out Node currentNode))
                {
                    return;
                }

                if (currentNode.h == 0)
                {
                    Console.WriteLine("Goal Reached");
                    Console.WriteLine(currentNode.g) ;
                    ShowFullPath(currentNode);
                    Tools.DisplayGrid(initialState.grid);
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
                    if (buckets.TryGetValue(key, out tmp) && tmp.IsClosed)
                    {
                        continue;
                    }
                    if (!buckets.ContainsKey(key))
                    {
                        buckets.Add(key, node);

                        openList.Enqueue(key, node.f);
                    }
                    else
                    {
                        if (buckets.TryGetValue(key, out  tmp) && node.g < tmp.g)
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
            Node tmp;
            Tools.DisplayGrid(currentNode.grid);
            while (currentNode.parent.g > 0)
            {
                Tools.DisplayGrid(currentNode.grid);
                currentNode = currentNode.parent;
            }
            Tools.DisplayGrid(currentNode.grid);
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

        private static  void ShowPath(Node start)
        {
            Tools.DisplayGrid(start.grid);
            for (int i = 1; i < solutionMoves.Count; i++)
            {
                start.grid = start.MovePos(solutionMoves[i], start.grid, start.pos);
                Tools.DisplayGrid(start.grid);
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

        private static int CompareNodes(Node node, List<Node> nodeList)
        {
            int i = 0; 
            foreach (Node node1 in nodeList)
            {

                if (Tools.CompareGrids(node.grid, node1.grid))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        private static Node GetMinfNode(List<Node> openList)
        {
            int f = 2000;
            Node tNode = null;
            foreach (Node node in openList)
            {
                if ( node.f < f)
                {
                    f = node.f;
                    tNode = node;
                }
            }
            return tNode;
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
