namespace LoginForm.Models;

public class EntityResponse
{
    public int EntityId { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? FTPHost { get; set; }
    public int FTPPort { get; set; }
}