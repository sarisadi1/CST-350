using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using App.Data;


namespace MinesweeperApp.Controllers
{
    public class GameController : Controller
    {

        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult StartGame()
        {
            return View();
        }

       [HttpPost]
        public IActionResult StartGame(int boardSize, string difficulty)
        {
            int mineCount = difficulty switch
            {
                "Easy" => boardSize * boardSize / 10,
                "Medium" => boardSize * boardSize / 5,
                "Hard" => boardSize * boardSize / 3,
                _ => boardSize * boardSize / 10
            };

            var board = new Board(boardSize, boardSize, mineCount);

            HttpContext.Session.SetString("GameStartTime", DateTime.Now.ToString());

            HttpContext.Session.SetString("CurrentBoard", JsonSerializer.Serialize(board));

            return RedirectToAction("MineSweeperBoard");
        }


        [HttpGet]
        public IActionResult MineSweeperBoard()
        {
            var boardJson = HttpContext.Session.GetString("CurrentBoard");

            if (string.IsNullOrEmpty(boardJson))
            {
                return RedirectToAction("StartGame");
            }

            var board = JsonSerializer.Deserialize<Board>(boardJson);
            return View(board);
        }

        private int CalculateScore(Board board, TimeSpan elapsedTime)
            {
                int baseScore = board.Rows * board.Columns * 10;
                double timePenalty = elapsedTime.TotalSeconds;
                return Math.Max((int)(baseScore - timePenalty), 0); 
            }


        [HttpPost]
        public IActionResult RevealCell(int row, int col)
        {
            var boardJson = HttpContext.Session.GetString("CurrentBoard");
            if (string.IsNullOrEmpty(boardJson))
            {
                return RedirectToAction("StartGame");
            }

            var board = JsonSerializer.Deserialize<Board>(boardJson);

            var cell = board.Cells[row][col];

            if (cell.IsRevealed || cell.IsFlagged)
            {
                return PartialView("_CellPartial", cell);
            }


            cell.Reveal();

            // If it's a mine, game over
            if (cell.IsMine)
            {
                HttpContext.Session.Remove("CurrentBoard");
                return Json(new { gameOver = true, redirectUrl = Url.Action("Loss") });
            }


            if (cell.AdjacentMines == 0)
            {
                RevealAdjacentCells(board, row, col);
            }

            if (board.CheckWinCondition())
            {
                HttpContext.Session.Remove("CurrentBoard");

                var startTime = HttpContext.Session.Get<DateTime>("GameStartTime");
                var elapsedTime = DateTime.Now - startTime;
                var score = CalculateScore(board, elapsedTime);

                return Json(new { gameOver = true, redirectUrl = Url.Action("Win", new { score }) });
            }


            HttpContext.Session.SetString("CurrentBoard", JsonSerializer.Serialize(board));

            return PartialView("_CellPartial", cell);
        }

        private void RevealAdjacentCells(Board board, int row, int col)
        {

            var neighbors = new (int, int)[]
            {
                (-1, -1), (-1, 0), (-1, 1), 
                (0, -1),          (0, 1), 
                (1, -1), (1, 0), (1, 1)
            };

            foreach (var (dx, dy) in neighbors)
            {
                int newRow = row + dx;
                int newCol = col + dy;

                if (newRow >= 0 && newRow < board.Rows && newCol >= 0 && newCol < board.Columns)
                {
                    var neighborCell = board.Cells[newRow][newCol];
                    if (!neighborCell.IsRevealed)
                    {
                        neighborCell.Reveal();
                        if (neighborCell.AdjacentMines == 0)
                        {
                            RevealAdjacentCells(board, newRow, newCol);
                        }
                    }
                }
            }
        }

        public IActionResult GameOver()
        {
            return View();
        }

        public IActionResult Win(int score)
        {
            return View(score);
        }

        public IActionResult Loss()
        {
            return View();
        }

   
        [HttpPost]
        public IActionResult ToggleFlag(int row, int col)
        {
            var boardJson = HttpContext.Session.GetString("CurrentBoard");
            if (string.IsNullOrEmpty(boardJson))
            {
                return RedirectToAction("StartGame");
            }

            var board = JsonSerializer.Deserialize<Board>(boardJson);
            var cell = board.Cells[row][col];

            if (!cell.IsRevealed)
            {
                cell.ToggleFlag();
            }

            HttpContext.Session.SetString("CurrentBoard", JsonSerializer.Serialize(board));

            return PartialView("_CellPartial", cell);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGame()
        {
            var boardJson = HttpContext.Session.GetString("CurrentBoard");

            if (string.IsNullOrEmpty(boardJson))
            {
                return Json(new { success = false, message = "No game state to save." });
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            var game = new Game
            {
                UserId = (int)userId,
                DateSaved = DateTime.Now,
                GameData = boardJson
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return RedirectToAction("ShowSavedGames");
        }


        [HttpGet]
        public IActionResult ShowSavedGames()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var games = _context.Games.Where(g => g.UserId == userId).ToList();
            return View(games);
        }

       [HttpGet]
        public async Task<IActionResult> LoadGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound(); 
            }

            var gameState = JsonSerializer.Deserialize<Board>(game.GameData);

            HttpContext.Session.SetString("CurrentBoard", game.GameData);

            return RedirectToAction("MineSweeperBoard");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return Json(new { success = false, message = "Game not found!" });
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Game deleted successfully!" });
        }



    }
}
