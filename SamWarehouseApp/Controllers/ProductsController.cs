using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamWarehouseApp.Data;
using SamWarehouseApp.Models;

namespace SamWarehouseApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly SamWarehouseAppContext _context;

        public ProductsController(SamWarehouseAppContext context)
        {
            _context = context;
        }

        // Get Product/GetProductList
        /// <summary>
        /// 
        /// Queries the database for products that contain the search query in either the Name Description or price of the Product.
        /// Excluding products that are already on the shopping list. 
        /// 
        /// </summary>
        /// <param name="id">The shopping list's Id.</param>
        /// <param name="search">The string to search for.</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult GetProductList(int id, string search)
        {
            var shoppingList = _context.ShoppingLists.Where(ld => ld.Id == id).Include(sl => sl.ListDetails).ThenInclude(ld => ld.Product).FirstOrDefault();
            var listDetails = shoppingList.ListDetails;
            var listId = shoppingList.Id;

            var productsOnList = new List<Product>();

            foreach (var item in listDetails)
            {
                productsOnList.Add(_context.Products.Where(p => p.Id == item.ProductId).FirstOrDefault());
            }

            if (String.IsNullOrEmpty(search))
            {
                var allProducts = _context.Products.ToList();

                var filteredResults = allProducts.Except(productsOnList).ToList();

                ViewBag.ListId = listId;
                return PartialView("_ProductList", filteredResults);
            }
            else
            {
                var filteredResults = _context.Products.Where(p => p.Name.Contains(search)
                || p.Description.Contains(search)
                || p.Price.ToString().Contains(search))
                    .ToList()
                    .Except(productsOnList)
                    .ToList();

                ViewBag.ShoppingListId = listId;
                return PartialView("_ProductList", filteredResults);
            }


        }
    }
}
