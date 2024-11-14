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
        public int lc;
        public bool IsClosed;
        
        public Node()
        {
            
        }

        public Node(Point _pos, int[,] _grid)
        {
            pos = _pos;
            grid = _grid;
            switch (Program.heuristicUsed)
            {
                case Program.Heuristic.Manhattan:
                    h = Tools.GetHValue(grid);
                    break;
                case Program.Heuristic.Misplaced:
                    h = Tools.GetHValueMisplaced(grid);
                    break;
                case Program.Heuristic.ManhattanLC:
                    h = Tools.GetHValueLC(grid, out lc);
                    break;
            }
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
            int h1 = 0;
            Point posInGoal = new Point();
            int preMoveLc_1 = 0;
            int preMoveLc_2 = 0;
            int posMoveLc_1 = 0;
            int posMoveLc_2 = 0;
            switch (dir)
            {
                case 0:
                    
                    preMoveLc_1 = Tools.GetVerticalConflicts(currentPos.Y, currenGrid);
                    preMoveLc_2 = Tools.GetVerticalConflicts(currentPos.Y + 1, currenGrid);
                    posInGoal = Tools.goalState[currenGrid[currentPos.X, currentPos.Y + 1]];
                    h1 = Math.Abs(posInGoal.X - currentPos.X) + Math.Abs(posInGoal.Y - (currentPos.Y + 1));
                    t = currenGrid[currentPos.X, currentPos.Y + 1];
                    currenGrid[currentPos.X, currentPos.Y + 1] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.Y += 1;
                    posMoveLc_1 = Tools.GetVerticalConflicts(currentPos.Y, currenGrid);
                    posMoveLc_2 = Tools.GetVerticalConflicts(currentPos.Y + 1, currenGrid);
                    break;
                case 1:
                    preMoveLc_1 = Tools.GetHorizontalConflicts(currentPos.X, currenGrid);
                    preMoveLc_2 = Tools.GetHorizontalConflicts(currentPos.X + 1, currenGrid);
                    posInGoal = Tools.goalState[currenGrid[currentPos.X + 1, currentPos.Y]];
                    h1 = Math.Abs(posInGoal.X - (currentPos.X + 1)) + Math.Abs(posInGoal.Y - currentPos.Y );
                    t = currenGrid[currentPos.X + 1, currentPos.Y];
                    currenGrid[currentPos.X + 1, currentPos.Y] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.X += 1;
                    posMoveLc_1 = Tools.GetHorizontalConflicts(currentPos.X, currenGrid);
                    posMoveLc_2 = Tools.GetHorizontalConflicts(currentPos.X + 1, currenGrid);
                    break;
                case 2:
                    preMoveLc_1 = Tools.GetVerticalConflicts(currentPos.Y, currenGrid);
                    preMoveLc_2 = Tools.GetVerticalConflicts(currentPos.Y - 1, currenGrid);
                    posInGoal = Tools.goalState[currenGrid[currentPos.X, currentPos.Y - 1]];
                    h1 = Math.Abs(posInGoal.X - currentPos.X) + Math.Abs(posInGoal.Y - (currentPos.Y -1));
                    t = currenGrid[currentPos.X, currentPos.Y - 1];
                    currenGrid[currentPos.X, currentPos.Y -1] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.Y -= 1;
                    posMoveLc_1 = Tools.GetVerticalConflicts(currentPos.Y, currenGrid);
                    posMoveLc_2 = Tools.GetVerticalConflicts(currentPos.Y - 1, currenGrid);
                    break;
                case 3:
                    preMoveLc_1 = Tools.GetHorizontalConflicts(currentPos.X, currenGrid);
                    preMoveLc_2 = Tools.GetHorizontalConflicts(currentPos.X - 1, currenGrid);
                    posInGoal = Tools.goalState[currenGrid[currentPos.X - 1, currentPos.Y]];
                    h1 = Math.Abs(posInGoal.X - (currentPos.X - 1)) + Math.Abs(posInGoal.Y - currentPos.Y);
                    t = currenGrid[currentPos.X -1, currentPos.Y];
                    currenGrid[currentPos.X -1, currentPos.Y] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.X -= 1;
                    posMoveLc_1 = Tools.GetHorizontalConflicts(currentPos.X, currenGrid) ;
                    posMoveLc_2 = Tools.GetHorizontalConflicts(currentPos.X - 1, currenGrid);
                    break;
            }
            posInGoal = Tools.goalState[currenGrid[currentPos.X, currentPos.Y]];
            int h2 = Math.Abs(posInGoal.X - currentPos.X) + Math.Abs(posInGoal.Y - currentPos.Y);
            h = parent.h - h1 + h2 - lc;
            lc -= (preMoveLc_1 + preMoveLc_2);
            lc += (posMoveLc_1 + posMoveLc_2);
            h += lc;
            return currenGrid;
        }
        
        public int[,] MovePos3(int dir, int[,] currenGrid, Point currentPos)
        {
            int t = 0;
            int h1 = 0;
            Point posInGoal = new Point();
            switch (dir)
            {
                case 0:
                    posInGoal = Tools.goalState[currenGrid[currentPos.X, currentPos.Y + 1]];
                    h1 = Math.Abs(posInGoal.X - currentPos.X) + Math.Abs(posInGoal.Y - (currentPos.Y + 1));
                    t = currenGrid[currentPos.X, currentPos.Y + 1];
                    currenGrid[currentPos.X, currentPos.Y + 1] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.Y += 1;
                    break;
                case 1:
                    posInGoal = Tools.goalState[currenGrid[currentPos.X + 1, currentPos.Y]];
                    h1 = Math.Abs(posInGoal.X - (currentPos.X + 1)) + Math.Abs(posInGoal.Y - currentPos.Y );
                    t = currenGrid[currentPos.X + 1, currentPos.Y];
                    currenGrid[currentPos.X + 1, currentPos.Y] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.X += 1;
                    break;
                case 2:
                    posInGoal = Tools.goalState[currenGrid[currentPos.X, currentPos.Y - 1]];
                    h1 = Math.Abs(posInGoal.X - currentPos.X) + Math.Abs(posInGoal.Y - (currentPos.Y -1));
                    t = currenGrid[currentPos.X, currentPos.Y - 1];
                    currenGrid[currentPos.X, currentPos.Y -1] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.Y -= 1;
                    break;
                case 3:
                    posInGoal = Tools.goalState[currenGrid[currentPos.X - 1, currentPos.Y]];
                    h1 = Math.Abs(posInGoal.X - (currentPos.X - 1)) + Math.Abs(posInGoal.Y - currentPos.Y);
                    t = currenGrid[currentPos.X -1, currentPos.Y];
                    currenGrid[currentPos.X -1, currentPos.Y] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.X -= 1;
                    break;
            }
            posInGoal = Tools.goalState[currenGrid[currentPos.X, currentPos.Y]];
            int h2 = Math.Abs(posInGoal.X - currentPos.X) + Math.Abs(posInGoal.Y - currentPos.Y);
            h = parent.h - h1 + h2;
            return currenGrid;
        }

        public int[,] MovePos2(int dir, int[,] currenGrid, Point currentPos){
               int t = 0;
            Point posInGoal = new Point();
            switch (dir)
            {
                case 0:
                    posInGoal = Tools.goalState[currenGrid[currentPos.X, currentPos.Y + 1]];
                    t = currenGrid[currentPos.X, currentPos.Y + 1];
                    currenGrid[currentPos.X, currentPos.Y + 1] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.Y += 1;
                    break;
                case 1:
                    posInGoal = Tools.goalState[currenGrid[currentPos.X + 1, currentPos.Y]];
                    t = currenGrid[currentPos.X + 1, currentPos.Y];
                    currenGrid[currentPos.X + 1, currentPos.Y] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.X += 1;
                    break;
                case 2:
                    posInGoal = Tools.goalState[currenGrid[currentPos.X, currentPos.Y - 1]];
                    t = currenGrid[currentPos.X, currentPos.Y - 1];
                    currenGrid[currentPos.X, currentPos.Y -1] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.Y -= 1;
                    break;
                case 3:
                    posInGoal = Tools.goalState[currenGrid[currentPos.X - 1, currentPos.Y]];
                    t = currenGrid[currentPos.X -1, currentPos.Y];
                    currenGrid[currentPos.X -1, currentPos.Y] = currenGrid[currentPos.X, currentPos.Y];
                    currenGrid[currentPos.X, currentPos.Y] = t;
                    pos.X -= 1;
                    break;
            }
            posInGoal = Tools.goalState[currenGrid[currentPos.X, currentPos.Y]];
            h = Tools.GetHValueMisplaced(currenGrid);
            return currenGrid;
        }

        public void CopyNode(Node currentState, int moveDir)
        {
            pos = currentState.pos;
            parent = currentState;
            CopyGrid(parent.grid);
            lc = parent.lc;
            switch (Program.heuristicUsed)
            {
                case Program.Heuristic.Manhattan:
                    grid = MovePos3(moveDir, grid, pos);
                    break;
                case Program.Heuristic.Misplaced:
                    grid = MovePos2(moveDir, grid, pos);
                    break;
                case Program.Heuristic.ManhattanLC:
                    grid = MovePos(moveDir, grid, pos);
                    break;
            }
            SetHashCode();
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
