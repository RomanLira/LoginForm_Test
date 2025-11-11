using LoginForm.Models;
using LoginForm.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoginForm.Controllers;

public class HomeController : Controller
{
    private readonly RequestService _service;

    public HomeController(RequestService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public IActionResult Index() => View(new LoginViewModel());
    
    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
        {
            model.Message = "Email and Password are required.";
            model.Success = false;
            return View(model);
        }

        var (success, message, entity) = await _service.LoginAsync(model.Email, model.Password);
        model.Message = message;
        model.Success = success;
        model.Entity = entity;

        return View(model);
    }
}