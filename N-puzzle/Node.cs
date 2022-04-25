using System.Drawing;

namespace n_puzzle
{

    class Node
    {
        
        public int[,] grid;
        public int h;
        public int f;
        public int g;
        public Node parent;
        public Point pos;
        public int key;
        public bool IsClosed { get; set;}
        
        public Node()
        {
            
        }
        public Node(Point _pos, int[,] _grid)
        {
            pos = _pos;
            grid = _grid;
            h = Tools.GetHValue(grid);
            g = 0;
            f = h;
        }
        private void CopyGrid(int[,] source)
        {
            key = 1430287;
            if (grid == null)
            {
                 grid = new int[Tools.n, Tools.n];
            }
            for (int i = 0; i < Tools.n; i++)
            {
                for (int j = 0; j < Tools.n; j++)
                {
                    grid[i, j] = source[i,j];
                    key = key * 7302013 ^ source[i, j].GetHashCode();
                }
            }
        }

        public int[,] MovePos(int dir, int[,] currenGrid, Point currentPos)
        {
            int t = 0;
            switch (dir)
            {
                case 0:
                    t = currenGrid[currentPos.X, currentPos.Y + 1];
                    currenGrid[currentPos.X, currentPos.Y + 1] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.Y += 1;
                    break;
                case 1:
                    t = currenGrid[currentPos.X + 1, currentPos.Y];
                    currenGrid[currentPos.X + 1, currentPos.Y] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.X += 1;

                    break;
                case 2:
                    t = currenGrid[currentPos.X, currentPos.Y - 1];
                    currenGrid[currentPos.X, currentPos.Y -1] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.Y -= 1;

                    break;
                case 3:
                    t = currenGrid[currentPos.X -1, currentPos.Y];
                    currenGrid[currentPos.X -1, currentPos.Y] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.X -= 1;

                    break;
            }
            return currenGrid;
        }

        public void CopyNode(Node currentState, int moveDir)
        {
            pos = currentState.pos;
            parent = currentState;
            CopyGrid(parent.grid);
            grid = MovePos(moveDir, grid, pos);
            SetHashCode();
            h = Tools.GetHValue(grid);
            g = currentState.g + 1;
            f = g + h;
            IsClosed = false;
        }

        private void SetHashCode()
        {
            key = 1430287;

            for (int i = 0; i < Tools.n; i++)
            {
                for (int j = 0; j < Tools.n; j++)
                {
                    key = key * 7302013 ^ grid[i, j].GetHashCode();

                }
            }
        }

        public Node CloneNode(Node node)
        {
            pos = node.pos;
            parent = node.parent;
            CopyGrid(node.grid);
            h = node.h;
            g = node.g;
            f = node.f;
            IsClosed = false;
            return this;
        }
    }
}
