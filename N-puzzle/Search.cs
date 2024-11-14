using System.Diagnostics;
using System.Drawing;

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
                    ShowFullPath(currentNode);                    
                    Console.WriteLine("Goal Reached");
                    Console.WriteLine("Total number of states in open set: " + openList.Count);
                    Console.WriteLine("Total number of states in memory: " + dictionary.Count);
                    Console.WriteLine("Number of steps to reach goal: "  + currentNode.g) ;
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
