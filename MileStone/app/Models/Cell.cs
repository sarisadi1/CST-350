
namespace app.Models
{
    public class Cell
    {
        public bool IsMine { get; set; }
        public bool IsRevealed { get; set; }
        public int AdjacentMines { get; set; }

        public Cell()
        {
            IsMine = false;
            IsRevealed = false;
            AdjacentMines = 0;
        }
    }
}
