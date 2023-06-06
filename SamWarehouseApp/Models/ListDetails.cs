namespace SamWarehouseApp.Models
{
    public class ListDetails
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public ShoppingList? ShoppingList { get; set; }
        public Product? Product { get; set; }
    }
}
