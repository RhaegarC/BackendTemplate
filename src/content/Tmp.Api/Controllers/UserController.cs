namespace Tmp.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Tmp.Interface.Service;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : Controller
{
    private readonly IUserService _userService = userService;

    [HttpGet("index")]
    public IActionResult Index()
    {
        return Ok("Welcome");
    }
}
