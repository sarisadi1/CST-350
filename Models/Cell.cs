namespace app.Models
{
    public class Cell
    {
        public bool IsMine { get; set; }
        public bool IsRevealed { get; set; }
        public int AdjacentMines { get; set; }
        public bool IsFlagged { get; set; }  

        public Cell()
        {
            IsMine = false;
            IsRevealed = false;
            AdjacentMines = 0;
            IsFlagged = false;  //flag state
        }

        public void Reveal()
        {
            IsRevealed = true;
        }

        public void ToggleFlag()
        {
            IsFlagged = !IsFlagged;
        }
    }
}
