using Microsoft.AspNetCore.Mvc;
using app.Models;
// using Board;
using System.Text.Json;

namespace MinesweeperApp.Controllers
{
    public class GameController : Controller
    {
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

            // Save the game start time to session
            HttpContext.Session.SetString("GameStartTime", DateTime.Now.ToString());

            // Serialize and save the board to session
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

            board.RevealCell(row, col);

            if (board.Cells[row][col].IsMine)
            {
                HttpContext.Session.Remove("CurrentBoard");
                return Json(new { gameOver = true, redirectUrl = Url.Action("Loss") });
            }

            if (board.CheckWinCondition())
            {
                HttpContext.Session.Remove("CurrentBoard");

                var startTime = DateTime.Parse(HttpContext.Session.GetString("GameStartTime"));
                var elapsedTime = DateTime.Now - startTime;
                var score = CalculateScore(board, elapsedTime);

                return Json(new { gameOver = true, redirectUrl = Url.Action("Win", new { score }) });
            }

            // Update the board in the session
            HttpContext.Session.SetString("CurrentBoard", JsonSerializer.Serialize(board));

            return PartialView("_CellPartial", board.Cells[row][col]);
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



    }
}
