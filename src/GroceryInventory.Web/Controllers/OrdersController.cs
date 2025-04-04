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

        private async Task PopulateDropDowns(int? selectedGroceryItemId = null, int? selectedSupplierId = null)
        {
            var groceryItems = await _apiService.GetGroceryItemsAsync();
            var suppliers = await _apiService.GetSuppliersAsync();

            var groceryItemsList = new List<SelectListItem>();
            var suppliersList = new List<SelectListItem>();

            foreach (var item in groceryItems)
            {
                groceryItemsList.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = $"{item.Name} ({item.Quantity} {item.Unit})",
                    Selected = selectedGroceryItemId.HasValue && item.Id == selectedGroceryItemId.Value
                });
            }

            foreach (var supplier in suppliers)
            {
                suppliersList.Add(new SelectListItem
                {
                    Value = supplier.Id.ToString(),
                    Text = supplier.Name,
                    Selected = selectedSupplierId.HasValue && supplier.Id == selectedSupplierId.Value
                });
            }

            ViewBag.GroceryItems = groceryItemsList;
            ViewBag.Suppliers = suppliersList;
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
            await PopulateDropDowns();
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

            await PopulateDropDowns(order.GroceryItemId, order.SupplierId);
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

            await PopulateDropDowns(order.GroceryItemId, order.SupplierId);
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

            await PopulateDropDowns(order.GroceryItemId, order.SupplierId);
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