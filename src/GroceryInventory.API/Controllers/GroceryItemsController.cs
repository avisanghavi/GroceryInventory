using GroceryInventory.Core.Models;
using GroceryInventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryInventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroceryItemsController : ControllerBase
{
    private readonly GroceryInventoryContext _context;

    public GroceryItemsController(GroceryInventoryContext context)
    {
        _context = context;
    }

    // GET: api/GroceryItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroceryItem>>> GetGroceryItems()
    {
        return await _context.GroceryItems.ToListAsync();
    }

    // GET: api/GroceryItems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GroceryItem>> GetGroceryItem(int id)
    {
        var groceryItem = await _context.GroceryItems.FindAsync(id);

        if (groceryItem == null)
        {
            return NotFound();
        }

        return groceryItem;
    }

    // PUT: api/GroceryItems/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGroceryItem(int id, GroceryItem groceryItem)
    {
        if (id != groceryItem.Id)
        {
            return BadRequest();
        }

        _context.Entry(groceryItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GroceryItemExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/GroceryItems
    [HttpPost]
    public async Task<ActionResult<GroceryItem>> PostGroceryItem(GroceryItem groceryItem)
    {
        _context.GroceryItems.Add(groceryItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGroceryItem), new { id = groceryItem.Id }, groceryItem);
    }

    // DELETE: api/GroceryItems/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroceryItem(int id)
    {
        var groceryItem = await _context.GroceryItems.FindAsync(id);
        if (groceryItem == null)
        {
            return NotFound();
        }

        _context.GroceryItems.Remove(groceryItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool GroceryItemExists(int id)
    {
        return _context.GroceryItems.Any(e => e.Id == id);
    }
}