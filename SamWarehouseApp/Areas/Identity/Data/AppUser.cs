using Microsoft.AspNetCore.Identity;
using SamWarehouseApp.Models;

namespace SamWarehouseApp.Areas.Identity.Data
{
    public class AppUser : IdentityUser
    {
        public ICollection<ShoppingList> ShoppingLists { get; set; }
    }
}
