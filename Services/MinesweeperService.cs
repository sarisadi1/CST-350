using app.Models;

namespace MinesweeperApp.Services
{
    public class MinesweeperService
    {
        public Board CreateBoard(int rows, int cols, int mines)
        {
            return new Board(rows, cols, mines);
        }

        public void RevealCell(Board board, int row, int col)
        {
            board.RevealCell(row, col);
        }

        public bool IsWin(Board board)
        {
            foreach (var row in board.Cells)
            {
                foreach (var cell in row)
                {
                    if (!cell.IsRevealed && !cell.IsMine)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CheckWinCondition(Board board)
        {
            return board.CheckWinCondition();
        }
    }
}
