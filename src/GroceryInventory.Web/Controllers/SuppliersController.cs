using GroceryInventory.Core.Models;
using GroceryInventory.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroceryInventory.Web.Controllers;

public class SuppliersController : Controller
{
    private readonly ApiService _apiService;

    public SuppliersController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var suppliers = await _apiService.GetSuppliersAsync();
        return View(suppliers);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Supplier supplier)
    {
        if (ModelState.IsValid)
        {
            await _apiService.CreateSupplierAsync(supplier);
            return RedirectToAction(nameof(Index));
        }
        return View(supplier);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await _apiService.GetSupplierAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }
        return View(supplier);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Supplier supplier)
    {
        if (id != supplier.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _apiService.UpdateSupplierAsync(id, supplier);
            return RedirectToAction(nameof(Index));
        }
        return View(supplier);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await _apiService.GetSupplierAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }
        return View(supplier);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _apiService.DeleteSupplierAsync(id);
        return RedirectToAction(nameof(Index));
    }
}