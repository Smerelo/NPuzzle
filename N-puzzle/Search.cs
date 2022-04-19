using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace n_puzzle
{
    class Search
    {
        private static List<Node> nextNodes;
        private  static List<int> possibleMoves;
        private static List<int> solutionMoves;

        public static void Solve(Node initialState)
        {
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();
            List<Node> currentNodes = new List<Node>();

            openList.Add(initialState);
            int i = -1;
            while (openList.Count != 0)
            {
                Node currentNode = GetMinfNode(openList);
                //Tools.DisplayGrid(currentNode.grid);
                if (currentNode.h == 0)
                {
                    Console.WriteLine("Goal Reached");
                    Console.WriteLine(currentNode.g) ;
                    Tools.DisplayGrid(currentNode.grid);
                    break;
                }
                closedList.Add(currentNode);
                openList.Remove(currentNode);
                GetNextNodes(currentNode);
                foreach (Node node in  nextNodes)
                {
                    if (CompareNodes(node, closedList) != -1)
                    {
                        /*Console.WriteLine($"In ClosedList h: {node.h} g: {node.g}");
                        Tools.DisplayGrid(node.grid);*/
                        continue;
                    }
                    if ((i = CompareNodes(node,openList)) == -1)
                    {
                        /*Console.WriteLine($"Not in openList h: {node.h} g: {node.g}");
                        Tools.DisplayGrid(node.grid);*/

                        openList.Add(node);
                    }
                    else
                    {
                       /* Console.WriteLine($"In openList h: {node.h} g: {node.g}");
                        Tools.DisplayGrid(node.grid);*/
                        if (node.g < openList[i].g)
                        {
                            openList[i].g = node.g;
                            openList[i].f = node.f;
                            openList[i].parent = node.parent;
                        }
                    }
                }
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
                nextNodes[i].CopyNode(currentState, possibleMoves[i]);
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
