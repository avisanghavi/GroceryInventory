using GroceryInventory.Core.Models;
using GroceryInventory.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace GroceryInventory.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApiService _apiService;

        public OrdersController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _apiService.GetOrdersAsync();
            return View(orders);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            var groceryItems = await _apiService.GetGroceryItemsAsync();
            var suppliers = await _apiService.GetSuppliersAsync();

            ViewBag.GroceryItems = new SelectList(groceryItems.Select(i => new
            {
                Id = i.Id,
                DisplayName = $"{i.Name} ({i.Quantity} {i.Unit})"
            }), "Id", "DisplayName");

            ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name");

            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                await _apiService.CreateOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }

            var groceryItems = await _apiService.GetGroceryItemsAsync();
            var suppliers = await _apiService.GetSuppliersAsync();

            ViewBag.GroceryItems = new SelectList(groceryItems.Select(i => new
            {
                Id = i.Id,
                DisplayName = $"{i.Name} ({i.Quantity} {i.Unit})"
            }), "Id", "DisplayName", order.GroceryItemId);

            ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name", order.SupplierId);

            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _apiService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var groceryItems = await _apiService.GetGroceryItemsAsync();
            var suppliers = await _apiService.GetSuppliersAsync();

            ViewBag.GroceryItems = new SelectList(groceryItems.Select(i => new
            {
                Id = i.Id,
                DisplayName = $"{i.Name} ({i.Quantity} {i.Unit})"
            }), "Id", "DisplayName", order.GroceryItemId);

            ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name", order.SupplierId);

            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _apiService.UpdateOrderAsync(id, order);
                return RedirectToAction(nameof(Index));
            }

            var groceryItems = await _apiService.GetGroceryItemsAsync();
            var suppliers = await _apiService.GetSuppliersAsync();

            ViewBag.GroceryItems = new SelectList(groceryItems.Select(i => new
            {
                Id = i.Id,
                DisplayName = $"{i.Name} ({i.Quantity} {i.Unit})"
            }), "Id", "DisplayName", order.GroceryItemId);

            ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name", order.SupplierId);

            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _apiService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}