
using app.Models;

    public class Board
    {
        public int Rows { get; }
        public int Columns { get; }
        public int Mines { get; }
        public List<List<Cell>> Cells { get; private set; }
        public bool IsGameOver { get; private set; } = false;

        private static readonly Random _random = new Random();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public Board(int rows, int columns, int mines)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            Rows = rows;
            Columns = columns;
            Mines = mines;
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            Cells = new List<List<Cell>>();

            
            for (int i = 0; i < Rows; i++)
            {
                var row = new List<Cell>();
                for (int j = 0; j < Columns; j++)
                {
                    row.Add(new Cell());
                }
                Cells.Add(row);
            }

            PlaceMines();
            CalculateAdjacentMines();
        }

        private void PlaceMines()
        {
            int minesPlaced = 0;

            while (minesPlaced < Mines)
            {
                int row = _random.Next(Rows);
                int col = _random.Next(Columns);

                if (!Cells[row][col].IsMine)
                {
                    Cells[row][col].IsMine = true;
                    minesPlaced++;
                }
            }
        }

        private void CalculateAdjacentMines()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (!Cells[row][col].IsMine)
                    {
                        Cells[row][col].AdjacentMines = CountAdjacentMines(row, col);
                    }
                }
            }
        }

        private int CountAdjacentMines(int row, int col)
        {
            int mineCount = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;

                    if (IsWithinBounds(newRow, newCol) && Cells[newRow][newCol].IsMine)
                    {
                        mineCount++;
                    }
                }
            }

            return mineCount;
        }

        private bool IsWithinBounds(int row, int col)
        {
            return row >= 0 && row < Rows && col >= 0 && col < Columns;
        }

        public void RevealCell(int row, int col)
        {
            var cell = Cells[row][col];
            if (cell.IsMine)
            {
                IsGameOver = true;
            }
            cell.IsRevealed = true;
        }
    }
