namespace InventoryMS.Models;

public sealed class User
{
    public int UserId { get; set;}
    public string Firstname { get; set;} = string.Empty;
    public string Lastname { get; set;} = string.Empty;
    public int Age { get; set;}
    public string Gender { get; set;} = string.Empty;
    public string Address { get; set;} = string.Empty;
    public string Phone { get; set;} = string.Empty;
    public string Email { get; set;} = string.Empty;
    public string Role { get; set;} = string.Empty;
    public string PasswordHash { get; set;} = string.Empty;
    public DateTime CreatedAt { get; set;}
    public ICollection<Order> Orders { get; set;} = new List<Order>();
}