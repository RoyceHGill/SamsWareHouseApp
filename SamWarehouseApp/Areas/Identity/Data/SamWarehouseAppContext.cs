using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SamWarehouseApp.Areas.Identity.Data;
using SamWarehouseApp.Models;

namespace SamWarehouseApp.Data;

public class SamWarehouseAppContext : IdentityDbContext<AppUser>
{
    public SamWarehouseAppContext(DbContextOptions<SamWarehouseAppContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<ListDetails> ListDetails { get; set; }





    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<ListDetails>()
            .HasOne(listDetail => listDetail.ShoppingList)
            .WithMany(shoppingList => shoppingList.ListDetails)
            .HasForeignKey(listDetails => listDetails.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ListDetails>()
            .HasOne(listDetails => listDetails.Product)
            .WithMany(product => product.ListDetails)
            .HasForeignKey(listDetails => listDetails.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<Product>()
            .HasData(
                new Product { Id = 1, Name = "Granny Smith Apples", Unit = "1kg", Price = 5.50f },
                new Product { Id = 2, Name = "Fresh tomatoes", Unit = "500g", Price = 5.90f },
                new Product { Id = 3, Name = "Watermelon", Unit = "Whole", Price = 6.60f },
                new Product { Id = 4, Name = "Cucumber", Unit = "1 whole", Price = 1.90f },
                new Product { Id = 5, Name = "Red potato washed", Unit = "1kg", Price = 4.00f },
                new Product { Id = 6, Name = "Red tipped bananas", Unit = "1kg", Price = 4.90f },
                new Product { Id = 7, Name = "Red onion", Unit = "1kg", Price = 3.50f },
                new Product { Id = 8, Name = "Carrots", Unit = "1kg", Price = 2.00f },
                new Product { Id = 9, Name = "Iceburg Lettuce", Unit = "1", Price = 2.50f },
                new Product { Id = 10, Name = "Helga's Wholemeal", Unit = "1", Price = 3.70f },
                new Product { Id = 11, Name = "Free range chicken", Unit = "1kg", Price = 7.50f },
                new Product { Id = 12, Name = "Manning Valley 6-pk", Unit = "6 eggs", Price = 3.60f },
                new Product { Id = 13, Name = "A2 light milk", Unit = "1 litre", Price = 2.90f },
                new Product { Id = 14, Name = "Chobani Strawberry Yoghurt", Unit = "1", Price = 1.50f },
                new Product { Id = 15, Name = "Lurpak Salted Blend", Unit = "250g", Price = 5.00f },
                new Product { Id = 16, Name = "Bega Farmers Tasty", Unit = "250g", Price = 4.00f },
                new Product { Id = 17, Name = "Babybel Mini,", Unit = "100g", Price = 4.20f },
                new Product { Id = 18, Name = "Cobram EVOO", Unit = "375ml", Price = 8.00f },
                new Product { Id = 19, Name = "Heinz Tomato Soup", Unit = "535g", Price = 2.50f },
                new Product { Id = 20, Name = "John West Tuna can", Unit = "95g", Price = 1.50f },
                new Product { Id = 21, Name = "Cadbury Dairy Milk", Unit = "200g", Price = 5.00f },
                new Product { Id = 22, Name = "Coca Cola", Unit = "2 litre", Price = 2.85f },
                new Product { Id = 23, Name = "Smith's Original Share Pack Crisps", Unit = "170g", Price = 3.29f },
                new Product { Id = 24, Name = "Birds Eye Fish Fingers", Unit = "375g", Price = 4.50f },
                new Product { Id = 25, Name = "Berri Orange Juice", Unit = "2 litre", Price = 6.00f },
                new Product { Id = 26, Name = "Vegemite", Unit = "380g", Price = 6.00f },
                new Product { Id = 27, Name = "Cheddar Shapes", Unit = "175g", Price = 2.00f },
                new Product { Id = 28, Name = "Colgate Total Toothpaste Original", Unit = "110g", Price = 3.50f },
                new Product { Id = 29, Name = "Weet Bix Saniatarium Organic", Unit = "750g", Price = 5.33f },
                new Product { Id = 30, Name = "Lindt Excellence 70% Cocoa Block", Unit = "100g", Price = 4.25f },
                new Product { Id = 31, Name = "Milo Chocolate Malt", Unit = "200g", Price = 4.00f },
                new Product { Id = 32, Name = "Original Tim Tams Choclate", Unit = "200g", Price = 3.65f },
                new Product { Id = 33, Name = "Philadelphia Original Cream Cheese", Unit = "250g", Price = 4.30f },
                new Product { Id = 34, Name = "Moccana Classic Instant Medium Roast", Unit = "100g", Price = 6.00f },
                new Product { Id = 35, Name = "Capilano Squeezable Honey", Unit = "500g", Price = 7.35f },
                new Product { Id = 36, Name = "Nutella jar", Unit = "400g", Price = 4.00f },
                new Product { Id = 37, Name = "Arnott's Scotch Finger", Unit = "250g", Price = 2.85f },
                new Product { Id = 38, Name = "South Cape Greek Feta", Unit = "200g", Price = 5.00f },
                new Product { Id = 39, Name = "Sacla Pasta Tomato Basil Sauce", Unit = "420g", Price = 4.50f },
                new Product { Id = 40, Name = "Primo English Ham", Unit = "100g", Price = 3.00f },
                new Product { Id = 41, Name = "Primo Short cut rindless Bacon", Unit = "175g", Price = 5.00f },
                new Product { Id = 42, Name = "Golden Circle Pineapple Pieces in natural juice", Unit = "440g", Price = 3.25f },
                new Product { Id = 43, Name = "San Remo Linguine Pasta No 1", Unit = "500g", Price = 1.95f }
                );



    }
}
