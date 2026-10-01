using Microsoft.AspNetCore.Mvc;
using UrlShortener.Model;

namespace UrlShortener.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpPost(Name = "CreateUser")]
    public IActionResult CreateUser([FromBody] User user)
    {
        // Implementation for creating a new user
        return Ok();
    }

    [HttpGet("{userId}", Name = "GetUser")]
    public IActionResult GetUser(int userId)
    {
        // Implementation for retrieving a user by ID
        return Ok();
    }
}
