using SamWarehouseApp.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace SamWarehouseApp.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Name { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string SamsAppUserId { get; set; }

        public ICollection<ListDetails>? ListDetails { get; set; }

        public AppUser? SamsAppUser { get; set; }
    }
}
