using Microsoft.AspNetCore.Mvc;
using App.Data;

namespace MinesweeperApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameApiController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        [HttpGet("showSavedGames")]
        public IActionResult ShowAllGames()
        {
            var games = _context.Games.ToList();
            return Ok(games);
        }

        
        [HttpGet("showSavedGames/{id}")]
        public async Task<IActionResult> ShowGameById(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound(new { message = "Game not found" });
            }

            return Ok(game);
        }

       
        [HttpDelete("deleteOneGame/{id}")]
        public async Task<IActionResult> DeleteGameById(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound(new { message = "Game not found" });
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Game deleted successfully" });
        }
    }
}
