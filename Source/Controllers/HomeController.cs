using Microsoft.AspNetCore.Mvc;
using SRkMatchmakerAPI.Framework;
using SRkMatchmakerAPI.Framework.DTO;
using SRkMatchmakerAPI.Framework.Mappers;
using SRkMatchmakerAPI.Persistence;
using SRkMatchmakerAPI.Seeders;

namespace SRkMatchmakerAPI.Controllers;

[ApiController]
[Route("/")]
public class HomeController : ControllerBase
{
    readonly MatchmakerDbContext ctx;
    //readonly MatchmakingTool mmTool;

    public HomeController()
    {
        ctx = new MatchmakerDbContext();
        //mmTool = new MatchmakingTool();
    }

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    
    [HttpPost("makeseed")]
    public async Task<IActionResult> MakeGame()
    {
        var response = await Task.Run(() =>
        {
            var players = SeederDev.Make100Players();
            var mmTool = new MatchmakingTool(players.Select(p => p.ToPlayerDTO()).ToArray());

            return mmTool.Start();
        });

        return Ok(response);
    }


    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(Summaries);
    }

    [HttpGet("players/get")]
    public IActionResult GetAllPlayers()
    {
        return Ok(ctx.Players);
    }

    [HttpPost("players/add")]
    public IActionResult AddPlayer(PlayerDTO p)
    {
        System.Diagnostics.Debug.WriteLine($"Players before: {ctx.Players.Count}");
        if (ctx.Players.ContainsKey(p.User.Email)) 
        {
            return BadRequest(new { Message = $"Player({p.User.Email}) is already in!" });
        }

        var ply = new Player(p.User.Id, p.User.Email, p.MMR, p.Pos);

        ctx.Players.Add(ply.Email, ply);

        return Ok(new 
        { 
            Message = $"Player({ply.Email}) added succesfully!",  
            ctx.Players.Count 
        });
    }
}
