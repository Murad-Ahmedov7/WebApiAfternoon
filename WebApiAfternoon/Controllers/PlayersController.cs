using Microsoft.AspNetCore.Mvc;
using WebApiAfternoon.Dtos;
using WebApiAfternoon.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAfternoon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

   
    public class PlayersController : ControllerBase
    {
        private static List<Player> players = new List<Player>
        {
            new Player
            {
                Id = 1,
                PlayerName="John Doe",
                City="Los Angelos",
                Score=90
            },
            new Player
            {
                Id=2,
                PlayerName="Lebron James",
                City="New York",
                Score=99
            }
        };
        // GET: api/<PlayersController api/players>
        [HttpGet]
        public ActionResult <IEnumerable<PlayerDto>> GetPlayers()
        {
            var datatoReturn = players.Select(p => new PlayerDto
            {
                City = p.City,
                PlayerName = p.PlayerName,
                Score = p.Score,

            });
            return Ok (datatoReturn);
        }

        // GET api/<PlayersController>/5
        [HttpGet("{id}")]
        public ActionResult<Player> Get(int id)
        {
            var player = players.FirstOrDefault(x=>x.Id==id);
            //if(player == null)
            //{
            //    return NotFound();
            //}
            var dataToReturn = new PlayerDto
            {
                PlayerName = player.PlayerName,
                City = player.City,
                Score = player.Score,
            };
            return Ok(dataToReturn);
        }

        // POST api/<PlayersController>
        [HttpPost]
        public ActionResult<PlayerAddDto> Post([FromBody] PlayerAddDto player)
        {
            var newPlayer = new Player();

            newPlayer.Id = players.Any() ? players.Max(x => x.Id) + 1 : 1;

            newPlayer.PlayerName = player.PlayerName;
            newPlayer.City = player.City;

            players.Add(newPlayer);
            //return Ok(newPlayer); //200 and object
            //return Created();//201
            return CreatedAtAction(nameof(Get), new { id = newPlayer.Id }, player); //201 and object
        }

        // PUT api/<PlayersController>/5
        [HttpPut("{id}")]
        public ActionResult Put (int id, [FromBody] Player updatePlayer)
        {
            var player = players.FirstOrDefault(p => p.Id == id);
            if(player == null)
            {
                return NotFound();
            }
            player.City = updatePlayer.City;
            player.Score = updatePlayer.Score;
            player.PlayerName = updatePlayer.PlayerName;
            return NoContent();
        }

        // DELETE api/<PlayersController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete (int id)
        {
            var player=players.FirstOrDefault(x=>x.Id==id);
            if(player == null)
            {
                return NotFound();
            }
            players.Remove(player);
            return NoContent();
        }
    }
}
