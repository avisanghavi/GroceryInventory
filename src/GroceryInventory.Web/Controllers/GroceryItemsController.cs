using GroceryInventory.Core.Models;
using GroceryInventory.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroceryInventory.Web.Controllers;

public class GroceryItemsController : Controller
{
    private readonly ApiService _apiService;

    public GroceryItemsController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _apiService.GetGroceryItemsAsync();
        return View(items);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GroceryItem item)
    {
        if (ModelState.IsValid)
        {
            await _apiService.CreateGroceryItemAsync(item);
            return RedirectToAction(nameof(Index));
        }
        return View(item);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _apiService.GetGroceryItemAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GroceryItem item)
    {
        if (id != item.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _apiService.UpdateGroceryItemAsync(id, item);
            return RedirectToAction(nameof(Index));
        }
        return View(item);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await _apiService.GetGroceryItemAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _apiService.DeleteGroceryItemAsync(id);
        return RedirectToAction(nameof(Index));
    }
}