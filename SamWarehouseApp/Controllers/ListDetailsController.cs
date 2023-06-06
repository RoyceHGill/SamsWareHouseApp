using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using SamWarehouseApp.Data;
using SamWarehouseApp.Models;

namespace SamWarehouseApp.Controllers
{
    public class ListDetailsController : Controller
    {
        private readonly SamWarehouseAppContext _context;

        public ListDetailsController(SamWarehouseAppContext context)
        {
            _context = context;
        }

        // GET: ListDetails/Create
        /// <summary>
        /// 
        /// Returns the partial view for creating list details. Connecting shopping lists and products. 
        /// 
        /// </summary>
        /// <param name="id">The shopping list's id.</param>
        /// <returns> The add produtct partial views. </returns>
        [Authorize]
        [HttpGet]
        public IActionResult Create([FromQuery] int id)
        {
            var shoppingList = _context.ShoppingLists.Where(sl => sl.Id == id).FirstOrDefault();

            return PartialView("_AddProduct", shoppingList);
        }

        // GET: ListDetails/Edit/?productId=5&listId=5&listDetailsId=5
        /// <summary>
        /// 
        /// Returns the partial view for adjusting the Quantity for a product on a shopping list.
        /// 
        /// </summary>
        /// <param name="productId">The products id. </param>
        /// <param name="listId"> The id of the shopping list.</param>
        /// <param name="listDetailsId"> The id for the relationship between the shopping list and the product.</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult Edit([FromQuery] int productId, [FromQuery] int listId, [FromQuery] int listDetailsId)
        {
            var listDetails = _context.ListDetails.Where(ld => ld.Id == listDetailsId).FirstOrDefault();

            if (listDetails == null)
            {
                listDetails = new ListDetails { ListId = listId, ProductId = productId };
            }
            return PartialView("_EditQuantity", listDetails);
        }

        /// <summary>
        /// 
        /// Creates the a relationship between the shopping list and a product.
        /// 
        /// Notes the quantity of that product on the shopping list. 
        /// 
        /// </summary>
        /// <param name="listDetails"> Id's and quantity of the Detail. </param>
        /// <returns>The user back to the edit page with edit page with the changes made.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ListDetails listDetails)
        {
            if (ModelState.IsValid && listDetails.Quantity > 0)
            {
                _context.Add(listDetails);
                await _context.SaveChangesAsync();
                var shoppinglist = _context.ShoppingLists.Where(sl => sl.Id == listDetails.ListId).FirstOrDefault();
                return RedirectToAction("Edit", "ShoppingLists", shoppinglist);
            }
            else
            {
                var shoppinglist = _context.ShoppingLists.Where(sl => sl.Id == listDetails.ListId).FirstOrDefault();
                return RedirectToAction("Edit", "ShoppingLists", shoppinglist);
            }
        }

        /// <summary>
        /// 
        /// Receives the input from the quantity adjuster.
        /// if there is not existing relationship between list and product, create a new one. 
        /// 
        /// if there is no quantity provided. Delete the relationship. 
        /// 
        /// if the input is valid the relationships quantity is updated. 
        /// 
        /// </summary>
        /// <param name="listDetails">listId, ProductId, Quantity.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(ListDetails listDetails)
        {
            ShoppingList shoppinglist = new ShoppingList();
            if (listDetails.Id < 1)
            {
                return await Create(listDetails);
            }
            else
            {
                if (listDetails.Quantity < 1)
                {
                    return await DeleteConfirmed(listDetails.Id);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(listDetails);

                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!ListDetailsExists(listDetails.Id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        shoppinglist = _context.ShoppingLists.Where(sl => sl.Id == listDetails.ListId).FirstOrDefault();
                        return RedirectToAction("Edit", "ShoppingLists", shoppinglist);
                    }

                }
            }
            shoppinglist = _context.ShoppingLists.Where(sl => sl.Id == listDetails.ListId).FirstOrDefault();

            return RedirectToAction("Edit", "ShoppingLists", shoppinglist);
        }

        // GET: ListDetails/Delete/5
        /// <summary>
        /// 
        /// Get the delete confirmation partial view. 
        /// 
        /// </summary>
        /// <param name="listDetailsId"> The list's detials id</param>
        /// <returns> the delete partial view.</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int? listDetailsId)
        {
            if (listDetailsId == null || _context.ListDetails == null)
            {
                return NotFound();
            }

            var listDetails = await _context.ListDetails
                .Include(l => l.Product)
                .Include(l => l.ShoppingList)
                .FirstOrDefaultAsync(m => m.Id == listDetailsId);
            if (listDetails == null)
            {
                return NotFound();
            }

            return PartialView("_DeleteConfirm", listDetails);
        }

        // POST: ListDetails/Delete/5
        /// <summary>
        /// Removes the relationship between the shopping list and the product. 
        /// </summary>
        /// <param name="Id">The id of the list's detail.</param>
        /// <returns>the user is retured to the Edit page with the changes taking effect.</returns>
        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            if (_context.ListDetails == null)
            {
                return Problem("Entity set 'SamWarehouseAppContext.ListDetails'  is null.");
            }
            var listDetails = await _context.ListDetails.Where(ld => ld.Id == Id).Include(ld => ld.ShoppingList).FirstOrDefaultAsync();
            var shoppinglist = listDetails.ShoppingList;
            if (listDetails != null)
            {
                _context.ListDetails.Remove(listDetails);
            }



            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "ShoppingLists", shoppinglist);
        }

        /// <summary>
        /// Checks if there is a list detail in the database already.
        /// </summary>
        /// <param name="id">Id for the list detail. </param>
        /// <returns> true if the list details exits,
        /// false if not</returns>
        private bool ListDetailsExists(int id)
        {
            return (_context.ListDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
