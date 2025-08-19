namespace ArtStore.Domain.Entities;

public class Tenant : IEntity<int>
{
    public int Id { get; set; } 
    public string? Name { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();


}