using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamWarehouseApp.Areas.Identity.Data;
using SamWarehouseApp.Data;
using SamWarehouseApp.Models;

namespace SamWarehouseApp.Controllers
{
    public class ShoppingListsController : Controller
    {
        private readonly SamWarehouseAppContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ShoppingListsController(SamWarehouseAppContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: ShoppingLists
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var samWarehouseAppContext = await _context.ShoppingLists.Where(sl => sl.SamsAppUserId == userId)
                .Include(s => s.SamsAppUser)
                .Include(m => m.ListDetails)
                .ThenInclude(ld => ld.Product)
                .ToListAsync();

            if (TempData["DidNamingWork"] == null)
            {
                return View(samWarehouseAppContext);
            }
            else
            {
                ViewBag.DidNamingWork = (bool)TempData["DidNamingWork"];
                return View(samWarehouseAppContext);
            }



        }

        // GET: ShoppingLists/Details/5
        /// <summary>
        /// Returns the partial view to display the a shopoing lists details. 
        /// 
        /// calculates the total cost of the products on shopping list.
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// The partial view for the list details to be displayed. 
        /// 
        /// the total cost of the shopping list.
        /// 
        /// shopping list , Products, ListDetails
        /// 
        /// </returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ShoppingLists == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            var shoppingList = _context.ShoppingLists
                .Include(sl => sl.ListDetails)
                .ThenInclude(ld => ld.Product)
                .Where(sl => sl.Id == id)
                .FirstOrDefault();
            if (shoppingList == null)
            {
                return NotFound();
            }

            float total = GetTotalPrice(shoppingList);

            ViewBag.Total = total;

            return PartialView("_Details", shoppingList);
        }



        // GET: ShoppingLists/Create
        /// <summary>
        /// 
        /// returns the partial view for the create interface.
        /// 
        /// </summary>
        /// <returns>
        /// A partial view for creating new shopping lists.
        /// </returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewBag.SamsAppUserId = _userManager?.GetUserId(User);
            return PartialView("_Create");
        }

        /// <summary>
        /// 
        /// End point for Posting new Shopping lists to the database. 
        /// 
        /// </summary>
        /// <param name="shoppingList">contains the name input for creating a shopping list.</param>
        /// <returns>
        /// 
        /// The user is sent to the Index page if the name entered is already contained within database 
        /// with a false boolean data type to indicate that the Insertion failed.
        /// 
        /// If the input is valid the user is sent to the list's page. 
        /// 
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string Name)
        {
            var userId = _userManager.GetUserId(User);
            if (ListNameValidation(Name))
            {

                var shoppingList = new ShoppingList() { Name = Name, SamsAppUserId = userId };
                _context.Add(shoppingList);
                await _context.SaveChangesAsync();
                var createdShoppingList = _context.ShoppingLists.Where(sl => sl.Name.Equals(shoppingList.Name)).Include(s => s.ListDetails).FirstOrDefault();


                ViewBag.Total = GetTotalPrice(createdShoppingList);
                return RedirectToAction("Edit", createdShoppingList);
            }
            TempData["DidNamingWork"] = false;

            ViewBag.SamsAppUserId = userId;
            var shoppingLists = await _context.ShoppingLists.Where(sl => sl.SamsAppUserId == userId)
                .Include(s => s.SamsAppUser)
                .Include(m => m.ListDetails)
                .ThenInclude(ld => ld.Product)
                .ToListAsync();
            return RedirectToAction("Index");
        }

        // GET: ShoppingLists/Edit/5
        /// <summary>
        /// 
        /// For retrieving the List's Edit page. 
        /// 
        /// </summary>
        /// <param name="id"> A shopping list's Id</param>
        /// <returns> 
        /// 
        /// The Edit View for the shopping list. 
        /// 
        /// </returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ShoppingLists == null)
            {
                return NotFound();
            }

            var shoppingList = _context.ShoppingLists.Include(sl => sl.ListDetails)
                .ThenInclude(c => c.Product)
                .Where(sl => sl.Id == id)
                .FirstOrDefault();
            if (shoppingList == null)
            {
                return NotFound();
            }
            ViewBag.Total = GetTotalPrice(shoppingList);
            return View(shoppingList);
        }

        // Get: ShoppingLists/EditName/5
        /// <summary>
        /// 
        /// For returning the Renaming Editing partial view for adding or changing a list's Name.
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditName(int? id)
        {
            var shoppingList = _context.ShoppingLists.Where(sl => sl.Id == id).FirstOrDefault();
            return PartialView("_Rename", shoppingList);
        }

        // Put: ShoppingLists/Edit/5
        /// <summary>
        /// 
        /// Updates the name of the shopping list. 
        /// 
        /// </summary>
        /// <param name="id">Id of the shopping list.</param>
        /// <param name="name">The new name of the shopping list.</param>
        /// <returns>The user is return to the home page of shopping lists.</returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditName([FromQuery] int id, [FromQuery] string name)
        {
            var shoppingList = _context.ShoppingLists.Where(sl => sl.Id == id).FirstOrDefault();
            shoppingList.Name = name;
            if (ListNameValidation(name))
            {
                try
                {
                    _context.Update(shoppingList);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingListExists(shoppingList.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                TempData["DidNamingWork"] = false;
            }
            var userId = _userManager.GetUserId(User);
            var shoppingLists = _context.ShoppingLists.Where(sl => sl.SamsAppUserId == userId).ToList();
            return BadRequest();
        }


        // GET: ShoppingLists/Delete/5
        /// <summary>
        /// 
        /// For retrieving the Delete Confirmation partial view. 
        /// 
        /// </summary>
        /// <param name="id">The id of the shopping list.</param>
        /// <returns> The user is returned to the home page of shopping lists.</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ShoppingLists == null)
            {
                return NotFound();
            }

            var shoppingList = await _context.ShoppingLists
                .Include(s => s.SamsAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingList == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", shoppingList);
        }

        // DELETE: ShoppingLists/Delete/5
        /// <summary>
        /// 
        /// Delete End point for removing the shopping list and its contents form the database.
        /// 
        /// </summary>
        /// <param name="id">The Id for the shopping list. </param>
        /// <returns>  
        /// 
        /// The user is returned to the Hone page of shopping lists.
        /// 
        /// </returns>
        [Authorize]
        [HttpDelete, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ShoppingLists == null)
            {
                return Problem("Entity set 'SamWarehouseAppContext.ShoppingLists'  is null.");
            }
            var shoppingList = await _context.ShoppingLists.FindAsync(id);
            if (shoppingList != null)
            {
                _context.ShoppingLists.Remove(shoppingList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingListExists(int id)
        {
            return (_context.ShoppingLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        /// <summary>
        /// Returns true if the shopping list name is unique.
        /// And more validation here. 
        /// </summary>
        /// <param name="shoppingList"></param>
        /// <returns></returns>
        private bool ListNameValidation(string Name)
        {
            var userId = _userManager.GetUserId(User);
            var list = _context.ShoppingLists
                .Where(sl => sl.Name.Equals(Name)
                && sl.SamsAppUserId.Equals(userId))
                .FirstOrDefault();
            return list == null;
        }


        /// <summary>
        /// Calculates the total cost for the shopping list. 
        /// </summary>
        /// <param name="shoppingList">The shopping list object</param>
        /// <returns>The total price of the Shopping list as a float.</returns>
        private static float GetTotalPrice(ShoppingList shoppingList)
        {
            var total = 0f;

            if (shoppingList.ListDetails == null)
            {
                return total;
            }

            foreach (var line in shoppingList.ListDetails)
            {
                total = total + line.Product.Price * line.Quantity;

            }
            return total;
        }
    }
}
