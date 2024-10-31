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

        [HttpPost]
        public IActionResult RevealCell(int row, int col)
        {
            var boardJson = HttpContext.Session.GetString("CurrentBoard");
            if (string.IsNullOrEmpty(boardJson))
            {
                return RedirectToAction("StartGame");
            }

            var board = JsonSerializer.Deserialize<Board>(boardJson);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            board.RevealCell(row, col);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            // Re-serialize and save updated board to session
            HttpContext.Session.SetString("CurrentBoard", JsonSerializer.Serialize(board));

            if (board.IsGameOver)
            {
                return RedirectToAction("GameOver");
            }

            return RedirectToAction("MineSweeperBoard");
        }

        public IActionResult GameOver()
        {
            return View();
        }
    }
}
