namespace DAL.Entities;

public class Item
{
    public long ID { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public int CategoryID { get; set; }

	public string ImagePath { get; set; }

    public Category Category { get; set; }

    public Item()
    {
        Name= String.Empty;
        ImagePath= String.Empty;
        Category = new Category();
    }
}

