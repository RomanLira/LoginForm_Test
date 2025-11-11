using System.ComponentModel.DataAnnotations;

namespace LoginForm.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    
    public string? Message { get; set; }
    public bool Success { get; set; }
    public EntityResponse? Entity { get; set; }
}