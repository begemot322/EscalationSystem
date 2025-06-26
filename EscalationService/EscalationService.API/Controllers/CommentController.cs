using Microsoft.AspNetCore.Mvc;

namespace EscalationService.API.Controllers;

public class CommentController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}