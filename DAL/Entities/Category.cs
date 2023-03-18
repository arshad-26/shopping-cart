namespace DAL.Entities;

public class Category
{
    public int CategoryID { get; set; }

    public string Name { get; set; }

    public ICollection<Item>? Items { get; set; }

    public Category()
    {
        Name = String.Empty;
	}
}

